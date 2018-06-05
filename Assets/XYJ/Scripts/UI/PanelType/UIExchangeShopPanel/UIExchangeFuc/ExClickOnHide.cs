using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExClickOnHide : MonoBehaviour
{

    private int ExOnlyId;

    private void OnEnable()
    {
        ExOnlyId = xys.UI.EventHandler.pointerClickHandler.Add(OnPress);
    }

    bool OnPress(GameObject gobj, BaseEventData basedata)
    {
        if (gobj != this.gameObject)
        {
            this.transform.parent.gameObject.SetActive(false);
        }
        return true;
    }

    private void OnDisable()
    {
        xys.UI.EventHandler.pointerClickHandler.Remove(ExOnlyId);
    }
}
