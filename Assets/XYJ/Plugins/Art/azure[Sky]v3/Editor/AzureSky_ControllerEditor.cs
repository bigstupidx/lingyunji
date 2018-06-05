using UnityEditor;
using UnityEngine;
using System.Collections;
using UnityEditorInternal;

[CustomEditor(typeof(AzureSky_Controller))]
public class AzureSky_ControllerEditor : Editor
{
	private string[] spaceColor    = new string[]{"Default", "Linear To Gamma"};
	private string[] ambientSource = new string[]{"Skybox", "Gradient", "Color"};
	private string[] cloudMode     = new string[]{"Null", "Pre-Rendered", "Procedural (WIP)"};
	private string[] reflectionRefreshMode = new string[]{"On Awake", "Every Frame", "Via Scripting"};
	private string[] reflectionTimeSlicing = new string[]{"All faces at once", "Individual faces", "No time slicing"};
	private string[] unityFogMode     = new string[]{"Linear", "Exponential", "Exponential Square"};

	private int Day;
	private string Sunday    ="Sanday";
	private string Monday    ="M";
	private string Tuesday   ="T";
	private string Wednesday ="W";
	private string Thursday  ="T";
	private string Friday    ="F";
	private string Saturday  ="S";

	private string installPath;
	private string inspectorGUIPath;
	private int offset = 20;
	private Color curveColor = Color.yellow;
	private float curveWidth = 126;
	private float curveValueWidth = 15;

	private ReorderableList    reorderCurveList;
	private ReorderableList    reorderGradientList;
	private SerializedProperty serCurve;
	private SerializedProperty serGradient;

	Texture2D ShowHideTimeOfDay;
	Texture2D ShowHideObjectsAndMaterials;
	Texture2D ShowHideScattering;
	Texture2D ShowHideSkySettings;
	Texture2D ShowHideFogSettings;
	Texture2D ShowHideCloudSettings;
	Texture2D ShowHideAmbient;
	Texture2D ShowHideLighting;
	Texture2D ShowHideTextures;
	Texture2D ShowHideOptions;
	Texture2D ShowHideOutput;

	SerializedObject   serObj;
//	SerializedProperty SunGradientColor;
	SerializedProperty SunsetGradientColor;
	SerializedProperty MoonGradientColor;
	SerializedProperty MoonBrightGradientColor;

	SerializedProperty AmbientColorGradient;
	SerializedProperty SkyAmbientColorGradient;
	SerializedProperty EquatorAmbientColorGradient;
	SerializedProperty GroundAmbientColorGradient;
	SerializedProperty SunDirLightColorGradient;
	SerializedProperty MoonDirLightColorGradient;

	SerializedProperty NightGroundCloseGradientColor;
	SerializedProperty NightGroundFarGradientColor;

	SerializedProperty ReflectionProbeCullingMask;

	SerializedProperty EdgeColorGradientColor;
	SerializedProperty DarkColorGradientColor;

	SerializedProperty NormalFogGradientColor;
	SerializedProperty GlobalFogGradientColor;
	SerializedProperty DenseFogGradientColor;

	SerializedProperty WispyDarknessGradientColor;
	SerializedProperty WispyBrightGradientColor;
	SerializedProperty WispyColorGradientColor;

	SerializedProperty UnityFogGradientColor;

	void OnEnable()
	{
		string scriptLocation = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
		installPath = scriptLocation.Replace ("/Editor/AzureSky_ControllerEditor.cs", "");
		inspectorGUIPath = installPath + "/Editor/InspectorGUI";
		//-------------------------------------------------------------------------------------------------------
		//Gradient Color Serialize
		serObj                  	= new SerializedObject(target);
//		SunGradientColor        	= serObj.FindProperty ("SunGradientColor");
		SunsetGradientColor         = serObj.FindProperty ("SunsetGradientColor");
		MoonGradientColor      	    = serObj.FindProperty ("MoonGradientColor");
		MoonBrightGradientColor     = serObj.FindProperty ("MoonBrightGradientColor");
		AmbientColorGradient        = serObj.FindProperty ("AmbientColorGradient");
		SkyAmbientColorGradient     = serObj.FindProperty ("SkyAmbientColorGradient");
		EquatorAmbientColorGradient = serObj.FindProperty ("EquatorAmbientColorGradient");
		GroundAmbientColorGradient  = serObj.FindProperty ("GroundAmbientColorGradient");
		SunDirLightColorGradient    = serObj.FindProperty ("SunDirLightColorGradient");
		MoonDirLightColorGradient   = serObj.FindProperty ("MoonDirLightColorGradient");
		ReflectionProbeCullingMask = serObj.FindProperty ("ReflectionProbeCullingMask");

		NightGroundCloseGradientColor = serObj.FindProperty("NightGroundCloseGradientColor");
		NightGroundFarGradientColor = serObj.FindProperty("NightGroundFarGradientColor");

		EdgeColorGradientColor = serObj.FindProperty ("EdgeColorGradientColor");
		DarkColorGradientColor = serObj.FindProperty ("DarkColorGradientColor");

		NormalFogGradientColor = serObj.FindProperty ("NormalFogGradientColor");
		GlobalFogGradientColor = serObj.FindProperty ("GlobalFogGradientColor");
		DenseFogGradientColor  = serObj.FindProperty ("DenseFogGradientColor");

		WispyDarknessGradientColor = serObj.FindProperty ("WispyDarknessGradientColor");
		WispyBrightGradientColor   = serObj.FindProperty("WispyBrightGradientColor");
		WispyColorGradientColor    = serObj.FindProperty ("WispyColorGradientColor");

		UnityFogGradientColor = serObj.FindProperty ("UnityFogGradientColor");
		//=======================================================================================================
		//-----------------------------------------CREATE OUTPUT LISTs-------------------------------------------
		serCurve  = serializedObject.FindProperty ("OutputCurveList");
		reorderCurveList = new ReorderableList (serializedObject, serCurve, false, true, true, true);
		reorderCurveList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
		{
			rect.y += 2;
			EditorGUI.LabelField(rect, "element index " + index.ToString());
			EditorGUI.PropertyField(new Rect (rect.x+100, rect.y, rect.width-100, EditorGUIUtility.singleLineHeight), serCurve.GetArrayElementAtIndex(index), GUIContent.none);
		};
		reorderCurveList.onAddCallback = (ReorderableList l) =>
		{
			var index = l.serializedProperty.arraySize;
			l.serializedProperty.arraySize++;
			l.index = index;
			serCurve.GetArrayElementAtIndex(index).animationCurveValue = AnimationCurve.Linear(0,0,24,0);
		};
		reorderCurveList.drawHeaderCallback = (Rect rect) =>
		{
			EditorGUI.LabelField(rect, "Curve Output", EditorStyles.boldLabel);
			EditorGUI.LabelField(new Rect (rect.x+90, rect.y, rect.width, rect.height), "(as Float)", EditorStyles.miniBoldLabel);
		};
		reorderCurveList.drawElementBackgroundCallback = (rect, index, active, focused) => {
			Texture2D tex = new Texture2D (1, 1);
			tex.SetPixel (0, 0, new Color (0.35f, 0.55f, 1f, 0.55f));
			tex.Apply ();
			if (active)
				GUI.DrawTexture (rect, tex as Texture);
		};
		//=======================================================================================================
		//-------------------------------------------------------------------------------------------------------
		serGradient  = serializedObject.FindProperty ("OutputGradientList");
		reorderGradientList = new ReorderableList (serializedObject, serGradient, false, true, true, true);
		reorderGradientList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
		{
			rect.y += 2;
			EditorGUI.LabelField(rect, "element index " + index.ToString());
			EditorGUI.PropertyField(new Rect (rect.x+100, rect.y, rect.width-100, EditorGUIUtility.singleLineHeight), serGradient.GetArrayElementAtIndex(index), GUIContent.none);
		};
		reorderGradientList.drawHeaderCallback = (Rect rect) =>
		{
			EditorGUI.LabelField(rect, "Gradient Output", EditorStyles.boldLabel);
			EditorGUI.LabelField(new Rect (rect.x+107, rect.y, rect.width, rect.height), "(as Color)", EditorStyles.miniBoldLabel);
		};
		reorderGradientList.drawElementBackgroundCallback = (rect, index, active, focused) => {
			Texture2D tex = new Texture2D (1, 1);
			tex.SetPixel (0, 0, new Color (0.35f, 0.55f, 1f, 0.55f));
			tex.Apply ();
			if (active)
				GUI.DrawTexture (rect, tex as Texture);
		};
		//=======================================================================================================
		//-------------------------------------------------------------------------------------------------------
	}


