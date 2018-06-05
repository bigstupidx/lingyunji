using System.Collections.Generic;

namespace xys
{
    public class ParserSymbol
    {
        public delegate string SwitchKey(string key, object obj);

        class Data
        {
            public string key;
            public SwitchKey fun;

            public Data(string k, SwitchKey f)
            {
                key = k;
                fun = f;
            }
        }

        List<Data> mDataList = new List<Data>();

        Data Find(string key)
        {
            for (int i = 0; i < mDataList.Count; ++i)
            {
                if (mDataList[i].key == key)
                    return mDataList[i];
            }

            return null;
        }

        public void AddKey(string key, SwitchKey fun)
        {
            if (Find(key) != null)
            {
                Debuger.ErrorLog("ParserSymbol key:{0} repeat!", key);
                return;
            }

            mDataList.Add(new Data(key, fun));
        }

        public void Add(string key, SwitchKey fun)
        {
            mDataList.Add(new Data(key, fun));
        }

        //static System.Text.StringBuilder s_sb = new System.Text.StringBuilder();

        // text从start位置起，是否包含了key,nkey是最终查找到的字符串
        static bool IsSame(string text, int start, string key, out string nkey)
        {
            nkey = null;
            if (text.Length - start < key.Length)
                return false;

            // 要先判断下，是否含有通配符，只支持'*'至少要匹配1个
            int code = key.IndexOf('*');
            if (code == -1)
            {
                int lenght = key.Length;
                for (int i = 0; i < lenght; ++i)
                {
                    if (text[start + i] != key[i])
                        return false;
                }

                nkey = key;
                return true;
            }
            else
            {
                for (int i = 0; i < code; ++i)
                {
                    if (text[start + i] != key[i])
                        return false;
                }

                int pos = text.IndexOf(key.Substring(code + 1), start + code);
                if (pos != -1)
                {
                    nkey = text.Substring(start, pos - start + 1);
                    return true;
                }

                return false;
            }
        }

        Data Find(string text, int startid, out string nk)
        {
            Data d = null;
            for (int i = 0; i < mDataList.Count; ++i)
            {
                d = mDataList[i];
                if (IsSame(text, startid, d.key, out nk))
                    return d;
            }

            nk = null;
            return null;
        }

        public string To(string text, object obj = null)
        {
            if (string.IsNullOrEmpty(text))
                return "";

            System.Text.StringBuilder sb = new System.Text.StringBuilder(text.Length);
            Data d = null;
            string nk = null;
            for (int i = 0; i < text.Length;)
            {
                if ((d = Find(text, i, out nk)) == null)
                {
                    sb.Append(text[i]);
                    ++i;
                }
                else
                {
                    sb.Append(d.fun(nk, obj));
                    i += nk.Length;

                    nk = null;
                }
            }

            return sb.ToString();
        }
    }
}