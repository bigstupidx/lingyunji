using System;
using UnityEngine;
using System.Collections;

namespace FMODUnity
{
    public class StudioListenerAuto
    {
        public StudioListenerAuto(int number)
        {
            ListenerNumber = number;
        }

        int ListenerNumber = 0;

        public void OnDisable()
        {
            RuntimeManager.HasListener[ListenerNumber] = false;
        }

        public void OnEnable()
        {
            RuntimeManager.HasListener[ListenerNumber] = true;
        }

        Camera mainCamera; // 当前主相机
        GameObject mainObject;
        Rigidbody rigidBody;

        public void Update()
        {
            Camera current = Camera.main;
            if (current != mainCamera && current != null)
            {
                mainCamera = current;
                if (mainCamera != null)
                {
                    rigidBody = mainCamera.GetComponent<Rigidbody>();
                    mainObject = mainCamera.gameObject;
                }
            }

            if (mainObject == null)
                return;

            RuntimeManager.HasListener[ListenerNumber] = true;
            RuntimeManager.SetListenerLocation(ListenerNumber, mainObject, rigidBody);
        }
    }
}
