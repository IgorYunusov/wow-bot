using AmeisenBot.Character;
using AmeisenBot.Character.Objects;
using AmeisenBot.Clients;
using AmeisenBotCombat;
using AmeisenBotCombat.CombatPackages;
using AmeisenBotCombat.Interfaces;
using AmeisenBotCombat.MovementStrategies;
using AmeisenBotCombat.SpellStrategies;
using AmeisenBotCore;
using AmeisenBotData;
using AmeisenBotPersistence;
using AmeisenBotFSM;
using AmeisenBotFSM.Enums;
using AmeisenBotLogger;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Enums;
using AmeisenBotUtilities.Objects;
using AmeisenMovement;
using AmeisenMovement.Formations;
using Magic;
using Microsoft.CSharp;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using Unit = AmeisenBotUtilities.Unit;

namespace AmeisenBotManager
{
    /// <summary>
    /// This Singleton provides an Interface to the bot at a single point
    /// </summary>
    public class BotManager
    {
        /// <summary>
        /// Create a new AmeisenBotManager to manage the bot's functionality
        /// </summary>
        public BotManager()
        {
            IsBlackmagicAttached = false;
            IsEndsceneHooked = false;

            AmeisenDataHolder = new AmeisenDataHolder();
            AmeisenSettings = new AmeisenSettings(AmeisenDataHolder);
            AmeisenClient = new AmeisenClient(AmeisenDataHolder);
            AmeisenDBManager = new AmeisenDBManager(AmeisenDataHolder);
        }

        public List<WowObject> ActiveWoWObjects { get { return AmeisenDataHolder.ActiveWoWObjects; } }

        public AmeisenDBManager AmeisenDBManager { get; private set; }

        public MeCharacter Character => AmeisenCharacterManager.Character;

        public IAmeisenCombatPackage CombatPackage { get; private set; }

        public string CurrentCombatClassName => CombatPackage.SpellStrategy == null ? "n/a" : CombatPackage.SpellStrategy.ToString().Split('.').Last();

        public BotState CurrentFSMState => AmeisenStateMachineManager.StateMachine.GetCurrentState();

        public int HookJobsInQueue => AmeisenHook.JobCount;

        public bool IsAllowedToAssistParty
        {
            get { return AmeisenDataHolder.IsAllowedToAssistParty; }
            set { AmeisenDataHolder.IsAllowedToAssistParty = value; }
        }

        public bool IsAllowedToBuff
        {
            get { return AmeisenDataHolder.IsAllowedToBuff; }
            set { AmeisenDataHolder.IsAllowedToBuff = value; }
        }

        public bool IsAllowedToDoOwnStuff
        {
            get { return AmeisenDataHolder.IsAllowedToDoOwnStuff; }
            set { AmeisenDataHolder.IsAllowedToDoOwnStuff = value; }
        }

        public bool IsAllowedToDoRandomEmotes
        {
            get { return AmeisenDataHolder.IsAllowedToDoRandomEmotes; }
            set { AmeisenDataHolder.IsAllowedToDoRandomEmotes = value; }
        }

        public bool IsAllowedToFollowParty
        {
            get { return AmeisenDataHolder.IsAllowedToFollowParty; }
            set { AmeisenDataHolder.IsAllowedToFollowParty = value; }
        }

        public bool IsAllowedToReleaseSpirit
        {
            get { return AmeisenDataHolder.IsAllowedToReleaseSpirit; }
            set { AmeisenDataHolder.IsAllowedToReleaseSpirit = value; }
        }

        public bool IsAllowedToRevive
        {
            get { return AmeisenDataHolder.IsAllowedToRevive; }
            set { AmeisenDataHolder.IsAllowedToRevive = value; }
        }

        public bool IsBlackmagicAttached { get; private set; }

        public bool IsConnectedToDB
        {
            get { return AmeisenDataHolder.IsConnectedToDB; }
            set { AmeisenDataHolder.IsConnectedToDB = value; }
        }

