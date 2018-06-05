using UnityEngine;
using System.Collections;


/// <summary>
/// 创建一把御气的武器,特效名字等于武器名字加上extraName
/// </summary>
public class EffectCreateWeapon_YuQi : EffecttBaseEvent 
{
	public string extraName = "_yuqi";
    GameObject m_weapon;

    protected override void PlayEvent(AnimationEffectManage effectmanage)
    {
        ModelPartSetting1 set = effectmanage.GetComponent<ModelPartSetting1>();
        if (set == null)
            return;
		string weaponName = set.GetPartName(ModelPartType.Weapon_R) + extraName;
        XYJObjectPool.LoadPrefab(weaponName, OnLoad, weaponName);
    }

    void OnLoad( GameObject go,object para )
    {
        if (go == null)
        {
            Debuger.LogError("找不到剑士的御气武器 " + (string)para);
            return;
        }

        if (this.gameObject.activeSelf)
        {
            go.transform.parent = this.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            m_weapon = go;
        }
        else
        {
            DestroyWeapon(go);
        }

    }

    //武器也要使用对象池
    void DestroyWeapon(GameObject weapon)
    {
        if (weapon != null)
        {
            XYJObjectPool.Destroy(weapon);
        }
        m_weapon = null;
    }

    void OnDisable()
    {
        DestroyWeapon(m_weapon);
    }
     
}
