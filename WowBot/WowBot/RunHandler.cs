using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowBot
{
	class RunHandler
	{
		public Vector3 Target { get; private set; } = new Vector3();
		public Vector3 Position { get; private set; } = new Vector3();

		public void Update()
		{
			Target.x = Memory.ReadFloat((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_X);
			Target.y = Memory.ReadFloat((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_Y);
			Target.z = Memory.ReadFloat((int)ClickToMove.CTM_Base + (int)ClickToMove.CTM_Z);

			Position.x = Memory.ReadFloat((int)Coords.X);
			Position.y = Memory.ReadFloat((int)Coords.Y);
			Position.z = Memory.ReadFloat((int)Coords.Z);
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