        public bool IsConnectedToServer
        {
            get { return AmeisenDataHolder.IsConnectedToServer; }
            set { AmeisenDataHolder.IsConnectedToServer = value; }
        }

        public bool IsEndsceneHooked { get; private set; }

        public bool IsIngame => AmeisenCore.IsWorldLoaded();

        public bool IsLoadingScreenCheckerActive { get; private set; }

        public bool IsRegisteredAtServer => AmeisenClient.IsRegistered;

        public bool IsSpecA
        {
            get { return AmeisenDataHolder.IsAllowedToAttack; }
            set { AmeisenDataHolder.IsAllowedToAttack = value; }
        }

        public bool IsSpecB
        {
            get { return AmeisenDataHolder.IsAllowedToHeal; }
            set { AmeisenDataHolder.IsAllowedToHeal = value; }
        }

        public bool IsSpecC
        {
            get { return AmeisenDataHolder.IsAllowedToTank; }
            set { AmeisenDataHolder.IsAllowedToTank = value; }
        }

        public string LoadedConfigName { get { return AmeisenSettings.loadedconfName; } }

        public Thread LoadingScreenCheckerThread { get; private set; }

        public int MapID { get { return AmeisenCore.GetMapId(); } }

        public Me Me { get { return AmeisenDataHolder.Me; } }

        /// <summary>
        /// Returns copper, silver, gold as an array
        /// </summary>
        public int[] Money
        {
            get
            {
                int copper = AmeisenCharacterManager.Character.Money % 100;
                int silver = (AmeisenCharacterManager.Character.Money - copper) % 10000 / 100;
                int gold = (AmeisenCharacterManager.Character.Money - (silver * 100 + copper)) / 10000;
                return new int[] { copper, silver, gold };
            }
        }

        public List<NetworkBot> NetworkBots
        {
            get
            {
                if (AmeisenClient.IsRegistered)
                {
                    return AmeisenClient.Bots;
                }
                else
                {
                    return null;
                }
            }
        }

        public List<Unit> Partymembers
        {
            get { return AmeisenDataHolder.Partymembers; }
            set { AmeisenDataHolder.Partymembers = value; }
        }

        public Unit Pet { get { return AmeisenDataHolder.Pet; } }

        public List<WowExe> RunningWows { get { return AmeisenCore.GetRunningWows(); } }

        public Settings Settings { get { return AmeisenSettings.Settings; } }

        public Unit Target { get { return AmeisenDataHolder.Target; } }

        public WowExe WowExe { get; private set; }

        public Process WowProcess { get; private set; }

        public int ZoneID { get { return AmeisenCore.GetZoneId(); } }

        /// <summary>
        /// Check if we remember a Unit by its Name, ZoneID and MapID
        /// </summary>
        /// <param name="name">name of the npc</param>
        /// <param name="zoneID">zoneid of the npc</param>
        /// <param name="mapID">mapid of the npc</param>
        /// <returns>RememberedUnit with if we remember it, its UnitTraits and position</returns>
        public RememberedUnit CheckForRememberedUnit(string name, int zoneID, int mapID)
            => AmeisenDBManager.CheckForRememberedUnit(name, zoneID, mapID);

        /// <summary>
        /// Equip all better items thats in the bots inventory
        /// </summary>
        public void EquipAllBetterItems() => AmeisenCharacterManager.EquipAllBetterItems();

        /// <summary>
        /// Load a given CombatClass *.cs file into the CombatManager by compiling it at runtime
        /// </summary>
        /// <param name="fileName">*.cs CombatClass file</param>
        public void LoadCombatClassFromFile(string fileName)
        {
            AmeisenSettings.Settings.combatClassPath = fileName;
            AmeisenSettings.SaveToFile(AmeisenSettings.loadedconfName);
            IAmeisenCombatPackage combatClass = CompileAndLoadCombatClass(fileName);
            AmeisenStateMachineManager.StateMachine.LoadNewCombatClass(AmeisenDataHolder, combatClass, AmeisenDBManager, AmeisenNavmeshClient);
        }

