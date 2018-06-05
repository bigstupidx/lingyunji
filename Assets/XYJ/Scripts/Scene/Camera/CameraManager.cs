using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.Test;
using Config;

namespace xys
{
    public partial class CameraManager : MonoBehaviour
    {
        //当前主摄像机
        public Camera m_mainCamera;
        public enum CameraViewType
        {
            ThreeD,                 //3d镜头
            TwoPointFiveD,          //2.5d镜头
        }

        public enum CameraLockType
        {
            Null,                   //未锁定
            New,                    //新锁定
            Old,                    //旧锁定
        }

        //默认摄像机(如果场景设置了主摄像机则使用场景的)
        public Camera m_defaultCamera;

        public CameraViewType m_cameraViewType { get; protected set; }      //摄像机模式

        public Transform m_player;       //主角(公布出来方便编辑器调试)
        protected GameObject m_cameraOffset; //相机偏移
        protected Transform m_cameraRoot;   //通过该位置来移动和旋转摄像机
        protected GameObject m_playerCamera;    //角色摄像机

        public bool m_duringSkill;
        public bool m_specialCamera;
        public bool m_isScenarioLens;//是否是剧情的镜头

        private const int cullingMask = 1 << 5;//maincamera 不渲染ui层
        private const int m_depthCullingMassk = 1 << 5 | 1 << 11;  //景深模糊的时候不渲染ui以及player层

        private CameraLockType lockState = CameraLockType.Null;
        private string m_waitLockRefresh = "";
        private float m_screenPercentage = 1;

        //private int m_lockTimerHandle = -1;
        private float m_unlockTime = 5;
        private bool m_firstEnter = false;

        //CameraTodo
        //大招屏幕遮挡
        //public FxFadeAlpha m_stuntMaskObj;
        ////景深效果
        //DepthOfFieldDeprecated m_depthOfField;

        void Awake()
        {
            //CameraTodo
            //Utils.EventDispatcher.Instance.AddEventListener<LevelPrototype>(EventDefine.SceneEvents.InitFinishLoadScene, OnInitFinishLoadScene);
            //Utils.EventDispatcher.Instance.AddEventListener<BaseRole>(EventDefine.RoleEvents.G_FinishCreateMe, FinishCreateMe);
            //Utils.EventDispatcher.Instance.AddEventListener<BaseRole>(EventDefine.RoleEvents.G_FinishLoadModel, FinishLoadModel);
            //Utils.EventDispatcher.Instance.AddEventListener<bool, bool, bool>(EventDefine.RoleEvents.G_DoSkillCamera, DoSkillCamera);
            //Utils.EventDispatcher.Instance.AddEventListener<bool>(EventDefine.RoleEvents.G_DoSpecialCamera, DoSpecialCamera);
            //Utils.EventDispatcher.Instance.AddEventListener<Battle.BaseRole, int>(EventDefine.RoleEvents.G_ObjectBeginDead, OnDestroyDead);
            //Utils.EventDispatcher.Instance.AddEventListener(EventDefine.SceneEvents.FinishCg, OnFinishCg);
            //Utils.EventDispatcher.Instance.AddEventListener(EventDefine.SceneEvents.StartCg, OnStartCg);

            //Utils.EventDispatcher.Instance.AddEventListener(LoginSystem.Event.EnterGame, OnEnterGame);
            //Utils.EventDispatcher.Instance.AddEventListener(EventDefine.MainEvents.LoginScene, OnLoginScene);
            //Instance = this;

            m_cameraViewType = CameraViewType.ThreeD;                       //默认3d镜头
            m_mobileProp = m_screenPercentage = (float)Screen.width / 1334.0f;

            //ProFlareBatch.get_default_camera = () => { return Instance == null ? null : Instance.m_mainCamera; };
        }

