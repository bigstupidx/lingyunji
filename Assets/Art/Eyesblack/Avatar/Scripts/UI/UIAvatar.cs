#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    public enum enAvatar
    {
        normal,// 默认
        //body,//身形
        skinMerge,//化妆
        shape,//脸型
        max
    }

    public class UIAvatar : MonoBehaviour
    {
        public static string[] Avatar_Type = new string[] { "默认", "化妆", "脸型" };

        static UIAvatar _instance;
        public static UIAvatar Instance
        {
            get { return _instance; }
        }

        // UI资源
        public xys.UI.UIGroup m_right;
        public xys.UI.UIGroup m_top;
        public UIAvatarShapePage m_shapePage;
        public UIAvatarSkinMergePage m_skinMergePage;
        public UIAvatarPrefabPage m_prefabPage;
        public GameObject m_unknownPage;
        public Button m_btnReset;

        public RectTransform m_rttUIObject;
        

        public string m_modelName;// 展示的模型
        public RTTModelPart m_rtt;
        //RTTModelPartHandler m_rttHandler;
        ModelPartManage m_modelManager;

        // 预设配置
        public int m_career = 1;
        public int m_sex = 0;
        public int m_faceStyle = 0;
        RoleDisguiseCareer m_disguisePartConfig;
        RoleDisguiseHandle m_disguiseHandle = new RoleDisguiseHandle();


        // 化妆配置
        public string m_skinConfigName;// 化妆配置female
        RoleSkinPart m_skinPartConfig;
        RoleSkinHandle m_skinMerge;

        // 脸型配置
        public string m_shapeConfigName;// 脸型配置rs_nvguigu
        RoleShapeConfig m_shapeConfig;
        RoleShapeHandle m_shapeMgr;

        public RoleDisguiseOverallData GetOverallData()
        {
            return m_disguiseHandle.GetOverallData();
        }

        public void SetOverallData(RoleDisguiseOverallData data)
        {
            m_disguiseHandle.SetEditorOverallData(data);
        }

        public void SetSkinData(RoleSkinPartData skinData)
        {
            m_skinMerge.SetData(skinData);
        }

        public RoleSkinPartData GetSkinData()
        {
            return m_skinMerge.GetData();
        }

        public void SetShapeData(RoleShapePartData shapeData)
        {
            m_shapeMgr.SetData(shapeData);
        }

        public RoleShapePartData GetShapeData()
        {
            return m_shapeMgr.GetData();
        }

        void Destroy()
        {
            _instance = null;
        }

        void Start()
        {
            _instance = this;

            m_rtt.Init(m_rttUIObject, true);

            InitConfig();
            m_rtt.LoadModelWithAppearance(m_disguiseHandle, (GameObject go) => { m_rtt.ResetCamScale(); });

            //m_rttHandler = new RTTModelPartHandler("RTT_Avatar", m_rttUIObject, m_modelName, true, Vector3.zero);
        }

        // Update is called once per frame
        void Update()
        {
            //if (m_rtt != null)
            //{
            //    m_modelManager = m_rtt.GetModelManger();
            //}

            //if (m_faceMesh == null && m_modelManager!=null && m_modelManager.IsNormal)
            //{
            //    ModelPart part = null;
            //    m_modelManager.GetModelPart(ModelPartType.Face, out part);//暂时这样，美术并不知道顺序，通常是乱填的
            //    if (part == null || part.gameObject == null)
            //        return;

            //    // 获取脸部
            //    SkinnedMeshRenderer mesh = part.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
            //    if (mesh == null)
            //        return;

            //    // 初始化配置
            //    InitConfig(mesh);
            //    m_rtt.PlayAnim("face_idle");

                //检查模型加载完了，初始化下界面
                //var objs = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
                //foreach (var go in objs)
                //{
                //    var setting = go.GetComponent<ModelPartSetting1>();
                //    if (setting == null)
                //        continue;

                //    ModelPartManage partManage = setting.GetModelPartManage();
                //    if (partManage == null)
                //        continue;

                //    ModelPart part = null;
                //    partManage.GetModelPart(ModelPartType.Face, out part);//暂时这样，美术并不知道顺序，通常是乱填的
                //    if (part == null || part.gameObject==null)
                //        return;

                //    m_mesh = part.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
                //    if (m_mesh == null)
                //        return;

                //    InitConfig(m_mesh);
                //    break;
                //}
            //}
        }

        void InitConfig()
        {
            Debuger.LogWarning("SetMesh...");

            // shape
            RoleShapeConfig.TryGet(m_shapeConfigName, out m_shapeConfig);

            // skin
            m_skinPartConfig = RoleSkinConfig.instance.Get(m_skinConfigName);

            // default
            m_disguisePartConfig = RoleDisguiseConfig.Get(m_career, m_sex, m_faceStyle);
            m_disguiseHandle.InitConfig(m_modelName, m_disguisePartConfig, m_skinPartConfig, m_shapeConfig);
            m_skinMerge = m_disguiseHandle.m_skinHandle;
            m_shapeMgr = m_disguiseHandle.m_shapeHandle;

            // UI
            m_right.SetCount((int)enAvatar.max);
            m_right.AddSel(OnSelPage);
            for (int i = 0; i < (int)enAvatar.max; ++i)
            {
                m_right.Get<UIAvatarPageItem>(i).text.text = Avatar_Type[i];
            }

            // default select
            m_right.SetSel((int)enAvatar.normal);

            m_btnReset.onClick.RemoveAllListeners();
            m_btnReset.onClick.AddListener(OnReset);
        }

        void OnSelPage(int idx)
        {
            m_btnReset.gameObject.SetActive(false);
            m_top.gameObject.SetActive(false);

            m_prefabPage.gameObject.SetActive(false);
            m_skinMergePage.gameObject.SetActive(false);
            m_shapePage.gameObject.SetActive(false);
            m_unknownPage.SetActive(false);

            if (idx == (int)enAvatar.normal)
            {
                m_prefabPage.gameObject.SetActive(true);

                m_top.gameObject.SetActive(true);
                m_top.SetCount(UIAvatarPrefabPage.s_tabs.Length);
                m_top.AddSel(OnSelPart, true);
                m_top.SetSel(m_prefabPage.m_curTab);
                for (int i = 0; i < UIAvatarPrefabPage.s_tabs.Length; ++i)
                {
                    m_top.Get<UIAvatarTopItem>(i).text.text = UIAvatarPrefabPage.s_tabs[i];
                }
                m_rtt.SetCamState(0, true);
            }
            else if (idx == (int)enAvatar.skinMerge && m_skinMergePage != null)
            {
                m_btnReset.gameObject.SetActive(true);
                m_skinMergePage.gameObject.SetActive(true);
                m_top.gameObject.SetActive(true);
                m_top.SetCount(m_skinPartConfig.tabsName.Length);
                m_top.AddSel(OnSelPart, true);
                m_top.SetSel(0);
                for (int i = 0; i < m_skinPartConfig.tabsName.Length; ++i)
                {
                    m_top.Get<UIAvatarTopItem>(i).text.text = m_skinPartConfig.tabsName[i];
                }
                m_rtt.SetCamState(2, true);
            }
            else if (idx == (int)enAvatar.shape)
            {
                m_btnReset.gameObject.SetActive(true);
                m_shapePage.gameObject.SetActive(true);
                m_top.gameObject.SetActive(true);
                m_top.SetCount(m_shapeConfig.faceParts.Count);
                m_top.AddSel(OnSelPart, true);
                m_top.SetSel(0);
                for (int i = 0; i < m_shapeConfig.faceParts.Count; ++i)
                {
                    m_top.Get<UIAvatarTopItem>(i).text.text = m_shapeConfig.faceParts[i].name;
                }
                m_rtt.SetCamState(2, true);
            }
            else
                m_unknownPage.SetActive(true);

        }

        void OnSelPart(int idx)
        {
            if (m_right.CurIdx == (int)enAvatar.normal)
            {
                m_prefabPage.Set(idx, m_disguiseHandle);
            }
            if (m_right.CurIdx == (int)enAvatar.skinMerge && m_skinMergePage != null)
            {
                m_skinMergePage.Set(idx, m_skinMerge);
            }
            else if (m_right.CurIdx == (int)enAvatar.shape)
            {
                m_shapePage.Set(m_shapeConfig.faceParts[idx], m_shapeMgr);
            }

        }

        void OnReset()
        {
            //捏脸重置
            if (m_right.CurIdx == (int)enAvatar.shape)
            {
                m_shapeMgr.Reset();
                //刷新下界面
                m_top.SetSel(m_top.CurIdx);
            }
            else if (m_right.CurIdx == (int)enAvatar.skinMerge)
            {
                m_skinMerge.Reset();
                //刷新下界面
                m_top.SetSel(m_top.CurIdx);
            }

        }

        void OnGUI()
        {
            if (GUILayout.Button("重置镜头"))
            {
                m_rtt.ResetCamScale();
            }
        }


        [ContextMenu("重载模型和配置")]
        void ResetModelOrConfig()
        {
            if (Application.isPlaying)
            {
                InitConfig();
                m_rtt.LoadModelWithAppearance(m_disguiseHandle, null);
                m_rtt.ResetCamScale();
                Debuger.LogWarning("ResetModelOrConfig");
            }
        }

    }
}
#endif