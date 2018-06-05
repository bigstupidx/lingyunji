using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys.UI;

namespace xys
{
    //摄像机手势相关
    public partial class CameraManager : MonoBehaviour
    {
        public DuringType GetDuringType()
        {
            return m_DuringType;
        }

        //改变手势滑动的镜头旋转速度
        public void ChangeRotateSpeed(float speed1, float speed2)
        {
            m_dragSpeed = speed1;
        }

        //判断是否点击在ui上
        bool IsOverUI()
        {
            return App.my.uiSystem.isContain();
        }

        bool isInMain()
        {
            //CameraTodo
            GameObject go = App.my.uiSystem.PointerOverGameObject();
            if (go != null)
            {
                UIMainPanel panel = (UIMainPanel)UISystem.Get(PanelType.UIMainPanel);
                if (panel != null && go.transform.IsChildOf(panel.cachedTransform))
                {
                    // 点在主界面上了
                    return true;
                }
            }

            return false;
        }

        bool IsInJoystick()
        {
            GameObject go = App.my.uiSystem.PointerOverGameObject();
            if (go != null)
            {
                UIMainPanel panel = (UIMainPanel)UISystem.Get(PanelType.UIMainPanel);
                if (panel != null)
                {
                    RectTransform joystick = panel.transform.Find("Offset/joystick").GetComponent<RectTransform>();
                    if(joystick != null && go.transform.IsChildOf(joystick))
                    return true;
                }
            }

            return false;
        }

        //判断手势是放大还是缩小
        bool IsEnlarge(Vector2 oPoint1, Vector2 oPoint2, Vector2 nPoint1, Vector2 nPoint2)
        {
            var leng1 = Mathf.Sqrt((oPoint1.x - oPoint2.x) * (oPoint1.x - oPoint2.x) + (oPoint1.y - oPoint2.y) * (oPoint1.y - oPoint2.y));
            var leng2 = Mathf.Sqrt((nPoint1.x - nPoint2.x) * (nPoint1.x - nPoint2.x) + (nPoint1.y - nPoint2.y) * (nPoint1.y - nPoint2.y));
            if (leng1 < leng2)
            {
                //放大手势
                return true;
            }
            else
            {
                //缩小手势
                return false;
            }
        }

