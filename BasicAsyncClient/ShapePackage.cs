using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BasicAsyncClient
{
    class ShapePackage
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public ShapePackage(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        // Serialize object
        public ShapePackage(byte[] data)
        {
            X = BitConverter.ToInt32(data, 0);
            Y = BitConverter.ToInt32(data, 1);
            Width = BitConverter.ToInt32(data, 2);
            Height = BitConverter.ToInt32(data, 3);
        }

        public byte[] ToByteArray()
        {
            List<byte> byteList = new List<byte>();
            byteList.AddRange(BitConverter.GetBytes(X));
            byteList.AddRange(BitConverter.GetBytes(Y));
            byteList.AddRange(BitConverter.GetBytes(Width));
            byteList.AddRange(BitConverter.GetBytes(Height));

            return byteList.ToArray();
        }

    }
}
