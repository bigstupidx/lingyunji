namespace xys
{
    // CD的类型
    public enum CDType
    {
        Item = 1, // 道具
        Skill = 2, // 技能
        PackageArrange = 3, // 整理包裹
        Title = 4, // 称号
        BloodPool = 5, // 血池
    }

    public static class CDUtils
    {
        public static int Combine(CDType type, short id)
        {
            return ((ushort)type << 16) | (ushort)id;
        }

        public static void Combine(int uid, out CDType type, out short id)
        {
            type = (CDType)(uid >> 16);
            id = (short)(uid & 0x0000ffff);
        }
    }
}