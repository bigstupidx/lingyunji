using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoleSkinHandle
{

    #region Org Texture Cache

    static Dictionary<Texture2D, Color[]> _texCache = new Dictionary<Texture2D, Color[]>();

    public static Color[] GetTexturePixels(Texture2D tex)
    {
        if (!Application.isPlaying) //非运行时就不要缓存了，万一图片改了不能实时刷新
            return tex.GetPixels();

        Color[] clrs = null;
        if (!_texCache.TryGetValue(tex, out clrs))
        {
            clrs = tex.GetPixels();
            _texCache[tex] = clrs;
        }

        int size = tex.width * tex.height;
        Color[] copyClrs = new Color[size];
        System.Array.Copy(clrs, 0, copyClrs, 0, size);
        return copyClrs;
    }

    public static void ClearTexCache()
    {
        _texCache.Clear();
    }

    #endregion

    // 贴图融合从底层到最顶层顺序：唇色  晒红  面纹  眼线  眼影  眉毛  胡子

    Texture2D m_targetTex;
    SkinnedMeshRenderer m_meshRenderer;

    RoleSkinPart m_config;
    RoleSkinPartData m_data;
    RoleSkinPartData m_defaultData;

    public RoleSkinPart GetConfig()
    {
        return m_config;
    }

    public RoleSkinPartData GetData()
    {
        return m_data;
    }

    /// <summary>
    /// 初始化配置和数据
    /// </summary>
    /// <param name="config"></param>
    /// <param name="data"></param>
    /// <param name="defaultData"></param>
    public void InitInfo(RoleSkinPart config, RoleSkinPartData data, RoleSkinPartData defaultData)
    {
        m_config = config;
        m_data = data;
        if (m_data == null)
            m_data = new RoleSkinPartData();

        if (m_defaultData == null)
            m_defaultData = new RoleSkinPartData();
        m_defaultData.Clone(defaultData);
    }

    /// <summary>
    /// 初始化模型
    /// </summary>
    /// <param name="renderer"></param>
    public void InitModel(SkinnedMeshRenderer renderer)
    {
        m_meshRenderer = renderer;

        SetData(m_data);
    }

    void ResetTexture()
    {
        if (m_targetTex != null)
            Object.DestroyImmediate(m_targetTex);

        if (m_config != null && m_config.units.Count > 0)
        {
            m_targetTex = new Texture2D(m_config.orgTexture.width, m_config.orgTexture.height, TextureFormat.RGB24, true);
            m_meshRenderer.material.mainTexture = m_targetTex;
        }
    }

    /// <summary>
    /// 设置数据，并刷新模型贴图渲染
    /// </summary>
    public void SetData(RoleSkinPartData data, bool withRefresh=true)
    {
        m_data.Clone(data);

        if (withRefresh)
            RefreshTexture();
    }

    /// <summary>
    /// 设置数据，并刷新模型贴图渲染
    /// </summary>
    /// <param name="unitData"></param>
    /// <param name="withRefresh"></param>
    public void SetUnitData(RoleSkinUnitData unitData, bool withRefresh=true)
    {
        m_data.CloneUnit(unitData.key, unitData);

        if (withRefresh)
            RefreshTexture();
    }

    

    /// <summary>
    /// 重置所有化妆
    /// </summary>
    public void Reset()
    {
        m_data.Clone(m_defaultData);

        RefreshTexture();
    }

    /// <summary>
    /// 重置某个部位
    /// </summary>
    /// <param name="index"></param>
    public void ResetByIndex(int index)
    {
        string key = m_config.GetKeyName(index);
        
        if (!string.IsNullOrEmpty(key))
        {
            m_data.CloneUnit(key, m_defaultData.Get(key));

            RefreshTexture();
        }
    }

    class Info
    {
        public bool isContinue = true;
    }

#if !USE_RESOURCESEXPORT
    void CacheColor()
    {
        if (m_config == null || m_data == null || m_config.units.Count == 0)
            return;

        // 原始贴图开始混合
        m_config.orgTexture.Cache();

        foreach (var unitConfig in m_config.units)
        {
            RoleSkinUnitData unitData = m_data.Get(unitConfig.key);

            // 获取样式
            int texStyleIndex = unitData.texStyle;
            if (texStyleIndex < 0 || texStyleIndex >= unitConfig.texStyles.Count)
            {
                Debug.LogErrorFormat("不存在的纹理数据:{0}的{1}的第{2}个图片", m_config.skinKey, unitConfig.key, texStyleIndex);
                continue;
            }

            var texData = unitConfig.texStyles[texStyleIndex];
            var tex = texData.texName;
            var mask = texData.maskName == null ? unitConfig.shareMask : texData.maskName;
            var offset = texData.offset == Vector2.zero ? unitConfig.shareOffset : texData.offset;
            var mirror = texData.mirror;
            if (tex == null || mask == null)
            {
                Debug.LogErrorFormat("纹理或遮罩图为空，请检查预制体:{0}的{1}的第{2}个图片", m_config.skinKey, unitConfig.key, texStyleIndex);
                continue;
            }

            mask.Cache();
            tex.Cache();
        }
    }
#endif
    void RefreshTextureThread(Info info)
    {
        if (m_config == null || m_data == null || m_config.units.Count == 0 || !info.isContinue)
            return;

        TimeCheck tc = new TimeCheck(true);
        // 原始贴图开始混合
        Color32[] texClrs = m_config.orgTexture.GetPixels32();
        Color32[] partM = new Color32[512 * 512];
        Color32[] partC = new Color32[512 * 512];
        if (!info.isContinue)
            return;

        int src_width = m_config.orgTexture.width;
        int src_height = m_config.orgTexture.height;

        foreach (var unitConfig in m_config.units)
        {
            if (!info.isContinue)
                return;

            RoleSkinUnitData unitData = m_data.Get(unitConfig.key);

            // 获取样式
            int texStyleIndex = unitData.texStyle;
            if (texStyleIndex < 0 || texStyleIndex >= unitConfig.texStyles.Count)
            {
                Debug.LogErrorFormat("不存在的纹理数据:{0}的{1}的第{2}个图片", m_config.skinKey, unitConfig.key, texStyleIndex);
                continue;
            }
            var texData = unitConfig.texStyles[texStyleIndex];
            var tex = texData.texName;
            var mask = texData.maskName == null ? unitConfig.shareMask : texData.maskName;
            var offset = texData.offset == Vector2.zero ? unitConfig.shareOffset : texData.offset;
            var mirror = texData.mirror;
            if (tex == null || mask == null)
            {
                Debug.LogErrorFormat("纹理或遮罩图为空，请检查预制体:{0}的{1}的第{2}个图片", m_config.skinKey, unitConfig.key, texStyleIndex);
                continue;
            }

            mask.GetPixels32(ref partM);
            tex.GetPixels32(ref partC);
            if (!info.isContinue)
                return;

            HSV(unitData.h, unitData.s, unitData.v, partC, partC, tex.width * tex.height);

            // 纹理混合
            AlphaBlendMx(texClrs, partC, partM, src_width, src_height, tex.width, tex.height, (int)offset.x, (int)offset.y, false, texClrs);
            // 镜像情况再混合一次
            if (mirror)
            {
                AlphaBlendMx(texClrs, partC, partM, src_width, src_height, tex.width, tex.height, src_width / 2, (int)offset.y, true, texClrs);
            }
        }

        partM = null;
        partC = null;
        if (!info.isContinue)
            return;

        // Apply Texture
        xys.App.my.mainDispatcher.Message((Info i) => 
        {
            if (!i.isContinue)
                return;
            ResetTexture();
            m_targetTex.SetPixels32(texClrs);
            texClrs = null;

            m_targetTex.Apply(true, true);
            Debug.LogFormat("妆容耗时:{0}", tc.renew);
        }, info);
    }

    Info current_info = null;

    /// <summary>
    /// 刷新纹理图
    /// </summary>
    void RefreshTexture()
    {
#if !USE_RESOURCESEXPORT
        CacheColor();
#endif
        if (current_info != null)
            current_info.isContinue = false;

        current_info = new Info();
        xys.App.my.threadDispatcher.Message(RefreshTextureThread, current_info);
    }

    // 计算HSV值
    void HSV(float h, float s, float v, Color32[] src, Color32[] target, int lenght)
    {
        float hh, ss, vv;
        //float rr, gg, bb;
        Color rgb = new Color();
        Color32 color;
        for (int i = 0; i < lenght; i++)
        {
            color = src[i];
            //rgb = color;
            RGBToHSV(color.r, color.g, color.b, out hh, out ss, out vv);
            //RGBToHSV(rgb.r, rgb.g, rgb.b, out hh, out ss, out vv);

            hh += h;
            hh = hh % 360;

            ss *= s;
            vv *= v;

            HSVToRGB(hh, ss, vv, out rgb.r, out rgb.g, out rgb.b);
            rgb.a = color.a * color_byte_float;

            target[i] = rgb;
        }
    }

    /// <summary>
    /// 混合贴图颜色
    /// </summary>
    void AlphaBlendMx(
                    Color32[] aBottom,
                    Color32[] aTop,
                    Color32[] aMask,
                    int BotW,
                    int BotH,
                    int TopW,
                    int TopH,
                    int TopOfX,
                    int TopOfY,
                    bool mirror,//正常情况传进来false
                    Color32[] dest)
    {
        Color32 B;
        Color32 T;
        Color32 M;
        Color32 R;
        //Color B;
        //Color T;
        //Color M;
        //Color R;

        int writePixel;
        int readPixel;
        byte alpha = 0;
        for (int i = 0; i < TopH; i++)
        {
            for (int j = 0; j < TopW; j++)
            {
                writePixel = BotW * (BotH - TopOfY + i) + TopOfX + j;
                if (mirror)
                    readPixel = TopW * i + (TopW - j) - 1;
                else
                    readPixel = TopW * i + j;

                if (aBottom.Length <= writePixel)
                {
                    Debug.Log(string.Format("AlphaBlendMx 的 writePixel 大小超出范围，all={0}", writePixel));
                    continue;
                }

                B = aBottom[writePixel];
                T = aTop[readPixel];
                M = aMask[readPixel];
                //R = (T * M.a + B * (1 - M.a));//这样写很慢，估计这过程中产生了不少临时的struct，下面r、g、b分别计算快了一倍
                // R.a = B.a;

                alpha = (byte)(255 - M.a);
                R.r = (byte)((T.r * M.a + B.r * alpha) * color_byte_float);
                R.g = (byte)((T.g * M.a + B.g * alpha) * color_byte_float);
                R.b = (byte)((T.b * M.a + B.b * alpha) * color_byte_float);
                R.a = B.a;

                //R.r = T.r * M.a + B.r * (1 - M.a);
                //R.g = T.g * M.a + B.g * (1 - M.a);
                //R.b = T.b * M.a + B.b * (1 - M.a);
                //R.a = B.a;

                dest[writePixel] = R;
            }
        }
    }

    static float color_byte_float = 1f / 255f;

    void RGBToHSV(byte r, byte g, byte b, out float h, out float s, out float v)
    {
        byte max = r > g ? (r > b ? r : b) : (g > b ? g : b);
        byte min = r < g ? (r < b ? r : b) : (g < b ? g : b);
        byte max_min = (byte)(max - min);
        if (r == max)
        {
            h = 1f * (g - b) / max_min;
        }
        else if (g == max)
        {
            h = 2 + 1f * (b - r) / max_min;
        }
        else
        {
            h = 4 + 1f * (r - g) / max_min;
        }

        h = h * 60;
        if (h < 0)
            h = h + 360;

        s = 1f * max_min / max;
        v = max * color_byte_float;
    }

    void RGBToHSV(float r, float g, float b, out float h, out float s, out float v)
    {
        float max = r > g ? (r > b ? r : b) : (g > b ? g : b);
        float min = r < g ? (r < b ? r : b) : (g < b ? g : b);
        if (r == max)
        {
            h = (g - b) / (max - min);
        }
        else if (g == max)
        {
            h = 2 + (b - r) / (max - min);
        }
        else
        {
            h = 4 + (r - g) / (max - min);
        }
        h = h * 60;
        if (h < 0)
            h = h + 360;

        s = (max - min) / max;
        v = max;
    }

    void HSVToRGB(float h, float s, float v, out float r, out float g, out float b)
    {
        if (s == 0)
        {
            r = g = b = v;
        }
        else
        {
            h = h / 60;
            int i = (int)h;
            float f = h - (float)i;
            float aa = v * (1 - s);
            float bb = v * (1 - s * f);
            float cc = v * (1 - s * (1 - f));
            switch (i)
            {
                case 0: r = v; g = cc; b = aa; break;
                case 1: r = bb; g = v; b = aa; break;
                case 2: r = aa; g = v; b = cc; break;
                case 3: r = aa; g = bb; b = v; break;
                case 4: r = cc; g = aa; b = v; break;
                default: r = v; g = aa; b = bb; break;
            }
        }
    }

}
