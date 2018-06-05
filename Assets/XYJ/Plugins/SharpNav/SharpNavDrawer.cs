#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SharpNav;

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class SharpNavDrawer : MonoBehaviour
{
    private TiledNavMesh tiledNavMesh;
    private int vertsPerPoly;

    public float pointSize = 0.2f;
    public Color pointColor = new Color(0.9f, 0.0f, 0.0f, 0.7f);
    public Color lineColor = new Color(0.0f, 0.0f, 1.0f, 0.4f);
    public Color faceColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
    public Color areaColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);


    [SerializeField]
    Vector3 Offset;

    [SerializeField]
    Vector3 AreaOffset;

    static Material _navMaterial;
    static void CreateLineMaterial()
    {
        if (!_navMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            _navMaterial = new Material(shader);
            _navMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            _navMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            _navMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            _navMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            _navMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Will be called after all regular rendering is done
    void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        _navMaterial.SetPass(0);

        DrawNavMesh();
    }

    // Will be called after a camera finished rendering the scene.
    //void OnPostRender()
    //{
    //    CreateLineMaterial();
    //    // Apply the line material
    //    lineMaterial.SetPass(0);

    //    DrawNavMesh();
    //}

    public void SetNavMesh(TiledNavMesh mesh, int vPerPloy)
    {
        tiledNavMesh = mesh;
        vertsPerPoly = vPerPloy;
    }


    /// <summary>
    /// 绘制SharpNav导航网格
    /// </summary>
    public void DrawNavMesh()
    {
        if (tiledNavMesh == null)
            return;

        var tile = tiledNavMesh.GetTileAt(0, 0, 0);

        GL.PushMatrix();

        // 三角面
        GL.Begin(GL.TRIANGLES);
        GL.Color(faceColor);

        for (int i = 0; i < tile.Polys.Length; i++)
        {
            //if (!tile.Polys[i].Area.IsWalkable)
            //continue;

            for (int j = 2; j < vertsPerPoly; j++)
            {
                if (tile.Polys[i].Verts[j] == 0)
                    break;

                int vertIndex0 = tile.Polys[i].Verts[0];
                int vertIndex1 = tile.Polys[i].Verts[j - 1];
                int vertIndex2 = tile.Polys[i].Verts[j];

                var v = tile.Verts[vertIndex0];
                GL.Vertex3(v.X + Offset.x, v.Y + Offset.y, v.Z + Offset.z);

                v = tile.Verts[vertIndex1];
                GL.Vertex3(v.X + Offset.x, v.Y + Offset.y, v.Z + Offset.z);

                v = tile.Verts[vertIndex2];
                GL.Vertex3(v.X + Offset.x, v.Y + Offset.y, v.Z + Offset.z);
            }
        }

        {
            GL.Color(areaColor);
            var startPos = tiledNavMesh.Origin + new SharpNav.Geometry.Vector3(AreaOffset.x, AreaOffset.y, AreaOffset.z);
            float width = tiledNavMesh.TileWidth;
            float height = tiledNavMesh.TileHeight;
            GL.Vertex3(startPos.X, startPos.Y, startPos.Z);
            GL.Vertex3(startPos.X + width, startPos.Y, startPos.Z);
            GL.Vertex3(startPos.X + width, startPos.Y, startPos.Z + height);

            GL.Vertex3(startPos.X, startPos.Y, startPos.Z);
            GL.Vertex3(startPos.X, startPos.Y, startPos.Z + height);
            GL.Vertex3(startPos.X + width, startPos.Y, startPos.Z + height);
        }

        GL.End();


        // 每个面的线

        //neighbor edges
        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        for (int i = 0; i < tile.Polys.Length; i++)
        {
            for (int j = 0; j < vertsPerPoly; j++)
            {
                if (tile.Polys[i].Verts[j] == 0)
                    break;
                if (PolyMesh.IsBoundaryEdge(tile.Polys[i].Neis[j]))
                    continue;

                int nj = (j + 1 >= vertsPerPoly || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

                int vertIndex0 = tile.Polys[i].Verts[j];
                int vertIndex1 = tile.Polys[i].Verts[nj];

                var v = tile.Verts[vertIndex0];
                GL.Vertex3(v.X + Offset.x, v.Y + Offset.y, v.Z + Offset.z);

                v = tile.Verts[vertIndex1];
                GL.Vertex3(v.X + Offset.x, v.Y + Offset.y, v.Z + Offset.z);
            }
        }

        GL.End();

        //boundary edges
        GL.Begin(GL.LINES);
        GL.Color(lineColor);

        for (int i = 0; i < tile.Polys.Length; i++)
        {
            for (int j = 0; j < vertsPerPoly; j++)
            {
                if (tile.Polys[i].Verts[j] == 0)
                    break;

                if (PolyMesh.IsInteriorEdge(tile.Polys[i].Neis[j]))
                    continue;

                int nj = (j + 1 >= vertsPerPoly || tile.Polys[i].Verts[j + 1] == 0) ? 0 : j + 1;

                int vertIndex0 = tile.Polys[i].Verts[j];
                int vertIndex1 = tile.Polys[i].Verts[nj];

                var v = tile.Verts[vertIndex0];
                GL.Vertex3(v.X + Offset.x, v.Y + Offset.y, v.Z + Offset.z);

                v = tile.Verts[vertIndex1];
                GL.Vertex3(v.X + Offset.x, v.Y + Offset.y, v.Z + Offset.z);
            }
        }

        GL.End();

        GL.PopMatrix();

        // 点
        for (int i = 0; i < tile.Verts.Length; i++)
        {
            var v = tile.Verts[i];
            Vector3 v3 = new Vector3(v.X, v.Y, v.Z) + Offset;
            DrawCircle(pointColor, v3, Vector2.one * pointSize, Quaternion.identity);
        }
    }

    /// <summary>
    /// 绘制圆
    /// </summary>
    /// <param name="clr"></param>
    /// <param name="center"></param>
    /// <param name="scale"></param>
    /// <param name="dir"></param>
    public void DrawCircle(Color clr, Vector3 center, Vector2 scale, Quaternion dir)
    {
        int CIRCLE_COUNT = 30;
        Quaternion stepEuler = Quaternion.Euler(0, 360f / CIRCLE_COUNT, 0);

        GL.PushMatrix();
        GL.MultMatrix(Matrix4x4.TRS(center, dir, new Vector3(scale.x / 2, 1, scale.y / 2)));

        GL.Begin(GL.TRIANGLES);
        GL.Color(clr);

        Vector3 v = Vector3.forward;
        for (int i = 0; i < CIRCLE_COUNT; ++i)
        {
            GL.Vertex(Vector3.zero);
            GL.Vertex(v);
            v = stepEuler * v;
            GL.Vertex(v);
        }

        GL.End();
        GL.PopMatrix();
    }
}
#endif