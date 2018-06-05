using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace xys.UI
{
    public class Acceleration : MonoBehaviour
    {
        [System.Serializable]
        public class Element
        {
            public RectTransform trans;

            public float Speed = 1.0f;

            [System.NonSerialized]
            public Vector3 startPosition;

            [System.NonSerialized]
            public Vector2 offsetPosition;

            public void Init()
            {
                startPosition = trans.anchoredPosition;
                offsetPosition = Vector3.zero;
            }

            public void LateUpdate(float delta)
            {
                if (offsetPosition.sqrMagnitude <= 0.1f)
                {
                    trans.anchoredPosition = startPosition;
                    return;
                }

                Vector3 localPosition = trans.anchoredPosition;

                Vector3 newPos = Vector3.Lerp(localPosition - startPosition, offsetPosition, delta * Speed);
                trans.anchoredPosition = startPosition + newPos;
            }
        }

        [SerializeField]
        Element[] elements;

        [SerializeField]
        Vector2 Shaft = Vector3.zero;

        [SerializeField]
        float Speed = 1f;

        void Start()
        {
            if (elements == null || elements.Length == 0)
                enabled = false;
            else
            {
                for (int i = 0; i < elements.Length; ++i)
                {
                    elements[i].Init();
                }
            }
        }

        void UpdateByPosition()
        {
            if (elements == null || elements.Length == 0)
                return;

            float delta = Time.unscaledDeltaTime;
            for (int i = 0; i < elements.Length; ++i)
                elements[i].LateUpdate(delta * Speed);
        }

        float last_update_time = -1f;

#if UNITY_EDITOR
        Vector3 testOffset;
#endif

        public Vector3 acceleration
        {
            get
            {
#if UNITY_EDITOR
                return testOffset;
#else
            Vector3 acc = Input.acceleration;
            acc.z = 0f;
            return acc;
#endif

            }
        }


        void Update()
        {
            UpdateByPosition();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.A))
            {
                testOffset.x = Random.Range(-1f, 1f);
                testOffset.y = Random.Range(-1f, 1f);
                testOffset.z = 0;
                //testOffset.Normalize();
            }
#endif

            if (elements == null || elements.Length == 0)
                return;

            float realtimeSinceStartup = Time.realtimeSinceStartup;
            if (last_update_time == -1f)
                last_update_time = realtimeSinceStartup;
            else if (realtimeSinceStartup - last_update_time <= 0.1f)
                return;

            last_update_time = realtimeSinceStartup;
            Vector2 offset = acceleration;
            offset.x *= Shaft.x;
            offset.y *= Shaft.y;
            for (int i = 0; i < elements.Length; ++i)
                elements[i].offsetPosition = offset;
        }
    }
}