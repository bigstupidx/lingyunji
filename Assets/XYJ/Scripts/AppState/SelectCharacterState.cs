#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using NetProto.Hot;
    using System.Collections;
    using System.Collections.Generic;

    class EnterSelectCharacter
    {
        public C2GLoginRequest request;
        public List<CharacterData> chars;
    }

    class SelectCharacterState : AppStateBase
    {
        public SelectCharacterState(HotAppStateBase parent) : base(parent)
        {

        }

        List<CharacterData> Characters;
        C2GLoginRequest request;

        // 进入此状态
        protected override void OnEnter(object p)
        {
            var esc = p as EnterSelectCharacter;
            Characters = esc.chars;
            request = esc.request;

            Event.Subscribe(EventID.OfflineGate, OnGateOffline);
            Event.Subscribe<CharacterData>(EventID.Login_SelectRole, SelectRole);
            Event.Subscribe<CharacterData>(EventID.Login_DeleteRole, DeleteRole);
            Event.Subscribe<CharacterData>(EventID.Login_WaitDeleteRole, WaitDeleteRole);
            Event.Subscribe<CharacterData>(EventID.Login_RestoreRole, RestoreRole);
            Event.Subscribe(EventID.Login_EnterCreate, EnterCreate);
        }

        void SelectRole(CharacterData roleData)
        {
            App.my.mainCoroutine.StartCoroutine(Select(roleData));
        }

        void DeleteRole(CharacterData roleData)
        {
            App.my.mainCoroutine.StartCoroutine(Delete(roleData));
        }

        void WaitDeleteRole(CharacterData roleData)
        {
            App.my.mainCoroutine.StartCoroutine(WaitDelete(roleData));
        }

        void RestoreRole(CharacterData roleData)
        {
            App.my.mainCoroutine.StartCoroutine(Restore(roleData));
        }

        void EnterCreate()
        {
            App.my.appStateMgr.Enter(AppStateType.CreateCharacter, request);
        }

        protected IEnumerator Select(CharacterData sd)
        {
            //这个必须提前设置，不然没发保证执行顺序LocalPlayer.BeginChangeScene
            //必须保证没返回之前不能再执行
            var localPlayer = App.my.localPlayer;
            localPlayer.BeginEnter(sd);
            var yyd = request.SelectCharacterYield(new sFixed64() { value = sd.charid });
            yield return yyd.yield;
            if (yyd.code != wProtobuf.RPC.Error.Success)
                yield break;

            if (yyd.result.result != SelectCharacterRespone.Result.SCRR_OK)
            {
                Debuger.ErrorLog("SelectCharacterYield:{0}", yyd.result.result);
                yield break;
            }
        }

        protected IEnumerator Delete(CharacterData sd)
        {
            var yyd = request.DeleteCharacterYield(new Int64() { value = sd.charid });
            yield return yyd.yield;
            if (yyd.code != wProtobuf.RPC.Error.Success)
                yield break;

            if(!yyd.result.value)
            {
                Debuger.ErrorLog("删除角色失败" + sd.charid);
                yield break;
            }

            //删除成功，刷新角色列表
            Event.FireEvent<long>(EventID.Login_DeleteRoleRet, sd.charid);
        }

        protected IEnumerator WaitDelete(CharacterData sd)
        {
            var yyd = request.WaitDeleteCharacterYield(new Int64() { value = sd.charid });
            yield return yyd.yield;

            if (yyd.code != wProtobuf.RPC.Error.Success)
                yield break;

            if (!yyd.result.value)
            {
                Debuger.ErrorLog("延迟删除角色失败" + sd.charid);
                yield break;
            }

            //延迟删除成功，刷新角色列表
            Event.FireEvent<long>(EventID.Login_WaitDeleteRoleRet, sd.charid);
        }

        protected IEnumerator Restore(CharacterData sd)
        {
            var yyd = request.RestoreDeleteCharacterYield(new Int64() { value = sd.charid });
            yield return yyd.yield;

            if (yyd.code != wProtobuf.RPC.Error.Success)
                yield break;

            if (!yyd.result.value)
            {
                Debuger.ErrorLog("恢复角色失败" + sd.charid);
                yield break;
            }

            //恢复成功，刷新角色列表
            Event.FireEvent<long>(EventID.Login_RestoreRoleRet, sd.charid);
        }

        // 网关掉线了
        protected void OnGateOffline()
        {

        }
    }
}
#endif