using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys
{
    public partial class CameraManager : MonoBehaviour
    {
        #region 配置变量
        [Tooltip("目标位置和摄像机的夹角")]
        public float m_targetAngle = 3;
        [Tooltip("第一次进入场景时摄像机夹角")]
        public float m_firstEnterCameraAngle = 12;
        [Tooltip("目标高度")]
        public float m_targetHeight = 1;
        [Tooltip("玩家高度")]
        public float m_playerHeight = 2;
        [Tooltip("摄像机和玩家间的距离")]
        public float m_playerDistance = 8;
        [Tooltip("普通镜头时，摄像机和玩家的夹角")]
        public float m_normalCameraAngle = 12;
        [Tooltip("初始fov值")]
        public float m_defaultFov = 35;
        [Tooltip("实时fov值")]
        public float m_currFov = 35;
        [Tooltip("普通镜头旋转速度")]
        public float m_norRotSpeed = 5.0f;
        [Tooltip("普通镜头移动速度")]
        public float m_norPosSpeed = 5.0f;
        [Tooltip("锁定镜头旋转速度")]
        public float m_lockRotSpeed = 2.5f;
        [Tooltip("距离越近旋转速度要越慢：最终旋转速度 = m_lockRotSpeed+m_lockRotFixSpeed*摄像机和目标距离+修正距离 ")]
        public float m_lockRotFixSpeed = 0.5f;
        [Tooltip("锁定镜头移动速度")]
        public float m_lockPosSpeed = 10.0f;
        [Tooltip("距离越近位置速度要越慢：最终旋转速度 = m_lockPosSpeed+m_lockPosFixSpeed*摄像机和目标距离+修正距离 ")]
        public float m_lockPosFixSpeed = 0.4f;
        [Tooltip("锁敌时最小距离")]
        public float m_lockMinDisToTarget = 4.0f;
        [Tooltip("锁敌时最大距离")]
        public float m_lockMaxDisToTarget = 20.0f;
        [Tooltip("锁定敌人时的最大角度")]
        public float m_lockMaxAngle = 30.0f;
        [Tooltip("锁定敌人时的最小角度")]
        public float m_lockMinAngle = -20.0f;
        [Tooltip("锁定镜头最小不改变距离")]
        public float m_lockRangeMin = 4f;
        [Tooltip("锁定镜头最大不改变距离")]
        public float m_lockRangeMax = 15f;
        [Tooltip("锁定镜头最大距离")]
        public float m_lockMaxDistance = 16f;


        [Tooltip("向普通镜头转变的速度")]
        public float m_speedToNor = 10.0f;
        [Tooltip("缩放最大距离")]
        public float m_maxDis = 10.0f;
        [Tooltip("缩放最小距离")]
        public float m_minDis = 3.0f;
        [Tooltip("第二阶段缩放最小距离")]
        public float m_twoStageMinDis = 1.0f;
        [Tooltip("第二阶段缩放最大高度")]
        public float m_twoStageHeight = 1.4f;
        [Tooltip("旋转最小角度")]
        public float m_rotMinAngle = -30f;
        [Tooltip("旋转最大角度")]
        public float m_rotMaxAngle = 60f;
        [Tooltip("手指拖动跟随速度")]
        public float m_dragSpeed = 2f;
        [Tooltip("手指拖动变化率")]
        public float m_dragOffset = 1f;
        [Tooltip("进入特写镜头距离")]
        public float m_featureDis = 3.5f;
        [Tooltip("2.5D进入特写镜头距离")]
        public float m_25DFeatureDis = 5.0f;
        [Tooltip("特写镜头高度")]
        public float m_featureHeight = 1.0f;
        [Tooltip("摄像机进入碰撞的速度")]
        public float m_enterColliderSpeed = 2f;

        //2.5D镜头
        [Tooltip("2.5D最小缩放距离")]
        public float m_25DMinDis = 5.0f;
        [Tooltip("2.5D最大缩放距离")]
        public float m_25DMaxDis = 15.0f;
        [Tooltip("2.5D摄像机和玩家之间的距离")]
        public float m_2_5DPlayerDistance = 12.0f;
        [Tooltip("2.5D摄像机和玩家之间的夹角")]
        public float m_2_5DCameraAngle = 45;

        [Tooltip("3D镜头初始化，角度变换速度")]
        public float m_TDCameraRotSpeed = 20.0f;
        [Tooltip("3D镜头缩放速率")]
        public float m_scaleSpeed = 2.0f;

        //NPC镜头
        [Tooltip("NPC镜头距离")]
        public float m_npcCamDis = 2.5f;
        [Tooltip("NPC镜头高度")]
        public float m_npcCamHeight = 1.4f;

        //技能镜头返回普通镜头速率
        [Tooltip("技能镜头返回普通镜头速率")]
        public float m_returnToNorSpd = 5f;

        [Tooltip("新锁定镜头偏移阀值")]
        public float m_newLockThreshold = 50;
        [Tooltip("新锁定镜头修正值")]
        public float m_newLockCorrection = 100;
        [Tooltip("新锁定解锁时间")]
        public int m_newUnlockTime = 5;
        [Tooltip("旧锁定解锁距离")]
        public float m_oldUnlockDis = 30;
        #endregion

        #region 摄像机通用参数
        bool m_isForceLock = true;      //是否强锁定，将对象定在屏幕中心
        #endregion

        #region 摄像机移动的参数
        //普通镜头真是跟随的位置
        private Vector3 m_followPos;
        //碰撞距离
        private float m_tempColliderDistance;
        //临时存放角度
        private float m_tempNormalCameraAngle;
        //是否进入碰撞
        private bool m_enterCollider = false;
        //是否在过度镜头
        private bool m_excessiveMove = false;
        //普通镜头用距离
        public float m_noramlCamDis = 0.0f;
        //是否在npc镜头
        private bool m_npcCam = false;
        //当前NPC
        private Transform m_curNpc;
        private int m_curNpcID;
        //npc临时变量
        private Vector3 m_TempPos;
        private Quaternion m_TempRot;
        //是否在展示镜头
        private bool m_showCam = false;
        //镜头锁定目标
        private Transform m_lockTarget;
        //锁定目标
        private bool m_isLockTarget;
        //目标ratation
        private Quaternion m_toRot;
        //目标position
        private Vector3 m_toPos;
        //禁止摄像机旋转
        private bool m_isForbidRotate;

        //默认玩家高度,游戏过程会修改玩家高度
        public float m_defaultPlayerHeight { get; private set; }
        //默认敌人高度
        public float m_defaultTargetHeight { get; private set; }
        //默认最大镜头仰角
        public float m_defaultLockMinAngle { get; private set; }
        //默认摄像机玩家距离
        public float m_defaultPlayerDistance { get; private set; }

        //按Z旋转角度
        float m_zRotate = 0;
        float m_targetZRotate = 0;
        float m_zRotateSpeed = 0;

        //需要半透明的物体列表
        private Dictionary<int, GameObject> m_transDic = new Dictionary<int, GameObject>();
        #endregion

        #region 摄像机手势的参数
        float m_mobileProp = 0.4f;

        //临时变量
        private float m_tempDistance;
        private float m_tempHeight;
        private float m_temp25DDistance;
        private float m_tempMaxDis;
        private float m_tempMinDis;
        //x和y的偏移值
        float m_x = Mathf.Infinity;
        float m_y = Mathf.Infinity;
        float m_m = Mathf.Infinity;
        float m_n = Mathf.Infinity;
        //滚轮的偏移值
        float m_ScrollWheel = 0;
        //摄像机和人物之间的单位向量
        Vector3 m_normalized;
        //是否在转变到普通镜头的过程中
        bool m_toNor = false;
        //向普通镜头转变x轴
        float m_toNorX = 12f;
        //是否进入特写镜头
        bool m_isFeature = false;
        //是否在特写镜头过度
        DuringType m_DuringType = DuringType.Null;

        float m_lastDis;
        float m_currDis;

        bool m_duringScale = false;
        bool m_duringRatate = false;
        bool m_isRunning = false;
        Vector3 m_forwardDir;


        public enum DuringType
        {
            Null,
            Forward,
            During,
            Back,
            MoveBack,
        }
        #endregion
    }
}