        public EventAgent m_events = new EventAgent();
        void Start()
        {
            m_events.Subscribe<bool>(EventID.FinishLoadSceneParam, OnInitFinishLoadScene);
            m_events.Subscribe<object[]>(EventID.Camera_DoSkill, DoSkillCamera);
            //PackTool.PrefabLoad.Load("AimPoint", OnLoadAimPointFinsh, null);//由于allresource 在awake初始化，因此要load资源应该写在awake之后
        }

        void OnDestroy()
        {
            m_events.Release();
            //CameraTodo
            //Utils.EventDispatcher.Instance.RemoveEventListener<LevelPrototype>(EventDefine.SceneEvents.InitFinishLoadScene, OnInitFinishLoadScene);
            //Utils.EventDispatcher.Instance.RemoveEventListener<BaseRole>(EventDefine.RoleEvents.G_FinishCreateMe, FinishCreateMe);
            //Utils.EventDispatcher.Instance.RemoveEventListener<BaseRole>(EventDefine.RoleEvents.G_FinishLoadModel, FinishLoadModel);
            //Utils.EventDispatcher.Instance.RemoveEventListener<bool, bool, bool>(EventDefine.RoleEvents.G_DoSkillCamera, DoSkillCamera);
            //Utils.EventDispatcher.Instance.RemoveEventListener<bool>(EventDefine.RoleEvents.G_DoSpecialCamera, DoSpecialCamera);
            //Utils.EventDispatcher.Instance.RemoveEventListener<Battle.BaseRole, int>(EventDefine.RoleEvents.G_ObjectBeginDead, OnDestroyDead);
            //Utils.EventDispatcher.Instance.RemoveEventListener(EventDefine.SceneEvents.FinishCg, OnFinishCg);
            //Utils.EventDispatcher.Instance.RemoveEventListener(EventDefine.SceneEvents.StartCg, OnStartCg);

            //Utils.EventDispatcher.Instance.RemoveEventListener(LoginSystem.Event.EnterGame, OnEnterGame);
            //Utils.EventDispatcher.Instance.RemoveEventListener(EventDefine.MainEvents.LoginScene, OnLoginScene);
            //Instance = null;
        }

        //激活/关闭摄像机
        public void EnableCamera(bool enable)
        {
            if (m_mainCamera == null)
            {
                Debug.LogError("摄像机没有初始化");
                return;
            }
            m_duringSkill = !enable;
            m_specialCamera = !enable;
            m_mainCamera.gameObject.SetActive(enable);
        }

        //放在[Camera]目录下的摄像机都是不需要关闭的
        bool IsUseCamera(Transform trans)
        {
            while (trans.parent != null)
            {
                if (trans.parent.name == "[Camera]")
                    return true;
                trans = trans.parent;
            }
            return false;
        }

