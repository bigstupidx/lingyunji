namespace xys
{
    using NetProto;
    using System.Collections;
    using System.Collections.Generic;

    class GameInState : AppStateBase
    {
        public GameInState() : base(AppStateType.GameIn)
        {

        }

        protected override void OnEnter(object p)
        {
            ChangeSceneData data = p as ChangeSceneData;
            App.my.uiSystem.ShowPanel("UIMainPanel");
            ZoneType zt; int auid; ushort serverid; ushort mapid;
            Common.Utility.Zone(data.zoneId, out zt, out auid, out serverid, out mapid);
            
            App.my.eventSet.FireEvent<long>(EventID.Level_Start, data.zoneId);
            App.my.mainCoroutine.StartCoroutine(WaitPrepared());
        }

        IEnumerator WaitPrepared()
        {
            yield return 0;
            App.my.eventSet.fireEvent(EventID.Level_Prepared);
        }
    }
}