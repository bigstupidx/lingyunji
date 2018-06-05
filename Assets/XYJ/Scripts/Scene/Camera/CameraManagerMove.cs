using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

namespace xys
{
    //摄像机移动相关
    public partial class CameraManager : MonoBehaviour
    {
        //禁止摄像机旋转
        public void ForbidRotate(bool forbid)
        {
            m_isForbidRotate = forbid;
        }

        //恢复玩家高度
        public void ResetPlayerHeight()
        {
            m_playerHeight = m_defaultPlayerHeight;
        }

        //设置玩家高度
        public void SetPlayerHeight(float toheitgh)
        {
            m_playerHeight = toheitgh;
        }

        //恢复摄像机玩家距离
        public void ResetPlayerDistance()
        {
            m_noramlCamDis = m_playerDistance = m_defaultPlayerDistance;
        }
        //设置摄像机玩家距离
        public void SetPlayerDistance(float toDistance)
        {
            m_noramlCamDis = m_playerDistance = toDistance;
        }

        //设置敌人高度
        public void SetTargetHeight(float height)
        {
            //没有配置，使用默认的
            if (height == float.MinValue)
                m_targetHeight = m_defaultTargetHeight;
            else
                m_targetHeight = height;
        }
        //恢复敌人高度
        public void ResetTargetHeight()
        {
            m_targetHeight = m_defaultTargetHeight;
        }

        //摄像机最大仰角
        public void SetLockMinAngle(float angle)
        {
            if (angle == float.MaxValue)
                m_lockMinAngle = m_defaultTargetHeight;
            else
                m_lockMinAngle = angle;
        }

        public void ResetLockMinAngle()
        {
            m_lockMinAngle = m_defaultLockMinAngle;
        }

        //设置锁定摄像机参数
        public void SetLockCameraPara(string para)
        {
            //自己高度|主角距离|主角高度|fov
            if (string.IsNullOrEmpty(para))
            {
                //使用默认参数
                //CameraTodo
                //para = ConfigLocalBattle.instance.targetHeight + "|" + ConfigLocalBattle.instance.playerHeight + "|" + ConfigLocalBattle.instance.playerDistance + "|" + ConfigLocalBattle.instance.defaultFov;
            }

            string[] temp = para.Split('|');
            float height, playerDis, playerHeight, fov, unLockDis = 0;
            if (float.TryParse(temp[0], out height) &&
                float.TryParse(temp[2], out playerDis) &&
                float.TryParse(temp[1], out playerHeight) &&
                float.TryParse(temp[3], out fov)
                )
            {
                m_targetHeight = height;
                m_playerDistance = playerDis;
                m_playerHeight = playerHeight;
                m_currFov = fov;
                if (temp.Length == 5)
                {
                    if (float.TryParse(temp[4], out unLockDis))
                    {
                        m_oldUnlockDis = unLockDis;
                    }
                }
            }
        }

        //恢复锁定摄像机参数
        public void ResetLockCameraPara()
        {
            //CameraTodo
            //m_targetHeight = ConfigLocalBattle.instance.targetHeight;
            //m_playerDistance = ConfigLocalBattle.instance.playerDistance;
            //m_playerHeight = ConfigLocalBattle.instance.playerHeight;
            //m_defaultFov = ConfigLocalBattle.instance.defaultFov;
            FadeToDefaultFov();
        }

        //设置z旋转
        public void SetZRotate(float z, float speed = 5)
        {
            m_zRotate = z;
            m_zRotateSpeed = speed;
        }

        //恢复默认z旋转
        public void ResetZRotate()
        {
            SetZRotate(0, 1000);
        }

        //设置归位旋转速度
        public void SetTDCameraRotSpeed(float time)
        {
            //通过时间计算速度
            float speed = 0;
            float toAngle = m_cameraViewType == CameraViewType.TwoPointFiveD ? m_2_5DCameraAngle : m_normalCameraAngle;
            Quaternion rotationPlayer = Quaternion.Euler(toAngle, Quaternion.LookRotation(m_player.forward).eulerAngles.y, 0);

            Vector3 targetFoward = m_cameraRoot.transform.position - m_followPos;
            Quaternion rotationCamera = Quaternion.LookRotation(-targetFoward);

            float deltaAngle = Quaternion.Angle(rotationPlayer, rotationCamera);
            speed = deltaAngle / (time * 60);

            m_TDCameraRotSpeed = speed;
        }

        //摄像机默认归位速度
        public void ResetTDCameraRotSpeed()
        {
            //CameraTodo
            //m_TDCameraRotSpeed = ConfigLocalBattle.instance.TDCameraRotSpeed;
        }

