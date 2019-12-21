using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowBot
{
	class RunHandler
	{
		Vector3 target = new Vector3();
		public Vector3 Target
		{
			get
			{
				target.x = Memory.ReadFloat((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_X);
				target.y = Memory.ReadFloat((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_Y);
				target.z = Memory.ReadFloat((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_Z);

				return target;
			}
		}

		Vector3 position = new Vector3();
		public Vector3 Position
		{
			get
			{
				position.x = Memory.ReadFloat((int)Coords.X);
				position.y = Memory.ReadFloat((int)Coords.Y);
				position.z = Memory.ReadFloat((int)Coords.Z);

				return position;
			}
		}

		public void MoveTo(Vector3 target)
		{
			Memory.Write((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_X, target.x);
			Memory.Write((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_Y, target.y);
			Memory.Write((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_Z, target.z);

			Memory.Write((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_Action, (int)CTMAction.Move);
		}

		public void Stop()
		{
			Memory.Write((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_Action, (int)CTMAction.Stop);
		}
	}
}
