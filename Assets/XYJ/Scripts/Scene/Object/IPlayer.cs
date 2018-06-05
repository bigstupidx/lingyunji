namespace xys
{
    abstract public class IPlayer : ObjectBase
    {
        public IPlayer(int charSceneId) : base(NetProto.ObjectType.Player, charSceneId)
        {

        }
    }
}