        /// <summary>
        /// Loads the Settings from a given file
        /// </summary>
        /// <param name="filename">file to load the Settings from</param>
        public void LoadSettingsFromFile(string filename) => AmeisenSettings.LoadFromFile(filename);

        /// <summary>
        /// Reload the CombatClass if you changed the role or something alike
        /// </summary>
        public void RefreshCombatClass()
        {
            CombatPackage = LoadDefaultClassForSpec();
            AmeisenStateMachineManager.UpdateCombatPackage(CombatPackage);
        }

        /// <summary>
        /// Refresh the characters equipment & inventory
        /// </summary>
        public void RefreshCurrentItems() => AmeisenCharacterManager.UpdateCharacterAsync();

        /// <summary>
        /// Add a RememberedUnit to the RememberedUnits Database to remember its position and UnitTraits
        /// </summary>
        /// <param name="rememberedUnit">Unit that you want to remember</param>
        public void RememberUnit(RememberedUnit rememberedUnit)
            => AmeisenDBManager.RememberUnit(rememberedUnit);

        /// <summary>
        /// Save the current Settings to the given file
        /// </summary>
        /// <param name="filename">file to save the Settings to</param>
        public void SaveSettingsToFile(string filename) => AmeisenSettings.SaveToFile(filename);

        /// <summary>
        /// Starts the bots mechanisms, hooks, ...
        /// </summary>
        /// <param name="wowExe">WowExe to start the bot on</param>
        public void StartBot(WowExe wowExe)
        {
            AmeisenLogger.Instance.currentUsername = wowExe.characterName;
            AmeisenLogger.Instance.RefreshLogName();
            WowExe = wowExe;
            LootableUnits = new Queue<Unit>();

            // Load Settings
            AmeisenSettings.LoadFromFile(wowExe.characterName);

            // Load old WoW Position
            if (AmeisenSettings.Settings.saveBotWindowPosition)
            {
                if (AmeisenSettings.Settings.wowRectL > 0
                && AmeisenSettings.Settings.wowRectR > 0
                && AmeisenSettings.Settings.wowRectT > 0
                && AmeisenSettings.Settings.wowRectB > 0)
                {
                    AmeisenCore.SetWindowPosition(
                    wowExe.process.MainWindowHandle,
                    (int)AmeisenSettings.Settings.wowRectL,
                    (int)AmeisenSettings.Settings.wowRectT,
                    (int)AmeisenSettings.Settings.wowRectB - (int)AmeisenSettings.Settings.wowRectT,
                    (int)AmeisenSettings.Settings.wowRectR - (int)AmeisenSettings.Settings.wowRectL);
                }
            }

            // Connect to DB
            if (AmeisenSettings.Settings.databaseAutoConnect)
            {
                ConnectToDB();
            }

            // Connect to NavmeshServer
            if (AmeisenSettings.Settings.navigationServerAutoConnect)
            {
                AmeisenNavmeshClient = new AmeisenNavmeshClient(
                    AmeisenSettings.Settings.navigationServerIp,
                    AmeisenSettings.Settings.navigationServerPort
                );
            }

            // Attach to Proccess
            Blackmagic = new BlackMagic(wowExe.process.Id);
            IsBlackmagicAttached = Blackmagic.IsProcessOpen;
            // TODO: make this non static
            AmeisenCore.BlackMagic = Blackmagic;

            // Hook EndScene
            AmeisenHook = new AmeisenHook(Blackmagic);
            IsEndsceneHooked = AmeisenHook.isHooked;
            // TODO: make this non static
            AmeisenCore.AmeisenHook = AmeisenHook;

            // Unlimit fps to speed up loading, we will limit them later again
            AmeisenCore.RunSlashCommand($"/console maxfps 30");
            AmeisenCore.RunSlashCommand($"/console maxfpsbk 30");

            // Init our CharacterMangager to keep track of our stats/items/money
            AmeisenCharacterManager = new AmeisenCharacterManager();
            AmeisenCharacterManager.UpdateCharacterAsync();

            // Hook Events
            AmeisenEventHook = new AmeisenEventHook();
            AmeisenEventHook.Init();
            AmeisenEventHook.Subscribe(WowEvents.PLAYER_ENTERING_WORLD, OnPlayerEnteringWorld);
            AmeisenEventHook.Subscribe(WowEvents.LOOT_OPENED, OnLootWindowOpened);
            AmeisenEventHook.Subscribe(WowEvents.LOOT_BIND_CONFIRM, OnLootBindOnPickup);
            AmeisenEventHook.Subscribe(WowEvents.READY_CHECK, OnReadyCheck);
            AmeisenEventHook.Subscribe(WowEvents.PARTY_INVITE_REQUEST, OnPartyInvitation);
            AmeisenEventHook.Subscribe(WowEvents.CONFIRM_SUMMON, OnSummonRequest);
            AmeisenEventHook.Subscribe(WowEvents.RESURRECT_REQUEST, OnResurrectRequest);
            AmeisenEventHook.Subscribe(WowEvents.PLAYER_REGEN_DISABLED, OnRegenDisabled);
            AmeisenEventHook.Subscribe(WowEvents.PLAYER_REGEN_ENABLED, OnRegenEnabled);
            AmeisenEventHook.Subscribe(WowEvents.START_LOOT_ROLL, OnStartLootRoll);
            AmeisenEventHook.Subscribe(WowEvents.ITEM_PUSH, OnNewItem);
            AmeisenEventHook.Subscribe(WowEvents.PARTY_MEMBERS_CHANGED, OnGroupChanged);
            //AmeisenEventHook.Subscribe(WowEvents.COMBAT_LOG_EVENT_UNFILTERED, OnCombatLogEvent);

            // LoadingscreenChecker, stops our hook if we are in loadingscreens
            IsLoadingScreenCheckerActive = true;
            LoadingScreenCheckerThread = new Thread(new ThreadStart(LoadingScreenChecker));
            LoadingScreenCheckerThread.Start();

            // Start our object updates
            AmeisenObjectManager = new AmeisenObjectManager(AmeisenDataHolder, AmeisenDBManager);
            AmeisenObjectManager.Start();

            // Load the combatclass
            // CombatClass = CompileAndLoadCombatClass(AmeisenSettings.Settings.combatClassPath);
            if (CombatPackage == null)
            {
                CombatPackage = LoadDefaultClassForSpec();
            }

            // Init our MovementEngine to position ourself according to our formation
            AmeisenMovementEngine = new AmeisenMovementEngine(new DefaultFormation())
            {
                MemberCount = 40
            };

            // Start the StateMachine
            AmeisenStateMachineManager = new AmeisenStateMachineManager(
                AmeisenDataHolder,
                AmeisenDBManager,
                AmeisenMovementEngine,
                CombatPackage,
                AmeisenCharacterManager,
                AmeisenNavmeshClient);

            // Deafult Idle state
            AmeisenStateMachineManager.StateMachine.PushAction(BotState.Idle);
            AmeisenStateMachineManager.Start();

            // Connect to Server
            if (Settings.serverAutoConnect)
            {
                ConnectToServer();
            }

            // Ultralow Gfx
            if (Settings.autoUltralowGfx)
            {
                AmeisenCore.RunSlashCommand("/console farclip 350");
                AmeisenCore.RunSlashCommand("/console groundEffectDensity 0");
                AmeisenCore.RunSlashCommand("/console groundEffectDistance 0");
                AmeisenCore.RunSlashCommand("/console environmentDetail 0");
                AmeisenCore.RunSlashCommand("/console particleDensity 10");
                AmeisenCore.RunSlashCommand("/console shadowMode 0");
                AmeisenCore.RunSlashCommand("/console waterDetail 0");
                AmeisenCore.RunSlashCommand("/console reflectionMode 0");
                AmeisenCore.RunSlashCommand("/console sunShafts 0");
                AmeisenCore.RunSlashCommand("/console basemip 1");
                AmeisenCore.RunSlashCommand("/console terrainMipLevel 1");
                AmeisenCore.RunSlashCommand("/console projectedTextures 0");
                AmeisenCore.RunSlashCommand("/console weatherDensity 0");
                AmeisenCore.RunSlashCommand("/console componentTextureLevel 0");
                AmeisenCore.RunSlashCommand("/console textureFilteringMode 0");
            }

            // Limit fps
            AmeisenCore.RunSlashCommand($"/console maxfps {Settings.maxFpsForeground}");
            AmeisenCore.RunSlashCommand($"/console maxfpsbk {Settings.maxFpsBackground}");            
        }

