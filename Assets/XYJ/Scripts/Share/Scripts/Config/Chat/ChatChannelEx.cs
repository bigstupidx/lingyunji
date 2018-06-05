namespace Config
{
    public partial class ChatChannel
    {
        public int GetConsumeItemId()
        {
            return string.IsNullOrEmpty(items) ? -1 : int.Parse(items.Split('|')[0]);
        }

        public int GetConsumeItemNum()
        {
            return string.IsNullOrEmpty(items) ? -1 : int.Parse(items.Split('|')[1]);
        }
    }
}

