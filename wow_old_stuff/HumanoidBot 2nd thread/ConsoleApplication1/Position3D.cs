using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningBot {
    class Position3D {
        public float x;
        public float y;
        public float z;

        public Position3D(float x, float y, float z){
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Position to2DPosition() {
            return new Position(x, y);
        }

        public override string ToString() {
            return "x: " + x + ", y: " + y + ", z: " + z;
        }

        public override bool Equals(System.Object obj) {
            if (obj == null) {
                return false;
            }

            Position3D p = obj as Position3D;
            if ((System.Object)p == null) {
                return false;
            }
            //nem pontos egyezés kell, emrt nem mukodik jol és am sem kell oylan pontosan
            return ((int)x == (int)p.x) && ((int)y == (int)p.y) && ((int)z == (int)p.z);
        }

        public float distance(Position3D other) {
            float deltaX = x - other.x;
            float deltaY = y - other.y;
            float deltaZ = z - other.z;

            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }

        public float distance(float othX, float othY, float othZ) {
            float deltaX = x - othX;
            float deltaY = y - othY;
            float deltaZ = z - othZ;

            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }
    }
}