        //相机旋转
        public void RotateCamera()
        {
            if (m_cameraRoot == null || m_player == null) return;
            if (m_duringSkill || m_specialCamera) return;
            //CameraTodo
            //if (MainTime.IsPauseInput()) return;//骑乘动画的时候不操作
            //if (LevelSystem.Instance.isChanging) return;
            if (m_isLockTarget) return;//旧锁定的时候不能操作

            if (IsOverUI() && !IsInJoystick())
            {
                if (m_duringRatate || m_duringScale)
                {
                    if (!isInMain())
                    {
                        CloseGestrues();
                        return;
                    }
                }
                else
                {
                    CloseGestrues();
                    return;
                }
            }

            if (m_DuringType != DuringType.Null)
            {
                Quaternion rotation;
                if (m_DuringType == DuringType.Forward)
                {
                    rotation = Quaternion.Euler(0, Quaternion.LookRotation(m_forwardDir).eulerAngles.y + 180, 0);
                    m_noramlCamDis = m_playerDistance = Mathf.Lerp(m_playerDistance, m_minDis, Time.deltaTime);
                    CaculateHeight(ref m_followPos);
                    m_cameraRoot.transform.rotation = Quaternion.Lerp(m_cameraRoot.transform.rotation, rotation, m_norPosSpeed * Time.deltaTime);
                    m_cameraRoot.transform.position = m_cameraRoot.transform.rotation * new Vector3(0.0f, 0.0f, -m_playerDistance) + m_followPos;
                    m_cameraRoot.transform.localEulerAngles -= new Vector3(0, 0, 1) * m_cameraRoot.transform.localEulerAngles.z;
                    m_toNorX = m_cameraRoot.transform.localEulerAngles.x;
                }
                else if (m_DuringType == DuringType.MoveBack)
                {
                    m_DuringType = DuringType.Null;
                    CloseGestrues();
                }
                else
                {
                    rotation = Quaternion.Euler(m_toNorX, Quaternion.LookRotation(m_cameraRoot.forward).eulerAngles.y, 0);
                    m_noramlCamDis = m_playerDistance = Mathf.Lerp(m_playerDistance, m_featureDis + 0.5f, Time.deltaTime);
                    CaculateHeight(ref m_followPos);
                    m_cameraRoot.transform.rotation = Quaternion.Lerp(m_cameraRoot.transform.rotation, rotation, m_norPosSpeed * Time.deltaTime);
                    m_cameraRoot.transform.position = m_cameraRoot.transform.rotation * new Vector3(0.0f, 0.0f, -m_playerDistance) + m_followPos;
                    m_cameraRoot.transform.localEulerAngles -= new Vector3(0, 0, 1) * m_cameraRoot.transform.localEulerAngles.z;
                    m_toNorX = m_cameraRoot.transform.localEulerAngles.x;
                }
                return;
            }

            Vector3 playerTarget = m_followPos;

            //旋转
            //CameraTodo
            if (GetRotateProp() /*&& !BattleSystem.IsMyLogicPause*/)
            {
                StopCoroutine("CloseRotete");
                m_duringScale = false;
                m_duringRatate = true;
                if (m_isLockTarget && m_isForceLock)
                {
                    LockMeToTarget(null);
                }

                SetExcessiveCamera(false);
            }
            if (!m_duringScale && m_duringRatate)
            {
                if (Mathf.Infinity != m_x && Mathf.Infinity != m_y)
                {
                    //m_y *= m_screenPercentage;
                    //m_x *= m_screenPercentage;
                    m_m = Mathf.Lerp(m_m, m_x, Time.deltaTime * 4);
                    m_n = Mathf.Lerp(m_n, m_y, Time.deltaTime * 4);
                    var rotation = Quaternion.Euler(m_n, m_m, 0);
                    m_cameraRoot.transform.rotation = rotation;
                    //m_cameraRoot.transform.rotation = Quaternion.Lerp(m_cameraRoot.transform.rotation, rotation, Time.deltaTime * m_dragSpeed);
                    m_cameraRoot.transform.localEulerAngles -= new Vector3(0, 0, 1) * m_cameraRoot.transform.localEulerAngles.z;

                    //判断这个位置是否在碰撞中
                    Vector3 pos = m_cameraRoot.transform.rotation * new Vector3(0.0f, 0.0f, -m_playerDistance) + playerTarget;
                    m_cameraRoot.transform.position = EnterCollider(pos);

                    //赋初始值，防止抖动
                    m_toNorX = m_cameraRoot.transform.localEulerAngles.x;
                }
            }

            //拉伸
            playerTarget = m_player.position + Vector3.up * m_playerHeight;
            if (GetScaleProp(playerTarget))       //拉伸
            {
                m_duringScale = true;
                m_duringRatate = false;
                SetExcessiveCamera(false);
            }

            if (!m_duringRatate && m_duringScale)
            {
                if (App.my.input.IsMoving()) return;
                if (m_playerDistance <= m_tempDistance && m_cameraViewType == CameraViewType.ThreeD)
                {
                    CaculateHeight(ref playerTarget);
                }
                m_cameraRoot.transform.position = playerTarget + m_normalized * m_playerDistance;

                if (m_cameraViewType == CameraViewType.ThreeD)
                {
                    if (!m_isFeature)
                    {
                        if (m_playerDistance - m_featureDis < 0.09f)
                        {
                            //进入特写镜头
                            //m_playerDistance = m_minDis;
                            //CaculateHeight(ref playerTarget);
                            OnEnterFeature(DuringType.Forward);
                        }
                    }
                    else
                    {
                        if (m_playerDistance > m_featureDis + 0.09f)
                        {
                            //退出特写镜头
                            //m_playerDistance = m_featureDis + 0.3f;
                            OnEnterFeature(DuringType.Back);
                        }
                    }
                }
            }
        }

        void CaculateHeight(ref Vector3 playerTarget)
        {
            if (!m_isFeature)
                m_playerHeight = ((m_playerDistance - m_minDis) / (m_tempDistance - m_minDis)) * (m_tempHeight - m_featureHeight) + m_featureHeight;
            else
                m_playerHeight = (m_playerDistance - m_minDis) * ((m_featureHeight - m_twoStageHeight) / (m_minDis - m_twoStageMinDis)) + m_featureHeight;

            if (m_playerHeight < m_featureHeight)
            {
                m_playerHeight = m_featureHeight;
            }

            if (m_playerHeight > m_twoStageHeight && m_isFeature)
            {
                m_playerHeight = m_twoStageHeight;
            }
            playerTarget = m_player.position + Vector3.up * m_playerHeight;
        }