        //[Camera]下为要使用的摄像机，MainCamera为美术配置的主射相机
        //若有场景配置摄像机，则使用场景配置摄像机
        //若全部没有则使用程序的默认摄像机
        void OnInitFinishLoadScene(bool isSeamless)
        {
            if (isSeamless)
                return;

            //场景摄像机命名为 MainCamera
            Camera sceneCamera = null;

            //关卡摄像机可以通过配置表配置
            string sceneCameraName = "MainCamera";
            //CameraTodo
            //if (!string.IsNullOrEmpty(p.levelCamera))
            //    sceneCameraName = p.levelCamera;

            //场景可能会有摄像机忘记关了
            Camera[] allCamera = Camera.allCameras;
            for (int i = 0; i < allCamera.Length; i++)
            {
                Camera c = allCamera[i];
                //ui摄像机
                if (c.gameObject.layer == ComLayer.Layer_UI || c.gameObject.layer == ComLayer.Layer_RTT)
                    continue;

                //当前激活的非法摄像机全部隐藏
                if (!IsUseCamera(c.transform))
                {
                    c.gameObject.SetActive(false);
                }
            }

            GameObject cameraParent = GameObject.Find("[Camera]");
            if (null != cameraParent)
            {
                for (int i = 0; i < cameraParent.transform.childCount; i++)
                {
                    GameObject c = cameraParent.transform.GetChild(i).gameObject;

                    AudioListener audioListener = c.GetComponent<AudioListener>();
                    if (audioListener != null)
                        audioListener.enabled = false;

                    if (c.name == sceneCameraName)
                    {
                        c.SetActive(true);
                        sceneCamera = c.GetComponent<Camera>();
                    }
                    else
                    {
                        c.SetActive(false);
                    }
                }
            }

            //使用默认摄像机
            if (sceneCamera == null)
            {
                Debug.LogError("场景未配置主射相机或者关卡摄像机");
                m_defaultCamera.gameObject.SetActive(true);
                m_mainCamera = m_defaultCamera;
            }
            //使用场景摄像机
            else
            {
                m_defaultCamera.gameObject.SetActive(false);
                sceneCamera.gameObject.SetActive(true);
                //CameraTodo
                //if (!string.IsNullOrEmpty(p.levelCamera) && p.levelCameraType == LevelPrototypeManage.LevelCameraType.FixPosition)
                //{
                //    //场景固定摄像机
                //    m_mainCamera = m_defaultCamera;
                //    m_mainCamera.gameObject.SetActive(false);
                //}
                //else
                {
                    m_mainCamera = sceneCamera;
                }
            }

            //默认景深
            //CameraTodo
            //m_depthOfField = m_defaultCamera.GetComponent<DepthOfFieldDeprecated>();

            //InitCameraEffect(m_mainCamera);

            //EffectDepthOfField(false);

            //设置摄像机的一些参数
            SetCameraPara();

            m_cameraRoot = m_mainCamera.transform;
            m_cameraOffset = m_mainCamera.gameObject;
            m_mainCamera.fieldOfView = m_defaultFov;
            m_mainCamera.cullingMask = ~cullingMask;
            m_mainCamera.gameObject.tag = "MainCamera";

            //使用天空盒
            m_mainCamera.clearFlags = CameraClearFlags.Skybox;
            //角色创建和场景加载完的顺序不能保证
            //CameraTodo
            //if (ObjectManage.Instance.mainPlayer != null)
            //    FinishCreateMe(ObjectManage.Instance.mainPlayer);
            //不使用hdr
            m_mainCamera.allowHDR = false;

            //避免默认值0.01，导致一些奇怪的现象
            m_mainCamera.nearClipPlane = 0.25f;

            //m_oldLockTarget = null;

            InitCameraByPlayer();
        }

        //CameraTodo
        //void OnDestroyDead(BaseRole role, int type)
        //{
        //    if (role != null && role.m_modelManage.m_bodyTrans == m_lockTarget)
        //        SetLockEnemy(null);
        //    if (role != null && role == m_oldLockTarget)
        //        m_oldLockTarget = null;
        //}

