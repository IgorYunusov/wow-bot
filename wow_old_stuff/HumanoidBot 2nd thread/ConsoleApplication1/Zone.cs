using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiningBot {
    class Zone {
        private int id;
        private String text;
        private double x1;
        private double x2;
        private double y1;
        private double y2;

        private static int ZONE_ID = 0x00BD080C;
        private static Zone[] zones;    

        public Zone(int id, String text, double x1, double x2, double y1, double y2) {
            this.id = id;
            this.text = text;
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }

        public int getId() {
            return id;
        }

        public String getText() {
            return text;
        }

        public float getRelativeX(float x) {
            return (float)Math.Abs((x - x1) / (x2 - x1));
        }

        public float getRelativeY(float y) {
            return (float)Math.Abs((y - y1) / (y2 - y1));
        }

        public static Position getRelativePosition(float x, float y) {
            if(zones == null){
                createZones();
            }
            Zone zone = getZoneById(MemoryHandler.readInt(ZONE_ID));
            return new Position(zone.getRelativeX(x), zone.getRelativeY(y));
        }

        public string toString() {
            return id + ", " + text + ", " + x1 + ", " + x2 + ", " + y1 + ", " + y2;
        }

        private static void createZones() {
            zones = new Zone[108];
            int i = 0;
            String line;
            String[] values;

            System.IO.StreamReader file =
            new System.IO.StreamReader(@"..\..\resources\WorldMapArea.csv");
            while ((line = file.ReadLine()) != null) {
                values = line.Split(',');
                zones[i] = new Zone(int.Parse(values[0]), values[1],
                    Convert.ToDouble(values[2], System.Globalization.CultureInfo.InvariantCulture),
                    Convert.ToDouble(values[3], System.Globalization.CultureInfo.InvariantCulture),
                    Convert.ToDouble(values[4], System.Globalization.CultureInfo.InvariantCulture),
                    Convert.ToDouble(values[5], System.Globalization.CultureInfo.InvariantCulture));
                ++i;
            }
            file.Close();
        }

        private static Zone getZoneById(int id) {
            for (int i = 0; i < zones.Length; ++i) {
                if (zones[i].getId() == id) {
                    return zones[i];
                }
            }
            return null;
        }
    }
}
