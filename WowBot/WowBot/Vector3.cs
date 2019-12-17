using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WowBot
{
	class Vector3
	{
		public float x;
		public float y;
		public float z;

		public Vector3() { }

		public Vector3(float x, float y, float z)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public override string ToString()
		{
			return x + ", " + y + ", " + z;
		}

		public static Vector3 operator+ (Vector3 left, Vector3 right)
		{
			return new Vector3(left.x + right.x, left.y + right.y, left.z + right.z);
		}
	}
}