        void SetCameraPara()
        {
            //CameraTodo
            m_targetAngle = kvClient.GetFloat("targetAngle", 3);
            m_targetHeight = kvClient.GetFloat("targetHeight", 1);
            m_playerHeight = kvClient.GetFloat("playerHeight", 1.8f);
            m_playerDistance = kvClient.GetFloat("playerDistance", 6.5f);
            m_normalCameraAngle = kvClient.GetFloat("normalCameraAngle", 8f);
            m_currFov = m_defaultFov = kvClient.GetFloat("defaultFov", 50f);
            m_norRotSpeed = kvClient.GetFloat("norRotSpeed", 5f);
            m_norPosSpeed = kvClient.GetFloat("norPosSpeed", 7f);
            m_lockRotSpeed = kvClient.GetFloat("lockRotSpeed", 0.5f);
            m_lockRotFixSpeed = kvClient.GetFloat("lockRotFixSpeed", 0.05f);
            m_lockPosSpeed = kvClient.GetFloat("lockPosSpeed", 13f);
            m_lockPosFixSpeed = kvClient.GetFloat("lockPosFixSpeed", 0f);
            m_lockMinDisToTarget = kvClient.GetFloat("lockMinDisToTarget", 5f);
            m_lockMaxDisToTarget = kvClient.GetFloat("lockMaxDisToTarget", 20f);
            m_lockMaxAngle = kvClient.GetFloat("lockMaxAngle", 30f);
            m_lockMinAngle = kvClient.GetFloat("lockMinAngle", -20f);
            m_speedToNor = kvClient.GetFloat("speedToNor", 0.4f);
            m_maxDis = kvClient.GetFloat("maxDis", 10f);
            m_minDis = kvClient.GetFloat("minDis", 2f);
            m_twoStageMinDis = kvClient.GetFloat("twoStageMinDis", 0.8f);
            m_twoStageHeight = kvClient.GetFloat("twoStageHeight", 1.475f);
            m_rotMinAngle = kvClient.GetFloat("rotMinAngle", -50f);
            m_rotMaxAngle = kvClient.GetFloat("rotMaxAngle", 60f);
            m_dragSpeed = kvClient.GetFloat("dragSpeed", 5f);
            m_dragOffset = kvClient.GetFloat("dragOffset", 0.3f);
            m_featureDis = kvClient.GetFloat("featureDis", 3.5f);
            m_25DFeatureDis = kvClient.GetFloat("TFDFeatureDis", 5.5f);
            m_featureHeight = kvClient.GetFloat("featureHeight", 1f);
            m_enterColliderSpeed = kvClient.GetFloat("enterColliderSpeed", 4f);
            m_25DMaxDis = kvClient.GetFloat("TFDMaxDis", 15f);
            m_25DMinDis = kvClient.GetFloat("TFDMinDis", 5f);
            m_2_5DPlayerDistance = kvClient.GetFloat("TFDPlayerDistance", 8f);
            m_2_5DCameraAngle = kvClient.GetFloat("TFDCameraAngle", 33f);
            m_TDCameraRotSpeed = kvClient.GetFloat("TDCameraRotSpeed", 20f);
            m_scaleSpeed = kvClient.GetFloat("scaleSpeed", 0.02f);
            m_npcCamDis = kvClient.GetFloat("npcCamDis", 2.5f);
            m_npcCamHeight = kvClient.GetFloat("npcCamHeight", 1.4f);
            m_returnToNorSpd = kvClient.GetFloat("returnToNorSpd", 5f);
            m_newUnlockTime = kvClient.GetInt("newUnlockTime", 8);
            m_oldUnlockDis = kvClient.GetFloat("oldUnlockDis", 30f);
            m_lockRangeMin = kvClient.GetFloat("lockRangeMin", 4f);
            m_lockRangeMax = kvClient.GetFloat("lockRangeMax", 15f);
            m_lockMaxDistance = kvClient.GetFloat("lockMaxDistance", 16f);

            m_tempDistance = m_playerDistance;
            m_noramlCamDis = m_playerDistance;
            m_tempHeight = m_playerHeight;
            m_temp25DDistance = m_2_5DPlayerDistance;
            m_tempMaxDis = m_maxDis;
            m_tempMinDis = m_minDis;
            m_tempNormalCameraAngle = m_normalCameraAngle;

            m_defaultPlayerHeight = m_playerHeight;
            m_defaultPlayerDistance = m_playerDistance;
            m_defaultTargetHeight = m_targetHeight;
            m_defaultLockMinAngle = m_lockMinAngle;

            m_isFeature = false;

            //SetCameraViewPara();
        }

        public void FinishCreateMe( Transform trans )
        {
            m_player = trans;
            InitCameraByPlayer();
        }

        //角色创建和场景加载完的顺序不能保证
        void InitCameraByPlayer()
        {
            if (m_cameraOffset == null || m_player == null)
                return;
            TransitionFinishCreateMe();
            InitDragPara();
        }

        //CameraTodo
        //void FinishLoadModel(BaseRole role)
        //{
        //    string refreshId = role.m_objectData.refreshId;
        //    if (!string.IsNullOrEmpty(m_waitLockRefresh) && refreshId == m_waitLockRefresh && m_isForceLock)
        //    {
        //        if (null != role)
        //        {
        //            SetOldLock(true, role);
        //        }
        //    }
        //}

