using AmeisenBotUtilities;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;

namespace AmeisenBot.Map
{
    public class AmeisenMap
    {
        public List<WowObject> ActiveUnits { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }

        private readonly Brush backgroundBrush = new SolidBrush(Color.FromArgb(255, 220, 220, 220));
        private readonly Brush unitBrush = new SolidBrush(Color.FromArgb(255, 255, 173, 65));
        private readonly Brush playerBrush = new SolidBrush(Color.FromArgb(255, 66, 138, 255));
        private readonly Brush meBrush = new SolidBrush(Color.FromArgb(255, 255, 65, 65));

        public AmeisenMap(int initialSizeX, int initialSizeY)
        {
            SizeX = initialSizeX;
            SizeY = initialSizeY;

            ActiveUnits = new List<WowObject>();
        }

        public BitmapImage GenerateBitmap(Me me, bool drawBackground = true)
        {
            BitmapImage bitmapImageMap = new BitmapImage();

            using (Bitmap bitmap = new Bitmap(SizeX, SizeY))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    if (drawBackground) { DrawBackground(graphics); }

                    if (ActiveUnits.Count > 0) { DrawUnits(graphics, me); }

                    graphics.FillRectangle(
                        meBrush,
                        new Rectangle(SizeX / 2, SizeY / 2, 1, 1));
                }

                using (MemoryStream memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Png);
                    memory.Position = 0;
                    bitmapImageMap.BeginInit();
                    bitmapImageMap.StreamSource = memory;
                    bitmapImageMap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImageMap.EndInit();
                }
            }

            return bitmapImageMap;
        }

        private void DrawUnits(Graphics graphics, Me me)
        {
            Brush activeBrush = unitBrush;
            foreach (Unit u in ActiveUnits)
            {
                if (u.GetType() == typeof(Player))
                {
                    activeBrush = playerBrush;
                }
                else if (u.GetType() == typeof(Unit))
                {
                    activeBrush = unitBrush;
                }
                else { continue; }

                int mapX = (int)(me.pos.X - u.pos.X);
                int mapY = (int)(me.pos.Y - u.pos.Y);

                if (IsPositionInBoundaries(me, mapX, mapY))
                {
                    graphics.FillRectangle(
                        activeBrush,
                        new Rectangle(mapX, mapY, 1, 1));
                }
            }
        }

        private void DrawBackground(Graphics graphics)
        {
            for (int x = 0; x < SizeX; x++)
            {
                for (int y = 0; y < SizeY; y++)
                {
                    graphics.FillRectangle(
                        backgroundBrush,
                        new Rectangle(x, y, 1, 1));
                }
            }
        }

        public bool IsPositionInBoundaries(Me me, int x, int y)
        {
            int xOuterBoudary = (int)me.pos.X + (SizeX / 2);
            int xInnerBoudary = (int)me.pos.X - (SizeX / 2);
            int yOuterBoudary = (int)me.pos.Y + (SizeY / 2);
            int yInnerBoudary = (int)me.pos.Y - (SizeY / 2);

            return x > xInnerBoudary
                && x < xOuterBoudary
                && y > yInnerBoudary
                && y < yOuterBoudary;
        }
    }
}