        //玩家锁定目标
        void LockMeToTarget(Transform target)
        {
#if COM_DEBUG
            //CameraTodo
            //if (!GM_UI.s_isLockMonster)
            //    target = null;
#endif

            if (target == null || m_player == null || target.transform == null || m_player.transform == null)
            {
                m_lockTarget = null;
                m_isLockTarget = false;
                return;
            }

            //锁定敌人的时候，关闭手势
            CloseGestrues();

            //2.5D镜头
            if (m_cameraViewType == CameraViewType.TwoPointFiveD)
            {
                return;
            }

            m_lockTarget = target;
            m_isLockTarget = true;

            OnEnterFeature(DuringType.MoveBack);
            if (!m_enterCollider && m_isForceLock)
            {
                //m_playerDistance = m_tempDistance;
                //m_playerHeight = m_tempHeight;
            }
        }

        //是否锁定目标
        public bool IsLockToTarget()
        {
            return m_isLockTarget;
        }

        //主角创建结束的回调
        void TransitionFinishCreateMe()
        {
            m_cameraOffset.transform.localRotation = Quaternion.identity;
            m_cameraOffset.transform.localPosition = Vector3.zero;

            InitCamPos();
        }

        //初始化摄像机位置
        public void InitCamPos()
        {
            m_followPos = m_player.position + Vector3.up * m_playerHeight;

            //摄像机初始化在角色背后
            m_cameraRoot.transform.position = m_player.transform.TransformPoint(0, 0, -m_playerDistance);
            if (m_firstEnter)
            {
                //CameraTodo
                m_toNorX = /*ConfigLocalBattle.instance.firstEnterCameraAngle*/m_firstEnterCameraAngle;
                m_firstEnter = false;
            }
            else
            {
                SetCameraToDefaultAngle();
            }
            UpdateNormalCamera(m_player, true);
        }

        //普通镜头
        void UpdateNormalCamera(Transform lookAtTarget, bool immediately = false)
        {
            //该点位置跟随角色
            //float height = m_isFeature ? m_featureHeight : m_playerHeight;
            //if (m_duringScale || immediately)
            //    m_followPos = targetPos;
            //else
            //    m_followPos = Vector3.Lerp(m_followPos, targetPos, m_norPosSpeed * Time.deltaTime);
            SetFollowPos(m_duringScale || immediately);

            if (!m_enterCollider)
                m_noramlCamDis = Mathf.Lerp(m_noramlCamDis, m_playerDistance, m_returnToNorSpd * Time.deltaTime);
            else
                m_noramlCamDis = m_playerDistance;

            Vector3 targetFoward = m_cameraRoot.transform.position - m_followPos;
            m_targetZRotate = Mathf.Lerp(m_targetZRotate, m_zRotate, m_zRotateSpeed);
            m_toRot = Quaternion.Euler(m_toNorX, Quaternion.LookRotation(-targetFoward).eulerAngles.y, m_targetZRotate);
            m_toPos = m_toRot * new Vector3(0.0f, 0.0f, -m_noramlCamDis) + m_followPos;

            //当有旋转和缩放的时候，镜头跟随旋转和缩放
            if (!m_duringScale && !m_duringRatate)
            {
                m_cameraRoot.transform.rotation = m_toRot;
                m_cameraRoot.transform.position = m_toPos;
            }
        }

        //摄像机过度
        void UpdateExcessiveCamera(float distance = 0, float height = 0, float angle = 0)
        {
            if (0 != distance)
            {
                m_playerDistance = Mathf.Lerp(m_playerDistance, distance, m_norPosSpeed * Time.deltaTime);
            }
            if (0 != height)
            {
                m_playerHeight = Mathf.Lerp(m_playerHeight, height, m_norPosSpeed * Time.deltaTime);
                SetFollowPos();
            }

            Quaternion rotation = Quaternion.Euler(angle, Quaternion.LookRotation(m_player.forward).eulerAngles.y, 0);
            m_cameraRoot.transform.rotation = Quaternion.Lerp(m_cameraRoot.transform.rotation, rotation, m_TDCameraRotSpeed * Time.deltaTime);

            Vector3 position = m_cameraRoot.transform.rotation * new Vector3(0.0f, 0.0f, -m_playerDistance) + /*m_player.position + Vector3.up * m_playerHeight*/ m_followPos;
            m_cameraRoot.transform.position = position;
            m_cameraRoot.transform.localEulerAngles -= new Vector3(0, 0, 1) * m_cameraRoot.transform.localEulerAngles.z;

            DebugDrawLine(m_cameraRoot.transform.position, m_followPos, Color.black);

            m_toNorX = m_cameraRoot.transform.localEulerAngles.x;
            m_x = m_cameraRoot.transform.localEulerAngles.y;
            m_noramlCamDis = m_playerDistance;
        }

