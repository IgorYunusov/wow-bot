using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowBot.Utils
{
	static class CTM
	{

		public static void SetPosition(Vector3 pos)
		{
			Memory.Write((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_X, pos.x);
			Memory.Write((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_Y, pos.y);
			Memory.Write((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_Z, pos.z);
		}

		public static void SetGuid(ulong guid)
		{
			Memory.Write((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_GUID, guid);
		}

		public static void SetAction(CTMAction action)
		{
			Memory.Write((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_Action, (int)action);
		}
	}
}