        void DoSkillCamera(object[] param)
        {
            bool isDuringSkill, back, isScenarioLens;
            isDuringSkill = (bool)param[0];
            back = (bool)param[1];
            isScenarioLens = (bool)param[2];

            if (!isDuringSkill && m_cameraRoot != null && m_player != null)
            {
                //镜头返回的时候，设置一下距离
                m_noramlCamDis = Vector3.Distance(m_cameraRoot.position, m_player.position + Vector3.up * m_playerHeight);
                if (!IsLockToTarget() && back)
                {
                    Quaternion rotation = Quaternion.Euler(m_cameraRoot.transform.localEulerAngles.x, Quaternion.LookRotation(m_player.forward).eulerAngles.y, 0);
                    m_cameraRoot.transform.position = rotation * new Vector3(0.0f, 0.0f, -m_noramlCamDis) + m_followPos;
                }

                //设置摄像机角度
                m_toNorX = m_cameraRoot.transform.localEulerAngles.x;
            }
            m_duringSkill = isDuringSkill;
            m_isScenarioLens = isScenarioLens;

            //Debuger.LogError("m_duringSkill : " + m_duringSkill + " m_toNorX : " + m_toNorX);
        }

        void DoSpecialCamera(bool during)
        {
            m_specialCamera = during;
        }

        void LateUpdate()
        {
            if (m_mainCamera == null)
                return;
            TransitionUpdate();
            UpdateFieldOfView();
            //CameraTodo
            //ChooseTargetUpdate();
            UpdateSelectedTarget();
            UpdateToNormal();
            RotateCamera();
        }


        void Update()
        {
            if (
#if COM_DEBUG
/*!GM_UI.s_isLockMonster &&*/
#endif
            lockState != CameraLockType.Null)
            {
                //SetLockEnemy(null);
            }

            m_unlockTime -= Time.deltaTime;
            if (m_unlockTime <= 0 && !m_isForceLock)
            {
                //过一段时间解锁
                //SetLockEnemy(null);
            }

            //if (m_isForceLock == true && null != m_player && null != m_oldLockTarget)
            //{
                //计算距离
                //CameraTodo
                //float dis = GetDistance(m_player.position, m_oldLockTarget.transform.position);
                //if (dis > m_oldUnlockDis && lockState == CameraLockType.Old)
                //{
                //    //暂时解锁
                //    ResetLockMinAngle();
                //    ResetLockCameraPara();
                //    LockMeToTarget(null);
                //    lockState = CameraLockType.Null;
                //}
                //else if (dis <= m_oldUnlockDis && lockState == CameraLockType.Null && null != m_oldLockTarget && m_oldLockTarget.isAlive)
                //{
                //    //恢复锁定
                //    SetLockCameraPara(m_oldLockTarget.m_objPrototype.lockCameraPara);
                //    SetLockMinAngle(m_oldLockTarget.m_objPrototype.cameraLockAngle);
                //    LockMeToTarget(m_oldLockTarget.m_modelManage.m_bodyTrans);
                //    lockState = CameraLockType.Old;
                //}
            //}
        }

        //void OnGUI()
        //{
        //    //GUI.Label(new Rect(200, 200, 200, 50), "m_normalDistance : " + m_normalDistance.ToString() + " m_playerHeight : " + m_playerHeight
        //    //    + " m_playerDistance : " + m_playerDistance + " m_isTouchRotate : " + m_isTouchRotate);
        //}

        IEnumerator CloseDuringFeature()
        {
            yield return new WaitForSeconds(1.0f);
            m_duringScale = false;
            //m_x = Mathf.Infinity;
            //m_y = Mathf.Infinity;
            m_DuringType = DuringType.Null;
            m_ScrollWheel = Input.GetAxis("Mouse ScrollWheel");
        }

        IEnumerator CloseRotete()
        {
            yield return new WaitForSeconds(0.5f);
            m_duringRatate = false;
        }

