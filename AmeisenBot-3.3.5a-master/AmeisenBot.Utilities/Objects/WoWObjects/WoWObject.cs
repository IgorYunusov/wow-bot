using Magic;
using System.Text;

namespace AmeisenBotUtilities
{
    public class WowObject
    {
        public Vector3 pos;

        public WowObject(uint baseAddress, BlackMagic blackMagic)
        {
            BaseAddress = baseAddress;
            BlackMagicInstance = blackMagic;

            try
            {
                Descriptor = BlackMagicInstance.ReadUInt(BaseAddress + 0x8);
                Guid = BlackMagicInstance.ReadUInt64(BaseAddress + 0x30);
            }
            catch { }
        }

        public uint BaseAddress { get; set; }
        public BlackMagic BlackMagicInstance { get; set; }
        public uint Descriptor { get; set; }
        public double Distance { get; set; }
        public ulong Guid { get; set; }
        public int MapId { get; set; }
        public string Name { get; set; }
        public float Rotation { get; set; }
        public int ZoneId { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("WOWOBJECT");
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

            return sb.ToString();
        }

        public virtual void Update()
        {
        }
    }
}