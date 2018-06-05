namespace Config
{
    public struct LevelPos
    {
        public ushort levelId { get; private set; }
        public string pointId { get; private set; }

        public static LevelPos InitConfig(string text)
        {
            LevelPos levelPos = new LevelPos();
            string[] temp = text.Split('|');
            if(2 == temp.Length)
            {
                ushort id;
                if(ushort.TryParse(temp[0], out id))
                {
                    levelPos.levelId = id;
                    levelPos.pointId = temp[1];
                }
            }
            return levelPos;
        }
    }

    //选取位置方式
    public enum SelectPosType
    {
        Order,
        Random,
    }

    //选取怪物方式
    public enum SelectNpcType
    {
        Order,
        Random,
    }

    public partial class NpcSpawn
    {

    }
}