        /// <summary>
        /// Stops the bots mechanisms, hooks, ...
        /// </summary>
        public void StopBot()
        {
            IsLoadingScreenCheckerActive = false;
            LoadingScreenCheckerThread.Join();

            // Disconnect from Server
            AmeisenClient.Unregister(
                Me,
                IPAddress.Parse(AmeisenSettings.Settings.ameisenServerIp),
                AmeisenSettings.Settings.ameisenServerPort);

            // Save WoW's window positions
            SafeNativeMethods.Rect wowRect = AmeisenCore.GetWowDiemsions(WowExe.process.MainWindowHandle);
            AmeisenSettings.Settings.wowRectT = wowRect.Top;
            AmeisenSettings.Settings.wowRectB = wowRect.Bottom;
            AmeisenSettings.Settings.wowRectL = wowRect.Left;
            AmeisenSettings.Settings.wowRectR = wowRect.Right;

            // Stop object updates
            AmeisenObjectManager.Stop();

            // Stop the statemachine
            AmeisenStateMachineManager.Stop();

            // Unhook Events
            AmeisenEventHook?.Stop();

            // Unhook the EndScene
            AmeisenHook.DisposeHooking();

            // Detach BlackMagic, causing weird crash right now...
            //Blackmagic.Close();

            // Stop logging
            AmeisenLogger.Instance.StopLogging();
            AmeisenSettings.SaveToFile(AmeisenSettings.loadedconfName);
        }

