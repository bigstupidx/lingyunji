using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class CamPointsObject : PointsObject<CamPoints>
    {

        public float m_rotateSpeed = 2.0f;// 旋转速度
        public float m_rotateSmoothTime = 5.0f;// 旋转的平滑时间

        public bool m_lockZAxis = true;

        private float MinimumX = -180F;
        private float MaximumX = 180F;

        private Quaternion m_SelfTargetRot;

        private Transform m_CameraObject;

        private Camera m_sampleCamera;

        Animation m_cameraAnim;
        AnimationClip m_cameraAnimClip;

        // Use this for initialization
        void Start()
        {
            //InitMouseInput();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateMouseInput();

            UpdateFovInput();
        }

        void FixedUpdate()
        {
            UpdateKeyInput();
        }

        public override int GetSampleType()
        {
            return 2;
        }

        #region 镜头控制

        public void InitMouseInput()
        {
            m_SelfTargetRot = this.transform.rotation;

            if (m_CameraObject == null)
                FindCamera();

            if (m_CameraObject == null)
                return;

            this.transform.localPosition = m_CameraObject.localPosition;
            this.transform.localRotation = m_CameraObject.localRotation;
            m_SelfTargetRot = m_CameraObject.localRotation;
        }

        void FindCamera()
        {
            m_CameraObject = Camera.main.transform;

            if (m_CameraObject != null)
                m_sampleCamera = m_CameraObject.GetComponent<Camera>();
        }

        void UpdateMouseInput()
        {
            if (!m_hasSelected)
                return;

            if (Input.GetMouseButton(1))
            {
                float yRot = Input.GetAxis("Mouse X") * m_rotateSpeed;
                float xRot = Input.GetAxis("Mouse Y") * m_rotateSpeed;

                this.transform.localRotation = m_SelfTargetRot *= Quaternion.Euler(-xRot, yRot, 0f);

                //m_SelfTargetRot = ClampRotationAroundXAxis(m_SelfTargetRot);

                //this.transform.localRotation = Quaternion.Slerp(this.transform.localRotation, m_SelfTargetRot, m_rotateSmoothTime * Time.deltaTime);

                if (m_lockZAxis)
                    this.transform.localRotation = m_SelfTargetRot = LockRotationZAxis(this.transform.localRotation);
            }

            if (m_CameraObject == null)
                FindCamera();

            if (m_CameraObject == null)
                return;

            m_CameraObject.localPosition = this.transform.localPosition;
            m_CameraObject.localRotation = this.transform.localRotation;

        }

        Quaternion LockRotationZAxis(Quaternion q)
        {
            return Quaternion.Euler(q.eulerAngles.x, q.eulerAngles.y, 0);
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

            angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        void UpdateFovInput()
        {
            if (m_sampleCamera == null)
                FindCamera();

            if (m_sampleCamera == null)
                return;

            float value = Input.GetAxis("Mouse ScrollWheel");
            if (value == 0.0f)
                return;
            float fov = SampleCamFov();
            fov += value;
            if (fov > 89f)
                fov = 89;
            else if (fov < 1)
                fov = 1;
            m_sampleCamera.fieldOfView = fov;

        }

        float SampleCamFov()
        {
            if (m_sampleCamera == null)
                FindCamera();

            if (m_sampleCamera != null)
                return m_sampleCamera.fieldOfView;
            else
                return CamPoints.DefaultFieldOfView;
        }

        public void SetCamFov(float fov)
        {
            if (m_sampleCamera == null)
                FindCamera();

            if (m_sampleCamera != null)
                m_sampleCamera.fieldOfView = fov;
        }
        
        /// <summary>
        /// 播放动画
        /// </summary>
        public void PlayAnim ()
        {
            if (m_CameraObject!=null && editData.Count > 1)
            {
                editData.BuildCameraAnimClip(ref m_cameraAnimClip);
                if (m_cameraAnim == null)
                {
                    m_cameraAnim = m_CameraObject.GetComponent<Animation>();
                    if (m_cameraAnim == null)
                        m_cameraAnim = m_CameraObject.gameObject.AddComponent<Animation>();
                    m_cameraAnim.AddClip(m_cameraAnimClip, CamPoints.CameraAnimName);
                }
                else
                {
                    m_cameraAnim.Stop();
                    m_cameraAnim.RemoveClip(m_cameraAnimClip);
                    m_cameraAnim.AddClip(m_cameraAnimClip, CamPoints.CameraAnimName);
                }
                
                m_cameraAnim.Play(CamPoints.CameraAnimName);
            }
        }

        #endregion

        #region Override Methods

        public override void Set(int index)
        {
            base.Set(index);

            editData.SetCamData(index, SampleCamFov());
        }

        public override void Add()
        {
            base.Add();

            editData.AddCamData(SampleCamFov());
        }

        public override void Insert(int index)
        {
            base.Insert(index);

            editData.InsertCamData(index, SampleCamFov());
        }

        public override void RemoveAt(int index)
        {
            base.RemoveAt(index);

            editData.RemoveCamDataAt(index);
        }

        public override void Move(int sourceIndex, int targetIndex)
        {
            base.Move(sourceIndex, targetIndex);

            editData.MoveCamData(sourceIndex, targetIndex);
        }

        #endregion

        #region static methods

        /// <summary>
        /// 创建点集对象
        /// </summary>
        /// <param name="orgPos"></param>
        /// <returns></returns>
        public static CamPointsObject Create()
        {
            GameObject go = new GameObject();
            CamPointsObject camPoints = go.AddComponent<CamPointsObject>();
            go.name = camPoints.objectName;
            go.transform.parent = PointsRoot;

            return camPoints;
        }

        #endregion
    }

}
