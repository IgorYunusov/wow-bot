using AmeisenBotUtilities.Enums;
using Magic;
using System.Collections.Generic;
using System.Text;

namespace AmeisenBotUtilities
{
    public class Me : Player
    {
        public Me(uint baseAddress, BlackMagic blackMagic) : base(baseAddress, blackMagic)
        {
            Update();
        }

        public int AreaId { get; set; }
        public WowClass Class { get; set; }
        public UnitState CurrentState { get; set; }
        public int Exp { get; set; }
        public int MaxExp { get; set; }
        public ulong PartyleaderGuid { get; set; }
        public List<ulong> PartymemberGuids { get; set; }
        public ulong PetGuid { get; set; }
        public uint PlayerBase { get; set; }
        public WowRace Race { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("ME");
            sb.Append($" >> Address: {BaseAddress.ToString("X")}");
            sb.Append($" >> Descriptor: {Descriptor.ToString("X")}");
            sb.Append($" >> Name: {Name}");
            sb.Append($" >> GUID: {Guid}");
            sb.Append($" >> PosX: {pos.X}");
            sb.Append($" >> PosY: {pos.Y}");
            sb.Append($" >> PosZ: {pos.Z}");
            sb.Append($" >> Rotation: {Rotation}");
            sb.Append($" >> Distance: {Distance}");
            sb.Append($" >> MapID: {MapId}");
            sb.Append($" >> ZoneID: {ZoneId}");

            if (TargetGuid != 0)
            {
                sb.Append($" >> TargetGUID: {TargetGuid.ToString()}");
            }
            else
            {
                sb.Append(" >> Target: none");
            }

            sb.Append($" >> currentState: {CurrentState}");
            sb.Append($" >> level: {Level}");
            sb.Append($" >> health: {Health}");
            sb.Append($" >> maxHealth: {MaxHealth}");
            sb.Append($" >> energy: {Mana}");
            sb.Append($" >> maxEnergy: {MaxMana}");

            sb.Append($" >> exp: {Exp}");
            sb.Append($" >> maxExp: {MaxExp}");

            sb.Append($" >> partyLeader: {PartyleaderGuid}");

            int count = 1;
            foreach (ulong guid in PartymemberGuids)
            {
                sb.Append($" >> partymember{count}: {guid}");
                count++;
            }
            return sb.ToString();
        }

        public override void Update()
        {
            base.Update();

            try
            {
                if (PlayerBase == 0)
                {
                    PlayerBase = BlackMagicInstance.ReadUInt(Offsets.playerBase);
                    PlayerBase = BlackMagicInstance.ReadUInt(PlayerBase + 0x34);
                    PlayerBase = BlackMagicInstance.ReadUInt(PlayerBase + 0x24);
                }

                Race = (WowRace)BlackMagicInstance.ReadByte(Offsets.playerRace);
                Class = (WowClass)BlackMagicInstance.ReadByte(Offsets.playerClass);

                Name = BlackMagicInstance.ReadASCIIString(Offsets.playerName, 12);
                Exp = BlackMagicInstance.ReadInt(PlayerBase + 0x3794);
                MaxExp = BlackMagicInstance.ReadInt(PlayerBase + 0x3798);

                // Somehow this is really sketchy, need to replace this...
                //uint castingState = BlackMagicInstance.ReadUInt((uint)BlackMagicInstance.MainModule.BaseAddress + Offsets.localPlayerCharacterState);
                //castingState = BlackMagicInstance.ReadUInt(castingState + Offsets.localPlayerCharacterStateOffset1);
                //castingState = BlackMagicInstance.ReadUInt(castingState + Offsets.localPlayerCharacterStateOffset2);
                //CurrentState = (UnitState)BlackMagicInstance.ReadInt(castingState + Offsets.localPlayerCharacterStateOffset3);

                TargetGuid = BlackMagicInstance.ReadUInt64(Descriptor + 0x48);
                PetGuid = BlackMagicInstance.ReadUInt64(Offsets.petGuid);

                PartymemberGuids = new List<ulong>();
                //PartyleaderGuid = BlackMagicInstance.ReadUInt64(Offsets.partyLeader);
                PartyleaderGuid = BlackMagicInstance.ReadUInt64(Offsets.raidLeader);

                PartymemberGuids.Add(BlackMagicInstance.ReadUInt64(Offsets.partyPlayer1));
                PartymemberGuids.Add(BlackMagicInstance.ReadUInt64(Offsets.partyPlayer2));
                PartymemberGuids.Add(BlackMagicInstance.ReadUInt64(Offsets.partyPlayer3));
                PartymemberGuids.Add(BlackMagicInstance.ReadUInt64(Offsets.partyPlayer4));

                // try to add raidmembers
                for (uint p = 0; p < 40; p++)
                {
                    try
                    {
                        uint address = Offsets.raidGroupStart + (p * Offsets.raidPlayerOffset);
                        ulong guid = BlackMagicInstance.ReadUInt64(address);
                        if (!PartymemberGuids.Contains(guid))
                        {
                            PartymemberGuids.Add(guid);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}