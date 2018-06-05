using UnityEngine;
using System.Collections;

public class MyUICenterOnChild : UICenterOnChild
{
    public delegate void OnFinishedCallback(string name);
    public OnFinishedCallback onFinishedHandler;
    public OnFinishedCallback onPressedHandler;

    // Use this for initialization
    new void Start()
    {
        Recenter();
        onFinished = _OnFinished;

        //mScrollView.onPressed = _onPressed;
    }

    void _OnFinished()
    {
        if (onFinishedHandler != null)
        {
            onFinishedHandler(centeredObject.name);
        }

       // Debug.Log("-----_OnFinished----------");
    }

    void _onPressed()
    {

       // Debug.Log("-----_onPressed---------- " + centeredObject.name);

        if (onPressedHandler != null)
        {
            onPressedHandler(centeredObject.name);
        }
    }

    

    

}
