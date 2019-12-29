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

		public static float Distance(Vector3 v1, Vector3 v2)
		{
			double xComponent = Math.Pow((v1.x - v2.x), 2.0);
			double yComponent = Math.Pow((v1.y - v2.y), 2.0);
			double zComponent = Math.Pow((v1.z - v2.z), 2.0);

			return (float)Math.Sqrt(xComponent + yComponent + zComponent);
		}
	}
}
