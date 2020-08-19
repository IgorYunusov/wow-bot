﻿using Magic;
using System.Text;

namespace AmeisenBotUtilities
{
    public class Container : WowObject
    {
        public Container(uint baseAddress, BlackMagic blackMagic) : base(baseAddress, blackMagic)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("CONTAINER");
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
        }
    }
}