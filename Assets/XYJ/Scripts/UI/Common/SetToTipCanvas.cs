using UnityEngine;
using System.Collections;

public class SetToTipCanvas : MonoBehaviour
{
	void OnEnable ()
    {
#if !SCENE_DEBUG
        transform.SetParent(xys.App.my.uiSystem.TipCanvas.transform);
        transform.localPosition = Vector3.zero;
        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.zero;
#endif
    }
}
