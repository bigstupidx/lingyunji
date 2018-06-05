using UnityEngine;
using UnityEditor;
using System.Collections;
#pragma warning disable 618

[CustomEditor(typeof(BitmapFont))]
public class BitmapFontEditor : Editor
{
    BitmapFont mFont;

    EditorPageBtn mBtn = new EditorPageBtn();

    int selectid = -1;

    public override void OnInspectorGUI()
    {
        EditorGUIUtility.labelWidth = 80;
        mFont = target as BitmapFont;
        mFont.font = EditorGUILayout.ObjectField("font", mFont.font, typeof(Font), false) as Font;

        Font font = mFont.font;
        if (font == null)
            return;

        CharacterInfo[] infos = font.characterInfo;
        mBtn.total = infos.Length;
        mBtn.pageNum = 5;
        mBtn.OnRender();
        GUI.changed = false;
        for (int i = mBtn.beginIndex; i < mBtn.endIndex; ++i)
        {
            CharacterInfo info = infos[i];
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("char:" + (char)info.index);
            if (GUILayout.Button("Select"))
            {
                GUI.changed = false;
                selectid = info.index;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            info.minX = EditorGUILayout.IntField("minX", info.minX);
            info.minY = EditorGUILayout.IntField("minY", info.minY);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            info.maxX = EditorGUILayout.IntField("maxX", info.maxX);
            info.maxY = EditorGUILayout.IntField("maxY", info.maxY);
            EditorGUILayout.EndHorizontal();

            infos[i] = info;
        }

        if (GUI.changed)
        {
            font.characterInfo = infos;

            EditorUtility.SetDirty(font);
            AssetDatabase.SaveAssets();
        }
    }

    public override bool HasPreviewGUI() { return true; }

    public override void OnPreviewGUI(Rect rect, GUIStyle background)
    {
        if (selectid == -1 || mFont == null || mFont.font == null || mFont.font.material == null)
            return;

        Texture2D t2d = mFont.font.material.mainTexture as Texture2D;
        if (t2d == null)
            return;

        CharacterInfo info;
        mFont.font.GetCharacterInfo((char)selectid, out info);

        int borderLeft = info.minX;
        int borderBottom = info.maxY;
        int borderRight = info.glyphWidth - info.maxX;
        int borderTop = info.glyphHeight - info.minY;

        DrawSprite(t2d, rect, Color.white, null, info.uv, borderLeft, borderBottom, borderRight, borderTop);
    }

    static public void DrawTiledTexture(Rect rect, Texture tex)
    {
        GUI.BeginGroup(rect);
        {
            int width = Mathf.RoundToInt(rect.width);
            int height = Mathf.RoundToInt(rect.height);

            for (int y = 0; y < height; y += tex.height)
            {
                for (int x = 0; x < width; x += tex.width)
                {
                    GUI.DrawTexture(new Rect(x, y, tex.width, tex.height), tex);
                }
            }
        }
        GUI.EndGroup();
    }

    static Texture2D CreateCheckerTex(Color c0, Color c1)
    {
        Texture2D tex = new Texture2D(16, 16);
        tex.name = "[Generated] Checker Texture";
        tex.hideFlags = HideFlags.DontSave;

        for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c1);
        for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) tex.SetPixel(x, y, c0);
        for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c0);
        for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) tex.SetPixel(x, y, c1);

        tex.Apply();
        tex.filterMode = FilterMode.Point;
        return tex;
    }

    static Texture2D mContrastTex;
    static Texture2D mBackdropTex;
    static public Texture2D backdropTexture
    {
        get
        {
            if (mBackdropTex == null) mBackdropTex = CreateCheckerTex(
                new Color(0.1f, 0.1f, 0.1f, 0.5f),
                new Color(0.2f, 0.2f, 0.2f, 0.5f));
            return mBackdropTex;
        }
    }

    static public Texture2D contrastTexture
    {
        get
        {
            if (mContrastTex == null) mContrastTex = CreateCheckerTex(
                new Color(0f, 0.0f, 0f, 0.5f),
                new Color(1f, 1f, 1f, 0.5f));
            return mContrastTex;
        }
    }

    static public void DrawSprite(Texture2D tex, Rect drawRect, Color color, Material mat, Rect uv, int borderLeft, int borderBottom, int borderRight, int borderTop)
    {
        if (!tex) return;

        int width = (int)(tex.width * uv.width);
        int height = (int)(tex.height * uv.height);

        // Create the texture rectangle that is centered inside rect.
        Rect outerRect = drawRect;
        outerRect.width = width;
        outerRect.height = height;

        if (width > 0)
        {
            float f = drawRect.width / outerRect.width;
            outerRect.width *= f;
            outerRect.height *= f;
        }

        if (drawRect.height > outerRect.height)
        {
            outerRect.y += (drawRect.height - outerRect.height) * 0.5f;
        }
        else if (outerRect.height > drawRect.height)
        {
            float f = drawRect.height / outerRect.height;
            outerRect.width *= f;
            outerRect.height *= f;
        }

        if (drawRect.width > outerRect.width) outerRect.x += (drawRect.width - outerRect.width) * 0.5f;

        // Draw the background
        DrawTiledTexture(outerRect, backdropTexture);

        // Draw the sprite
        GUI.color = color;

        if (mat == null)
        {
            GUI.DrawTextureWithTexCoords(outerRect, tex, uv, true);
        }
        else
        {
            // NOTE: There is an issue in Unity that prevents it from clipping the drawn preview
            // using BeginGroup/EndGroup, and there is no way to specify a UV rect... le'suq.
            UnityEditor.EditorGUI.DrawPreviewTexture(outerRect, tex, mat);
        }

        if (Selection.activeGameObject == null || Selection.gameObjects.Length == 1)
        {
            // Draw the border indicator lines
            GUI.BeginGroup(outerRect);
            {
                tex = contrastTexture;
                GUI.color = Color.white;

                if (borderLeft > 0)
                {
                    float x0 = (float)borderLeft / width * outerRect.width - 1;
                    DrawTiledTexture(new Rect(x0, 0f, 1f, outerRect.height), tex);
                }

                if (borderRight > 0)
                {
                    float x1 = (float)(width - borderRight) / width * outerRect.width - 1;
                    DrawTiledTexture(new Rect(x1, 0f, 1f, outerRect.height), tex);
                }

                if (borderBottom > 0)
                {
                    float y0 = (float)(height - borderBottom) / height * outerRect.height - 1;
                    DrawTiledTexture(new Rect(0f, y0, outerRect.width, 1f), tex);
                }

                if (borderTop > 0)
                {
                    float y1 = (float)borderTop / height * outerRect.height - 1;
                    DrawTiledTexture(new Rect(0f, y1, outerRect.width, 1f), tex);
                }
            }
            GUI.EndGroup();

            // Draw the lines around the sprite
            Handles.color = Color.black;
            Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMin), new Vector3(outerRect.xMin, outerRect.yMax));
            Handles.DrawLine(new Vector3(outerRect.xMax, outerRect.yMin), new Vector3(outerRect.xMax, outerRect.yMax));
            Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMin), new Vector3(outerRect.xMax, outerRect.yMin));
            Handles.DrawLine(new Vector3(outerRect.xMin, outerRect.yMax), new Vector3(outerRect.xMax, outerRect.yMax));

            // Sprite size label
            string text = string.Format("Sprite Size: {0}x{1}", Mathf.RoundToInt(width), Mathf.RoundToInt(height));
            EditorGUI.DropShadowLabel(GUILayoutUtility.GetRect(Screen.width, 18f), text);
        }
    }
}
