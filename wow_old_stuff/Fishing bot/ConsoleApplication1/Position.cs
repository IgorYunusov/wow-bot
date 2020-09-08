using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FishingBot {
    class Position {
        public float x;
        public float y;

        public Position(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(System.Object obj) {
            if (obj == null) {
                return false;
            }

            Position p = obj as Position;
            if ((System.Object)p == null) {
                return false;
            }

            // Return true if the fields match:
            return (x == p.x) && (y == p.y);
        }

        public float distance(Position other) {
            float deltaX = x - other.x;
            float deltaY = y - other.y;

            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        public float distance(float othX, float othY) {
            float deltaX = x - othX;
            float deltaY = y - othY;

            return (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}