	public override void OnInspectorGUI()
	{
		//Get target
		AzureSky_Controller Target = (AzureSky_Controller)target;
		Undo.RecordObject (Target, "Undo azure[Sky] Values");
		serializedObject.Update();
		serObj.Update ();
		Day = Target.DAY_of_WEEK;
		//=======================================================================================================
		//-----------------------------------------GUI Variables-------------------------------------------------
		curveColor = Target.CurveColorField;
		Texture2D Show  = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/Show.png", typeof (Texture2D))as Texture2D;
		Texture2D Hide  = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/Hide.png", typeof (Texture2D))as Texture2D;

		if (Target.showTimeOfDay) ShowHideTimeOfDay             = Hide; else ShowHideTimeOfDay = Show;
		if (Target.showObj_and_Mat) ShowHideObjectsAndMaterials = Hide; else ShowHideObjectsAndMaterials = Show;
		if (Target.showScattering) ShowHideScattering           = Hide; else ShowHideScattering = Show;
		if (Target.showSkySettings) ShowHideSkySettings         = Hide; else ShowHideSkySettings = Show;
		if (Target.showFogSettings) ShowHideFogSettings         = Hide; else ShowHideFogSettings = Show;
		if (Target.showCloudSettings) ShowHideCloudSettings     = Hide; else ShowHideCloudSettings = Show;
		if (Target.showAmbient)  ShowHideAmbient                = Hide; else ShowHideAmbient = Show;
		if (Target.showLighting) ShowHideLighting               = Hide; else ShowHideLighting = Show;
		if (Target.showTextures) ShowHideTextures               = Hide; else ShowHideTextures = Show;
		if (Target.showOptions)  ShowHideOptions                = Hide; else ShowHideOptions = Show;
		if (Target.showOutput)   ShowHideOutput                 = Hide; else ShowHideOutput = Show;

		if (Target.DAY_of_WEEK == 0) Sunday    = "Sunday";	    else Sunday    = "S";
		if (Target.DAY_of_WEEK == 1) Monday    = "Monday";	    else Monday    = "M";
		if (Target.DAY_of_WEEK == 2) Tuesday   = "Tuesday";	    else Tuesday   = "T";
		if (Target.DAY_of_WEEK == 3) Wednesday = "Wednesday";	else Wednesday = "W";
		if (Target.DAY_of_WEEK == 4) Thursday  = "Thursday";	else Thursday  = "T";
		if (Target.DAY_of_WEEK == 5) Friday    = "Friday";	    else Friday    = "F";
		if (Target.DAY_of_WEEK == 6) Saturday  = "Saturday";	else Saturday  = "S";


		//=======================================================================================================
		//-----------------------------------------GUI Layout----------------------------------------------------
		Rect bgRect = EditorGUILayout.GetControlRect ();
		bgRect = new Rect (bgRect.x+1, bgRect.y-18, bgRect.width-20, bgRect.height+1);
		Texture2D bgTex;
		Texture2D logoTex      = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/azure[Sky]_Titlebar4.png", typeof (Texture2D))as Texture2D;
		Texture2D tab          = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/InspectorTab.png", typeof (Texture2D))as Texture2D;
		Texture2D GradientTab  = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/GradientTime3.png", typeof (Texture2D))as Texture2D;
		Texture2D TimeOfDayTab = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/TimeOfDay.png", typeof (Texture2D))as Texture2D;
		Texture2D ObjectsAndMaterialsTab = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/ObjectsAndMaterials.png", typeof (Texture2D))as Texture2D;
		Texture2D ScatteringTab  = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/Scattering.png", typeof (Texture2D))as Texture2D;
		Texture2D SkySettingsTab = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/SkySettings.png", typeof (Texture2D))as Texture2D;
		Texture2D FogSettingsTab = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/FogSettings.png", typeof (Texture2D))as Texture2D;
		Texture2D CloudSettingsTab = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/CloudSettings.png", typeof (Texture2D))as Texture2D;
		Texture2D AmbientTab     = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/Ambient.png", typeof (Texture2D))as Texture2D;
		Texture2D LightingTab    = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/Lighting.png", typeof (Texture2D))as Texture2D;
		Texture2D TexturesTab    = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/Textures.png", typeof (Texture2D))as Texture2D;
		Texture2D OptionsTab     = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/Options.png", typeof (Texture2D))as Texture2D;
		Texture2D OutputTab      = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/Output.png", typeof (Texture2D))as Texture2D;

		if (EditorGUIUtility.isProSkin) {
			bgTex = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/DarkSkin.jpg", typeof(Texture2D))as Texture2D;
		} else {
			bgTex = AssetDatabase.LoadAssetAtPath (inspectorGUIPath + "/LightSkin.jpg", typeof(Texture2D))as Texture2D;
		}
		EditorGUI.DrawPreviewTexture (bgRect, bgTex);
		GUI.DrawTexture (new Rect (10,bgRect.y+13, bgRect.width+25,46), logoTex);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		if (Target.showCurveValue) {
			curveWidth = 84;
			curveValueWidth = 38;
		} else curveWidth = 126;

		//=======================================================================================================
		//------------------------------------------//TIME OF DAY TAB\\------------------------------------------
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y, bgRect.width + offset, 14), tab);
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x+1, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-4, 113, 18), TimeOfDayTab);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("");
		Target.showTimeOfDay = EditorGUILayout.Toggle (Target.showTimeOfDay, EditorStyles.foldout,GUILayout.Width(15));
		EditorGUILayout.EndHorizontal ();
		GUI.DrawTexture (new Rect (bgRect.width-26, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-16, 58, 14), ShowHideTimeOfDay);

		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		if (Target.showTimeOfDay) {
			//Select the Day of the Week
			GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 1).y, bgRect.width + offset, 2), tab);
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button (Sunday)) {
				Target.DAY_of_WEEK = 0;
			}
			if (GUILayout.Button (Monday)) {
				Target.DAY_of_WEEK = 1;
			}
			if (GUILayout.Button (Tuesday)) {
				Target.DAY_of_WEEK = 2;
			}
			if (GUILayout.Button (Wednesday)) {
				Target.DAY_of_WEEK = 3;
			}
			if (GUILayout.Button (Thursday)) {
				Target.DAY_of_WEEK = 4;
			}
			if (GUILayout.Button (Friday)) {
				Target.DAY_of_WEEK = 5;
			}
			if (GUILayout.Button (Saturday)) {
				Target.DAY_of_WEEK = 6;
			}
			EditorGUILayout.EndHorizontal ();
			GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y-3, bgRect.width + offset, 2), tab);
			//Timeline
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ("Timeline");
			Target.TIME_of_DAY  =  EditorGUILayout.Slider(Target.TIME_of_DAY,0.0f, 24.0f);
			EditorGUILayout.EndHorizontal ();
			//UTC
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ("UTC");
			Target.UTC  =  EditorGUILayout.IntSlider(Target.UTC,-12, 12, GUILayout.Width(130));
			EditorGUILayout.EndHorizontal ();
			//Longitude
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ("Longitude");
			Target.Longitude =  EditorGUILayout.Slider(Target.Longitude,-180.0f, 180.0f, GUILayout.Width(130));
			EditorGUILayout.EndHorizontal ();
			//Number of Days
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ("Number of Days");
			Target.NUMBER_of_DAYS =  EditorGUILayout.IntSlider(Target.NUMBER_of_DAYS,1, 7, GUILayout.Width(130));
			EditorGUILayout.EndHorizontal ();
			//Day Cycle
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label ("Day Cycle in Minutes");
			Target.DAY_CYCLE =  EditorGUILayout.FloatField(Target.DAY_CYCLE, GUILayout.Width(50));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Space ();
			//Time Curve
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Day and Night Length", EditorStyles.boldLabel);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Space ();
			//Set Time by Curve?
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Set Time of Day by Curve?");
			Target.SetTime_By_Curve = EditorGUILayout.Toggle (Target.SetTime_By_Curve, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal ();
			//Day and Night Length Curve Field
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("R", GUILayout.Width(25), GUILayout.Height(25))) { Target.DayNightLengthCurve = AnimationCurve.Linear (0, 0, 24, 24); }
			Target.DayNightLengthCurve = EditorGUILayout.CurveField (Target.DayNightLengthCurve, curveColor, new Rect(0,0,24,24), GUILayout.Height(25));
			EditorGUILayout.EndHorizontal ();
			//Show Current Time of Day by Curve
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Current Time by Curve:");
			GUILayout.TextField (Target.TIME_of_DAY_by_CURVE.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
		}
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//=======================================================================================================
		//------------------------------------//OBJECTS AND MATERIALS TAB\\--------------------------------------
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y, bgRect.width + offset, 14), tab);
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x+1, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-4, 102, 18), ObjectsAndMaterialsTab);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("");
		Target.showObj_and_Mat = EditorGUILayout.Toggle (Target.showObj_and_Mat, EditorStyles.foldout,GUILayout.Width(15));
		EditorGUILayout.EndHorizontal ();
		GUI.DrawTexture (new Rect (bgRect.width-26, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-16, 58, 14), ShowHideObjectsAndMaterials);
