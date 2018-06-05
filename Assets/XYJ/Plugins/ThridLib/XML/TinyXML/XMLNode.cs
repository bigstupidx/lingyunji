using System.Collections.Generic;

namespace TinyXML
{
    public class XMLNode
    {
        XMLNode()
        {

        }

        public string text = string.Empty; // 文本

        public string name = string.Empty;

        // 子结点
        public List<XMLNode> childList = new List<XMLNode>();

        // 属性列表
        public Dictionary<string, string> attributes = new Dictionary<string, string>();

        // 父结点
        public XMLNode parent = null;

        public void Release()
        {
            text = string.Empty;
            name = string.Empty;
            parent = null;
            attributes.Clear();

            for (int i = 0; i < childList.Count; ++i)
                childList[i].Release();
            childList.Clear();

            if (xmlNodes != null)
                xmlNodes.Add(this);
        }

        static List<XMLNode> xmlNodes = null;

        public static void Init(int lenght = 256)
        {
            if (xmlNodes == null)
            {
                xmlNodes = new List<XMLNode>(256);
                for (int i = 0; i < lenght; ++i)
                    xmlNodes.Add(new XMLNode());
            }
        }

        public static void ReleaseAll()
        {
            if (xmlNodes != null)
            {
                xmlNodes.Clear();
                xmlNodes = null;
            }
        }

        public static XMLNode Get()
        {
            if (xmlNodes == null || xmlNodes.Count == 0)
                return new XMLNode();

            XMLNode node = xmlNodes[xmlNodes.Count - 1];
            xmlNodes.RemoveAt(xmlNodes.Count - 1);
            return node;
        }

        public XMLNode GetChildByName(string name)
        {
            for (int i = 0; i < childList.Count; ++i)
            {
                if (childList[i].name == name)
                    return childList[i];
            }

            return null;
        }

        public XMLNode Get(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                return null;

            string[] s = filepath.Split('/');
            XMLNode n = GetChildByName(s[0]);
            for (int i = 1; i < s.Length; ++i)
            {
                n = n.GetChildByName(s[i]);
                if (n == null)
                    return null;
            }

            return n;
        }
    }
}