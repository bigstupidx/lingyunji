#if MEMORY_CHECK
using XTools;
using Network;
using System.Collections.Generic;

// 加载基础资源
namespace PackTool
{
    public class AssetSyncInfo
    {
        public List<string> textures = new List<string>(); // 纹理
        public List<string> materials = new List<string>(); // 材质
        public List<string> atlas = new List<string>(); // 图集
        public List<string> meshs = new List<string>(); // 网格
        public List<string> controls = new List<string>(); // 动画控制器
        public List<string> avatars = new List<string>(); // Avatars
        public List<string> audioclips = new List<string>(); // 声音资源
        public List<PrefabBeh.Data> mPrefabs = new List<PrefabBeh.Data>(); // 当前预置体的资源

        public long ticks;

        static void Writes(List<string> list, Network.BitStream bs)
        {
            bs.Write(list.Count);
            for (int i = 0; i < list.Count; ++i)
                bs.Write(list[i]);
        }

        public void Write(Network.BitStream bs)
        {
            bs.Write(ticks);
            Writes(textures, bs);
            Writes(materials, bs);
            Writes(atlas, bs);
            Writes(meshs, bs);
            Writes(controls, bs);
            Writes(audioclips, bs);

            bs.Write(mPrefabs.Count);
            for (int i = 0; i < mPrefabs.Count; ++i)
                mPrefabs[i].Write(bs);
        }

        static void Readers(List<string> list, Network.BitStream bs)
        {
            int count = bs.ReadInt32();
            list.Capacity += count;
            for (int i = 0; i < count; ++i)
                list.Add(bs.ReadString());
        }

        public void Reader(Network.BitStream bs)
        {
            ticks = bs.ReadInt64();
            Readers(textures, bs);
            Readers(materials, bs);
            Readers(atlas, bs);
            Readers(meshs, bs);
            Readers(controls, bs);
            Readers(audioclips, bs);

            int count = bs.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                PrefabBeh.Data d = new PrefabBeh.Data();
                d.Reader(bs);
                mPrefabs.Add(d);
            }
        }

        public string Text
        {
            get
            {
                return string.Format("time:{8} 贴图:{0} 材质:{1} 图集:{2} 网格:{3} 控制器:{4} Avatar:{5} 音频:{6} 预置体:{7}", textures.Count, materials.Count, atlas.Count, meshs.Count, controls.Count, avatars.Count, audioclips.Count, mPrefabs.Count, GetTime());
            }
        }

        public string GetTime()
        {
            System.DateTime time = new System.DateTime(ticks);
            return string.Format("{0}:{1}:{2}", time.Hour, time.Minute, time.Second);
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
#endif