        private readonly string sqlConnectionString = "server={0};port={1};database={2};uid={3};password={4};";

        private AmeisenCharacterManager AmeisenCharacterManager { get; set; }
        private AmeisenClient AmeisenClient { get; set; }
        private AmeisenDataHolder AmeisenDataHolder { get; set; }
        private AmeisenEventHook AmeisenEventHook { get; set; }
        private AmeisenHook AmeisenHook { get; set; }
        private AmeisenMovementEngine AmeisenMovementEngine { get; set; }
        private AmeisenNavmeshClient AmeisenNavmeshClient { get; set; }
        private AmeisenObjectManager AmeisenObjectManager { get; set; }
        private AmeisenSettings AmeisenSettings { get; set; }
        private AmeisenStateMachineManager AmeisenStateMachineManager { get; set; }
        private BlackMagic Blackmagic { get; set; }

        private Queue<Unit> LootableUnits
        {
            get => AmeisenDataHolder.LootableUnits;
            set => AmeisenDataHolder.LootableUnits = value;
        }

        private void CheckForLoot()
        {
            foreach (WowObject obj in ActiveWoWObjects)
            {
                if (obj.GetType() == typeof(Player)
                    || obj.GetType() == typeof(Unit)
                    || obj.GetType() == typeof(Me))
                {
                    if (!((Unit)obj).IsDead)
                    {
                        continue; // We cant loot alive targets lel
                    }

                    obj.Update();
                    if (((Unit)obj).IsLootable)
                    {
                        LootableUnits.Enqueue((Unit)obj);
                    }
                    continue;
                }
            }
        }

        /// <summary>
        /// Compile a CombatClass *.cs file and return its Instance
        /// </summary>
        /// <param name="combatclassPath">*.cs CombatClass file</param>
        /// <returns>Instance of the built Class, if its null somethings gone wrong</returns>
        private IAmeisenCombatPackage CompileAndLoadCombatClass(string combatclassPath)
        {
            if (File.Exists(combatclassPath))
            {
                try
                {
                    return CompileCombatClass(combatclassPath);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString(), "Compilation error", MessageBoxButton.OK, MessageBoxImage.Error);
                    AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Error while compiling CombatClass: {Path.GetFileName(combatclassPath)}", this);
                    AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"{e.Message}", this);
                }
            }