        IEnumerator CloseExcessiveMove()
        {
            yield return new WaitForSeconds(1.0f);
            m_excessiveMove = false;
            ResetTDCameraRotSpeed();
        }

        void InitDragPara()
        {
            m_m = m_x = m_cameraRoot.transform.localEulerAngles.y;
            m_n = m_y = ClampAngle(m_cameraRoot.transform.localEulerAngles.x, m_rotMinAngle, m_rotMaxAngle);
        }

        //玩家锁定敌人,修改摄像机目标高度
        public void SetLockEnemy(IObject target)
        {
            if (target != null && target.isAlive)
            {
                //锁怪后倒计时，结束自动解锁
                m_unlockTime = m_newUnlockTime;

                //锁定同一个对象，无需执行下面操作了
                if (target.root == m_lockTarget)
                    return;

                //如果当前使用了旧锁定镜头，则跳过其他逻辑
                if (!m_isForceLock)
                {
                    //SetTargetHeight(target.m_objPrototype.cameraLockHeight);
                    //SetLockMinAngle(target.m_objPrototype.cameraLockAngle);
                    LockMeToTarget(target.root);
                    lockState = CameraLockType.New;
                }
            }
            else
            {
                if (!m_isForceLock)
                {
                    ResetLockMinAngle();
                    //ResetLockCameraPara();
                    LockMeToTarget(null);
                    lockState = CameraLockType.Null;
                    m_playerDistance = m_tempDistance;
                }

                //接触锁定的时候，关闭计时器
                //Utils.TimerManage.DelTimer(m_lockTimerHandle);
            }
        }

        //设置锁定
        IObject m_oldLockTarget;
        public void SetOldLock(bool doLock, IObject target)
        {
            //CameraTodo
            if (doLock)
            {
                m_isForceLock = true;   //开启强行锁定镜头

                //SetLockCameraPara(target.m_objPrototype.lockCameraPara);
                //SetLockMinAngle(target.m_objPrototype.cameraLockAngle);
                LockMeToTarget(target.root);
                lockState = CameraLockType.Old;
                m_oldLockTarget = target;
            }
            else
            {
                m_isForceLock = false;//关闭旧锁定镜头
                m_waitLockRefresh = "";

                ResetLockMinAngle();
                ResetLockCameraPara();
                LockMeToTarget(null);
                lockState = CameraLockType.Null;
                m_oldLockTarget = null;
            }
        }

        public void SetWaitLock(string refreshId)
        {
            m_waitLockRefresh = refreshId;
            m_isForceLock = true;
        }

        //过场动画播放完
        public void OnFinishCg()
        {

        }

        //开始播放过场动画
        void OnStartCg()
        {
            LockMeToTarget(null);
        }

        void OnEnterGame()
        {
            m_firstEnter = true;
        }

        void OnLoginScene()
        {
            m_firstEnter = true;
            m_player = null;
        }

        //设置摄像机角度到默认值，过场动画结束事件相关
        public void SetCameraToDefaultAngle()
        {
            m_toNorX = m_normalCameraAngle;
        }

        //设置当前模式
        public void ChangeCameraViewType()
        {
            if (m_cameraViewType == CameraViewType.ThreeD)
            {
                m_cameraViewType = CameraViewType.TwoPointFiveD;
                //停止3d镜头的过度
                m_excessiveMove = false;
            }
            else
            {
                m_cameraViewType = CameraViewType.ThreeD;
                SetExcessiveCamera(true);
            }
            SetCameraViewPara();
            //StopCoroutine("CloseExcessiveMove");
        }

        void SetCameraViewPara()
        {
            if (m_cameraViewType == CameraViewType.TwoPointFiveD)
            {
                //m_2_5DPlayerDistance = m_playerDistance;
                //m_tempColliderDistance = m_playerDistance = m_temp25DDistance;
                m_tempColliderDistance = m_2_5DPlayerDistance = m_temp25DDistance;
                m_maxDis = m_25DMaxDis;
                m_minDis = m_25DMinDis;
                m_x = m_cameraRoot.transform.localEulerAngles.y;
                //m_x = 0;
            }
            else
            {
                //m_toNorX = m_normalCameraAngle;
                m_playerDistance = m_tempDistance;
                m_playerHeight = m_tempHeight;
                m_maxDis = m_tempMaxDis;
                m_minDis = m_tempMinDis;
            }
        }

