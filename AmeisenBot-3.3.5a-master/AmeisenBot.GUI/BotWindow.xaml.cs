using AmeisenBotLogger;
using AmeisenBotManager;
using AmeisenBotUtilities;
using AmeisenBotUtilities.Enums;
using AmeisenBotUtilities.Objects;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AmeisenBotGUI
{
    /// <summary>
    /// Interaktionslogik für mainscreenForm.xaml 💕
    /// </summary>
    public partial class BotWindow : Window
    {
        public static Dictionary<UnitTrait, string> UnitTraitSymbols =
            new Dictionary<UnitTrait, string> {
                { UnitTrait.SELL, "💰" },
                { UnitTrait.REPAIR, "🔧" },
                { UnitTrait.FOOD, "🍖" },
                { UnitTrait.DRINK, "🍹" },
                { UnitTrait.FLIGHTMASTER, "🛫" },
                { UnitTrait.AUCTIONMASTER, "💸" },
            };

        public BotWindow(WowExe wowExe, BotManager botManager)
        {
            InitializeComponent();
            BotManager = botManager;

            // Load Settings
            BotManager.LoadSettingsFromFile(wowExe.characterName);
            ApplyConfigColors();
            BotManager.StartBot(wowExe);

            if (Settings.saveBotWindowPosition)
            {
                if (Settings.oldXindowPosX != 0)
                {
                    Left = Settings.oldXindowPosX;
                }

                if (Settings.oldXindowPosY != 0)
                {
                    Top = Settings.oldXindowPosY;
                }
            }
        }

        private Settings Settings => BotManager.Settings;
        private string lastImgPath;
        private DispatcherTimer uiUpdateTimer;
        private BotManager BotManager { get; }
        private ulong LastGuid { get; set; }

        private void ApplyConfigColors()
        {
            ResourceDictionary resources = Application.Current.Resources;
            resources["AccentColor"] = ParseColor(Settings.accentColor);
            resources["BackgroundColor"] = ParseColor(Settings.backgroundColor);
            resources["TextColor"] = ParseColor(Settings.textColor);

            resources["MeNodeColor"] = ParseColor(Settings.meNodeColor);
            resources["WalkableNodeColorLow"] = ParseColor(Settings.walkableNodeColorLow);
            resources["WalkableNodeColorHigh"] = ParseColor(Settings.walkableNodeColorHigh);

            resources["HealthColor"] = ParseColor(Settings.healthColor);
            resources["EnergyColor"] = ParseColor(Settings.energyColor);
            resources["ExpColor"] = ParseColor(Settings.expColor);
            resources["TargetHealthColor"] = ParseColor(Settings.targetHealthColor);
            resources["TargetEnergyColor"] = ParseColor(Settings.targetEnergyColor);
            resources["holoLogoColor"] = ParseColor(Settings.holoLogoColor);
        }

        private void ButtonCobatClassEditor_Click(object sender, RoutedEventArgs e)
        {
            new GearWindow(BotManager).Show();
        }

        private void ButtonCombatEditor_Click(object sender, RoutedEventArgs e)
        {
            new CombatEditor(BotManager).ShowDialog();
        }

        private void ButtonDebugTest_Click(object sender, RoutedEventArgs e)
        {
            AmeisenBotCore.AmeisenCore.TargetGUID(ulong.Parse(textboxGuid.Text));
        }

        private void ButtonDebugTest2_Click(object sender, RoutedEventArgs e)
        {
            AmeisenBotCore.AmeisenCore.LuaDoString("TurnRightStop();");
        }

        private void ButtonEquiptAllBetter_Click(object sender, RoutedEventArgs e)
        {
            BotManager.EquipAllBetterItems();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
            => Close();

        private void ButtonExtendedDebugUI_Click(object sender, RoutedEventArgs e)
            => new DebugWindow(BotManager).Show();

        private void ButtonGroup_Click(object sender, RoutedEventArgs e)
            => new GroupWindow(BotManager).Show();

        private void ButtonMap_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                AddExtension = true,
                RestoreDirectory = true,
                Filter = "CombatClass *.cs|*.cs"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                BotManager.LoadCombatClassFromFile(openFileDialog.FileName);
            }
        }

        private void ButtonRefreshCharacterEquip_Click(object sender, RoutedEventArgs e)
        {
            BotManager.RefreshCurrentItems();
        }

        private void ButtonRememberUnit_Click(object sender, RoutedEventArgs e)
        {
            if (BotManager.Target != null && BotManager.Target.Guid != 0)
            {
                RememberUnitWindow rememberUnitWindow = new RememberUnitWindow(BotManager.Target)
                {
                    Topmost = Settings.topMost
                };
                rememberUnitWindow.ShowDialog();

                if (rememberUnitWindow.ShouldRemember)
                {
                    BotManager.RememberUnit(rememberUnitWindow.UnitToRemmeber);
                }
            }
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
            => new SettingsWindow(BotManager).ShowDialog();

        private void CheckBoxAssistGroup_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToAssistParty = (bool)checkBoxAssistGroup.IsChecked;

        private void CheckBoxAssistPartyBuff_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToBuff = (bool)checkBoxAssistPartyBuff.IsChecked;

        private void CheckBoxDoBotStuff_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToDoOwnStuff = (bool)checkBoxDoBotStuff.IsChecked;

        private void CheckBoxFollowMaster_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToFollowParty = (bool)checkBoxFollowParty.IsChecked;

        private void CheckBoxReleaseSpirit_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToReleaseSpirit = (bool)checkBoxReleaseSpirit.IsChecked;

        private void CheckBoxReleaseSpirit_Copy_Click(object sender, RoutedEventArgs e)
            => BotManager.IsAllowedToRevive = (bool)checkBoxRevive.IsChecked;

        private void CheckBoxTopMost_Click(object sender, RoutedEventArgs e)
            => SetTopMost();

        private void LoadViewSettings()
        {
            radiobuttonSpecA.IsChecked = Settings.behaviourAttack;
            BotManager.IsSpecA = Settings.behaviourAttack;

            radiobuttonSpecC.IsChecked = Settings.behaviourTank;
            BotManager.IsSpecC = Settings.behaviourTank;

            radiobuttonSpecB.IsChecked = Settings.behaviourHeal;
            BotManager.IsSpecB = Settings.behaviourHeal;
            BotManager.RefreshCombatClass();

            checkBoxAssistPartyBuff.IsChecked = Settings.behaviourBuff;
            BotManager.IsAllowedToBuff = Settings.behaviourBuff;

            checkBoxAssistGroup.IsChecked = Settings.assistParty;
            BotManager.IsAllowedToAssistParty = Settings.assistParty;

            checkBoxFollowParty.IsChecked = Settings.followMaster;
            BotManager.IsAllowedToFollowParty = Settings.followMaster;

            checkBoxReleaseSpirit.IsChecked = Settings.releaseSpirit;
            BotManager.IsAllowedToReleaseSpirit = Settings.releaseSpirit;

            checkBoxRandomEmotes.IsChecked = Settings.randomEmotes;
            BotManager.IsAllowedToDoRandomEmotes = Settings.randomEmotes;

            checkBoxDoBotStuff.IsChecked = Settings.doOwnStuff;
            BotManager.IsAllowedToDoOwnStuff = Settings.doOwnStuff;

            checkBoxRevive.IsChecked = Settings.revive;
            BotManager.IsAllowedToRevive = Settings.revive;

            sliderDistance.Value = Settings.followDistance;

            checkBoxTopMost.IsChecked = Settings.topMost;
            Topmost = Settings.topMost;
        }

        private void Mainscreen_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveViewSettings();
            Settings.oldXindowPosX = Left;
            Settings.oldXindowPosY = Top;
            BotManager.StopBot();
        }

        private void Mainscreen_Loaded(object sender, RoutedEventArgs e)
        {
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, "Loaded MainScreen", this);

            Title = $"AmeisenBot - {BotManager.WowExe.characterName} [{BotManager.WowExe.process.Id}]";

            UpdateUI();
            StartUIUpdateTime();

            LoadViewSettings();
        }

        private void Mainscreen_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); }
            catch { }
        }

        private Color ParseColor(string colorString) => (Color)ColorConverter.ConvertFromString(colorString);

        private string ProcessKMValue(double value)
        {
            if (value > 1000000)
            {
                return $"{(int)value / 1000000}M";
            }

            if (value > 1000)
            {
                return $"{(int)value / 1000}K";
            }

            return $"{value}";
        }

        private void RadioButtonSpecA_Click(object sender, RoutedEventArgs e)
        {
            BotManager.IsSpecA = true;
            BotManager.IsSpecB = false;
            BotManager.IsSpecC = false;
            BotManager.RefreshCombatClass();
        }

        private void RadioButtonSpecB_Click(object sender, RoutedEventArgs e)
        {
            BotManager.IsSpecA = false;
            BotManager.IsSpecB = true;
            BotManager.IsSpecC = false;
            BotManager.RefreshCombatClass();
        }

        private void RadioButtonSpecC_Click(object sender, RoutedEventArgs e)
        {
            BotManager.IsSpecA = false;
            BotManager.IsSpecB = false;
            BotManager.IsSpecC = true;
            BotManager.RefreshCombatClass();
        }

        private void SaveViewSettings()
        {
            Settings.behaviourAttack = (bool)radiobuttonSpecA.IsChecked;
            Settings.behaviourTank = (bool)radiobuttonSpecC.IsChecked;
            Settings.behaviourHeal = (bool)radiobuttonSpecB.IsChecked;
            Settings.behaviourBuff = (bool)checkBoxAssistPartyBuff.IsChecked;
            Settings.followMaster = (bool)checkBoxFollowParty.IsChecked;
            Settings.releaseSpirit = (bool)checkBoxReleaseSpirit.IsChecked;
            Settings.revive = (bool)checkBoxRevive.IsChecked;
            BotManager.SaveSettingsToFile(BotManager.LoadedConfigName);
        }

        private void SetTopMost()
        {
            Topmost = (bool)checkBoxTopMost.IsChecked;
            Settings.topMost = (bool)checkBoxTopMost.IsChecked;
        }

        private void SliderDistance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                labelDistance.Content = $"Follow Distance: {Math.Round(sliderDistance.Value, 2)}m";
                Settings.followDistance = Math.Round(sliderDistance.Value, 2);
            }
            catch { }
        }

        private void StartUIUpdateTime()
        {
            uiUpdateTimer = new DispatcherTimer();
            uiUpdateTimer.Tick += new EventHandler(UIUpdateTimer_Tick);
            uiUpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            uiUpdateTimer.Start();
            AmeisenLogger.Instance.Log(LogLevel.DEBUG, "Started UI-Update-Timer", this);
        }

        private void UIUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (BotManager.IsIngame)
            {
                UpdateUI();
            }
        }

        private void UpdateFSMViews() => labelFSMState.Content = $"Current State: {BotManager.CurrentFSMState}";

        private void UpdateGroupViews()
        {
            stackpanelGroupViews.Children.Clear();
            stackpanelGroupViews.Children.Add(new GroupView(BotManager.Me));
            foreach (Unit unit in BotManager.Partymembers)
            {
                stackpanelGroupViews.Children.Add(new GroupView(unit));
            }
        }

        private void UpdateMyViews()
        {
            try
            {
                if (Settings.picturePath != lastImgPath)
                {
                    if (Settings.picturePath.Length > 0)
                    {
                        botPicture.Source = new BitmapImage(new Uri(Settings.picturePath));
                        lastImgPath = Settings.picturePath;
                    }
                }
            }
            catch { AmeisenLogger.Instance.Log(LogLevel.ERROR, "Failed to load picture...", this); }

            labelName.Content = BotManager.Me.Name + " lvl." + BotManager.Me.Level;
            labelHP.Content = $"Health {ProcessKMValue(BotManager.Me.Health)} / {ProcessKMValue(BotManager.Me.MaxHealth)}";

            labelMapID.Content = $"MapId: {BotManager.Me.MapId}";
            labelZoneID.Content = $"ZoneId: {BotManager.Me.ZoneId}";

            labelLeaderGuid.Content = $"LeaderGuid: {BotManager.Me.PartyleaderGuid}";

            progressBarHP.Maximum = BotManager.Me.MaxHealth;
            progressBarHP.Value = BotManager.Me.Health;

            labelMoney.Content = $"{BotManager.Money[2]}g {BotManager.Money[1]}s {BotManager.Money[1]}c";

            labelHookQueue.Content = $"HookJobs pending: {BotManager.HookJobsInQueue}";
            progressbarHookQueue.Value = BotManager.HookJobsInQueue;

            switch (BotManager.Me.Class)
            {
                case WowClass.Warrior:
                    labelEnergy.Content = $"Energy {ProcessKMValue(BotManager.Me.Rage)} / {ProcessKMValue(BotManager.Me.MaxRage)}";
                    progressBarEnergy.Maximum = BotManager.Me.MaxRage;
                    progressBarEnergy.Value = BotManager.Me.Rage;
                    break;

                case WowClass.Rogue:
                    labelEnergy.Content = $"Energy {ProcessKMValue(BotManager.Me.Energy)} / {ProcessKMValue(BotManager.Me.MaxEnergy)}";
                    progressBarEnergy.Maximum = BotManager.Me.MaxEnergy;
                    progressBarEnergy.Value = BotManager.Me.Energy;
                    break;

                case WowClass.DeathKnight:
                    labelEnergy.Content = $"Energy {ProcessKMValue(BotManager.Me.RuneEnergy)} / {ProcessKMValue(BotManager.Me.MaxRuneEnergy)}";
                    progressBarEnergy.Maximum = BotManager.Me.MaxRuneEnergy;
                    progressBarEnergy.Value = BotManager.Me.RuneEnergy;
                    break;

                default:
                    labelEnergy.Content = $"Energy {ProcessKMValue(BotManager.Me.Mana)} / {ProcessKMValue(BotManager.Me.MaxMana)}";
                    progressBarEnergy.Maximum = BotManager.Me.MaxMana;
                    progressBarEnergy.Value = BotManager.Me.Mana;
                    break;
            }

            labelExp.Content = $"Exp {ProcessKMValue(BotManager.Me.Exp)} / {ProcessKMValue(BotManager.Me.MaxExp)}";

            progressBarXP.Maximum = BotManager.Me.MaxExp;
            progressBarXP.Value = BotManager.Me.Exp;
        }

        private void UpdateTargetViews()
        {
            labelNameTarget.Content = $"{BotManager.Target.Name} lvl.{BotManager.Target.Level}";

            labelTargetHP.Content = $"Health {ProcessKMValue(BotManager.Target.Health)} / {ProcessKMValue(BotManager.Target.MaxHealth)}";
            progressBarHPTarget.Maximum = BotManager.Target.MaxHealth;
            progressBarHPTarget.Value = BotManager.Target.Health;

            labelTargetEnergy.Content = $"Energy {ProcessKMValue(BotManager.Target.Mana)} / {ProcessKMValue(BotManager.Target.MaxMana)}";
            progressBarEnergyTarget.Maximum = BotManager.Target.MaxMana;
            progressBarEnergyTarget.Value = BotManager.Target.Mana;

            labelTargetDistance.Content = $"Distance: {Math.Round(BotManager.Target.Distance, 2)}m";

            Unit target = BotManager.Target;

            if (target != null && target.Guid != 0)
            {
                if (target.Guid != LastGuid)
                {
                    target.Update();
                    RememberedUnit rememberedUnit = BotManager.CheckForRememberedUnit(target.Name, target.ZoneId, target.MapId);

                    if (rememberedUnit != null)
                    {
                        labelRemember.Content = "I know this Unit";

                        StringBuilder sb = new StringBuilder();
                        foreach (UnitTrait u in rememberedUnit.UnitTraits)
                        {
                            sb.Append($"{UnitTraitSymbols[u]} ");
                        }

                        labelUnitTraits.Content = sb.ToString();
                    }
                    else
                    {
                        labelRemember.Content = "I don't know this Unit";
                        labelUnitTraits.Content = "-";
                    }
                    LastGuid = target.Guid;
                }
            }
        }

        /// <summary>
        /// This thing updates the UI... Note to myself: "may need to improve this thing in the future..."
        /// </summary>
        private void UpdateUI()
        {
            // TODO: find a better way to update this
            //AmeisenManager.Instance.GetObjects();

            Process currentProcess = Process.GetCurrentProcess();
            long memoryUsageMB = currentProcess.WorkingSet64 / 1000000;

            labelLoadedCombatClass.Content = $"{Path.GetFileName(Settings.combatClassPath)}.cs";
            labelLoadedCombatClassC.Content = $"{BotManager.CurrentCombatClassName}";
            labelRaceClass.Content = $"{BotManager.Me.Race.ToString()} {BotManager.Me.Class.ToString()}";

            if (BotManager.Me != null)
            {
                try
                {
                    UpdateFSMViews();
                    UpdateMyViews();
                    UpdateGroupViews();

                    if (BotManager.Target != null)
                    {
                        UpdateTargetViews();
                    }
                }
                catch (Exception e)
                {
                    AmeisenLogger.Instance.Log(LogLevel.ERROR, e.ToString(), this);
                }
            }
        }
    }
}