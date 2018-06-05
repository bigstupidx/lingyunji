using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GLUtils {

    public const int CIRCLE_TRIS_COUNT = 30;

    static Material _glMaterial;
    public static Material GLMatterial
    {
        get
        {
            if (_glMaterial == null)
            {
                var shader = Shader.Find("Hidden/Internal-Colored");
                _glMaterial = new Material(shader);
                _glMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                _glMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _glMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                _glMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                _glMaterial.SetInt("_ZWrite", 0);
            }
            return _glMaterial;

        }
    }

    //画3D的线
    public static void DrawLine(Color clr, Vector3 pos, Vector3 forward, float size)
    {
        GL.Begin(GL.LINES);
        GL.Color(clr);
        GL.Vertex(pos);
        GL.Vertex(pos + forward.normalized * size);
        GL.End();
    }

    //画3D的线
    public static void DrawLine(Color clr, params Vector3[] posList)
    {
        int end = posList.Length - 1;

        GL.Begin(GL.LINES);
        GL.Color(clr);
        for (int i = 0; i < end; ++i)
        {
            GL.Vertex(posList[i]);
            GL.Vertex(posList[i + 1]);
        }
        GL.End();
    }

    //画四方形
    public static void DrawQuad(Color clr, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        GL.PushMatrix();
        GL.Begin(GL.QUADS);
        GL.Color(clr);
        GL.Vertex(p1);
        GL.Vertex(p2);
        GL.Vertex(p3);
        GL.Vertex(p4);

        GL.End();
        GL.PopMatrix();
    }

    //画长方体,参数为颜色、中心点
    public static void DrawBox(Color clr, Vector3 center, Vector3 scale, Quaternion dir)
    {
        Vector3 bottomPos = new Vector3(0, -0.5f, 0);
        Vector3 topPos = new Vector3(0, 0.5f, 0);
        Vector3 offset1 = new Vector3(0.5f, 0, 0.5f);
        Vector3 offset2 = new Vector3(0.5f, 0, -0.5f);

        GL.PushMatrix();
        GL.MultMatrix(Matrix4x4.TRS(center, dir, scale));
        GL.Begin(GL.TRIANGLES);

        GL.Color(clr);
        SetRectVertex(bottomPos - offset1, bottomPos + offset1);
        GL.Color(clr);//* 0.9f
        SetRectVertex(bottomPos + offset1, topPos + offset2);
        GL.Color(clr);//* 0.8f
        SetRectVertex(topPos + offset2, bottomPos - offset1);
        GL.Color(clr);//* 0.7f
        SetRectVertex(bottomPos - offset1, topPos - offset2);
        GL.Color(clr);//* 0.6f
        SetRectVertex(topPos - offset2, bottomPos + offset1);
        GL.Color(clr);//* 0.5f
        SetRectVertex(topPos - offset2, topPos + offset2);


        GL.End();
        GL.PopMatrix();
    }


    //画球
    public static void DrawSphere(Color clr, Vector3 center, float size)
    {
        DrawSphere(clr, center, Vector3.one * size, Quaternion.identity);
    }

    //画球,
    public static void DrawSphere(Color clr, Vector3 center, Vector3 scale, Quaternion dir)
    {
        GL.PushMatrix();
        GL.MultMatrix(Matrix4x4.TRS(center, dir, scale));
        GL.Begin(GL.TRIANGLES);
        GL.Color(clr);
        int c = CIRCLE_TRIS_COUNT;//这个函数的计算方法可能有问题为15的情况下才正常
        Vector3 v;
        Quaternion q1 = Quaternion.LookRotation(Vector3.left), q2;
        Vector3 v1, v2, v3, v4;
        for (int i = 0; i < c; ++i)
        {
            v = Vector3.up * 0.5f;
            q2 = Quaternion.Euler(0, 360f / c, 0) * q1;
            for (int j = 0; j < c / 2; ++j)
            {
                v1 = q1 * v;
                v2 = Quaternion.Euler(0, 360f / c, 0) * v1;
                v = Quaternion.Euler(0, 0, 360f / c) * v;
                v3 = q2 * v;
                v4 = Quaternion.Euler(0, 360f / c, 0) * v3;
                SetRectVertex(v1, v2, v4, v3);
            }
            q1 = q2;
        }


        GL.End();
        GL.PopMatrix();
    }

    //画圆
    public static void DrawCircle(Color clr, Vector3 center, float size)
    {
        DrawCircle(clr, center, Vector2.one * size, Quaternion.identity);
    }

    //画圆
    public static void DrawCircle(Color clr, Vector3 center, Vector2 scale, Quaternion dir)
    {
        GL.PushMatrix();
        GL.MultMatrix(Matrix4x4.TRS(center, dir, new Vector3(scale.x / 2, 1, scale.y / 2)));
        GL.Begin(GL.TRIANGLES);
        GL.Color(clr);
        Vector3 v = Vector3.forward;
        for (int i = 0; i < CIRCLE_TRIS_COUNT; ++i)
        {
            GL.Vertex(Vector3.zero);
            GL.Vertex(v);
            v = Quaternion.Euler(0, 360f / CIRCLE_TRIS_COUNT, 0) * v;
            GL.Vertex(v);
        }


        GL.End();
        GL.PopMatrix();

    }
    //画圆柱
    public static void DrawCylinder(Color clr, Vector3 center, Vector3 scale, Quaternion dir)
    {
        GL.PushMatrix();
        scale.x /= 2;
        scale.z /= 2;
        GL.MultMatrix(Matrix4x4.TRS(center, dir, scale));
        GL.Begin(GL.TRIANGLES);
        GL.Color(clr);
        Vector3 v = Vector3.forward;
        Vector3 vTop = Vector3.zero - Vector3.down * 0.5f;
        Vector3 vBottom = Vector3.zero + Vector3.down * 0.5f;
        Vector3 v1, v2, v3, v4;
        for (int i = 0; i < CIRCLE_TRIS_COUNT; ++i)
        {
            v1 = v - Vector3.down * 0.5f;
            v3 = v + Vector3.down * 0.5f;
            v = Quaternion.Euler(0, 360f / CIRCLE_TRIS_COUNT, 0) * v;
            v2 = v - Vector3.down * 0.5f;
            v4 = v + Vector3.down * 0.5f;

            //下底
            GL.Color(clr * 0.8f);
            GL.Vertex(vTop);
            GL.Vertex(v1);
            GL.Vertex(v2);

            //侧面
            GL.Color(clr * 0.6f);
            SetRectVertex(v1, v2, v4, v3);

            //上底
            GL.Color(clr * 0.5f);
            GL.Vertex(vBottom);
            GL.Vertex(v3);
            GL.Vertex(v4);
        }


        GL.End();
        GL.PopMatrix();
    }

    //画扇形,这里算夹角的，也就是填90度的话就是一个半圆
    public static void DrawSector(Color clr, float angle, Vector3 center, Vector2 scale, Quaternion dir)
    {
        GL.PushMatrix();
        GL.MultMatrix(Matrix4x4.TRS(center, dir, new Vector3(scale.x / 2, 1, scale.y / 2)));
        GL.Begin(GL.TRIANGLES);
        GL.Color(clr);
        Vector3 v = Quaternion.Euler(0, -angle, 0) * Vector3.forward;
        for (int i = 0; i < CIRCLE_TRIS_COUNT; ++i)
        {
            GL.Vertex(Vector3.zero);
            GL.Vertex(v);
            v = Quaternion.Euler(0, (angle + angle) / CIRCLE_TRIS_COUNT, 0) * v;
            GL.Vertex(v);
        }



        GL.End();
        GL.PopMatrix();

    }

    //画扇柱,这里算夹角的，也就是填90度的话就是一个半圆
    public static void DrawSectorCylinder(Color clr, float angle, Vector3 center, Vector3 scale, Quaternion dir)
    {
        GL.PushMatrix();
        scale.x /= 2;
        scale.z /= 2;
        GL.MultMatrix(Matrix4x4.TRS(center, dir, scale));
        GL.Begin(GL.TRIANGLES);
        GL.Color(clr);
        Vector3 v = Quaternion.Euler(0, -angle, 0) * Vector3.forward;

        Vector3 vTop = Vector3.zero - Vector3.down * 0.5f;
        Vector3 vBottom = Vector3.zero + Vector3.down * 0.5f;
        Vector3 v1, v2, v3, v4;
        for (int i = 0; i < CIRCLE_TRIS_COUNT; ++i)
        {
            v1 = v - Vector3.down * 0.5f;
            v3 = v + Vector3.down * 0.5f;
            v = Quaternion.Euler(0, (angle + angle) / CIRCLE_TRIS_COUNT, 0) * v;
            v2 = v - Vector3.down * 0.5f;
            v4 = v + Vector3.down * 0.5f;

            //下底
            GL.Color(clr * 0.8f);
            GL.Vertex(vTop);
            GL.Vertex(v1);
            GL.Vertex(v2);

            //侧面
            GL.Color(clr * 0.6f);
            SetRectVertex(v1, v2, v4, v3);

            //上底
            GL.Color(clr * 0.5f);
            GL.Vertex(vBottom);
            GL.Vertex(v3);
            GL.Vertex(v4);

            //内壁
            if (i == 0)
                SetRectVertex(v1, vTop, vBottom, v3);
            else if (i == CIRCLE_TRIS_COUNT - 1)
                SetRectVertex(v2, vTop, vBottom, v4);
        }

        GL.End();
        GL.PopMatrix();

    }

    //在某平面上设置拼成矩形的三角形的顶点
    static void SetRectVertex(Vector3 p1, Vector3 p2)
    {
        if (p1.y == p2.y)
            SetRectVertex(p1, new Vector3(p1.x, p1.y, p2.z), p2, new Vector3(p2.x, p1.y, p1.z));
        else if (p1.x == p2.x)
            SetRectVertex(p1, new Vector3(p1.x, p1.y, p2.z), p2, new Vector3(p1.x, p2.y, p1.z));
        else if (p1.z == p2.z)
            SetRectVertex(p1, new Vector3(p1.x, p2.y, p1.z), p2, new Vector3(p2.x, p1.y, p1.z));
    }

    //设置拼成四边形的顶点，注意要准时针或逆时针
    static void SetRectVertex(Vector3 p1, Vector3 p2, Vector3 p3, Vector4 p4)
    {
        GL.Vertex(p1);
        GL.Vertex(p2);
        GL.Vertex(p3);
        GL.Vertex(p3);
        GL.Vertex(p4);
        GL.Vertex(p1);
    }
}
