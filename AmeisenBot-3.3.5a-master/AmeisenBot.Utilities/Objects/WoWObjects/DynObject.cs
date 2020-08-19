using Magic;
using System.Text;

namespace AmeisenBotUtilities
{
    public class DynObject : WowObject
    {
        public DynObject(uint baseAddress, BlackMagic blackMagic) : base(baseAddress, blackMagic)
        {
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("DYNOBJECT");
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

            try
            {
                pos.X = BlackMagicInstance.ReadFloat(BaseAddress + 0x798);
                pos.Y = BlackMagicInstance.ReadFloat(BaseAddress + 0x79C);
                pos.Z = BlackMagicInstance.ReadFloat(BaseAddress + 0x7A0);
                Rotation = BlackMagicInstance.ReadFloat(BaseAddress + 0x7A8);
            }
            catch { }
        }
    }
}