        //设置摄像机镜头
        //CameraTodo
        //public void SetNpcCam(bool isNpcCam, NpcRole npc)
        //{
        //    //进入景深的时候记录坐标
        //    if (!m_npcCam && isNpcCam)
        //    {
        //        m_TempRot = m_cameraRoot.transform.rotation;
        //        m_TempPos = m_cameraRoot.transform.position;
        //    }

        //    //如果当前进入特写，并且退出
        //    if (m_npcCam && !isNpcCam)
        //    {
        //        //返回普通镜头
        //        //SetExcessiveCamera(true);
        //        m_cameraRoot.transform.rotation = m_TempRot;
        //        m_cameraRoot.transform.position = m_TempPos;
        //        //退出景深
        //        EffectDepthOfField(false);
        //        if (null != npc)
        //        {
        //            SetChooseTarget(npc);
        //        }
        //    }

        //    if (m_npcCam == isNpcCam)
        //    {
        //        return;
        //    }
        //    m_npcCam = isNpcCam;
        //    m_npc = npc;
        //    if (m_npcCam)
        //    {
        //        //进入景深
        //        UpdateNormalCamera(m_player, true);
        //        EffectDepthOfField(true);
        //        ObjectManage.Instance.SetAllShowExceptRole(npc, false);

        //        Vector3 lookDir = (ObjectManage.Instance.mainPlayer.m_modelManage.transform.position - npc.m_modelManage.transform.position).normalized;
        //        Quaternion look = Quaternion.LookRotation(lookDir);
        //        npc.m_modelManage.transform.localEulerAngles = new Vector3(0, look.eulerAngles.y, 0);
        //        UI3D.TitleSystem.instance.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        if (null != m_player && m_player.gameObject != null)
        //            m_player.gameObject.SetActive(true);
        //        ObjectManage.Instance.SetAllShowExceptRole(m_npc, true);
        //        UI3D.TitleSystem.instance.gameObject.SetActive(true);
        //    }
        //}

        //是否进去展示镜头
        //CameraTodo
        //public void EnterShowCamera(bool showCam)
        //{
        //    //进入景深的时候记录坐标
        //    if (!m_showCam && showCam)
        //    {
        //        m_TempRot = m_cameraRoot.transform.rotation;
        //        m_TempPos = m_cameraRoot.transform.position;
        //    }

        //    //如果当前进入特写，并且退出
        //    if (m_showCam && !showCam)
        //    {
        //        //返回普通镜头
        //        m_cameraRoot.transform.rotation = m_TempRot;
        //        m_cameraRoot.transform.position = m_TempPos;
        //        //退出景深
        //        EffectDepthOfField(false);
        //    }

        //    if (m_showCam == showCam)
        //    {
        //        return;
        //    }
        //    m_showCam = showCam;
        //    if (m_showCam)
        //    {
        //        //进入景深
        //        UpdateNormalCamera(m_player, true);
        //        EffectDepthOfField(true);
        //        ObjectManage.Instance.SetAllShowExceptRole(null, false);
        //        UI3D.TitleSystem.instance.gameObject.SetActive(false);
        //    }
        //    else
        //    {
        //        if (null != m_player)
        //            m_player.gameObject.SetActive(true);
        //        ObjectManage.Instance.SetAllShowExceptRole(null, true);
        //        UI3D.TitleSystem.instance.gameObject.SetActive(true);
        //    }
        //}

        float GetDistance(Vector3 a, Vector3 b, bool ingoreY = true)
        {
            if (ingoreY)
                a.y = b.y;
            return Vector3.Distance(a, b);
        }
    }

}
