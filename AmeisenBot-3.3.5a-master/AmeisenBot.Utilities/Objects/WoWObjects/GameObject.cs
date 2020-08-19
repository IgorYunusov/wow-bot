﻿using Magic;
using System.Text;

namespace AmeisenBotUtilities
{
    public class GameObject : WowObject
    {
        public GameObject(uint baseAddress, BlackMagic blackMagic) : base(baseAddress, blackMagic)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("GAMEOBJECT");
            sb.Append($" >> Address: {BaseAddress.ToString("X")}");
            sb.Append($" >> Name: {Name}");
            sb.Append($" >> GUID: {Guid}");
            sb.Append($" >> PosX: {pos.X}");
            sb.Append($" >> PosY: {pos.Y}");
            sb.Append($" >> PosZ: {pos.Z}");
            sb.Append($" >> Rotation: {Rotation}");
            sb.Append($" >> Distance: {Distance}");
            sb.Append($" >> MapID: {MapId}");
            sb.Append($" >> ZoneID: {ZoneId}");

            return sb.ToString();
        }

        public override void Update()
        {
            base.Update();

            /*pos.x = BlackMagicInstance.ReadFloat(BaseAddress + 0x3C);
            pos.y = BlackMagicInstance.ReadFloat(BaseAddress + 0x40);
            pos.z = BlackMagicInstance.ReadFloat(BaseAddress + 0x44);
            Rotation = BlackMagicInstance.ReadFloat(BaseAddress + 0x28);*/

            // too cpu heavy
            /*try
            {
                distance = Utils.GetDistance(pos, AmeisenManager.GetInstance().Me().pos);
            }
            catch { }*/
        }
    }
}