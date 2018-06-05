namespace Config
{
    public partial class StandardReward : IParam
    {
        public int level { get { return id; } }

        public int this[string name]
        {
            get
            {
                switch (name)
                {
                case "基础经验": return exp;
                case "基础修为": return xiuwei;
                case "修为丹奖励": return pill;
                case "基础银贝": return silver;
                case "基础宠物经验": return petexp;
                }

                throw new System.Collections.Generic.KeyNotFoundException(name);
            }
        }
    }
}


