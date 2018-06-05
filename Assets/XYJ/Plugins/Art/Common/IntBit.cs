using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class IntBit
{
    public IntBit(int v)
    {
        value = v;
    }

    public IntBit()
    {
        value = 0;
    }
   
    public int value;
 
    public bool this[int pos]
    {
        get { return Get(pos); }
        set { Set(pos, value); }
    }

    public bool Get(int pos)
    {
        if ((value & (1 << pos)) == 0)
            return false;

        return true;
    }

    public void Set(int pos, bool v)
    {
        if (v == true)
        {
            value = value | (1 << pos);
        }
        else
        {
            value = value & (~(1 << pos));
        }
    }
}