        //转变到普通
        public void UpdateToNormal()
        {
            if (m_toNor && m_cameraViewType == CameraViewType.ThreeD && !m_excessiveMove)
            {
                m_toNorX = Mathf.LerpAngle(m_cameraRoot.transform.localEulerAngles.x, m_normalCameraAngle, m_speedToNor * Time.deltaTime);
                m_toNor = false;
            }
        }

        float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;

            if (angle > 300 && angle < 360)
            {
                angle -= 360;
            }

            return Mathf.Clamp(angle, min, max);
        }

        float ClampYRotate(float angle)
        {
            if (Mathf.Abs(angle) > 10)
            {
                if (angle < 0)
                {
                    return -10;
                }
                else
                {
                    return 10;
                }
            }
            return angle;
        }

        //当手势结束和切换到锁定状态的时候，关闭手势，并初始化
        public void CloseGestrues()
        {
            StopCoroutine("CloseDuringFeature");
            StopCoroutine("CloseRotete");
            m_duringRatate = false;
            m_duringScale = false;
        }

        public void LerpToNormal(bool isRun)
        {
            m_isRunning = isRun;
            if (!isRun) { return; }
            m_toNor = true;

            if (m_isFeature)
            {
                m_playerDistance = m_featureDis + 0.3f;
                OnEnterFeature(DuringType.MoveBack);
            }
            m_duringScale = false;
            //CloseGestrues();
        }

