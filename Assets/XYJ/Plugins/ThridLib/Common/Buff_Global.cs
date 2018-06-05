#if !NOPOOL
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XTools
{
    public class AllBuff
    {
#if COM_DEBUG
        public interface Buf
        {
            void GetBufInfo(System.Text.StringBuilder sb);
        }

        static List<Buf> BufList = new List<Buf>();

        static public void Add(Buf b)
        {
            BufList.Add(b);
        }

        public static void GetBuffInfo(System.Text.StringBuilder sb)
        {
            for (int i = 0; i < BufList.Count; ++i)
            {
                BufList[i].GetBufInfo(sb);
            }
        }
#endif
    }
}
#endif