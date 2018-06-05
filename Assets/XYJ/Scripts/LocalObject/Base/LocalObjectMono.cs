namespace xys
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class LocalObjectMono : MonoBehaviour
    {
        public ILocalObject m_obj { get; private set; }

        public void Init(ILocalObject obj)
        {
            this.m_obj = obj;
        }

        void OnDestory()
        {
            m_obj = null;
        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
