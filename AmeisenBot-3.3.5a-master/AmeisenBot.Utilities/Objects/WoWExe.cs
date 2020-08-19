using System.Diagnostics;

namespace AmeisenBotUtilities
{
    public class WowExe
    {
        public bool alreadyHooked;
        public string characterName;
        public Process process;

        public override string ToString() => $"{process.Id.ToString()} - {characterName} {(alreadyHooked ? "- In Use" : "")}";
    }
}