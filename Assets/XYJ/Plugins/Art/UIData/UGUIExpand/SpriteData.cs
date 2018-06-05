using System.IO;
using UnityEngine;

namespace xys.UI
{
    public class SpriteData
    {
        public string name { get; private set; }
        public short width { get; private set; }
        public short height { get; private set; }

        public int area
        {
            get { return width * height; }
        }

        public void Init(Sprite s)
        {
            name = s.name;
            var size = s.rect.size;
            width = (short)size.x;
            height = (short)size.y;
        }

        public void Init(string n, short w, short h)
        {
            name = n;
            width = w;
            height = h;
        }

        public void Read(BinaryReader reader)
        {
            name = reader.ReadString();
            width = reader.ReadInt16();
            height = reader.ReadInt16();
        }

        public void Write(BinaryWriter write)
        {
            write.Write(name);
            write.Write(width);
            write.Write(height);
        }
    }
}