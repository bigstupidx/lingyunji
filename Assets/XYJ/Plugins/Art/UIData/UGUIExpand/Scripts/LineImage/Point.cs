using System;

namespace UI
{
    [Serializable]
    public struct Point
    {
        public short x;
        public short y;
        public Point(short x, short y)
        {
            this.x = x;
            this.y = y;
        }

        public short this[int index]
        {
            get
            {
                switch (index)
                {
                case 0:
                    return this.x;

                case 1:
                    return this.y;
                }
                throw new IndexOutOfRangeException("Invalid Vector2 index!");
            }
            set
            {
                switch (index)
                {
                case 0:
                    this.x = value;
                    break;

                case 1:
                    this.y = value;
                    break;

                default:
                    throw new IndexOutOfRangeException("Invalid Vector2 index!");
                }
            }
        }
        public void Set(short new_x, short new_y)
        {
            this.x = new_x;
            this.y = new_y;
        }

        public override int GetHashCode()
        {
            return (((ushort)x) << 16) | ((ushort)y);
        }

        public override string ToString()
        {
            return string.Format("({0},{1})", x, y);
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == GetHashCode();
        }

        public static bool operator ==(Point lhs, Point rhs)
        {
            return lhs.GetHashCode() == rhs.GetHashCode();
        }

        public static bool operator !=(Point lhs, Point rhs)
        {
            return lhs.GetHashCode() != rhs.GetHashCode();
        }
    }
}