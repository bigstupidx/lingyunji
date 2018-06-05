namespace xys
{
    // CD������
    public enum CDType
    {
        Item = 1, // ����
        Skill = 2, // ����
        PackageArrange = 3, // �������
        Title = 4, // �ƺ�
        BloodPool = 5, // Ѫ��
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