        //获取旋转的参数
        bool GetRotateProp()
        {
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    InitDragPara();
                }
                if (Input.GetMouseButton(1))        //旋转,电脑上用固定数值代替
                {
                    float x = Input.GetAxis("Mouse X") * m_dragOffset * 5;
                    x = ClampYRotate(x);
                    m_x += x;
                    if (m_cameraViewType == CameraViewType.ThreeD)
                        m_y -= Input.GetAxis("Mouse Y") * m_dragOffset * 5;

                    m_y = ClampAngle(m_y, m_rotMinAngle, m_rotMaxAngle);
                    return true;
                }
                if (Input.GetMouseButtonUp(1))
                {
                    StartCoroutine("CloseRotete");
                    //m_duringRatate = false;
                    return false;
                }
            }
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount == 1)
                {
                    if (App.my.input.IsMoving() || IsInJoystick()) return false; //如果只是摇杆，则不让旋转
                    Touch touch = Input.GetTouch(0);

                    if (touch.phase == TouchPhase.Began)
                    {
                        InitDragPara();
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        float x = Input.GetTouch(0).deltaPosition.x * m_dragOffset;
                        x = ClampYRotate(x);
                        m_x += x;
                        if (m_cameraViewType == CameraViewType.ThreeD)
                            m_y -= Input.GetTouch(0).deltaPosition.y * m_dragOffset;

                        m_y = ClampAngle(m_y, m_rotMinAngle, m_rotMaxAngle);
                        return true;
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        StartCoroutine("CloseRotete");
                        //m_duringRatate = false;
                        return false;
                    }
                }
                else if (Input.touchCount == 2)
                {
                    if (App.my.input.IsMoving())//如果是2个触摸必须有个是摇杆才能旋转
                    {
                        Touch touch1 = Input.GetTouch(0);
                        Touch touch2 = Input.GetTouch(1);

                        if (touch2.phase == TouchPhase.Began)
                        {
                            InitDragPara();
                        }

                        //这里需要判断下是哪个点击在ui上
                        if (touch1.position.x < Screen.width / 2 && touch2.position.x > Screen.width / 2)
                        {
                            if (touch2.phase == TouchPhase.Moved)
                            {
                                m_x += touch2.deltaPosition.x * m_dragOffset * m_mobileProp;
                                if (m_cameraViewType == CameraViewType.ThreeD)
                                    m_y -= touch2.deltaPosition.y * m_dragOffset * m_mobileProp;

                                m_y = ClampAngle(m_y, m_rotMinAngle, m_rotMaxAngle);
                                return true;
                            }

                            if (touch2.phase == TouchPhase.Ended)
                            {
                                StartCoroutine("CloseRotete");
                                //m_duringRatate = false;
                                return false;
                            }
                        }
                        else if (touch2.position.x < Screen.width / 2 && touch1.position.x > Screen.width / 2)
                        {
                            if (touch1.phase == TouchPhase.Moved)
                            {
                                m_x += touch1.deltaPosition.x * m_dragOffset * m_mobileProp;
                                if (m_cameraViewType == CameraViewType.ThreeD)
                                    m_y -= touch1.deltaPosition.y * m_dragOffset * m_mobileProp;

                                m_y = ClampAngle(m_y, m_rotMinAngle, m_rotMaxAngle);
                                return true;
                            }

                            if (touch1.phase == TouchPhase.Ended)
                            {
                                StartCoroutine("CloseRotete");
                                //m_duringRatate = false;
                                return false;
                            }
                        }
                        else
                        {
                            //CloseGestrues();
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        bool GetScaleProp(Vector3 playerTarget)
        {
            if (m_isLockTarget && m_isForceLock) return false;//锁定镜头不做处理
                                                                         //进入碰撞的时候不能缩放
            if (m_enterCollider)
            {
                return false;
            }
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                if (App.my.input.IsMoving())
                {
                    return false;
                }
                if (Input.GetAxis("Mouse ScrollWheel") != m_ScrollWheel)       //拉伸
                {
                    m_ScrollWheel = Input.GetAxis("Mouse ScrollWheel");
                    m_normalized = (m_cameraRoot.transform.position - playerTarget).normalized;

                    if (m_playerDistance >= m_minDis && m_playerDistance <= m_maxDis)
                    {
                        m_playerDistance -= Input.GetAxis("Mouse ScrollWheel");
                    }
                    if (!m_isFeature)
                    {
                        //还没有进入特写镜头
                        if (m_playerDistance < m_minDis)
                        {
                            m_playerDistance = m_minDis;
                        }
                    }
                    else
                    {
                        if (DuringType.Null == m_DuringType)
                        {
                            //进入了特写镜头，还能继续拉近
                            //Debuger.Log("进入了特写镜头，还能继续拉近");
                            m_playerDistance -= Input.GetAxis("Mouse ScrollWheel");

                            if (m_playerDistance < m_twoStageMinDis)
                            {
                                m_playerDistance = m_twoStageMinDis;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }

                    if (m_playerDistance > m_maxDis)
                    {
                        m_playerDistance = m_maxDis;
                    }
                    //2.5d缩放
                    m_2_5DPlayerDistance = m_playerDistance;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount == 2)
                {
                    if (!App.my.input.IsMoving())
                    {
                        if (Input.GetTouch(1).phase == TouchPhase.Began)
                        {
                            m_currDis = m_lastDis = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                        }

                        if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                        {
                            Touch touch1 = Input.GetTouch(0);
                            Touch touch2 = Input.GetTouch(1);

                            m_currDis = Vector2.Distance(touch1.position, touch2.position);
                            if (m_currDis > m_lastDis)
                            {
                                if (!m_isFeature)
                                    m_playerDistance -= Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition) * m_scaleSpeed;
                            }
                            else
                            {
                                if (m_isFeature)
                                    m_playerDistance += 0.7f;
                                else
                                    m_playerDistance += Vector2.Distance(touch1.deltaPosition, touch2.deltaPosition) * m_scaleSpeed;
                            }

                            if (!m_isFeature)
                            {
                                //还没有进入特写镜头
                                if (m_playerDistance < m_minDis)
                                {
                                    m_playerDistance = m_minDis;
                                }
                            }
                            else
                            {
                                if (DuringType.Null == m_DuringType)
                                {
                                    //进入了特写镜头，还能继续拉近
                                    m_playerDistance -= Time.deltaTime * 6f;

                                    if (m_playerDistance < m_twoStageMinDis)
                                    {
                                        m_playerDistance = m_twoStageMinDis;
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                            if (m_playerDistance > m_maxDis)
                            {
                                m_playerDistance = m_maxDis;
                            }

                            m_lastDis = m_currDis;

                            m_normalized = (m_cameraRoot.transform.position - playerTarget).normalized;

                            //2.5d缩放
                            m_2_5DPlayerDistance = m_playerDistance;

                            return true;
                        }

                        if (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(1).phase == TouchPhase.Ended)
                        {
                            //m_duringScale = false;
                            return false;
                        }
                    }
                }
            }
            return false;
        }

        void OnEnterFeature(DuringType type)
        {
            m_duringScale = true;
            bool enter = type == DuringType.Forward ? true : false;
            m_DuringType = type;
            //CameraTodo
            //EffectDepthOfField(enter);
            m_isFeature = enter;
            if (m_DuringType == DuringType.Forward)
            {
                m_forwardDir = m_player.forward;
                StartCoroutine(CloseDuringFeature());
            }

            else if (m_DuringType == DuringType.Back)
            {
                StartCoroutine(CloseDuringFeature());
            }
            //CameraTodo
            //Utils.EventDispatcher.Instance.TriggerEvent<bool>(EventDefine.RoleEvents.G_EnterFeatureCamera, enter);
        }
    }
}
