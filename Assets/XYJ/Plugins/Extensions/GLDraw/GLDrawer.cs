using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class GLDrawer : MonoBehaviour
{

    Camera m_cam;

	// Use this for initialization
	void Start () {
        m_cam = this.GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Will be called after all regular rendering is done
    void OnRenderObject()
    {
        GLUtils.GLMatterial.SetPass(0);

        OnGLDraw();
    }

    // Will be called after a camera finished rendering the scene.
    void OnPostRender()
    {

    }

    // 绘制
    public virtual void OnGLDraw() { }

    #region Public Methods

    //屏幕坐标画点
    public void DrawPoint2D(Color clr, float size, Vector2 pt)
    {
        float nearClip = m_cam.nearClipPlane + 0.00001f;
        GL.Begin(GL.QUADS);
        GL.Color(clr);

        Vector3 offset1 = new Vector3(size, size, 0);
        Vector3 offset2 = new Vector3(size, -size, 0);
        Vector3 offset3 = new Vector3(0, 0, nearClip);
        Vector3 v = new Vector3(pt.x, pt.y, 0);

        GL.Vertex(m_cam.ScreenToWorldPoint(v - offset1 + offset3));
        GL.Vertex(m_cam.ScreenToWorldPoint(v - offset2 + offset3));
        GL.Vertex(m_cam.ScreenToWorldPoint(v + offset1 + offset3));
        GL.Vertex(m_cam.ScreenToWorldPoint(v + offset2 + offset3));
        GL.End();
    }

    //屏幕坐标画线
    public void DrawLine2D(Color clr, float lineWidth, params Vector2[] posList)
    {
        int w = Screen.width;
        int h = Screen.height;
        float thisWidth = 1f / Screen.width * lineWidth * 0.5f;
        float nearClip = m_cam.nearClipPlane + 0.00001f;
        int end = posList.Length - 1;

        //转成0~1,写得有点乱>_<
        for (int i = 0; i < posList.Length; ++i)
        {
            posList[i].x /= w;
            posList[i].y /= h;
        }

        if (lineWidth == 1)
        {
            GL.Begin(GL.LINES);
            GL.Color(clr);
            for (int i = 0; i < end; ++i)
            {
                GL.Vertex(m_cam.ViewportToWorldPoint(new Vector3(posList[i].x, posList[i].y, nearClip)));
                GL.Vertex(m_cam.ViewportToWorldPoint(new Vector3(posList[i + 1].x, posList[i + 1].y, nearClip)));
            }
            GL.End();
        }
        else
        {
            GL.Begin(GL.QUADS);
            GL.Color(clr);
            for (int i = 0; i < end; ++i)
            {
                Vector3 perpendicular = (new Vector3(posList[i + 1].y, posList[i].x, nearClip) -
                    new Vector3(posList[i].y, posList[i + 1].x, nearClip)).normalized * thisWidth;
                Vector3 v1 = new Vector3(posList[i].x, posList[i].y, nearClip);
                Vector3 v2 = new Vector3(posList[i + 1].x, posList[i + 1].y, nearClip);
                GL.Vertex(m_cam.ViewportToWorldPoint(v1 - perpendicular));
                GL.Vertex(m_cam.ViewportToWorldPoint(v1 + perpendicular));
                GL.Vertex(m_cam.ViewportToWorldPoint(v2 + perpendicular));
                GL.Vertex(m_cam.ViewportToWorldPoint(v2 - perpendicular));
            }
            GL.End();
        }
    }

    #endregion

}
