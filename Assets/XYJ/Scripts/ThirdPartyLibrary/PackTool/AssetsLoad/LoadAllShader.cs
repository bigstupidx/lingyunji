#if USE_RESOURCESEXPORT && (UNITY_EDITOR || COM_DEBUG)
using PackTool;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Profiling;
using System.Collections.Generic;

public class LoadAllShader : MonoBehaviour
{
    List<string> shaders = new List<string>();

    [SerializeField]
    Button button;

    [SerializeField]
    Text text;

    void SetChild(Transform parent, Transform local)
    {
        local.parent = parent;
    }

    List<string> files = new List<string>();

    List<KV> memorys = new List<KV>();

    int total = 0;

    public void OnNext()
    {
        if (files.Count == 0)
        {
            text.text = "没有了";
            return;
        }

        string file = files[files.Count - 1];
        files.RemoveAt(files.Count - 1);
        string dstfile = string.Empty;
        int offset = 0;
        StreamingAssetLoad.GetFile(file, out dstfile, out offset);
        AssetBundle ab = AssetBundle.LoadFromFile(dstfile, 0, (ulong)offset);
        Shader s = (ab.mainAsset as GameObject).GetComponent<ShaderExport>().mShader;
        ab.Unload(false);
        text.text = string.Format("加载shader:{0} {1}/{2}", file, files.Count, total);
    }

    class KV
    {
        public KV(string f, uint t)
        {
            file = f;
            total = t;
        }

        public string file;
        public uint total;

        public override string ToString()
        {
            return string.Format("total:{0} file:{1}", XTools.Utility.ToMb(total), file);
        }
    }

    public void OnResources()
    {
        Do((string file) => { return file.Contains("/Resources/"); });
    }

    public void OnBuiltin()
    {
        Do((string file) => { return file.Contains("/BuiltinShader/"); });
    }

    public void OnCustom()
    {
        Do((string file) => { return !file.Contains("/BuiltinShader/") && !file.Contains("/Resources/"); });
    }

    void Do(System.Func<string, bool> fun)
    {
        memorys.Clear();
        uint allt = 0;
        for (int i = 0; i < files.Count; ++i)
        {
            if (!fun(files[i]))
                continue;

            AssetBundle ab = AssetBundle.LoadFromFile(ResourcesPath.streamingAssetsPath + files[i]);
            Shader s = (ab.mainAsset as GameObject).GetComponent<ShaderExport>().mShader;
            uint v = (uint)Profiler.GetRuntimeMemorySizeLong(s);
            ab.Unload(true);
            memorys.Add(new KV(files[i], v));

            allt += v;
        }

        memorys.Sort((KV x, KV y) => { return y.total.CompareTo(x.total); });
        uint top_10 = 0;
        for (int i = 0; i < memorys.Count; ++i)
        {
            if (i >= 10)
                break;

            top_10 += memorys[i].total;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendFormat("Total:{0}/{1} 前十L{2}\n", XTools.Utility.ToMb(allt), memorys.Count, XTools.Utility.ToMb(top_10));
        for (int i = 0; i < memorys.Count; ++i)
        {
            sb.AppendLine(string.Format("{0}):{1}", i, memorys[i].ToString()));
            if (sb.Length >= 5000)
                break;
        }

        text.text = sb.ToString();

    }

    public void OnAuto()
    {
        Do((string f) => { return true; });
    }

    // Use this for initialization
    void Start()
    {
        button.gameObject.SetActive(false);
        StreamingAssetLoad.EachAllFile((string file) =>
        {
            if (file.EndsWith(".us"))
            {
                string n = file;
                files.Add(n);
                ++total;
//                 Button b = Instantiate<Button>(button);
//                 SetChild(transform, b.transform);
//                 b.gameObject.SetActive(true);
//                 b.GetComponentInChildren<Text>().text = System.IO.Path.GetFileNameWithoutExtension(n);
//                 b.onClick.AddListener(() =>
//                 {
//                     AssetBundle ab = AssetBundle.LoadFromFile(ResourcesPath.streamingAssetsPath + n);
//                     Shader s = (ab.mainAsset as GameObject).GetComponent<ShaderExport>().mShader;
//                     ab.Unload(false);
//                     text.text = "加载shader:" + n;
//                 });
            }
        });

        text.text = "总数:" + files.Count;
    }
}
#endif