        //2.5d镜头
        void Update25DCamera()
        {
            //该点位置跟随角色
            //m_followPos = Vector3.Lerp(m_followPos, m_player.position + Vector3.up * m_tempHeight, Time.deltaTime * m_norPosSpeed);
            SetFollowPos(false);
            m_playerDistance = Mathf.Lerp(m_playerDistance, m_2_5DPlayerDistance, Time.deltaTime);

            //Vector3 targetFoward = m_cameraRoot.transform.position - m_followPos;
            m_toRot = Quaternion.Lerp(m_cameraRoot.transform.rotation, Quaternion.Euler(m_2_5DCameraAngle, m_x, 0), m_norPosSpeed * Time.deltaTime);
            m_toPos = m_toRot * new Vector3(0.0f, 0.0f, -m_playerDistance) + m_followPos;

            //当有旋转和缩放的时候，镜头跟随旋转和缩放
            if (!m_duringScale && !m_duringRatate)
            {
                m_cameraRoot.transform.rotation = m_toRot;
                m_cameraRoot.transform.position = m_toPos;
                m_cameraRoot.transform.localEulerAngles -= new Vector3(0, 0, 1) * m_cameraRoot.transform.localEulerAngles.z;

                m_toNorX = m_cameraRoot.transform.localEulerAngles.x;
                m_noramlCamDis = m_playerDistance;
            }
        }

        //点击摇杆设置参数
        public void SetExcessiveCamera(bool isDo)
        {
            //特写镜头不处理
            if (m_isFeature)
            {
                return;
            }
            //只有在非锁定状态或者在2.5d下才能设置
            if (!m_isLockTarget || m_cameraViewType == CameraViewType.TwoPointFiveD)
            {
                if (isDo)
                {
                    m_excessiveMove = true;
                    CloseGestrues();
                    StartCoroutine("CloseExcessiveMove");

                }
                else
                {
                    m_excessiveMove = false;
                    StopCoroutine("CloseExcessiveMove");
                }
            }
        }

        public void SetFollowPos(bool immediately = true)
        {
            float height = /*m_isFeature ? m_featureHeight : */m_playerHeight;

            Vector3 targetPos = m_player.position + Vector3.up * height;
            //CameraTodo
            //BaseRole mainPlayer = ObjectManage.Instance.mainPlayer;
            //if (mainPlayer != null && mainPlayer.isAlive)
            //{
            //    if (RideSystem.IsOnRide(mainPlayer.uuid))
            //    {
            //        targetPos.y = targetPos.y - mainPlayer.m_modelManage.GetModelOffsetY();
            //        m_followPos = Vector3.Lerp(m_followPos, targetPos, m_norPosSpeed * Time.deltaTime);
            //    }
            //    else
            //    {
            //        if (immediately)
            //            m_followPos = targetPos;
            //        else
            //            m_followPos = Vector3.Lerp(m_followPos, targetPos, m_norPosSpeed * Time.deltaTime);
            //    }
            //}

            if (immediately)
                m_followPos = targetPos;
            else
                m_followPos = Vector3.Lerp(m_followPos, targetPos, m_norPosSpeed * Time.deltaTime);
        }

        float CaculateAngle(float distance = -1)
        {
            if (m_player == null || m_lockTarget == null || m_lockTarget.transform == null || m_player.transform == null)
                return 0f;

            distance = distance == -1 ? Vector3.Distance(m_player.position, m_lockTarget.position) : distance;
            //Debuger.Log("distance : " + distance);

            //两个高度点之间的距离
            float a = Mathf.Sqrt(Mathf.Pow(distance, 2) + Mathf.Pow(m_playerHeight - m_targetHeight, 2));
            //人物到目标，以及摄像机到目标之间的夹角
            float alphaAngle = Mathf.Asin(m_playerDistance * Mathf.Sin(m_targetAngle * Mathf.PI / 180.0f) / a);
            alphaAngle = alphaAngle * 180f / Mathf.PI;

            //人物到目标，人物到摄像机之间的夹角
            float angle = 180 - alphaAngle - m_targetAngle;

            //人物和对象高度差夹角
            float betaAngle = Mathf.Atan((m_playerHeight - m_targetHeight) / distance);
            betaAngle = betaAngle * 180f / Mathf.PI;
            //Debuger.Log("a : " + a + " alphaAngle : " + alphaAngle + " betaAngle : " + betaAngle + " angle : " + angle);

            Vector3 targetToPlayer = m_player.position - m_lockTarget.position;
            targetToPlayer = targetToPlayer.normalized;
            //计算法向量
            Vector3 upVector = Vector3.up;
            Vector3 normalVector = Vector3.Cross(targetToPlayer, upVector);

            //计算人物高度到目标高度的向量
            Quaternion rotation = Quaternion.AngleAxis(alphaAngle + betaAngle, normalVector);
            Vector3 finalVector = rotation * targetToPlayer;

            float length = Mathf.Sqrt(Mathf.Pow(a, 2) + Mathf.Pow(m_playerDistance, 2) - 2 * a * m_playerDistance * Mathf.Cos(angle * Mathf.PI / 180.0f));
            Vector3 cameraPos = finalVector * length + m_lockTarget.position + Vector3.up * m_targetHeight;
            //Debuger.Log("length : " + length);
            DebugDrawLine(m_lockTarget.position + Vector3.up * m_targetHeight, cameraPos, Color.white);
            //return cameraPos;

            //计算角度
            Quaternion cameraToPlayer = Quaternion.LookRotation(m_lockTarget.position + Vector3.up * m_playerHeight - cameraPos);
            return cameraToPlayer.eulerAngles.x + m_targetAngle;
        }

