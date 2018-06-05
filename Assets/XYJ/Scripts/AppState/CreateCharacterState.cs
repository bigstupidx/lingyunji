#if !USE_HOT
using NetProto;
using NetProto.Hot;
using System.Collections;
using System.Collections.Generic;

namespace xys.hot
{
    class CreateCharacterState : AppStateBase
    {
        public CreateCharacterState(HotAppStateBase parent) : base(parent)
        {

        }

        C2GLoginRequest request = null;

        protected override void OnEnter(object p)
        {
            request = p as C2GLoginRequest;
            Event.Subscribe<CreateCharVo>(EventID.Login_CreateRole, CreateRole);
            Event.Subscribe(EventID.Login_EnterChoose, EnterChoose);
        }

        void CreateRole(CreateCharVo vo)
        {
            App.my.mainCoroutine.StartCoroutine(CreateCharacter(vo));
        }

        IEnumerator CreateCharacter(CreateCharVo vo)
        {
            CreateCharRequest ccr = new CreateCharRequest();
            ccr.name = vo.name;
            ccr.career = vo.jobId;
            ccr.sex = vo.sex;
            ccr.appearance = vo.appearance;
            var yyd = request.CreateCharacterYield(ccr);
            yield return yyd.yield;
            if (yyd.code != wProtobuf.RPC.Error.Success)
                yield break;

            if(yyd.result.result == CreateCharRespone.Result.CCRR_Name)
            {
                //重名
                xys.UI.SystemHintMgr.ShowTipsHint(3202);
            }

            if (yyd.result.result != CreateCharRespone.Result.CCRR_OK)
                yield break;

            parent.Level();
            EnterSelectCharacter esc = new EnterSelectCharacter();
            esc.request = request;
            esc.chars = new List<CharacterData>();
            esc.chars.Add(yyd.result.charData);
            App.my.appStateMgr.Enter(AppStateType.SelectCharacter, esc);

            UnityEngine.PlayerPrefs.SetString("loginCharId", yyd.result.charData.charid.ToString());
            Event.FireEvent(EventID.Login_SelectRole, yyd.result.charData);
        }

        void EnterChoose()
        {
            EnterSelectCharacter esc = new EnterSelectCharacter();
            esc.chars = null;
            esc.request = request;
            App.my.appStateMgr.Enter(AppStateType.SelectCharacter, esc);
        }
    }
}
#endif