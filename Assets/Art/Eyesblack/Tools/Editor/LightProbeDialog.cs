using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using Eyesblack.EditorTools;

public class LightProbeDialog : EditorWindow
{
	string m_gridSize = "2";
	string m_collideHeight = "10";
	string m_probeHeight = "0.5";

	const float width = 120;

	[MenuItem("Tools/场景工具/LightProbe工具")]
	private static void Init()
	{
		EditorWindow.GetWindow(typeof(LightProbeDialog));		
	}

	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("LightProbe间矩", GUILayout.Width(width));
		m_gridSize = GUILayout.TextField(m_gridSize);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("LightProbe高度", GUILayout.Width(width));
		m_probeHeight = GUILayout.TextField(m_probeHeight);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("MeshCollider检测高度", GUILayout.Width(width));
		m_collideHeight = GUILayout.TextField(m_collideHeight);
		GUILayout.EndHorizontal();

		if (GUILayout.Button("生成"))
		{
			float size = float.Parse(m_gridSize);
			float height = float.Parse(m_collideHeight);
			float probeHeight = float.Parse(m_probeHeight);

			LightProbeTools.GenerateLightProbes(size, height, probeHeight);
		}

		GUILayout.BeginHorizontal();
		GUILayout.Label("LightProbe间矩", GUILayout.Width(width));
		m_gridSize = GUILayout.TextField(m_gridSize);
		GUILayout.EndHorizontal();

		if (GUILayout.Button("优化"))
		{
			float size = float.Parse(m_gridSize);
			LightProbeTools.OptimizeProbes(size);
		}
	}
}