        //直接设置锁定镜头位置,默认在角色背后
        public void SetLockPos()
        {
            float x = CaculateAngle(m_lockMinDisToTarget);
            Vector3 forward = m_player.forward;
            Quaternion toRot = Quaternion.LookRotation(forward);
            toRot = Quaternion.Euler(x, toRot.eulerAngles.y, toRot.eulerAngles.z);

            m_cameraRoot.transform.rotation = toRot;
            m_cameraRoot.transform.position = m_player.position + toRot * new Vector3(0.0f, 0.0f, -m_playerDistance) + Vector3.up * m_playerHeight;
        }

        //锁定镜头
        void UpdateLockCamera()
        {
            m_mainCamera.fieldOfView = m_currFov;
            //摄像机y轴的旋转是从 sourcePos看向targetPos的方向， 这里sourcePos选择player好像比camera要好一点
            Vector3 sourcePos = m_player.transform.position;
            Vector3 targetPos = m_lockTarget.transform.position;
            Vector3 lookDir = targetPos - sourcePos;
            lookDir.y = 0;
            lookDir.Normalize();

            //避免报错
            if (lookDir == Vector3.zero)
                return;

            //为了防止距离很小或者很大时，角度变化太大
            //float disTargetToPlayer = Vector3.Distance(m_player.position, m_lockTarget.transform.position);
            //if (disTargetToPlayer < m_lockMinDisToTarget)
            //    targetPos = sourcePos + lookDir * m_lockMinDisToTarget;
            //else if (disTargetToPlayer > m_lockMaxDisToTarget)
            //    targetPos = sourcePos + lookDir * m_lockMaxDisToTarget;

            //目标点和主角的距离
            float distance = Vector3.Distance(sourcePos, targetPos);
            if(distance < m_lockRangeMin)
            {
                m_playerDistance = m_lockMaxDistance;
            }
            else if(distance >= m_lockRangeMin && distance < m_lockRangeMax)
            {
                m_playerDistance = ((m_tempDistance - m_lockMaxDistance) / (m_lockRangeMax - m_lockRangeMin)) * distance + 
                    (m_lockMaxDistance - (m_tempDistance - m_lockMaxDistance) / (m_lockRangeMax - m_lockRangeMin) * m_lockRangeMin);
            }
            else
            {
                m_playerDistance = m_tempDistance;
            }

            //位置过渡
            if (m_playerDistance != m_noramlCamDis)
            {
                m_noramlCamDis = Mathf.Lerp(m_noramlCamDis, m_playerDistance, m_returnToNorSpd * Time.deltaTime);
            }
            Vector3 toPos = m_player.position + -m_cameraRoot.forward * m_noramlCamDis + Vector3.up * m_playerHeight;
            float posSpeed = m_lockPosSpeed + Vector3.Distance(m_cameraRoot.position, m_lockTarget.position) * m_lockPosFixSpeed;

            m_cameraRoot.position = Vector3.Slerp(m_cameraRoot.position, toPos, posSpeed * Time.deltaTime);

            //忽略摄像机旋转
            if (m_isForbidRotate)
                return;
            //摄像机的目标角度
            float toAngleX = m_cameraRoot.eulerAngles.x;
            //修正角度:两个向量的角度差是个定值，才能保证锁定的视点在屏幕高度不变
            float cameraToTarget = Quaternion.LookRotation(targetPos + Vector3.up * m_targetHeight - toPos).eulerAngles.x;
            float cameraToPlayer = Quaternion.LookRotation(m_player.position + Vector3.up * m_playerHeight - toPos).eulerAngles.x;
            float angleOff = m_targetAngle - (cameraToPlayer - cameraToTarget);
            toAngleX += angleOff;
            //玩家朝向目标
            Quaternion toRot = Quaternion.LookRotation(lookDir);

            //限制最大角度，可以设置
            //先通过距离来计算
            if (distance <= m_lockMinDisToTarget)
            {
                toAngleX = CaculateAngle(m_lockMinDisToTarget);
            }
            if (distance >= m_lockMaxDisToTarget)
            {
                toAngleX = CaculateAngle(m_lockMaxDisToTarget);
            }
            toAngleX = SetAngle(toAngleX);
            //再判断是否超过最大限制角度
            toAngleX = toAngleX >= m_lockMaxAngle ? m_lockMaxAngle : toAngleX;
            toAngleX = toAngleX <= m_lockMinAngle ? m_lockMinAngle : toAngleX;

            toRot = Quaternion.Euler(toAngleX, toRot.eulerAngles.y, toRot.eulerAngles.z);

            //朝向过渡
            float rotSpeed = m_lockRotSpeed + Vector3.Distance(m_cameraRoot.position, m_lockTarget.position) * m_lockRotFixSpeed;
            Vector3 eulers = Quaternion.Slerp(m_cameraRoot.rotation, toRot, rotSpeed * Time.deltaTime).eulerAngles;
            eulers.z = 0;//强制让z轴的旋转为零
            m_cameraRoot.eulerAngles = eulers;

            //该点位置跟随角色
            m_followPos = m_player.transform.position + Vector3.up * m_playerHeight;
            m_toNorX = toAngleX;
            m_x = m_cameraRoot.transform.localEulerAngles.y;
            //CaculateAngle();
            //Debuger.DrawLine(m_player.position + Vector3.up * m_playerHeight, m_lockTarget.transform.position + Vector3.up * m_targetHeight, Color.green);
            //Debuger.DrawLine(m_player.position, m_lockTarget.transform.position, Color.green);
            //Debuger.DrawLine(m_player.position + Vector3.up * m_playerHeight, m_player.position, Color.green);
        }

