using System;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace AmeisenBotUtilities
{
    public static class Utils
    {
        /// <summary>
        /// Decode base64 image to BitmapImage
        /// </summary>
        /// <param name="base64String">input base64 image string</param>
        /// <returns>BitmapImage</returns>
        public static BitmapImage Base64ToBitmapImage(string base64String, bool compressionUsed = false)
        {
            if (base64String == "")
            {
                return null;
            }

            byte[] imageBytes = Convert.FromBase64String(base64String);

            if (compressionUsed)
            {
                imageBytes = GZipDecompressBytes(imageBytes);
            }

            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
        }

        /// <summary>
        /// Convert a byte[] to string like this "0xFF 0xAA 0x1F"
        /// </summary>
        /// <param name="inputBytes">input byte[]</param>
        /// <returns>byte[] as string</returns>
        public static string ByteArrayToString(byte[] inputBytes)
        {
            StringBuilder hex = new StringBuilder(inputBytes.Length * 2);
            foreach (byte b in inputBytes)
            {
                hex.AppendFormat("0x{0:x2} ", b);
            }

            return hex.ToString();
        }

        /// <summary>
        /// Make the first char of a string Uppercase
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>output string</returns>
        public static string FirstCharToUpper(string input)
        {
            if (input.Length > 1)
            {
                return input?.First().ToString().ToUpper() + input?.Substring(1);
            }
            else
            {
                return input.ToUpper();
            }
        }

        public static string FloatToHex32(float input)
        {
            StringBuilder result = new StringBuilder();
            byte[] bytes = BitConverter.GetBytes(input);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            for (int i = 0; i < bytes.Length; i++)
            {
                result.Append(bytes[i].ToString("X2"));
            }

            return result.ToString();
        }

        /// <summary>
        /// Generate a random string with the given size out of the given chars
        /// </summary>
        /// <param name="lenght">string lenght</param>
        /// <param name="chars">chars to chose from</param>
        /// <returns>random string</returns>
        public static string GenerateRandonString(int lenght, string chars)
        {
            Random rnd = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lenght; i++)
            {
                sb.Append(chars[rnd.Next(0, chars.Length - 1)]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Get the distance between two Vector3 positions
        /// </summary>
        /// <param name="a">position A</param>
        /// <param name="b">position A</param>
        /// <returns>Distance between the two Positions</returns>
        public static double GetDistance(Vector3 a, Vector3 b)
            => Math.Sqrt((a.X - b.X) * (a.X - b.X) +
                         (a.Y - b.Y) * (a.Y - b.Y) +
                         (a.Z - b.Z) * (a.Z - b.Z));

        public static float GetFacingAngle(Vector3 myPosition, float myRotation, Vector3 targetPosition)
        {
            float angle = (float)Math.Atan2(targetPosition.Y - myPosition.Y, targetPosition.X - myPosition.X);

            if (angle < 0.0f)
            {
                angle = angle + (float)Math.PI * 2.0f;
            }
            else if (angle > (float)Math.PI * 2)
            {
                angle = angle - (float)Math.PI * 2.0f;
            }

            return angle;
        }

        /// <summary>
        /// Compress bytes using GZip
        /// </summary>
        /// <param name="bytesToCompress">byte[] to compress using GZip</param>
        /// <returns>GZip-Compressed byte[]</returns>
        public static byte[] GZipCompressBytes(byte[] rawInput)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory,
                    CompressionMode.Compress, true))
                {
                    gzip.Write(rawInput, 0, rawInput.Length);
                }
                return memory.ToArray();
            }
        }

        /// <summary>
        /// Decompress a GZip-Compressed byte[]
        /// </summary>
        /// <param name="compressedBytes">GZip-Compressed byte[]</param>
        /// <returns>Decompressed byte[]</returns>
        public static byte[] GZipDecompressBytes(byte[] compressedInput)
        {
            using (GZipStream stream = new GZipStream(new MemoryStream(compressedInput), CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }

        /// <summary>
        /// Convert an image to byte[]
        /// </summary>
        /// <param name="img">input image</param>
        /// <returns>bytes of the input image</returns>
        public static byte[] ImageToByte(Image img, bool compressionUsed = false)
        {
            byte[] imageBytes = (byte[])new ImageConverter().ConvertTo(img, typeof(byte[]));

            if (compressionUsed)
            {
                imageBytes = GZipCompressBytes(imageBytes);
            }

            return imageBytes;
        }

        /// <summary>
        /// Create Colorfading effect between 2 or more Colors
        /// </summary>
        /// <param name="colors">colors to fade between</param>
        /// <param name="factor">fading factor 0.0 - 1.0</param>
        /// <returns>faded color</returns>
        public static System.Windows.Media.Color InterpolateColors(System.Windows.Media.Color[] colors, double factor)
        {
            double r = 0.0, g = 0.0, b = 0.0, a = 0.0;
            double total = 0.0;
            double step = 1.0 / (colors.Length - 1);
            double mu = 0.0;
            double sigma_2 = 0.035;

            foreach (System.Windows.Media.Color color in colors)
            {
                total += Math.Exp(-(factor - mu) * (factor - mu) / (2.0 * sigma_2)) / Math.Sqrt(2.0 * Math.PI * sigma_2);
                mu += step;
            }

            mu = 0.0;
            foreach (System.Windows.Media.Color color in colors)
            {
                double percent = Math.Exp(-(factor - mu) * (factor - mu) / (2.0 * sigma_2)) / Math.Sqrt(2.0 * Math.PI * sigma_2);
                mu += step;

                a += color.A * percent / total;
                r += color.R * percent / total;
                g += color.G * percent / total;
                b += color.B * percent / total;
            }

            return System.Windows.Media.Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
        }

        /// <summary>
        /// Simple facing check (70% - 130% rotation)
        /// </summary>
        /// <param name="myPosition">My Position</param>
        /// <param name="myRotation">My rotaion</param>
        /// <param name="targetPosition">Targte position</param>
        /// <returns>true if the target is in our view area, false if not</returns>
        public static bool IsFacing(Vector3 myPosition, float myRotation, Vector3 targetPosition, double minRotation, double maxRotation)
        {
            float f = GetFacingAngle(myPosition, myRotation, targetPosition);

            return (f >= (myRotation * minRotation)) && (f <= (myRotation * maxRotation)) ? true : false;
        }

        /// <summary>
        /// Tries to parse an int, if it fails returns -1
        /// </summary>
        /// <param name="intToParse">int to try parse</param>
        /// <returns>parsed int or -1 if it failed</returns>
        public static int TryParseInt(string intToParse)
            => int.TryParse(intToParse, out int value) ? value : -1;
    }
}