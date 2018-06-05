#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XTools;

// 加载一份材质
namespace PackTool
{
    public class MaterialLoad : AssetLoad<Material>
    {
        static ResourceStream s_rs = new ResourceStream();

        class MatData
        {
            public int total;
        }

        void OnTextureLoadEnd(Texture t, object p)
        {
            object[] pp = p as object[];
            MatData md = (MatData)pp[0];
            string name = pp[1] as string;

            if (asset != null)
            {
                asset.SetTexture(name, t);
            }
            else
            {
                Debuger.ErrorLog("MaterialLoad:{0} asset = null!", url);
            }

            --md.total;
            if (md.total == 0)
            {
                AddDepRef(); // 增加依赖的计数
                OnEnd();
            }
        }

        protected override void LoadAsset(string name)
        {
            if (name.LastIndexOf('.') == -1)
            {
                asset = BuiltinResource.Instance.GetMat(name);
                if (asset == null)
                {
                    Debuger.LogError(string.Format("BuiltinResource mat:{0} not find!", name));
                }

                AddRef(); // 内置材质不需要删除
                NextFrame();
                return;
            }
            else if (name.StartsWith(":"))
            {
                // Resources目录下的资源
                asset = Resources.Load<Material>(name.Substring(1, name.Length - 5));
                if (asset == null)
                {
                    Debuger.LogError(string.Format("Resources mat:{0} not find!", name.Substring(1, name.Length - 5)));
                }

                NextFrame();
                return;
            }

            BuiltinResource.MaterialData md = BuiltinResource.Instance.GetMaterial(name.GetHashCode());
            if (md == null)
            {
                Debuger.LogError(string.Format("mat:{0} not find!", name));
                asset = null;
                NextFrame();
                return;
            }
            asset = new Material(md.mat);
            asset.name = md.mat.name;

            System.IO.Stream stream = ResourcesPack.FindBaseStream(name);
            if (stream == null || stream.Length == 0)
            {
                NextFrame();
                return;
            }

            System.IO.BinaryReader reader = new System.IO.BinaryReader(stream);
            byte total = reader.ReadByte();
            MatData matd = new MatData();
            matd.total = 0;
            for (int i = 0; i < total; ++i)
            {
                string textName = reader.ReadString();
                s_rs.Reader(reader);
                if (s_rs.GetRes())
                {
                    ++matd.total;
                    AddDep(s_rs.Key);
                    TextureLoad.Load(s_rs.Key, OnTextureLoadEnd, new object[] { matd, textName });
                }
            }

            reader.Close();
            stream.Close();

            if (matd.total == 0)
            {
                NextFrame();
            }
        }

        static public MaterialLoad.Data Load(string name, ResourcesEnd<Material> fun, object funp)
        {
            return MaterialLoad.LoadImp(name, fun, funp, Create);
        }

        static MaterialLoad Create(string name)
        {
            return CreateAsset<MaterialLoad>(name);
        }

        // 回收自身
        protected override void FreeSelf()
        {
            FreeAsset<MaterialLoad>(this);
        }
    }
}
#endif