        //基本与普通镜头一致，只有在怪物即将超出屏幕区域的时候才自动旋转
        bool m_correct = false;
        void UpdateLockCameraNew(Transform lookAtTarget, bool immediately = false)
        {
            if (m_duringScale || immediately)
                m_followPos = lookAtTarget.position + Vector3.up * m_playerHeight;
            else
                m_followPos = Vector3.Lerp(m_followPos, lookAtTarget.position + Vector3.up * m_playerHeight, m_norPosSpeed * Time.deltaTime * (m_correct ? 2 : 1));

            if (!m_enterCollider)
                m_noramlCamDis = Mathf.Lerp(m_noramlCamDis, m_playerDistance, m_returnToNorSpd * Time.deltaTime);
            else
                m_noramlCamDis = m_playerDistance;

            //Vector3 targetFoward = m_cameraRoot.transform.position - m_followPos;

            Vector3 targetPos = m_lockTarget.transform.position;
            Vector3 screenPos = m_mainCamera.WorldToScreenPoint(targetPos);
            //Vector3 uiPos = UICamera.currentCamera.ScreenToWorldPoint(screenPos);
            //Debuger.Log("Screen (" + Screen.width + " , " + Screen.height + ")  " + "screenPos : " + screenPos);

            //偏移系数,主角和锁定目标之间距离越远，偏移系数越小
            float dis = GetDistance(m_cameraRoot.position, m_lockTarget.position);
            float offsetPara = 1 / ((2 * dis / Screen.width) * Mathf.Tan(m_mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 150);
            //Debuger.LogError("offsetPara : " + offsetPara);
            float newLockThreshold = m_newLockThreshold * offsetPara;
            float newLockCorrection = m_newLockCorrection * offsetPara;

            if (screenPos.x < newLockThreshold || screenPos.x > Screen.width - newLockThreshold)
            {
                m_correct = true;
            }

            //执行偏差
            if ((screenPos.x < newLockCorrection || screenPos.x > Screen.width - newLockCorrection) && m_correct)
            {
                //m_followPos = lookAtTarget.position + Vector3.up * m_playerHeight;
                Vector3 look = m_lockTarget.position - m_followPos + Vector3.up * m_playerHeight;
                Quaternion targetRotation = Quaternion.Euler(m_toNorX, Quaternion.LookRotation(look).eulerAngles.y, 0);
                float minus = 0;
                if (screenPos.x < newLockCorrection)
                {
                    minus = newLockCorrection - screenPos.x;
                }
                if (screenPos.x > Screen.width - newLockCorrection)
                {
                    minus = screenPos.x - (Screen.width - newLockCorrection);
                }

                float speed = (0.02f * minus + 0.1f) >= 0.7f ? 0.7f : (0.02f * minus + 0.1f);
                speed = speed * offsetPara;

                m_toRot = Quaternion.Lerp(m_cameraRoot.transform.rotation, targetRotation, Time.deltaTime * speed);
                //Debuger.LogWarning("Screen (" + Screen.width + " , " + Screen.height + ")  " + "screenPos : " + screenPos);
            }
            else
            {
                if (m_correct)
                {
                    //m_correct = false;
                    ////强行复位
                    //m_followPos = lookAtTarget.position + Vector3.up * m_playerHeight;
                    //targetFoward = m_cameraRoot.transform.position - m_followPos;
                }
                //else
                {
                    m_targetZRotate = Mathf.Lerp(m_targetZRotate, m_zRotate, m_zRotateSpeed);
                    m_toRot = Quaternion.Euler(m_toNorX, /*Quaternion.LookRotation(-targetFoward).eulerAngles.y*/m_cameraRoot.transform.localEulerAngles.y, m_targetZRotate);
                }
                m_correct = false;
                //Debuger.LogError("Screen (" + Screen.width + " , " + Screen.height + ")  " + "screenPos : " + screenPos);
            }

            m_toPos = m_toRot * new Vector3(0.0f, 0.0f, -m_noramlCamDis) + m_followPos;

            //当有旋转和缩放的时候，镜头跟随旋转和缩放
            if (!m_duringScale && !m_duringRatate)
            {
                m_cameraRoot.transform.rotation = m_toRot;
                m_cameraRoot.transform.position = m_toPos;
            }
        }

        float SetAngle(float angle)
        {
            if (angle > 300)
            {
                angle -= 360;
            }
            return angle;
        }

        //NPC镜头
        //CameraTod
        void UpdateNpcCamera()
        {
            //float height;
            //float dis;
            //if (null == m_npc || m_npc.isDestroy)
            //{
            //    //Debuger.LogError("NPC镜头  null == m_npc");
            //    return;
            //}
            //else
            //{
            //    m_npcCamDis = m_npc.m_objPrototype.npcData.npcCamDis;
            //    m_npcCamHeight = m_npc.m_objPrototype.npcData.npcCamHeight;
            //}

            ////CameraTodo
            ////if (GM_UI.s_useGMNpcPara && float.TryParse(GM_UI.s_npcCamHeight, out height) && float.TryParse(GM_UI.s_npcCamDis, out dis))
            ////{
            ////}
            ////else
            //{
            //    height = m_npcCamHeight;
            //    dis = m_npcCamDis;
            //}

            //Vector3 forward = m_npc.m_modelManage.transform.position - m_player.position;
            ////取消Y的偏移
            //forward.y = 0;
            //Quaternion rotation = Quaternion.Euler(0, Quaternion.LookRotation(forward).eulerAngles.y, 0);
            ////m_cameraRoot.transform.rotation = Quaternion.Lerp(m_cameraRoot.transform.rotation, rotation, m_TDCameraRotSpeed * Time.deltaTime);
            //m_cameraRoot.transform.rotation = rotation;
            //Vector3 position = m_cameraRoot.transform.rotation * new Vector3(0.0f, 0.0f, -dis) + m_npc.m_modelManage.transform.position + Vector3.up * height;
            //m_cameraRoot.transform.position = position;
            //m_cameraRoot.transform.localEulerAngles -= new Vector3(0, 0, 1) * m_cameraRoot.transform.localEulerAngles.z;
        }

        //展示镜头
        //CameraTodo
        void UpdateShowCamera()
        {
            //Vector3 forward = m_player.forward;
            ////取消Y的偏移
            //forward.y = 0;
            //Quaternion rotation = Quaternion.Euler(0, Quaternion.LookRotation(-forward).eulerAngles.y, 0);
            //m_cameraRoot.transform.rotation = rotation;
            //Vector3 position = m_cameraRoot.transform.rotation * new Vector3(0.0f, 0.0f, -ConfigLocalBattle.instance.showCamDis) +
            //    m_player.transform.position + Vector3.up * ConfigLocalBattle.instance.showCamHeight;
            //m_cameraRoot.transform.position = position;
            //m_cameraRoot.transform.localEulerAngles -= new Vector3(0, 0, 1) * m_cameraRoot.transform.localEulerAngles.z;
        }

        [Conditional("COM_DEBUG")]
        void DebugDrawLine(Vector3 start, Vector3 end, Color color)
        {
            UnityEngine.Debug.DrawLine(start, end, color);
        }

        void TransitionUpdate()
        {
            if (m_cameraRoot == null || m_player == null || m_player.transform == null || m_cameraOffset == null || m_duringSkill || m_specialCamera)
            {
                if ((m_duringSkill || m_specialCamera) && m_player != null)
                {
                    m_followPos = m_player.position + Vector3.up * m_playerHeight;
                }
                return;
            }

            if (m_npcCam)
            {
                //NPC镜头
                UpdateNpcCamera();
                return;
            }

            if (m_showCam)
            {
                //展示镜头
                UpdateShowCamera();
                return;
            }

            //锁敌时，敌人销毁了
            if (m_isLockTarget && (m_lockTarget == null || m_lockTarget.transform == null))
                LockMeToTarget(null);

            switch (m_cameraViewType)
            {
                case CameraViewType.TwoPointFiveD:
                    //2.5D镜头
                    if (!m_excessiveMove)
                        Update25DCamera();
                    else
                        UpdateExcessiveCamera(m_playerDistance, m_playerHeight, m_2_5DCameraAngle);
                    break;
                case CameraViewType.ThreeD:
                    //3d镜头
                    if (m_isLockTarget)
                    {
                        if (!m_isForceLock)
                            UpdateLockCameraNew(m_player);
                        else
                            UpdateLockCamera();
                    }
                    else if (!m_excessiveMove)
                        UpdateNormalCamera(m_player);
                    else
                        UpdateExcessiveCamera(m_tempDistance, m_tempHeight, m_normalCameraAngle);
                    break;
            }

            DebugDrawLine(m_cameraRoot.position, m_followPos, Color.red);

            //进入碰撞
            EnterCollider(m_cameraRoot.position);

            //部分物品半透明
            EnterTransparentCollider();

            //3d镜头下移动的时候， 要跟地面保持一个角度
            //KeepTerrienAngle();
        }

        //进入地形碰撞
        Vector3 EnterCollider(Vector3 pos)
        {
            RaycastHit hit;
            Vector3 dir = pos - m_followPos;
            int layerMask = ComLayer.Layer_GroundMask | 1 << ComLayer.Layer_Wall;

            dir = dir.normalized;
            float colliderOffset = 0.1f;
            if (!m_enterCollider)
            {
                m_tempColliderDistance = m_playerDistance;
            }
            if (Physics.Raycast(m_followPos, dir, out hit, m_tempColliderDistance, layerMask))
            {
                m_enterCollider = true;
                DebugDrawLine(m_followPos, hit.point, Color.yellow);
                float dis = Vector3.Distance(hit.point, m_followPos);

                //加一点偏移值
                if (ComLayer.IsGroundLayer(hit.collider.gameObject.layer))
                    m_playerDistance = dis - colliderOffset;
                else
                    m_playerDistance = Mathf.Lerp(m_playerDistance, dis - colliderOffset, Time.deltaTime * m_enterColliderSpeed);

                if (m_playerDistance > m_tempColliderDistance)
                {
                    m_playerDistance = m_tempColliderDistance;
                }
            }
            else
            {
                if (m_cameraRoot.localEulerAngles.x > 0 && m_cameraRoot.localEulerAngles.x < 300)
                {
                    m_playerDistance = Mathf.Lerp(m_playerDistance, m_tempColliderDistance, Time.deltaTime * m_enterColliderSpeed);
                }
                else
                {
                    m_playerDistance = m_tempColliderDistance;
                }
                if (Mathf.Abs(m_playerDistance - m_tempColliderDistance) < 0.05f)
                    m_enterCollider = false;
            }

            return m_cameraRoot.transform.rotation * new Vector3(0.0f, 0.0f, -m_playerDistance) + m_followPos;
        }

        //判断摄像机进入半透明碰撞
        void EnterTransparentCollider()
        {
            int layerMask = 1 << ComLayer.Layer_SceneTransparent;
            Vector3 dir = m_cameraRoot.position - (m_player.position + Vector3.up * m_playerHeight);
            dir = dir.normalized;

            List<RaycastHit> hits = new List<RaycastHit>(Physics.RaycastAll(m_player.position + Vector3.up * m_playerHeight, dir, m_playerDistance, layerMask));

            List<int> removeList = new List<int>();
            //先找出需要显示的
            foreach (KeyValuePair<int, GameObject> pair in m_transDic)
            {
                bool contain = false;
                for (int i = 0; i < hits.Count; ++i)
                {
                    if (pair.Key == hits[i].collider.gameObject.GetInstanceID())
                    {
                        contain = true;
                        break;
                    }
                }
                if (!contain)
                {
                    //当前不需要隐藏
                    removeList.Add(pair.Key);
                }
            }
            //移除要显示的
            for (int i = 0; i < removeList.Count; ++i)
            {
                if (m_transDic.ContainsKey(removeList[i]))
                {
                    //m_transDic[removeList[i]].SetActive(true);
                    SetRenderShow(m_transDic[removeList[i]], true);
                    m_transDic.Remove(removeList[i]);
                }
            }

            if (null != hits && hits.Count > 0)
            {
                for (int i = 0; i < hits.Count; ++i)
                {
                    RaycastHit hit = hits[i];
                    GameObject obj = hit.collider.gameObject;

                    if (m_transDic.ContainsKey(obj.GetInstanceID()))
                    {
                        continue;
                    }
                    else
                    {
                        m_transDic.Add(obj.GetInstanceID(), obj);
                    }

                    //obj.SetActive(false);
                    SetRenderShow(obj, false);
                }
            }
        }

        float m_tempY = 0;
        Vector3 m_tempPos = Vector3.zero;
        //与地面保持角度
        void KeepTerrienAngle()
        {
            RaycastHit hit;
            if (Physics.Raycast(m_player.position + Vector3.up * m_playerHeight, -Vector3.up, out hit, m_playerHeight * 2, ComLayer.Layer_GroundMask))
            {
                if (hit.collider != null && hit.collider.gameObject != null)
                {
                    if (hit.collider.transform.localEulerAngles.z != 0)
                    {
                        //meshCollider不做平行角
                        if (hit.collider is MeshCollider)
                        {
                            m_normalCameraAngle = m_tempNormalCameraAngle;
                            return;
                        }
                        //碰撞盒碰撞
                        Vector3 colliderNormal = hit.collider.transform.rotation * Vector3.forward;
                        //Debuger.Log(Mathf.Acos(Vector3.Dot(colliderNormal, Vector3.up)) * 180.0f / Mathf.PI);
                        float angle = Mathf.Acos(Vector3.Dot(colliderNormal, Vector3.up)) * 180.0f / Mathf.PI;
                        if (Vector3.Dot(m_cameraRoot.forward, m_player.forward) < 0)
                        {
                            //反向判断上下坡
                            if (m_player.position.y - m_tempY > 0)
                            {
                                //上坡
                                m_normalCameraAngle = angle;
                            }
                            else
                            {
                                //下坡
                                m_normalCameraAngle = -angle;
                            }
                        }
                        else
                        {
                            //同向判断上下坡
                            if (m_player.position.y - m_tempY > 0)
                            {
                                //上坡
                                m_normalCameraAngle = -angle;
                            }
                            else
                            {
                                //下坡
                                m_normalCameraAngle = angle;
                            }
                        }
                    }
                    else
                    {
                        m_normalCameraAngle = m_tempNormalCameraAngle;
                    }
                }
                else
                {
                    m_normalCameraAngle = m_tempNormalCameraAngle;
                }
            }
            m_tempY = m_player.position.y;
            m_tempPos = m_player.position;
        }

        //设置是否可见
        void SetRenderShow(GameObject go, bool show)
        {
            MeshRenderer[] renders = go.GetComponentsInChildren<MeshRenderer>();
            if (renders != null)
            {
                for (int i = 0; i < renders.Length; i++)
                {
                    renders[i].enabled = show;
                }
            }
        }

        void SetHeight()
        {
            //CameraTodo
            //if (ObjectManage.Instance.mainPlayer.m_stateManage.curStateName == StateManage.StateName.Float)
            //    return;
            Vector3 playerTarget = m_player.position + Vector3.up * m_playerHeight;
            if (m_playerDistance <= m_tempDistance)
            {
                CaculateHeight(ref playerTarget);
            }
        }

        //void OnGUI()
        //{
        //    if (!isChooseTarget)
        //        return;
        //    Vector3 targePos = chooseTargetTrans.position;

        //    //目标和玩家高度点距离
        //    float targetPlayerDis = Vector3.Distance(targePos, m_player.position + Vector3.up * m_playerHeight);
        //    //目标摄像机法线距离

        //    GUI.Label(new Rect(200, 200, 200, 50), m_norPosSpeed.ToString() + "   " + m_norRotSpeed.ToString());
        //}

        void OnDrawGizmos()
        {
            if (m_mainCamera != null && m_lockTarget != null && m_player != null && m_player.transform != null)
            {
                Gizmos.color = Color.blue;

                Vector3 cameraToTarget = m_lockTarget.transform.position + Vector3.up * m_targetHeight;
                Vector3 cameraToPlayer = m_player.transform.position + Vector3.up * m_playerHeight;

                Gizmos.DrawLine(m_mainCamera.transform.position, cameraToTarget);
                Gizmos.DrawLine(m_mainCamera.transform.position, cameraToPlayer);
            }
            if (m_mainCamera != null && m_player != null && m_player.transform != null)
                Gizmos.DrawWireCube(m_followPos, 0.3f * Vector3.one);
        }
    }
}
