using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    [System.Serializable]
    public class CamPoints : Points
    {
        public const string CameraAnimName = "CamAnim";

        // Default Field Of View
        public const float DefaultFieldOfView = 60f;

        public List<CameraPointData> cameraData = new List<CameraPointData>();

        /// <summary>
        /// 用来构建镜头动画文件
        /// </summary>
        /// <param name="clip"></param>
        public void BuildCameraAnimClip(ref AnimationClip clip)
        {
            if (clip == null)
            {
                clip = new AnimationClip();
                clip.name = CameraAnimName;
                clip.legacy = true;
                clip.wrapMode = WrapMode.Default;
            }
            else
            {
                clip.ClearCurves();
            }

            // Gen Keyframes
            Keyframe[] pX = new Keyframe[Count];
            Keyframe[] pY = new Keyframe[Count];
            Keyframe[] pZ = new Keyframe[Count];

            Keyframe[] rX = new Keyframe[Count];
            Keyframe[] rY = new Keyframe[Count];
            Keyframe[] rZ = new Keyframe[Count];
            Keyframe[] rW = new Keyframe[Count];

            Keyframe[] fov = new Keyframe[Count];

            for (int i = 0; i < Count; ++i)
            {
                float time = cameraData[i].keyTime;
                fov[i] = new Keyframe(time, cameraData[i].fov);

                Vector3 pos = GetPosition(i);
                Quaternion rot = GetRotation(i);
                pX[i] = new Keyframe(time, pos.x);
                pY[i] = new Keyframe(time, pos.y);
                pZ[i] = new Keyframe(time, pos.z);

                rX[i] = new Keyframe(time, rot.x);
                rY[i] = new Keyframe(time, rot.y);
                rZ[i] = new Keyframe(time, rot.z);
                rW[i] = new Keyframe(time, rot.w);
            }

            // Set curves
            AnimationCurve curve = new AnimationCurve(pX);
            clip.SetCurve("", typeof(Transform), "localPosition.x", curve);
            curve = new AnimationCurve(pY);
            clip.SetCurve("", typeof(Transform), "localPosition.y", curve);
            curve = new AnimationCurve(pZ);
            clip.SetCurve("", typeof(Transform), "localPosition.z", curve);

            curve = new AnimationCurve(rX);
            clip.SetCurve("", typeof(Transform), "localRotation.x", curve);
            curve = new AnimationCurve(rY);
            clip.SetCurve("", typeof(Transform), "localRotation.y", curve);
            curve = new AnimationCurve(rZ);
            clip.SetCurve("", typeof(Transform), "localRotation.z", curve);
            curve = new AnimationCurve(rW);
            clip.SetCurve("", typeof(Transform), "localRotation.w", curve);

            curve = new AnimationCurve(fov);
            clip.SetCurve("", typeof(Camera), "field of view", curve);
            clip.EnsureQuaternionContinuity();
        }

        #region 镜头数据操作方法

        public void SetCamData(int index, float fov)
        {
            if (cameraData.Count > 0 && index >= 0 && index < cameraData.Count)
                cameraData[index].fov = fov;
        }

        public void AddCamData(float fov)
        {
            float keyTime = 0.0f;
            if (cameraData.Count > 0)
                keyTime = cameraData[cameraData.Count - 1].keyTime + 1.0f;
            cameraData.Add(new GameStory.CameraPointData(keyTime, fov, 0));
        }

        public void InsertCamData(int index, CameraPointData data)
        {
            cameraData.Insert(index, data);
        }

        public void InsertCamData(int index, float fov=60f)
        {
            float keyTime = 0.0f;
            if (cameraData.Count > 0)
            {
                if (index == 0)
                    keyTime = cameraData[0].keyTime * 0.5f;
                else if (index < cameraData.Count-1)
                    keyTime = (cameraData[index - 1].keyTime + cameraData[index].keyTime) * 0.5f;
                else
                    keyTime = cameraData[Count - 1].keyTime + 1.0f;
            }
            cameraData.Insert(index, new GameStory.CameraPointData(keyTime, fov, 0));
        }

        public void RemoveCamDataAt(int index)
        {
            cameraData.RemoveAt(index);
        }

        public void MoveCamData(int sourceIndex, int targetIndex)
        {
            if (sourceIndex != targetIndex &&
                sourceIndex >= 0 && sourceIndex <= positions.Count &&
                targetIndex >= 0 && targetIndex <= positions.Count)
            {
                InsertCamData(targetIndex, cameraData[sourceIndex]);
                RemoveCamDataAt(sourceIndex);
            }
        }

        #endregion

        public override void ClearAll()
        {
            base.ClearAll();

            cameraData.Clear();
        }
    }

    [System.Serializable]
    public class CameraPointData
    {
        public float fov = 60f;
        public float keyTime = 0.0f;
        public int keyMode = 0;

        public CameraPointData() { }

        public CameraPointData(float time, float fov, int mode)
        {
            this.keyTime = time;
            this.fov = fov;
            this.keyMode = mode;
        }
    }

}