//		EditorGUILayout.BeginHorizontal();
//		Target.showObj_and_Mat = EditorGUILayout.Foldout (Target.showObj_and_Mat,"Objects and Materials");
//		EditorGUILayout.EndHorizontal ();
//		EditorGUILayout.BeginHorizontal ();
//		GUILayout.Label("Objects and Materials");
//		GUILayout.Label (showObj_and_Mat, GUILayout.Width(43));
//		Target.showObj_and_Mat = EditorGUILayout.Toggle (Target.showObj_and_Mat, EditorStyles.foldout,GUILayout.Width(15));
//		EditorGUILayout.EndHorizontal ();

		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		if (Target.showObj_and_Mat) {
			//Sun Directional Light
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Sun" + "   DirLight");
			Target.Sun_DirectionalLight  =  (GameObject)EditorGUILayout.ObjectField (Target.Sun_DirectionalLight, typeof(GameObject), true,GUILayout.Width (130));
			EditorGUILayout.EndHorizontal ();
			//Moon Directional Light
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Moon DirLight");
			Target.Moon_DirectionalLight  =  (GameObject)EditorGUILayout.ObjectField (Target.Moon_DirectionalLight, typeof(GameObject), true,GUILayout.Width (130));
			EditorGUILayout.EndHorizontal ();
			//Sky Material
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Sky" + "   Material");
			Target.Sky_Material  =  (Material)EditorGUILayout.ObjectField (Target.Sky_Material, typeof(Material), true,GUILayout.Width (130));
			EditorGUILayout.EndHorizontal ();
			//Fog Material
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Fog" + "   Material");
			Target.Fog_Material  =  (Material)EditorGUILayout.ObjectField (Target.Fog_Material, typeof(Material), true,GUILayout.Width (130));
			EditorGUILayout.EndHorizontal ();
			//Moon Material
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Moon Material");
			Target.Moon_Material  =  (Material)EditorGUILayout.ObjectField (Target.Moon_Material, typeof(Material), true,GUILayout.Width (130));
			EditorGUILayout.EndHorizontal ();
			//Reflection Probe
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Reflection Probe");
			Target.AzureReflectionProbe  =  (ReflectionProbe)EditorGUILayout.ObjectField (Target.AzureReflectionProbe, typeof(ReflectionProbe), true,GUILayout.Width (130));
			EditorGUILayout.EndHorizontal ();
		}
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//=======================================================================================================
		//-------------------------------------------//SCATTERING TAB\\------------------------------------------
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y, bgRect.width + offset, 14), tab);
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x+1, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-4, 111, 18), ScatteringTab);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("");
		Target.showScattering = EditorGUILayout.Toggle (Target.showScattering, EditorStyles.foldout,GUILayout.Width(15));
		EditorGUILayout.EndHorizontal ();
		GUI.DrawTexture (new Rect (bgRect.width-26, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-16, 58, 14), ShowHideScattering);

		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		if (Target.showScattering) {
			///Wave-Length///
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Wave-Length", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			// R
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("R", EditorStyles.boldLabel);
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.LambdaCurveR[Day] = AnimationCurve.Linear(0,650.0f,24,650.0f); }
			Target.LambdaCurveR[Day] = EditorGUILayout.CurveField (Target.LambdaCurveR[Day], Color.red, new Rect(0,0,24,1000),GUILayout.Width (curveWidth-3));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.lambda.x.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			// G
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("G", EditorStyles.boldLabel);
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.LambdaCurveG[Day] = AnimationCurve.Linear(0,570.0f,24,570.0f); }
			Target.LambdaCurveG[Day] = EditorGUILayout.CurveField (Target.LambdaCurveG[Day], Color.green, new Rect(0,0,24,1000),GUILayout.MaxWidth (curveWidth-3));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.lambda.y.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			// B
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("B", EditorStyles.boldLabel);
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.LambdaCurveB[Day] = AnimationCurve.Linear(0,475.0f,24,475.0f); }
			Target.LambdaCurveB[Day] = EditorGUILayout.CurveField (Target.LambdaCurveB[Day], Color.blue, new Rect(0,0,24,1000),GUILayout.MaxWidth (curveWidth-3));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.lambda.z.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.Space();
			//Rayleigh
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Rayleigh");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.RayCoeffCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f); }
			Target.RayCoeffCurve[Day] = EditorGUILayout.CurveField (Target.RayCoeffCurve[Day], curveColor, new Rect(0,1,24,4),GUILayout.MaxWidth (curveWidth));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.RayCoeff.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Mie
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Mie");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MieCoeffCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f); }
			Target.MieCoeffCurve[Day] = EditorGUILayout.CurveField (Target.MieCoeffCurve[Day], curveColor, new Rect(0,1,24,4),GUILayout.MaxWidth (curveWidth));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.MieCoeff.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Turbidity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Turbidity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.TurbidityCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f); }
			Target.TurbidityCurve[Day] = EditorGUILayout.CurveField (Target.TurbidityCurve[Day], curveColor, new Rect(0,1,24,4),GUILayout.MaxWidth (curveWidth));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.Turbidity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//g
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("G");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.gCurve[Day] = AnimationCurve.Linear(0,0.75f,24,0.75f); }
			Target.gCurve[Day] = EditorGUILayout.CurveField (Target.gCurve[Day], curveColor, new Rect(0,0,24,1),GUILayout.MaxWidth (curveWidth));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.g.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Kr
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Kr");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.KrCurve[Day] = AnimationCurve.Linear(0,8.4f,24,8.4f); }
			Target.KrCurve[Day] = EditorGUILayout.CurveField (Target.KrCurve[Day], curveColor, new Rect(0,1,24,19),GUILayout.MaxWidth (curveWidth));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.Kr.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Km
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Km");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.KmCurve[Day] = AnimationCurve.Linear(0,1.25f,24,1.25f); }
			Target.KmCurve[Day] = EditorGUILayout.CurveField (Target.KmCurve[Day], curveColor, new Rect(0,1,24,19),GUILayout.MaxWidth (curveWidth));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.Km.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Altitude
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Altitude");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.AltitudeCurve[Day] = AnimationCurve.Linear(0,0.05f,24,0.05f); }
			Target.AltitudeCurve[Day] = EditorGUILayout.CurveField (Target.AltitudeCurve[Day], curveColor, new Rect(0,0,24,0.5f),GUILayout.MaxWidth (curveWidth));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.Altitude.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Sky Coefficient
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Sky Coefficient");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.SkyCoeffCurve[Day] = AnimationCurve.Linear(0,2000.0f,24,2000.0f); }
			Target.SkyCoeffCurve[Day] = EditorGUILayout.CurveField (Target.SkyCoeffCurve[Day], curveColor, new Rect(0,1000,24,2000), GUILayout.MaxWidth (curveWidth));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.SkyCoeff.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Sun Intensity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Sun Intensity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.SunIntensityCurve[Day] = AnimationCurve.Linear(0,100.0f,24,100.0f); }
			Target.SunIntensityCurve[Day] = EditorGUILayout.CurveField (Target.SunIntensityCurve[Day], curveColor, new Rect(0,25,24,75),GUILayout.MaxWidth (curveWidth));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.SunIntensity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Moon Intensity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Moon Intensity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MoonIntensityCurve[Day] = AnimationCurve.Linear(0,0.25f,24,0.25f); }
			Target.MoonIntensityCurve[Day] = EditorGUILayout.CurveField (Target.MoonIntensityCurve[Day], curveColor, new Rect(0,0,24,1),GUILayout.MaxWidth (curveWidth));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.MoonIntensity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
		}
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//=======================================================================================================
		//-----------------------------------------//SKY SETTINGS TAB\\------------------------------------------
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y, bgRect.width + offset, 14), tab);
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x+1, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-4, 129, 18), SkySettingsTab);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("");
		Target.showSkySettings = EditorGUILayout.Toggle (Target.showSkySettings, EditorStyles.foldout,GUILayout.Width(15));
		EditorGUILayout.EndHorizontal ();
		GUI.DrawTexture (new Rect (bgRect.width-26, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-16, 58, 14), ShowHideSkySettings);

		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		if (Target.showSkySettings) {
			///Sun///
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Sky", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Exposure
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Exposure");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.ExposureCurve[Day] = AnimationCurve.Linear(0,1.5f,24,1.5f); }
			Target.ExposureCurve[Day] = EditorGUILayout.CurveField (Target.ExposureCurve[Day], curveColor, new Rect(0,0.0f,24,5),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.Exposure.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Sky Luminance
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Luminance");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.SkyLuminanceCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f); }
			Target.SkyLuminanceCurve[Day] = EditorGUILayout.CurveField (Target.SkyLuminanceCurve[Day], curveColor, new Rect(0,0,24,2),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.SkyLuminance.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Sky Darkness
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Darkness");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.SkyDarknessCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f); }
			Target.SkyDarknessCurve[Day] = EditorGUILayout.CurveField (Target.SkyDarknessCurve[Day], curveColor, new Rect(0,0,24,3.5f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.SkyDarkness.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Sunset Power
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Sunset Power");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.SunsetPowerCurve[Day] = AnimationCurve.Linear(0,3.5f,24,3.5f);}
			Target.SunsetPowerCurve[Day] = EditorGUILayout.CurveField (Target.SunsetPowerCurve[Day], curveColor, new Rect(0,1.5f,24,6.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.SunsetPower.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Sunset Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Sunset Color");
			EditorGUILayout.PropertyField(SunsetGradientColor.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
			EditorGUILayout.EndHorizontal ();

			if(Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, 1).y-2, 119, 3), GradientTab);
			EditorGUILayout.EndVertical ();

//			EditorGUILayout.Space ();
//			EditorGUILayout.Space ();

			///Sun///
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Sun Disk", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Sun Disk Color
//			EditorGUILayout.BeginHorizontal ();
//			GUILayout.Label ("Sun Disk Color");
//			EditorGUILayout.PropertyField(SunGradientColor.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
//			EditorGUILayout.EndHorizontal ();
//
//			if(Target.showGradientTime)
//			GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y-2, 119, 3), GradientTab);

			//Sun Disk Size
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Size");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.SunDiskSizeCurve[Day] = AnimationCurve.Linear(0,250.0f,24,250.0f);}
			Target.SunDiskSizeCurve[Day] = EditorGUILayout.CurveField (Target.SunDiskSizeCurve[Day], curveColor, new Rect(0,100.0f,24,750.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.SunDiskSize.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Sun Disk Intensity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Intensity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.SunDiskIntensityCurve[Day] = AnimationCurve.Linear(0,3.0f,24,3.0f);}
			Target.SunDiskIntensityCurve[Day] = EditorGUILayout.CurveField (Target.SunDiskIntensityCurve[Day], curveColor, new Rect(0,2,24,8),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.SunDiskIntensity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Sun Disk Propagation
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Propagation");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.SunDiskPropagationCurve[Day] = AnimationCurve.Linear(0,-1.5f,24,-1.5f);}
			Target.SunDiskPropagationCurve[Day] = EditorGUILayout.CurveField (Target.SunDiskPropagationCurve[Day], curveColor, new Rect(0,-10.0f,24,9.25f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.SunDiskPropagation.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();

//			EditorGUILayout.Space ();
//			EditorGUILayout.Space ();

			///Stars///
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Stars", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Stars Intensity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Intensity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.StarsIntensityCurve[Day] = AnimationCurve.Linear(0,5.0f,24,5.0f);}
			Target.StarsIntensityCurve[Day] = EditorGUILayout.CurveField (Target.StarsIntensityCurve[Day], curveColor, new Rect(0,0.0f,24,10.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.StarsIntensity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Stars Extinction
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Extinction");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.StarsExtinctionCurve[Day] = AnimationCurve.Linear(0,0.5f,24,0.5f);}
			Target.StarsExtinctionCurve[Day] = EditorGUILayout.CurveField (Target.StarsExtinctionCurve[Day], curveColor, new Rect(0,0.1f,24,4.9f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.StarsExtinction.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Stars Scintillation
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Sparkle");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.StarsScintillation = 5.5f;}
			Target.StarsScintillation = EditorGUILayout.Slider(Target.StarsScintillation,1.0f, 10.0f, GUILayout.Width (119));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();

//			EditorGUILayout.Space ();
//			EditorGUILayout.Space ();

			///Stars///
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Moon", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Moon Texture Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Color");
			EditorGUILayout.PropertyField(MoonGradientColor.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
			EditorGUILayout.EndHorizontal ();

			if(Target.showGradientTime)
			GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y-2, 119, 3), GradientTab);

			//MoonLight Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Bright Color");
			EditorGUILayout.PropertyField(MoonBrightGradientColor.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
			EditorGUILayout.EndHorizontal ();

			if(Target.showGradientTime)
			GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y-2, 119, 3), GradientTab);

			//Moon Color Power
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Color Power");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MoonColorPowerCurve[Day] = AnimationCurve.Linear(0,2.15f,24,2.15f);}
			Target.MoonColorPowerCurve[Day] = EditorGUILayout.CurveField (Target.MoonColorPowerCurve[Day], curveColor, new Rect(0,1.0f,24,9.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.MoonColorPower.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();

			//Moon Size
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Size");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MoonSizeCurve[Day] = AnimationCurve.Linear(0,5.0f,24,5.0f);}
			Target.MoonSizeCurve[Day] = EditorGUILayout.CurveField (Target.MoonSizeCurve[Day], curveColor, new Rect(0,2.5f,24,10.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.MoonSize.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Moon Extinction
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Extinction");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MoonExtinctionCurve[Day] = AnimationCurve.Linear(0,0.5f,24,0.5f);}
			Target.MoonExtinctionCurve[Day] = EditorGUILayout.CurveField (Target.MoonExtinctionCurve[Day], curveColor, new Rect(0,0.1f,24,4.9f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.MoonExtinction.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.Space();
			//Moon Eclipse Shadow
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Eclipse?");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MoonEclipseShadow = 1;}
			Target.MoonEclipseShadow = EditorGUILayout.IntSlider(Target.MoonEclipseShadow,0, 1, GUILayout.MaxWidth (119));
			EditorGUILayout.EndHorizontal ();
			//Umbra
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Umbra");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.Umbra = 0.95f;}
			Target.Umbra = EditorGUILayout.Slider(Target.Umbra,0, 1, GUILayout.MaxWidth (119));
			EditorGUILayout.EndHorizontal ();
			//Umbra Size
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Umbra Size");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.UmbraSize = 0.25f;}
			Target.UmbraSize = EditorGUILayout.Slider(Target.UmbraSize,1, 0, GUILayout.MaxWidth (119));
			EditorGUILayout.EndHorizontal ();
			//Penumbra
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Penumbra");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.Penumbra = 3.0f;}
			Target.Penumbra = EditorGUILayout.Slider(Target.Penumbra,0, 10, GUILayout.MaxWidth (119));
			EditorGUILayout.EndHorizontal ();
			//Penumbra Size
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Penumbra Size");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.PenumbraSize = 0.5f;}
			Target.PenumbraSize = EditorGUILayout.Slider(Target.PenumbraSize,1, 0, GUILayout.MaxWidth (119));
			EditorGUILayout.EndHorizontal ();
			//Penumbra Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Penumbra Color");
			Target.PenumbraColor = EditorGUILayout.ColorField(Target.PenumbraColor,GUILayout.MaxWidth (119));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();

//			EditorGUILayout.Space ();
//			EditorGUILayout.Space ();

			///Milky Way///
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Milky Way", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Intensity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Intensity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MilkyWayIntensityCurve[Day] = AnimationCurve.Linear(0,0.0f,24,0.0f);}
			Target.MilkyWayIntensityCurve[Day] = EditorGUILayout.CurveField (Target.MilkyWayIntensityCurve[Day], curveColor, new Rect(0,0.0f,24,1.5f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.MilkyWayIntensity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Power
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Power");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MilkyWayPowerCurve[Day] = AnimationCurve.Linear(0,2.5f,24,2.5f);}
			Target.MilkyWayPowerCurve[Day] = EditorGUILayout.CurveField (Target.MilkyWayPowerCurve[Day], curveColor, new Rect(0,1.0f,24,3.0f),GUILayout.Width (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.MilkyWayPower.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Positions
			EditorGUILayout.Space();
			GUILayout.Label ("Position:");
			//X
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("X:");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MilkyWayPos.x = 0;}
			Target.MilkyWayPos.x = EditorGUILayout.Slider(Target.MilkyWayPos.x,0.0f, 360.0f, GUILayout.Width (118));
			EditorGUILayout.EndHorizontal ();
			//Y
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Y:");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MilkyWayPos.y = 0;}
			Target.MilkyWayPos.y = EditorGUILayout.Slider(Target.MilkyWayPos.y,0.0f, 360.0f, GUILayout.Width (118));
			EditorGUILayout.EndHorizontal ();
			//Z
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Z:");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MilkyWayPos.z = 0;}
			Target.MilkyWayPos.z = EditorGUILayout.Slider(Target.MilkyWayPos.z,0.0f, 360.0f, GUILayout.Width (118));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();

			///Ground Color///
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Night Sky Ground Color", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Close Ground Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Close Color");
			EditorGUILayout.PropertyField(NightGroundCloseGradientColor.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
			EditorGUILayout.EndHorizontal ();
			if(Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y-2, 119, 3), GradientTab);
//			EditorGUILayout.Space ();
//			EditorGUILayout.Space ();
			//Far Ground Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Far Color");
			EditorGUILayout.PropertyField(NightGroundFarGradientColor.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
			EditorGUILayout.EndHorizontal ();
			if(Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y-2, 119, 3), GradientTab);
			//Far Color Distance
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Distance");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.NightSkyFarColorDistanceCurve[Day] = AnimationCurve.Linear(0,0.5f,24,0.5f);}
			Target.NightSkyFarColorDistanceCurve[Day] = EditorGUILayout.CurveField (Target.NightSkyFarColorDistanceCurve[Day], curveColor, new Rect(0,0.0f,24,2.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.NightSkyFarColorDistance.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Far Color Power
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Power");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.NightSkyFarColorPowerCurve[Day] = AnimationCurve.Linear(0,0.25f,24,0.25f);}
			Target.NightSkyFarColorPowerCurve[Day] = EditorGUILayout.CurveField (Target.NightSkyFarColorPowerCurve[Day], curveColor, new Rect(0,0.0f,24,0.5f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.NightSkyFarColorPower.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
		}
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//=======================================================================================================
		//-----------------------------------------//FOG SETTINGS TAB\\------------------------------------------
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y, bgRect.width + offset, 14), tab);
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x+1, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-4, 128, 18), FogSettingsTab);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("");
		Target.showFogSettings = EditorGUILayout.Toggle (Target.showFogSettings, EditorStyles.foldout,GUILayout.Width(15));
		EditorGUILayout.EndHorizontal ();
		GUI.DrawTexture (new Rect (bgRect.width-26, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-16, 58, 14), ShowHideFogSettings);

		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		if (Target.showFogSettings) {
			//Linear Fog Toogle
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Linear Fog?");
			Target.LinearFog = EditorGUILayout.Toggle (Target.LinearFog, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal ();
			//Global Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Global Color");
			EditorGUILayout.PropertyField(GlobalFogGradientColor.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (123));
			EditorGUILayout.EndHorizontal ();
			if(Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y-2, 123, 3), GradientTab);
			//Scattering Fog distance
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Scattering Fog", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Distance");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.ScatteringFogDistanceCurve[Day] = AnimationCurve.Linear(0,3.0f,24,3.0f); }
			Target.ScatteringFogDistanceCurve[Day] = EditorGUILayout.CurveField (Target.ScatteringFogDistanceCurve[Day], curveColor, new Rect(0,0.0f,24,20.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.ScatteringFogDistance.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical();
			//Blend Point
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Blend Point");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.FogBlendPointCurve[Day] = AnimationCurve.Linear(0,3.0f,24,3.0f); }
			Target.FogBlendPointCurve[Day] = EditorGUILayout.CurveField (Target.FogBlendPointCurve[Day], curveColor, new Rect(0,0.0f,24,50.0f),GUILayout.MaxWidth (curveWidth-3));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.FogBlendPoint.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Normal Fog
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Simple Fog", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Distance");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.NormalFogDistanceCurve[Day] = AnimationCurve.Linear(0,10.0f,24,10.0f); }
			Target.NormalFogDistanceCurve[Day] = EditorGUILayout.CurveField (Target.NormalFogDistanceCurve[Day], curveColor, new Rect(0,0.0f,24,50.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.NormalFogDistance.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Normal Fog Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Color");
			EditorGUILayout.PropertyField(NormalFogGradientColor.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
			EditorGUILayout.EndHorizontal ();
			if(Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, 1).y-2, 119, 3), GradientTab);
			EditorGUILayout.EndVertical();
			//Normal Fog
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Dense Fog", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Dense Fog Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Color");
			EditorGUILayout.PropertyField(DenseFogGradientColor.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
			EditorGUILayout.EndHorizontal ();
			if(Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y-2, 119, 3), GradientTab);
			//"Dense Fog" Intensity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Intensity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.DenseFogIntensityCurve[Day] = AnimationCurve.Linear(0,0.0f,24,0.0f); }
			Target.DenseFogIntensityCurve[Day] = EditorGUILayout.CurveField (Target.DenseFogIntensityCurve[Day], curveColor, new Rect(0,0.0f,24,1.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.DenseFogIntensity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//"Dense Fog" Altitude
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Altitude");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.DenseFogAltitudeCurve[Day] = AnimationCurve.Linear(0,-0.8f,24,-0.8f); }
			Target.DenseFogAltitudeCurve[Day] = EditorGUILayout.CurveField (Target.DenseFogAltitudeCurve[Day], curveColor, new Rect(0,-1.0f,24,2.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.DenseFogAltitude.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical();


			//Normal Fog
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Unity's Fog", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Use Unity's Fog?");
			Target.UseUnityFog = EditorGUILayout.Toggle (Target.UseUnityFog, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal ();
			if (Target.UseUnityFog) {
				if (RenderSettings.fog == false) {
					RenderSettings.fog = true;
				}
				//Unity' Fog Color
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Fog Color");
				EditorGUILayout.PropertyField (UnityFogGradientColor.GetArrayElementAtIndex (Day), GUIContent.none, GUILayout.Width (123));
				EditorGUILayout.EndHorizontal ();
				//Fog Mode
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Fog Mode");
				Target.UnityFogModeIndex = EditorGUILayout.Popup (Target.UnityFogModeIndex, unityFogMode, GUILayout.Width (123));
				EditorGUILayout.EndHorizontal ();
				switch (Target.UnityFogModeIndex) {
				case 0:
					RenderSettings.fogMode = FogMode.Linear;
					//Start
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("Start");
					if (GUILayout.Button ("R", GUILayout.Width (18), GUILayout.Height (15))) {
						Target.UnityFogStartCurve [Day] = AnimationCurve.Linear (0, 0.0f, 24, 0.0f);
					}
					Target.UnityFogStartCurve [Day] = EditorGUILayout.CurveField (Target.UnityFogStartCurve [Day], curveColor, new Rect (0, 0.0f, 24, 1000.0f), GUILayout.MaxWidth (curveWidth - 7));
					if (Target.showCurveValue)
						GUILayout.TextField (Target.UnityFogStart.ToString (), GUILayout.Width (curveValueWidth));
					EditorGUILayout.EndHorizontal ();
					//End
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("End");
					if (GUILayout.Button ("R", GUILayout.Width (18), GUILayout.Height (15))) {
						Target.UnityFogEndCurve [Day] = AnimationCurve.Linear (0, 300.0f, 24, 300.0f);
					}
					Target.UnityFogEndCurve [Day] = EditorGUILayout.CurveField (Target.UnityFogEndCurve [Day], curveColor, new Rect (0, 0.0f, 24, 10000.0f), GUILayout.MaxWidth (curveWidth - 7));
					if (Target.showCurveValue)
						GUILayout.TextField (Target.UnityFogEnd.ToString (), GUILayout.Width (curveValueWidth));
					EditorGUILayout.EndHorizontal ();
					break;
				case 1:
					RenderSettings.fogMode = FogMode.Exponential;
					//Density
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("Density");
					if (GUILayout.Button ("R", GUILayout.Width (18), GUILayout.Height (15))) {
						Target.UnityFogDensityCurve [Day] = AnimationCurve.Linear (0, 1.0f, 24, 1.0f);
					}
					Target.UnityFogDensityCurve [Day] = EditorGUILayout.CurveField (Target.UnityFogDensityCurve [Day], curveColor, new Rect (0, 0.0f, 24, 100.0f), GUILayout.MaxWidth (curveWidth - 7));
					if (Target.showCurveValue)
						GUILayout.TextField (Target.UnityFogDensity.ToString (), GUILayout.Width (curveValueWidth));
					EditorGUILayout.EndHorizontal ();
					break;
				case 2:
					RenderSettings.fogMode = FogMode.ExponentialSquared;
					//Density
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("Density");
					if (GUILayout.Button ("R", GUILayout.Width (18), GUILayout.Height (15))) {
						Target.UnityFogDensityCurve [Day] = AnimationCurve.Linear (0, 1.0f, 24, 1.0f);
					}
					Target.UnityFogDensityCurve [Day] = EditorGUILayout.CurveField (Target.UnityFogDensityCurve [Day], curveColor, new Rect (0, 0.0f, 24, 100.0f), GUILayout.MaxWidth (curveWidth - 7));
					if (Target.showCurveValue)
						GUILayout.TextField (Target.UnityFogDensity.ToString (), GUILayout.Width (curveValueWidth));
					EditorGUILayout.EndHorizontal ();
					break;
				}
			} else {
				if (RenderSettings.fog == true) {
					RenderSettings.fog = false;
				}
			}
		

			EditorGUILayout.EndVertical();
		}
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//=======================================================================================================
		//---------------------------------------//CLOUD SETTINGS TAB\\------------------------------------------
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y, bgRect.width + offset, 14), tab);
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x+1, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-4, 150, 18), CloudSettingsTab);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("");
		Target.showCloudSettings = EditorGUILayout.Toggle (Target.showCloudSettings, EditorStyles.foldout,GUILayout.Width(15));
		EditorGUILayout.EndHorizontal ();
		GUI.DrawTexture (new Rect (bgRect.width-26, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-16, 58, 14), ShowHideCloudSettings);

		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		if (Target.showCloudSettings) {
			//Cloud Mode
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Cloud Mode");
			Target.cloudModeIndex = EditorGUILayout.Popup(Target.cloudModeIndex, cloudMode, GUILayout.Width (123));
			EditorGUILayout.EndHorizontal ();
		switch (Target.cloudModeIndex) {
		case 0:
			Target.ChangeShader (0);
			break;
	    ///////////////////////
		//Pre-Rendered Clouds//
		case 1:
			Target.ChangeShader (1);
			//Edge Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Edge Color");
			EditorGUILayout.PropertyField (EdgeColorGradientColor.GetArrayElementAtIndex (Day), GUIContent.none, GUILayout.Width (123));
			EditorGUILayout.EndHorizontal ();
			if (Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width - 89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y - 2, 123, 3), GradientTab);
			//Dark Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Dark Color");
			EditorGUILayout.PropertyField (DarkColorGradientColor.GetArrayElementAtIndex (Day), GUIContent.none, GUILayout.Width (123));
			EditorGUILayout.EndHorizontal ();
			if (Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width - 89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y - 2, 123, 3), GradientTab);
			//Cloud Density
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Density");
			if (GUILayout.Button ("R", GUILayout.Width (18), GUILayout.Height (15))) {
				Target.CloudDensityCurve [Day] = AnimationCurve.Linear (0, 1.0f, 24, 1.0f);
			}
			Target.CloudDensityCurve [Day] = EditorGUILayout.CurveField (Target.CloudDensityCurve [Day], curveColor, new Rect (0, 0.0f, 24, 5.0f), GUILayout.MaxWidth (curveWidth -3));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.CloudDensity.ToString (), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Cloud Extinction
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Extinction");
			if (GUILayout.Button ("R", GUILayout.Width (18), GUILayout.Height (15))) {
				Target.CloudExtinctionCurve [Day] = AnimationCurve.Linear (0, 3.0f, 24, 3.0f);
			}
			Target.CloudExtinctionCurve [Day] = EditorGUILayout.CurveField (Target.CloudExtinctionCurve [Day], curveColor, new Rect (0, 1.0f, 24, 9.0f), GUILayout.MaxWidth (curveWidth - 3));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.CloudExtinction.ToString (), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Alpha Saturation
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Alpha");
			if (GUILayout.Button ("R", GUILayout.Width (18), GUILayout.Height (15))) {Target.AlphaSaturationCurve [Day] = AnimationCurve.Linear (0, 2.0f, 24, 2.0f);}
			Target.AlphaSaturationCurve [Day] = EditorGUILayout.CurveField (Target.AlphaSaturationCurve [Day], curveColor, new Rect (0, 1.0f, 24, 4.0f), GUILayout.MaxWidth (curveWidth - 3));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.AlphaSaturation.ToString (), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Cloud Altitude
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Altitude");
			if (GUILayout.Button ("R", GUILayout.Width (18), GUILayout.Height (15))) {Target.PreRenderedCloudAltitudeCurve [Day] = AnimationCurve.Linear (0, 0.05f, 24, 0.05f);}
			Target.PreRenderedCloudAltitudeCurve [Day] = EditorGUILayout.CurveField (Target.PreRenderedCloudAltitudeCurve [Day], curveColor, new Rect (0, 0, 24, 0.5f), GUILayout.MaxWidth (curveWidth - 3));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.PreRenderedCloudAltitude.ToString (), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space ();
			GUILayout.Label ("Moon Bright", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Moon Bright Intensity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Intensity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MoonBrightIntensityCurve[Day] = AnimationCurve.Linear(0,3.0f,24,3.0f);}
			Target.MoonBrightIntensityCurve[Day] = EditorGUILayout.CurveField (Target.MoonBrightIntensityCurve[Day], curveColor, new Rect(0,1.0f,24,6.5f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.MoonBrightIntensity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Moon Bright Range
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Range");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MoonBrightRangeCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f);}
			Target.MoonBrightRangeCurve[Day] = EditorGUILayout.CurveField (Target.MoonBrightRangeCurve[Day], curveColor, new Rect(0,1.0f,24,4.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.MoonBrightRange.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();

			break;
			/////////////////////
			//Procedural Clouds//
			case 2:
				Target.ChangeShader (2);
				EditorGUILayout.BeginVertical ("Box");
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.Space ();
				GUILayout.Label ("Wispy Clouds", EditorStyles.miniLabel);
				EditorGUILayout.EndHorizontal ();

				//Wispy Covarage
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Covarage");
				if (GUILayout.Button ("R", GUILayout.Width (18), GUILayout.Height (15))) {
					Target.WispyCovarageCurve [Day] = AnimationCurve.Linear (0, 0.0f, 24, 0.0f);
				}
				Target.WispyCovarageCurve [Day] = EditorGUILayout.CurveField (Target.WispyCovarageCurve [Day], curveColor, new Rect (0, 0.0f, 24, 10.0f), GUILayout.MaxWidth (curveWidth - 7));
				if (Target.showCurveValue)
					GUILayout.TextField (Target.WispyCovarage.ToString (), GUILayout.Width (curveValueWidth));
				EditorGUILayout.EndHorizontal ();
				//Wispy Speed
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Speed");
				if (GUILayout.Button ("R", GUILayout.Width (18), GUILayout.Height (15))) {
					Target.WispyCloudSpeedCurve [Day] = AnimationCurve.Linear (0, 5.0f, 24, 5.0f);
				}
				Target.WispyCloudSpeedCurve [Day] = EditorGUILayout.CurveField (Target.WispyCloudSpeedCurve [Day], curveColor, new Rect (0, 0.0f, 24, 20.0f), GUILayout.MaxWidth (curveWidth - 7));
				if (Target.showCurveValue)
					GUILayout.TextField (Target.WispyCloudSpeed.ToString (), GUILayout.Width (curveValueWidth));
				EditorGUILayout.EndHorizontal ();
				//Cloud Direction
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Direction");
				if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.StarsScintillation = 0.0f;}
				Target.WispyCloudDirection = EditorGUILayout.Slider(Target.WispyCloudDirection,0.0f, 3.0f, GUILayout.Width (119));
				EditorGUILayout.EndHorizontal ();

				//Dark Color
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Darkness Color");
				EditorGUILayout.PropertyField (WispyDarknessGradientColor.GetArrayElementAtIndex (Day), GUIContent.none, GUILayout.Width (119));
				EditorGUILayout.EndHorizontal ();
				if (Target.showGradientTime)
					GUI.DrawTexture (new Rect (bgRect.width - 89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y - 2, 119, 3), GradientTab);
				//Bright Color
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Bright Color");
				EditorGUILayout.PropertyField (WispyBrightGradientColor.GetArrayElementAtIndex (Day), GUIContent.none, GUILayout.Width (119));
				EditorGUILayout.EndHorizontal ();
				if (Target.showGradientTime)
					GUI.DrawTexture (new Rect (bgRect.width - 89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y - 2, 119, 3), GradientTab);
				//Wispy Color
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Wispy Color");
				EditorGUILayout.PropertyField (WispyColorGradientColor.GetArrayElementAtIndex (Day), GUIContent.none, GUILayout.Width (119));
				EditorGUILayout.EndHorizontal ();
				if (Target.showGradientTime)
					GUI.DrawTexture (new Rect (bgRect.width - 89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y - 2, 119, 3), GradientTab);
				//Wispy Texture
				EditorGUILayout.Space ();
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Wispy Texture");
				Target.WispyCloudTexture = EditorGUILayout.ObjectField (Target.WispyCloudTexture, typeof(Texture2D), true, GUILayout.Width (119), GUILayout.Height (119)) as Texture2D;
				EditorGUILayout.EndHorizontal ();



				EditorGUILayout.EndVertical ();
				break;
			}
		}
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//=======================================================================================================
		//--------------------------------------------//AMBIENT TAB\\--------------------------------------------
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y, bgRect.width + offset, 14), tab);
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x+1, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-4, 80, 18), AmbientTab);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("");
		Target.showAmbient = EditorGUILayout.Toggle (Target.showAmbient, EditorStyles.foldout,GUILayout.Width(15));
		EditorGUILayout.EndHorizontal ();
		GUI.DrawTexture (new Rect (bgRect.width-26, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-16, 58, 14), ShowHideAmbient);

		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		if (Target.showAmbient) {
			EditorGUILayout.BeginVertical ("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space ();
			GUILayout.Label ("Ambient", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Ambient Source
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Source");
			Target.ambientSourceIndex = EditorGUILayout.Popup(Target.ambientSourceIndex, ambientSource, GUILayout.Width (119));
			EditorGUILayout.EndHorizontal ();
			//Ambient Intensity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Intensity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.AmbientIntensityCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f);}
			Target.AmbientIntensityCurve[Day] = EditorGUILayout.CurveField (Target.AmbientIntensityCurve[Day], curveColor, new Rect(0,0,24,8.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.AmbientIntensity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();

			switch (Target.ambientSourceIndex){
			case 0:
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
				break;
			case 1:
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
				//Sky Color
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Sky Color");
				EditorGUILayout.PropertyField(SkyAmbientColorGradient.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
				EditorGUILayout.EndHorizontal ();

				if(Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y-2, 119, 3), GradientTab);

				//Equator Color
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Equator Color");
				EditorGUILayout.PropertyField(EquatorAmbientColorGradient.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
				EditorGUILayout.EndHorizontal ();

				if(Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, -1).y-2, 119, 3), GradientTab);

				//Ground Color
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Ground Color");
				EditorGUILayout.PropertyField(GroundAmbientColorGradient.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
				EditorGUILayout.EndHorizontal ();

				if(Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, 1).y-2, 119, 3), GradientTab);

				break;
			case 2:
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

				//Ambient Color
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Ambient Color");
				EditorGUILayout.PropertyField(AmbientColorGradient.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (119));
				EditorGUILayout.EndHorizontal ();

				if(Target.showGradientTime)
				GUI.DrawTexture (new Rect (bgRect.width-89, GUILayoutUtility.GetRect (bgRect.width + offset, 1).y-2, 119, 3), GradientTab);

				break;

			}
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space ();
			GUILayout.Label ("Reflections", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Reflection Intensity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Intensity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.ReflectionIntensityCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f);}
			Target.ReflectionIntensityCurve[Day] = EditorGUILayout.CurveField (Target.ReflectionIntensityCurve[Day], curveColor, new Rect(0,0,24,1.0f),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.ReflectionIntensity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Reflection Bounces
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Bounces");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.ReflectionBouncesCurve[Day] = AnimationCurve.Linear(0,1,24,1);}
			Target.ReflectionBouncesCurve[Day] = EditorGUILayout.CurveField (Target.ReflectionBouncesCurve[Day], curveColor, new Rect(0,1,24,4),GUILayout.MaxWidth (curveWidth-7));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.ReflectionBounces.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			EditorGUILayout.EndVertical ();

			//Reflection
			if (Target.useReflectionProbe)
			{
				EditorGUILayout.BeginVertical ("Box");
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.Space ();
				GUILayout.Label ("Reflection Probe", EditorStyles.miniLabel);
				EditorGUILayout.EndHorizontal ();

				//Culling Mask
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Culling Mask");
				EditorGUILayout.PropertyField(ReflectionProbeCullingMask, GUIContent.none,GUILayout.Width (119));
				EditorGUILayout.EndHorizontal ();

				//Time Slicing
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Time Slicing");
				Target.ReflectionProbeTimeSlicing = EditorGUILayout.Popup (Target.ReflectionProbeTimeSlicing, reflectionTimeSlicing, GUILayout.Width (119));
				EditorGUILayout.EndHorizontal ();
				switch (Target.ReflectionProbeTimeSlicing) {
				case 0:
					Target.AzureReflectionProbe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
					break;
				case 1:
					Target.AzureReflectionProbe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.IndividualFaces;
					break;
				case 2:
					Target.AzureReflectionProbe.timeSlicingMode = UnityEngine.Rendering.ReflectionProbeTimeSlicingMode.NoTimeSlicing;
					break;
				}

				//Refresh Mode
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Refresh Mode");
				Target.ReflectionProbeRefreshMode = EditorGUILayout.Popup (Target.ReflectionProbeRefreshMode, reflectionRefreshMode, GUILayout.Width (119));
				EditorGUILayout.EndHorizontal ();
				switch (Target.ReflectionProbeRefreshMode) {
				case 0:
					Target.AzureReflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
					break;
				case 1:
					Target.AzureReflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
					break;
				case 2:
					Target.AzureReflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;
					//Refresh Mode
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("Time to Refresh");
					Target.ReflectionProbeTimeToUpdate = EditorGUILayout.FloatField(Target.ReflectionProbeTimeToUpdate, GUILayout.Width (119)) ;
					EditorGUILayout.EndHorizontal ();
					//Force Update at First Frame?
					EditorGUILayout.BeginHorizontal ();
					GUILayout.Label ("Force update at first frame?");
					Target.ForceProbeUpdateAtFirstFrame = EditorGUILayout.Toggle (Target.ForceProbeUpdateAtFirstFrame, GUILayout.Width(15));
					EditorGUILayout.EndHorizontal ();
					EditorGUILayout.Space ();
					break;
				}
				//Reflection Probe Intensity
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Intensity");
				if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.ReflectionProbeIntensityCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f);}
				Target.ReflectionProbeIntensityCurve[Day] = EditorGUILayout.CurveField (Target.ReflectionProbeIntensityCurve[Day], curveColor, new Rect(0,0,24,8.0f),GUILayout.MaxWidth (curveWidth-7));
				if (Target.showCurveValue)
					GUILayout.TextField (Target.ReflectionProbeIntensity.ToString(), GUILayout.Width (curveValueWidth));
				EditorGUILayout.EndHorizontal ();

				//Reflection Probe Size
				Target.ReflectionProbeSize = EditorGUILayout.Vector3Field("Size", Target.ReflectionProbeSize);
				Target.AzureReflectionProbe.size = Target.ReflectionProbeSize;

				EditorGUILayout.EndVertical ();
			}

		}
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//=======================================================================================================
		//-------------------------------------------//LIGHTING TAB\\--------------------------------------------
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y, bgRect.width + offset, 14), tab);
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x+1, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-4, 82, 18), LightingTab);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("");
		Target.showLighting = EditorGUILayout.Toggle (Target.showLighting, EditorStyles.foldout,GUILayout.Width(15));
		EditorGUILayout.EndHorizontal ();
		GUI.DrawTexture (new Rect (bgRect.width-26, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-16, 58, 14), ShowHideLighting);

		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		if (Target.showLighting) {
			//Sun Directional Light Intensity
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Sun Directional Light", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Intensity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Intensity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.SunDirLightIntensityCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f);}
			Target.SunDirLightIntensityCurve[Day] = EditorGUILayout.CurveField (Target.SunDirLightIntensityCurve[Day], curveColor, new Rect(0,0,24,8.0f),GUILayout.MaxWidth (curveWidth-3));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.SunDirLightIntensity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Color");
			EditorGUILayout.PropertyField(SunDirLightColorGradient.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (123));
			EditorGUILayout.EndHorizontal ();

			if(Target.showGradientTime)
			GUI.DrawTexture (new Rect (bgRect.width-93, GUILayoutUtility.GetRect (bgRect.width + offset, 1).y-2, 123, 3), GradientTab);

			//Flare Curve
			if (Target.UseSunLensFlare) {
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Flare Intensity");
				if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.SunFlareIntensityCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f);}
				Target.SunFlareIntensityCurve[Day] = EditorGUILayout.CurveField (Target.SunFlareIntensityCurve[Day], curveColor, new Rect(0,0,24,8.0f),GUILayout.MaxWidth (curveWidth-3));
				if (Target.showCurveValue)
					GUILayout.TextField (Target.SunFlareIntensity.ToString(), GUILayout.Width (curveValueWidth));
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.EndVertical();
			//Moon Directional Light Intensity
			EditorGUILayout.BeginVertical("Box");
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Moon Directional Light", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Intensity
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Intensity");
			if (GUILayout.Button ("R", GUILayout.Width(18), GUILayout.Height(15))) { Target.MoonDirLightIntensityCurve[Day] = AnimationCurve.Linear(0,1.0f,24,1.0f);}
			Target.MoonDirLightIntensityCurve[Day] = EditorGUILayout.CurveField (Target.MoonDirLightIntensityCurve[Day], curveColor, new Rect(0,0,24,8.0f),GUILayout.Width (curveWidth-3));
			if (Target.showCurveValue)
				GUILayout.TextField (Target.MoonDirLightIntensity.ToString(), GUILayout.Width (curveValueWidth));
			EditorGUILayout.EndHorizontal ();
			//Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Color");
			EditorGUILayout.PropertyField(MoonDirLightColorGradient.GetArrayElementAtIndex(Day), GUIContent.none,GUILayout.Width (123));
			EditorGUILayout.EndHorizontal ();

			if(Target.showGradientTime)
			GUI.DrawTexture (new Rect (bgRect.width-93, GUILayoutUtility.GetRect (bgRect.width + offset, 1).y-2, 123, 3), GradientTab);

			EditorGUILayout.EndVertical();
		}
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//=======================================================================================================
		//-------------------------------------------//TEXTURES TAB\\--------------------------------------------
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y, bgRect.width + offset, 14), tab);
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x+1, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-4, 94, 18), TexturesTab);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("");
		Target.showTextures = EditorGUILayout.Toggle (Target.showTextures, EditorStyles.foldout,GUILayout.Width(15));
		EditorGUILayout.EndHorizontal ();
		GUI.DrawTexture (new Rect (bgRect.width-26, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-16, 58, 14), ShowHideTextures);

		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		if (Target.showTextures) {
			//Moon Texture
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Moon (RenderTexture)");
			Target.MoonTexture = (RenderTexture)EditorGUILayout.ObjectField(Target.MoonTexture,typeof(RenderTexture),true,GUILayout.Width(60),GUILayout.Height(60));
			EditorGUILayout.EndHorizontal ();
			//Star Field Cubemap
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Star Field (Cubemap)");
			Target.StarField = (Cubemap)EditorGUILayout.ObjectField(Target.StarField,typeof(Cubemap),true,GUILayout.Width(60),GUILayout.Height(60));
			EditorGUILayout.EndHorizontal ();
			//Star Noise Cubemap
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Star Noise (Cubemap)");
			Target.StarNoise = (Cubemap)EditorGUILayout.ObjectField(Target.StarNoise,typeof(Cubemap),true,GUILayout.Width(60),GUILayout.Height(60));
			EditorGUILayout.EndHorizontal ();
			//Milky Way Cubemap
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Milky Way (Cubemap)");
			Target.MilkyWay = (Cubemap)EditorGUILayout.ObjectField(Target.MilkyWay,typeof(Cubemap),true,GUILayout.Width(60),GUILayout.Height(60));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.Space();
		}
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//=======================================================================================================
		//--------------------------------------------//OPTIONS TAB\\--------------------------------------------
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y, bgRect.width + offset, 14), tab);
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x+1, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-4, 79, 18), OptionsTab);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("");
		Target.showOptions = EditorGUILayout.Toggle (Target.showOptions, EditorStyles.foldout,GUILayout.Width(15));
		EditorGUILayout.EndHorizontal ();
		GUI.DrawTexture (new Rect (bgRect.width-26, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-16, 58, 14), ShowHideOptions);

		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		if (Target.showOptions) {
			//Sky Options
			EditorGUILayout.BeginVertical("Box");
		    
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Sky Options", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Sky Update?
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Sky Update?");
			Target.skyUpdate = EditorGUILayout.Toggle (Target.skyUpdate, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal ();
			//Sky HDR?
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("HDR Sky?");
			Target.SkyHDR = EditorGUILayout.Toggle (Target.SkyHDR, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal ();
			//Use Sun Lens Flare?
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Use Sun Lens Flare?");
			Target.UseSunLensFlare = EditorGUILayout.Toggle (Target.UseSunLensFlare, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal ();
			//Use Reflection Probe?
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Use Reflection Probe?");
			Target.useReflectionProbe = EditorGUILayout.Toggle (Target.useReflectionProbe, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal ();
			if (Target.useReflectionProbe) {
				Target.AzureReflectionProbe.gameObject.SetActive (true);
			} else {
					Target.AzureReflectionProbe.gameObject.SetActive (false);
			}
			//Space Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Space Color");
			Target.SpaceColorIndex = EditorGUILayout.Popup(Target.SpaceColorIndex, spaceColor, GUILayout.Width(127));
			EditorGUILayout.EndHorizontal ();
			switch(Target.SpaceColorIndex)
			{
			case 0:
				Target.ColorCorrection = 1.0f;
				Target.WispyColorCorrection = 2.2f;
				break;
			case 1:
				Target.ColorCorrection = 2.2f;
				Target.WispyColorCorrection = 1.0f;
				break;
			}

			EditorGUILayout.EndVertical();

			//Editor Options
			EditorGUILayout.BeginVertical("Box");

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.Space();
			GUILayout.Label ("Editor Options", EditorStyles.miniLabel);
			EditorGUILayout.EndHorizontal ();
			//Gradient Time Marker?
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Gradient Time Markers?");
			Target.showGradientTime = EditorGUILayout.Toggle (Target.showGradientTime, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal ();
			//show Curve Value?
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Show Current Curve Value?");
			Target.showCurveValue = EditorGUILayout.Toggle (Target.showCurveValue, GUILayout.Width(15));
			EditorGUILayout.EndHorizontal ();
			// Curve Color
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label("Curve Color");
			Target.CurveColorField = EditorGUILayout.ColorField(Target.CurveColorField,GUILayout.Width(127));
			EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical();
		}
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		//=======================================================================================================
		//--------------------------------------------//OUTPUT TAB\\---------------------------------------------
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y, bgRect.width + offset, 14), tab);
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x+1, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-4, 74, 18), OutputTab);
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label("");
		Target.showOutput = EditorGUILayout.Toggle (Target.showOutput, EditorStyles.foldout,GUILayout.Width(15));
		EditorGUILayout.EndHorizontal ();
		GUI.DrawTexture (new Rect (bgRect.width-26, GUILayoutUtility.GetRect (bgRect.width + offset, -3).y-16, 58, 14), ShowHideOutput);

		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		if (Target.showOutput) {
			//Output Values
//			serializedObject.Update();
			EditorGUILayout.Space();
			reorderCurveList.DoLayoutList();
			EditorGUILayout.Space();
			reorderGradientList.DoLayoutList ();
//			serializedObject.ApplyModifiedProperties();
		}
		GUI.DrawTexture (new Rect (GUILayoutUtility.GetRect (bgRect.width + offset, 2).x, GUILayoutUtility.GetRect (bgRect.width + offset, 2).y, bgRect.width + offset, 2), tab);
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();

		//=======================================================================================================
		//-------------------------------------------------------------------------------------------------------
		// Refresh the Inspector
		serObj.ApplyModifiedProperties ();
		serializedObject.ApplyModifiedProperties();
		EditorUtility.SetDirty(target);
	}
}