            return null;
        }

        /// <summary>
        /// Compile a combatclass *.cs file at runtime and load it into the bot
        /// </summary>
        /// <param name="combatclassPath">path to the *.cs file</param>
        /// <returns></returns>
        private IAmeisenCombatPackage CompileCombatClass(string combatclassPath)
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Compiling CombatClass: {Path.GetFileName(combatclassPath)}", this);

            CompilerParameters parameters = new CompilerParameters();
            // Include dependencies
            string ownPath = AppDomain.CurrentDomain.BaseDirectory;
            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add(ownPath + "/lib/AmeisenBot.Combat.dll");
            parameters.ReferencedAssemblies.Add(ownPath + "/lib/AmeisenBot.Utilities.dll");
            parameters.ReferencedAssemblies.Add(ownPath + "/lib/AmeisenBot.Logger.dll");
            parameters.ReferencedAssemblies.Add(ownPath + "/lib/AmeisenBot.Data.dll");
            parameters.GenerateInMemory = true; // generate no file
            parameters.GenerateExecutable = false; // to output a *.dll not a *.exe

            // compile it
            CompilerResults results = new CSharpCodeProvider().CompileAssemblyFromSource(parameters, File.ReadAllText(combatclassPath));

            if (results.Errors.HasErrors)
            {
                StringBuilder sb = new StringBuilder();

                foreach (CompilerError error in results.Errors)
                {
                    sb.AppendLine($"Error ({error.ErrorNumber}): {error.ErrorText}\nLine:{error.Line}");
                }

                throw new InvalidOperationException(sb.ToString());
            }

            // Create Instance of CombatClass
            IAmeisenCombatPackage result = (IAmeisenCombatPackage)results.CompiledAssembly.CreateInstance("AmeisenBotCombat.CombatClass");

            AmeisenLogger.Instance.Log(LogLevel.DEBUG, $"Successfully compiled CombatClass: {Path.GetFileName(combatclassPath)}", this);
            return result;
        }

        private bool ConnectToDB() => AmeisenDBManager.ConnectToMySQL(
                string.Format(sqlConnectionString,
                AmeisenSettings.Settings.databaseIp,
                AmeisenSettings.Settings.databasePort,
                AmeisenSettings.Settings.databaseName,
                AmeisenSettings.Settings.databaseUsername,
                AmeisenSettings.Settings.databasePasswort)
            );

        private bool ConnectToServer() => AmeisenClient.Register(
                Me,
                IPAddress.Parse(AmeisenSettings.Settings.ameisenServerIp),
                AmeisenSettings.Settings.ameisenServerPort
            );

        private IAmeisenCombatPackage LoadDefaultClassForSpec()
        {
            AmeisenDataHolder.IsHealer = false;

            while (!Character.FullyLoaded)
            {
                Thread.Sleep(250);
            }

            switch (Me.Class)
            {
                case WowClass.Rogue:
                    return new CPDefault(
                        Character.Spells,
                        new RogueCombat(Character.Spells),
                        new MovementClose(2.7)
                    );

                case WowClass.Warrior:
                    return new CPDefault(
                        Character.Spells,
                        new WarriorFury(Character.Spells),
                        new MovementClose(2.7)
                    );

                case WowClass.Warlock:
                    return new CPDefault(
                        Character.Spells,
                        new WarlockAffliction(Character.Spells),
                        new MovementClose(28)
                    );

                case WowClass.Paladin:
                    return new CPDefault(
                        Character.Spells,
                        new PaladinRetribution(Character.Spells),
                        new MovementClose(2.8)
                    );

                case WowClass.Druid:
                    AmeisenDataHolder.IsHealer = true;
                    return new CPDefault(
                        Character.Spells,
                        new DruidRestoration(Character.Spells),
                        new MovementClose(38)
                    );

                case WowClass.Priest:
                    if (IsSpecA)
                    {
                        AmeisenDataHolder.IsHealer = true;
                        return new CPDefault(
                            Character.Spells,
                            new PriestHoly(Character.Spells),
                            new MovementClose(38)
                        );
                    }
                    else if (IsSpecB)
                    {
                        AmeisenDataHolder.IsHealer = false;
                        return new CPDefault(
                            Character.Spells,
                            new PriestShadow(Character.Spells),
                            new MovementClose(28)
                        );
                    }
                    else
                    {
                        CombatPackage = null;
                        return new CPDefault(Character.Spells, null, new MovementClose());
                    }

                case WowClass.Mage:
                    return new CPDefault(
                        Character.Spells,
                        new MageFire(Character.Spells),
                        new MovementClose(28)
                    );

                case WowClass.Hunter:
                    return new CPDefault(
                        Character.Spells,
                        new HunterMarksman(Character.Spells),
                        new MovementClose(33)
                    );

                default:
                    return new CPDefault(Character.Spells, null, new MovementClose());
            }
        }

        private void LoadingScreenChecker()
        {
            while (IsLoadingScreenCheckerActive)
            {
                if (AmeisenCore.IsInLoadingScreen())
                {
                    AmeisenHook.IsNotInWorld = true;
                }
                else
                {
                    AmeisenHook.IsNotInWorld = false;
                }

                Thread.Sleep(250);
            }
        }

        private void OnCombatLogEvent(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"OnCombatLogEvent args: {JsonConvert.SerializeObject(args)}",
                this
            );
        }

        private void OnGroupChanged(long timestamp, List<string> args)
        {
            // Refresh Partymembers
            AmeisenDataHolder.Partymembers = CombatUtils.GetPartymembers(Me, ActiveWoWObjects);
        }

        private void OnLootBindOnPickup(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"OnLootBindOnPickup args: {JsonConvert.SerializeObject(args)}",
                this
            );
        }

        private void OnLootWindowOpened(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"OnLootWindowOpened args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCore.LootEveryThing();
        }

        private void OnNewItem(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"OnNewItem args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCharacterManager.UpdateCharacter();
            AmeisenCharacterManager.EquipAllBetterItems();
        }

        private void OnPartyInvitation(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"OnPartyInvitation args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCore.AcceptGroupInvite();
            AmeisenCore.RunSlashCommand("/click StaticPopup1Button1");
        }

        private void OnPlayerEnteringWorld(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"OnPlayerEnteringWorld args: {JsonConvert.SerializeObject(args)}",
                this
            );
        }

        private void OnReadyCheck(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"OnReadyCheck args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCore.ConfirmReadyCheck();
        }

        private void OnRegenDisabled(long timestamp, List<string> args)
            => Me.InCombatEvent = true;

        private void OnRegenEnabled(long timestamp, List<string> args)
        {
            Me.InCombatEvent = false;
            // CheckForLoot();
        }

        private void OnResurrectRequest(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"OnResurrectRequest args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCore.AcceptResurrect();
            AmeisenCore.RunSlashCommand("/click StaticPopup1Button1");
        }

        private void OnStartLootRoll(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"OnStartLootRoll args: {JsonConvert.SerializeObject(args)}",
                this
            );

            string ItemName = AmeisenCore.ReadRollItemName(args[0]).name;

            if (AmeisenCharacterManager.INeedThatItem(ItemName))
            {
                AmeisenLogger.Instance.Log(
                    LogLevel.DEBUG,
                    $"I could use that item: {ItemName}",
                    this
                );
            }
        }

        private void OnSummonRequest(long timestamp, List<string> args)
        {
            AmeisenLogger.Instance.Log(
                LogLevel.DEBUG,
                $"OnSummonRequest args: {JsonConvert.SerializeObject(args)}",
                this
            );

            AmeisenCore.AcceptSummon();
            AmeisenCore.RunSlashCommand("/click StaticPopup1Button1");
        }
    }
}