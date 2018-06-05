/*																	Sky based on paper of A. J. Preetham
 * 															https://www.cs.utah.edu/~shirley/papers/sunsky/sunsky.pdf
 * 
 * 																					and
 * 
 * 																			Ralf Stokholm Nielsen
 * 																http://etd.dtu.dk/thesis/58645/imm2554.pdf
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("azure[Sky]/Sky Controller")]
[ExecuteInEditMode]
public class AzureSky_Controller : MonoBehaviour {
	/////////////////////////////////////////////////////////////////////////////////////////////////////////
	//=========================================begins VARIABLES==============================================
	/////////////////////////////////////////////////////////////////////////////////////////////////////////
	//Used in the Editor Script only.
	public bool showTimeOfDay     = true;	public bool showObj_and_Mat   = false;
	public bool showScattering    = false;	public bool showSkySettings   = false;
	public bool showFogSettings   = false;	public bool showCloudSettings = false;
	public bool showAmbient       = false;	public bool showLighting      = false;
	public bool showTextures      = false;	public bool showOptions       = false;
	public bool showOutput        = false;
	//-------------------------------------------------------------------------------------------------------
	//========================================Time of Day Tab================================================
	//-------------------------------------------------------------------------------------------------------
	public int   DAY_of_WEEK          =  0;
	public int   NUMBER_of_DAYS       =  7;
	public float TIME_of_DAY          =  6.0f;
	public float TIME_of_DAY_by_CURVE =  6.0f;
	public int   UTC                  =  0;
	public float Longitude            =  0.0f;
	public float DAY_CYCLE            =  3.0f; // In Minutes.
	public float PassTime             =  0.0f;
//	public float azure_East           =  0.0f;
	private Vector3 sun_v3            =  Vector3.zero; // For the Sun Rotation
	public bool SetTime_By_Curve      =  false;
	public AnimationCurve DayNightLengthCurve  = AnimationCurve.Linear(0,0,24,24);
	//-------------------------------------------------------------------------------------------------------
	//=======================================Game Objects Tab================================================
	//-------------------------------------------------------------------------------------------------------
	public GameObject Sun_DirectionalLight;	public GameObject Moon_DirectionalLight;
	public Material   Sky_Material;     	public Material   Fog_Material;
	public Material   Moon_Material;		public ReflectionProbe AzureReflectionProbe;
	//-------------------------------------------------------------------------------------------------------
	//=================================For Scattering Tab and Equations======================================
	//-------------------------------------------------------------------------------------------------------
	public float RayCoeff      =  1.5f;   // Rayleigh coefficient.
	public float MieCoeff      =  1.0f;   // Mie coefficient.
	public float Turbidity     =  1.0f;
	public float g             =  0.75f;  // Directionality factor.
	public float SkyCoeff      =  2000.0f;
	public float SunIntensity  =  100.0f; // Intensity of the sunlight in the sky.
	public float MoonIntensity =  0.25f;  // Intensity of the moonlight in the sky.
	public float Kr            =  8.4f;
	public float Km            =  1.25f;
	public float Altitude      =  0.05f;
	public List<AnimationCurve> LambdaCurveR  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,650.0f,24,650.0f),
		AnimationCurve.Linear(0,650.0f,24,650.0f),
		AnimationCurve.Linear(0,650.0f,24,650.0f),
		AnimationCurve.Linear(0,650.0f,24,650.0f),
		AnimationCurve.Linear(0,650.0f,24,650.0f),
		AnimationCurve.Linear(0,650.0f,24,650.0f),
		AnimationCurve.Linear(0,650.0f,24,650.0f)
	};
	public List<AnimationCurve> LambdaCurveG  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,570.0f,24,570.0f),
		AnimationCurve.Linear(0,570.0f,24,570.0f),
		AnimationCurve.Linear(0,570.0f,24,570.0f),
		AnimationCurve.Linear(0,570.0f,24,570.0f),
		AnimationCurve.Linear(0,570.0f,24,570.0f),
		AnimationCurve.Linear(0,570.0f,24,570.0f),
		AnimationCurve.Linear(0,570.0f,24,570.0f)
	};
	public List<AnimationCurve> LambdaCurveB  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,475.0f,24,475.0f),
		AnimationCurve.Linear(0,475.0f,24,475.0f),
		AnimationCurve.Linear(0,475.0f,24,475.0f),
		AnimationCurve.Linear(0,475.0f,24,475.0f),
		AnimationCurve.Linear(0,475.0f,24,475.0f),
		AnimationCurve.Linear(0,475.0f,24,475.0f),
		AnimationCurve.Linear(0,475.0f,24,475.0f)
	};
	public List<AnimationCurve> RayCoeffCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<AnimationCurve> MieCoeffCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<AnimationCurve> TurbidityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<AnimationCurve> gCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.75f,24,0.75f),
		AnimationCurve.Linear(0,0.75f,24,0.75f),
		AnimationCurve.Linear(0,0.75f,24,0.75f),
		AnimationCurve.Linear(0,0.75f,24,0.75f),
		AnimationCurve.Linear(0,0.75f,24,0.75f),
		AnimationCurve.Linear(0,0.75f,24,0.75f),
		AnimationCurve.Linear(0,0.75f,24,0.75f)
	};
	public List<AnimationCurve> SkyCoeffCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,2000.0f,24,2000.0f),
		AnimationCurve.Linear(0,2000.0f,24,2000.0f),
		AnimationCurve.Linear(0,2000.0f,24,2000.0f),
		AnimationCurve.Linear(0,2000.0f,24,2000.0f),
		AnimationCurve.Linear(0,2000.0f,24,2000.0f),
		AnimationCurve.Linear(0,2000.0f,24,2000.0f),
		AnimationCurve.Linear(0,2000.0f,24,2000.0f)
	};
	public List<AnimationCurve> SunIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,100.0f,24,100.0f),
		AnimationCurve.Linear(0,100.0f,24,100.0f),
		AnimationCurve.Linear(0,100.0f,24,100.0f),
		AnimationCurve.Linear(0,100.0f,24,100.0f),
		AnimationCurve.Linear(0,100.0f,24,100.0f),
		AnimationCurve.Linear(0,100.0f,24,100.0f),
		AnimationCurve.Linear(0,100.0f,24,100.0f)
	};
	public List<AnimationCurve> MoonIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f)
	};
	public List<AnimationCurve> KrCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,8.4f,24,8.4f),
		AnimationCurve.Linear(0,8.4f,24,8.4f),
		AnimationCurve.Linear(0,8.4f,24,8.4f),
		AnimationCurve.Linear(0,8.4f,24,8.4f),
		AnimationCurve.Linear(0,8.4f,24,8.4f),
		AnimationCurve.Linear(0,8.4f,24,8.4f),
		AnimationCurve.Linear(0,8.4f,24,8.4f)
	};
	public List<AnimationCurve> KmCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.25f,24,1.25f),
		AnimationCurve.Linear(0,1.25f,24,1.25f),
		AnimationCurve.Linear(0,1.25f,24,1.25f),
		AnimationCurve.Linear(0,1.25f,24,1.25f),
		AnimationCurve.Linear(0,1.25f,24,1.25f),
		AnimationCurve.Linear(0,1.25f,24,1.25f),
		AnimationCurve.Linear(0,1.25f,24,1.25f)
	};
	public List<AnimationCurve> AltitudeCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f)
	};

	// Original value of lambda"(wavelength)" is Vector3(680e-9f, 550e-9f, 450e-9f).
	// Used integers for customization in the Inspector, will need...
	// ... convert to original scale before using in the equations, multiplying by 1.0e-9f.
	public Vector3 lambda      =  new Vector3(650.0f, 570.0f, 475.0f);
	private Vector3 K          =  new Vector3(686.0f, 678.0f, 666.0f);
	private const float n      =  1.0003f;   // Refractive index of air.
	private const float N      =  2.545E25f; // Molecular density.
	private const float pn     =  0.035f;    // Depolatization factor for standard air.
	private const float pi     =  Mathf.PI;  // 3.141592
	//-------------------------------------------------------------------------------------------------------
	//=========================================Sky Settings Tab==============================================
	//-------------------------------------------------------------------------------------------------------
	public float SkyLuminance       =  1.0f;
	public float SkyDarkness        =  1.0f;
	public float SunsetPower        =  3.5f;
	public float SunDiskSize        =  250.0f;
	public float SunDiskIntensity   =  3.0f;
	public float SunDiskPropagation = -1.5f;
	public float MoonSize           =  5.0f;
	public float StarsIntensity     =  5.0f;
	public float StarsExtinction    =  0.5f;
	public float MoonColorPower     =  2.15f;
	public float MoonExtinction     =  0.5f;
	public float MilkyWayIntensity  = 0.0f;
	public float MilkyWayPower      = 2.5f;
	public float Exposure           = 1.5f;

	public float NightSkyFarColorDistance = 0.5f;
	public float NightSkyFarColorPower    = 0.25f;

	public List<AnimationCurve> SkyLuminanceCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<AnimationCurve> SkyDarknessCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<AnimationCurve> SunsetPowerCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,3.5f,24,3.5f),
		AnimationCurve.Linear(0,3.5f,24,3.5f),
		AnimationCurve.Linear(0,3.5f,24,3.5f),
		AnimationCurve.Linear(0,3.5f,24,3.5f),
		AnimationCurve.Linear(0,3.5f,24,3.5f),
		AnimationCurve.Linear(0,3.5f,24,3.5f),
		AnimationCurve.Linear(0,3.5f,24,3.5f)
	};
	public List<AnimationCurve> SunDiskSizeCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,250.0f,24,250.0f),
		AnimationCurve.Linear(0,250.0f,24,250.0f),
		AnimationCurve.Linear(0,250.0f,24,250.0f),
		AnimationCurve.Linear(0,250.0f,24,250.0f),
		AnimationCurve.Linear(0,250.0f,24,250.0f),
		AnimationCurve.Linear(0,250.0f,24,250.0f),
		AnimationCurve.Linear(0,250.0f,24,250.0f)
	};
	public List<AnimationCurve> SunDiskIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f)
	};
	public List<AnimationCurve> SunDiskPropagationCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,-1.5f,24,-1.5f),
		AnimationCurve.Linear(0,-1.5f,24,-1.5f),
		AnimationCurve.Linear(0,-1.5f,24,-1.5f),
		AnimationCurve.Linear(0,-1.5f,24,-1.5f),
		AnimationCurve.Linear(0,-1.5f,24,-1.5f),
		AnimationCurve.Linear(0,-1.5f,24,-1.5f),
		AnimationCurve.Linear(0,-1.5f,24,-1.5f)
	};
	public List<AnimationCurve> MoonSizeCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f)
	};
	public List<AnimationCurve> MoonColorPowerCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,2.15f,24,2.15f),
		AnimationCurve.Linear(0,2.15f,24,2.15f),
		AnimationCurve.Linear(0,2.15f,24,2.15f),
		AnimationCurve.Linear(0,2.15f,24,2.15f),
		AnimationCurve.Linear(0,2.15f,24,2.15f),
		AnimationCurve.Linear(0,2.15f,24,2.15f),
		AnimationCurve.Linear(0,2.15f,24,2.15f)
	};
	public List<AnimationCurve> StarsIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f)
	};
	public List<AnimationCurve> StarsExtinctionCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f)
	};
	public List<AnimationCurve> MoonExtinctionCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f)
	};
	public List<AnimationCurve> MilkyWayIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f)
	};
	public List<AnimationCurve> MilkyWayPowerCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,2.5f,24,2.5f),
		AnimationCurve.Linear(0,2.5f,24,2.5f),
		AnimationCurve.Linear(0,2.5f,24,2.5f),
		AnimationCurve.Linear(0,2.5f,24,2.5f),
		AnimationCurve.Linear(0,2.5f,24,2.5f),
		AnimationCurve.Linear(0,2.5f,24,2.5f),
		AnimationCurve.Linear(0,2.5f,24,2.5f)
	};
	public List<AnimationCurve> ExposureCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.5f,24,1.5f),
		AnimationCurve.Linear(0,1.5f,24,1.5f),
		AnimationCurve.Linear(0,1.5f,24,1.5f),
		AnimationCurve.Linear(0,1.5f,24,1.5f),
		AnimationCurve.Linear(0,1.5f,24,1.5f),
		AnimationCurve.Linear(0,1.5f,24,1.5f),
		AnimationCurve.Linear(0,1.5f,24,1.5f)
	};
	public List<AnimationCurve> NightSkyFarColorDistanceCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f),
		AnimationCurve.Linear(0,0.5f,24,0.5f)
	};
	public List<AnimationCurve> NightSkyFarColorPowerCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f),
		AnimationCurve.Linear(0,0.25f,24,0.25f)
	};
//	public List<Gradient> SunGradientColor        = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> SunsetGradientColor     = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> MoonGradientColor       = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> MoonBrightGradientColor = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> NightGroundCloseGradientColor = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> NightGroundFarGradientColor   = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};

	public   int MoonEclipseShadow  = 0;
	public float Umbra = 0.95f;
	public float UmbraSize = 0.25f;
	public float Penumbra = 3.0f;
	public float PenumbraSize = 0.5f;
	public Color PenumbraColor = Color.red;
	public   float StarsScintillation = 5.5f;
	private  float     scintRot           =  0.0f;
	public   Vector3   MilkyWayPos;
	private  Matrix4x4 MilkyWayMatrix;
	private  Matrix4x4 NoiseRot;
	//-------------------------------------------------------------------------------------------------------
	//==============================================Fog Tab==================================================
	//-------------------------------------------------------------------------------------------------------
	public bool  LinearFog = true;
	public float ScatteringFogDistance = 3.0f;
	public float FogBlendPoint         = 3.0f;
	public float NormalFogDistance     = 10.0f;
	public float DenseFogIntensity     = 0.0f;
	public float DenseFogAltitude      = -0.8f;
	public bool  UseUnityFog           = false;
	public int UnityFogModeIndex       = 1;
	public float UnityFogDensity       = 1.0f;
	public float UnityFogStart         = 0.0f;
	public float UnityFogEnd           = 300.0f;
	public List<Gradient> NormalFogGradientColor   = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> GlobalFogGradientColor   = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> DenseFogGradientColor   = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> UnityFogGradientColor   = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<AnimationCurve> ScatteringFogDistanceCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f)
	};
	public List<AnimationCurve> FogBlendPointCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f)
	};
	public List<AnimationCurve> NormalFogDistanceCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,10.0f,24,10.0f),
		AnimationCurve.Linear(0,10.0f,24,10.0f),
		AnimationCurve.Linear(0,10.0f,24,10.0f),
		AnimationCurve.Linear(0,10.0f,24,10.0f),
		AnimationCurve.Linear(0,10.0f,24,10.0f),
		AnimationCurve.Linear(0,10.0f,24,10.0f),
		AnimationCurve.Linear(0,10.0f,24,10.0f)
	};
	public List<AnimationCurve> DenseFogIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.12f,24,0.12f),
		AnimationCurve.Linear(0,0.12f,24,0.12f),
		AnimationCurve.Linear(0,0.12f,24,0.12f),
		AnimationCurve.Linear(0,0.12f,24,0.12f),
		AnimationCurve.Linear(0,0.12f,24,0.12f),
		AnimationCurve.Linear(0,0.12f,24,0.12f),
		AnimationCurve.Linear(0,0.12f,24,0.12f)
	};
	public List<AnimationCurve> DenseFogAltitudeCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,-0.8f,24,-0.8f),
		AnimationCurve.Linear(0,-0.8f,24,-0.8f),
		AnimationCurve.Linear(0,-0.8f,24,-0.8f),
		AnimationCurve.Linear(0,-0.8f,24,-0.8f),
		AnimationCurve.Linear(0,-0.8f,24,-0.8f),
		AnimationCurve.Linear(0,-0.8f,24,-0.8f),
		AnimationCurve.Linear(0,-0.8f,24,-0.8f)
	};
	public List<AnimationCurve> UnityFogDensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<AnimationCurve> UnityFogStartCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f)
	};
	public List<AnimationCurve> UnityFogEndCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,300.0f,24,300.0f),
		AnimationCurve.Linear(0,300.0f,24,300.0f),
		AnimationCurve.Linear(0,300.0f,24,300.0f),
		AnimationCurve.Linear(0,300.0f,24,300.0f),
		AnimationCurve.Linear(0,300.0f,24,300.0f),
		AnimationCurve.Linear(0,300.0f,24,300.0f),
		AnimationCurve.Linear(0,300.0f,24,300.0f)
	};
	//-------------------------------------------------------------------------------------------------------
	//============================================Cloud Tab==================================================
	//-------------------------------------------------------------------------------------------------------
	public int cloudModeIndex = 0;
	private Shader noCloudsShader;
	private Shader preRenderedShader;
	private Shader proceduralCloudShader;
	private float preRenderedCloudLongitude =  0.0f;

	public List<Gradient> EdgeColorGradientColor   = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> DarkColorGradientColor   = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};

	public float CloudExtinction     = 3.0f;
	public float AlphaSaturation     = 2.0f;
	public float CloudDensity        = 1.0f;
	public float MoonBrightIntensity = 3.0f;
	public float MoonBrightRange     = 1.0f;
	public float PreRenderedCloudAltitude = 0.05f;
	public List<AnimationCurve> PreRenderedCloudAltitudeCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f),
		AnimationCurve.Linear(0,0.05f,24,0.05f)
	};
	public List<AnimationCurve> CloudExtinctionCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f)
	};
	public List<AnimationCurve> AlphaSaturationCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,2.0f,24,2.0f),
		AnimationCurve.Linear(0,2.0f,24,2.0f),
		AnimationCurve.Linear(0,2.0f,24,2.0f),
		AnimationCurve.Linear(0,2.0f,24,2.0f),
		AnimationCurve.Linear(0,2.0f,24,2.0f),
		AnimationCurve.Linear(0,2.0f,24,2.0f),
		AnimationCurve.Linear(0,2.0f,24,2.0f)
	};
	public List<AnimationCurve> CloudDensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<AnimationCurve> MoonBrightIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f),
		AnimationCurve.Linear(0,3.0f,24,3.0f)
	};
	public List<AnimationCurve> MoonBrightRangeCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	//Wispy Clouds
	public Texture2D WispyCloudTexture;
	public List<Gradient> WispyDarknessGradientColor   = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> WispyBrightGradientColor     = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> WispyColorGradientColor      = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public float WispyCovarage         = 0.0f;
	public float WispyCloudPosition    = 0.0f;
	public float WispyCloudSpeed       = 0.0f;
	public float WispyCloudDirection   = 0.0f;
	public List<AnimationCurve> WispyCovarageCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f),
		AnimationCurve.Linear(0,0.0f,24,0.0f)
	};
	public List<AnimationCurve> WispyCloudSpeedCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f),
		AnimationCurve.Linear(0,5.0f,24,5.0f)
	};

	//-------------------------------------------------------------------------------------------------------
	//===========================================Ambient Tab=================================================
	//-------------------------------------------------------------------------------------------------------
	public bool useReflectionProbe   = true;
	public int  ambientSourceIndex   = 0;
	public float AmbientIntensity    = 1.0f;
	public float ReflectionIntensity = 1.0f;
	public int   ReflectionBounces   = 1;
	public int ReflectionProbeRefreshMode = 1;
	public int ReflectionProbeTimeSlicing = 2;
	public float ReflectionProbeTimeToUpdate = 1.0f;
	private float timeSinceLastUpdate        = 0.0f;
	public float  ReflectionProbeIntensity    = 1.0f;
	public LayerMask ReflectionProbeCullingMask = 0;
	public Vector3 ReflectionProbeSize = new Vector3 (10.0f, 10.0f, 10.0f);
	public bool ForceProbeUpdateAtFirstFrame = true;
	public List<AnimationCurve> AmbientIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<AnimationCurve> ReflectionIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<AnimationCurve> ReflectionBouncesCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1,24,1),
		AnimationCurve.Linear(0,1,24,1),
		AnimationCurve.Linear(0,1,24,1),
		AnimationCurve.Linear(0,1,24,1),
		AnimationCurve.Linear(0,1,24,1),
		AnimationCurve.Linear(0,1,24,1),
		AnimationCurve.Linear(0,1,24,1)
	};
	public List<AnimationCurve> ReflectionProbeIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<Gradient> AmbientColorGradient        = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> SkyAmbientColorGradient     = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> EquatorAmbientColorGradient = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> GroundAmbientColorGradient  = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	//-------------------------------------------------------------------------------------------------------
	//===========================================Lighting Tab================================================
	//-------------------------------------------------------------------------------------------------------
	public float SunDirLightIntensity = 1.0f;
	public float MoonDirLightIntensity= 1.0f;
	public float SunFlareIntensity    = 1.0f;
	public List<AnimationCurve> SunDirLightIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<AnimationCurve> MoonDirLightIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<AnimationCurve> SunFlareIntensityCurve  = new List<AnimationCurve>()
	{
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f),
		AnimationCurve.Linear(0,1.0f,24,1.0f)
	};
	public List<Gradient> SunDirLightColorGradient  = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	public List<Gradient> MoonDirLightColorGradient = new List<Gradient>(){new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient(), new Gradient()};
	//-------------------------------------------------------------------------------------------------------
	//===========================================Textures Tab================================================
	//-------------------------------------------------------------------------------------------------------
	public  RenderTexture MoonTexture;
	public  Cubemap   StarField;
	public  Cubemap   StarNoise;
	public  Cubemap   MilkyWay;
	//-------------------------------------------------------------------------------------------------------
	//===========================================Options Tab=================================================
	//-------------------------------------------------------------------------------------------------------
	float getGradientTime;
	float getCurveTime;
	public bool skyUpdate        = true;
	public bool UseSunLensFlare  = true;
	public bool SkyHDR           = false;
	public bool showCurveValue   = false;
	public bool showGradientTime = true;
	public int  SpaceColorIndex  = 0;
	public Color CurveColorField = Color.yellow;
	public float ColorCorrection = 1.0f;
	public float WispyColorCorrection = 1.0f;
	//-------------------------------------------------------------------------------------------------------
	//To get lights components
	private Light     SunLightComponent;
	private Light     MoonLightComponent;
	private LensFlare SunFlareComponent;
	//-------------------------------------------------------------------------------------------------------
	//============================================Output Tab=================================================
	//-------------------------------------------------------------------------------------------------------
	public List<AnimationCurve> OutputCurveList    = new List<AnimationCurve>();
	public List<Gradient>       OutputGradientList = new List<Gradient>();//{new Gradient()};
	/////////////////////////////////////////////////////////////////////////////////////////////////////////
	//==========================================ends VARIABLES===============================================
	/////////////////////////////////////////////////////////////////////////////////////////////////////////



	//Use this for initialization.
	void Start () {
		MoonTexture.useMipMap = true;
		//Get lights components
		if (UseSunLensFlare){
			SunFlareComponent = Sun_DirectionalLight.GetComponent<LensFlare> ();
		}
		SunLightComponent  = Sun_DirectionalLight.GetComponent<Light> ();
		MoonLightComponent = Moon_DirectionalLight.GetComponent<Light> ();

		//Some updates
		SkyUpdate();
		SetSunPosition();
		SetTime (TIME_of_DAY, DAY_CYCLE);
		getGradientTime = TIME_of_DAY / 24;
		getCurveTime    = TIME_of_DAY;
		if (SetTime_By_Curve) {
			getCurveTime = TIME_of_DAY_by_CURVE;
			getGradientTime = TIME_of_DAY_by_CURVE / 24;
		}

		// First update the Reflection Probe
		if (useReflectionProbe && ReflectionProbeRefreshMode == 2) {
			if (ForceProbeUpdateAtFirstFrame) {
				AzureReflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame; // To update in the first frame.
				AzureReflectionProbe.RenderProbe (null);
				AzureReflectionProbe.refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.ViaScripting;// Back to previous refresh mode.
			} else {
				AzureReflectionProbe.RenderProbe (null);
			}
		}
		AzureReflectionProbe.cullingMask = ReflectionProbeCullingMask;

		// Only in gameplay
		if (Application.isPlaying) {
			ClearList ();
		}

		//Check Cloud Mode
		#if UNITY_EDITOR
		ChangeShader(cloudModeIndex);
		#endif
	}
	
	//Update is called once per frame.
	void Update () {
		//-------------------------------------------------------------------------------------------------------
		// Update the sky equations every frame ?
		if (skyUpdate)
		{
			SkyUpdate ();
		} else
			 {
//				getGradientTime = TIME_of_DAY / 24;
//				getCurveTime    = TIME_of_DAY;
//				if (SetTime_By_Curve)
//				{
//					getCurveTime = TIME_of_DAY_by_CURVE;
////					getGradientTime = TIME_of_DAY_by_CURVE / 24;
//				}
				WispyCloudPosition -= WispyCloudSpeed * (1e-3f * Time.deltaTime);
				Sky_Material.SetFloat ("_ProceduralCloudSpeed", WispyCloudPosition);
				StarsIntensity    = StarsIntensityCurve[DAY_of_WEEK].Evaluate (getCurveTime);
				Sky_Material.SetFloat ("_StarsIntensity",     StarsIntensity);
			 }
		// Needs constant update
		Sky_Material.SetVector ("_SunDir" ,   -Sun_DirectionalLight.transform.forward  );
		Sky_Material.SetVector ("_MoonDir",   -Moon_DirectionalLight.transform.forward );
		Sky_Material.SetMatrix ("_MoonMatrix", Moon_DirectionalLight.transform.worldToLocalMatrix);
		Sky_Material.SetMatrix ("_SunMatrix",  Sun_DirectionalLight.transform.worldToLocalMatrix);
		Fog_Material.SetVector ("_SunDir" ,   -Sun_DirectionalLight.transform.forward  );
		Fog_Material.SetVector ("_MoonDir",   -Moon_DirectionalLight.transform.forward );
		Fog_Material.SetMatrix ("_MoonMatrix", Moon_DirectionalLight.transform.worldToLocalMatrix);

		if (cloudModeIndex == 1) {
			preRenderedCloudLongitude = (1.0f / 360) * (Longitude + 180);
			Sky_Material.SetFloat ("_Longitude", preRenderedCloudLongitude);
			//To know if is morning or afternoon.
			if (TIME_of_DAY >= 12.0f) {
				Sky_Material.SetInt ("_Rise_or_Down", 0);
			} else {
				Sky_Material.SetInt ("_Rise_or_Down", 1);
			}
		}

		Moon_Material.SetVector ("_SunDir" ,   -Sun_DirectionalLight.transform.forward  );
		//For rotation of the noise texture in shader to apply star scintillation
		if (StarsScintillation > 0.0f)
		{
			scintRot += StarsScintillation * Time.deltaTime;
			Quaternion rot = Quaternion.Euler (scintRot, scintRot, scintRot);
			NoiseRot = Matrix4x4.TRS (Vector3.zero, rot, new Vector3 (1, 1, 1));
			Sky_Material.SetMatrix ("_NoiseMatrix", NoiseRot);
		}
		//Set Milky Way Position
		Quaternion milkyWayRot = Quaternion.Euler (MilkyWayPos);
		MilkyWayMatrix = Matrix4x4.TRS (Vector3.zero, milkyWayRot, new Vector3 (1, 1, 1));
		Sky_Material.SetMatrix ("_MilkyWayMatrix", MilkyWayMatrix);
		//-------------------------------------------------------------------------------------------------------
		/////////////////
		// Time of Day //
		sun_v3.x = SetSunPosition();
		sun_v3.y = Longitude;// + azure_East;
		if (TIME_of_DAY >= 24.0f) {
			if (DAY_of_WEEK < (NUMBER_of_DAYS-1)) {
				DAY_of_WEEK += 1;
			} else {
				DAY_of_WEEK = 0;
			}
			TIME_of_DAY = 0.0f;
		}
		if (TIME_of_DAY < 0.0f) {
			if (DAY_of_WEEK > 0) {
				DAY_of_WEEK -= 1;
			} else {
				DAY_of_WEEK = NUMBER_of_DAYS - 1;
			}
			TIME_of_DAY = 24.0f;
		}
		Sun_DirectionalLight.transform.localEulerAngles = sun_v3;

		// Only in gameplay
		if (Application.isPlaying)
		{
			// Pass the time of day //
			TIME_of_DAY += PassTime * Time.deltaTime;
			// Update the Reflection Probe
			if (useReflectionProbe) {
				UpdateReflections ();
			}
		}
		//-------------------------------------------------------------------------------------------------------
		// Only in Editor
		#if UNITY_EDITOR
		if (!Application.isPlaying)
		{
			// Get lights components
			if (UseSunLensFlare){
				SunFlareComponent = Sun_DirectionalLight.GetComponent<LensFlare> ();
			}
			SunLightComponent  = Sun_DirectionalLight.GetComponent<Light> ();
			MoonLightComponent = Moon_DirectionalLight.GetComponent<Light> ();

			// See "Reflection Probe Intensity" changes in the Editor
			ReflectionProbeIntensity = ReflectionProbeIntensityCurve [DAY_of_WEEK].Evaluate (TIME_of_DAY);
			AzureReflectionProbe.intensity = ReflectionProbeIntensity;
			//Update the sky in Editor
			SkyUpdate ();
		}
		AzureReflectionProbe.cullingMask = ReflectionProbeCullingMask;
		#endif
		//-------------------------------------------------------------------------------------------------------
		SunLightIntensity ();
		MoonLightIntensity ();
		SetAmbient ();
		if (UseUnityFog) {
			SetUnityFog ();
		}
		//-------------------------------------------------------------------------------------------------------
		TIME_of_DAY_by_CURVE = DayNightLengthCurve.Evaluate(TIME_of_DAY);

		getGradientTime = TIME_of_DAY / 24;
		getCurveTime    = TIME_of_DAY;
		if (SetTime_By_Curve) {
			getCurveTime = TIME_of_DAY_by_CURVE;
			getGradientTime = TIME_of_DAY_by_CURVE / 24;
		}
	}



	/////////////////////////////////////////////////////////////////////////////////////////////////////////
	//===============================================METHODS=================================================
	//-------------------------------------------------------------------------------------------------------
	// Get Beta Rayleight.
	private Vector3 BetaRay() {
		Vector3 converted_lambda = lambda * 1.0e-9f; // Converting the wavelength values given in Inpector for real scale used in formula.
		Vector3 Br;

		// The part (6.0f - 7.0f * pn) and (6.0f + 3.0f * pn), they are not included in this equation because there is no significant visual changes in the sky.
		////////////////
		// Without pn //
		//Br.x = ((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)) ) / (3.0f * N * Mathf.Pow(converted_lambda.x, 4.0f)))*SkyCoeff;
		//Br.y = ((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)) ) / (3.0f * N * Mathf.Pow(converted_lambda.y, 4.0f)))*SkyCoeff;
		//Br.z = ((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)) ) / (3.0f * N * Mathf.Pow(converted_lambda.z, 4.0f)))*SkyCoeff;

		///////////////////////
		// Original equation //
		Br.x = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(converted_lambda.x, 4.0f))*(6.0f-7.0f*pn) ))*SkyCoeff;
		Br.y = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(converted_lambda.y, 4.0f))*(6.0f-7.0f*pn) ))*SkyCoeff;
		Br.z = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f)))*(6.0f+3.0f*pn) ) / ((3.0f * N * Mathf.Pow(converted_lambda.z, 4.0f))*(6.0f-7.0f*pn) ))*SkyCoeff;

		return Br;
	}
	//-------------------------------------------------------------------------------------------------------
	// Get Beta Mie.
	private Vector3 BetaMie() {
		float c = (0.2f * Turbidity ) * 10.0f;
		Vector3 Bm;
		Bm.x = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / lambda.x, 2.0f) * K.x);
		Bm.y = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / lambda.y, 2.0f) * K.y);
		Bm.z = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / lambda.z, 2.0f) * K.z);

		Bm.x=Mathf.Pow(Bm.x,-1.0f);
		Bm.y=Mathf.Pow(Bm.y,-1.0f);
		Bm.z=Mathf.Pow(Bm.z,-1.0f);

		return Bm;
	}
	//-------------------------------------------------------------------------------------------------------
	// Get (3.0/(16.0*pi)) for rayleight phase function.
	private float pi316() {
		return 3.0f/(16.0f*pi);
	}
	//-------------------------------------------------------------------------------------------------------
	// Get (1.0/(4.0*pi)) for mie phase function.
	private float pi14() {
		return 1.0f/(4.0f*pi);
	}
	//-------------------------------------------------------------------------------------------------------
	// Get mie g constants
	private Vector3 GetMieG() {
		return new Vector3(1.0f-g*g,1.0f+g*g,2.0f*g);
	}
	//-------------------------------------------------------------------------------------------------------
	public float SetSunPosition() {
		float ret;
		if (SetTime_By_Curve) {
			ret = ((TIME_of_DAY_by_CURVE + UTC) * 360.0f / 24.0f) - 90.0f;
		} else {
			ret = ((TIME_of_DAY + UTC) * 360.0f / 24.0f) - 90.0f;
		}
		return ret;
	}
	//-------------------------------------------------------------------------------------------------------
	// Set "Time of Day" and "Day Duration"
	public void  SetTime(float hour, float dayDuration) {
		TIME_of_DAY = hour;
		DAY_CYCLE   = dayDuration;
		if (dayDuration > 0.0f) PassTime = (24.0f / 60.0f) / DAY_CYCLE; else PassTime = 0.0f;
	}
	//-------------------------------------------------------------------------------------------------------
	// Get "Time of Day""
	public float GetTime() {
		float  angle = Sun_DirectionalLight.transform.localEulerAngles.x;
		return angle / (360.0f / 24.0f);
	}
	//-------------------------------------------------------------------------------------------------------
	private void SunLightIntensity()
	{
		if (SunLightComponent != null)
		{
			SunDirLightIntensity        = SunDirLightIntensityCurve[DAY_of_WEEK].Evaluate(getCurveTime);
			SunLightComponent.intensity = SunDirLightIntensity;
			SunLightComponent.color     = SunDirLightColorGradient [DAY_of_WEEK].Evaluate (getGradientTime);
			if(SunLightComponent.intensity <= 0)
			{
				SunLightComponent.enabled = false;
			}else { SunLightComponent.enabled = true; }

			if (SunFlareComponent != null)
			{
				if (UseSunLensFlare)
				{
					SunFlareComponent.enabled    = true;
					SunFlareIntensity            = SunFlareIntensityCurve[DAY_of_WEEK].Evaluate(TIME_of_DAY);
					SunFlareComponent.brightness = SunFlareIntensity;
				}else { SunFlareComponent.enabled =false; }
			}
		}
	}
	//-------------------------------------------------------------------------------------------------------
	private void MoonLightIntensity()
	{
		if (MoonLightComponent != null)
		{
			MoonDirLightIntensity        = MoonDirLightIntensityCurve[DAY_of_WEEK].Evaluate(getCurveTime);
			MoonLightComponent.intensity = MoonDirLightIntensity;
			MoonLightComponent.color     = MoonDirLightColorGradient [DAY_of_WEEK].Evaluate (getGradientTime);
			if(MoonLightComponent.intensity <= 0)
			{
				MoonLightComponent.enabled = false;
			}else { MoonLightComponent.enabled = true; }
		}
	}
	//-------------------------------------------------------------------------------------------------------
	private void SetAmbient()
	{
		switch (ambientSourceIndex)
		{
		case 0:
			AmbientIntensity                   = AmbientIntensityCurve[DAY_of_WEEK].Evaluate (TIME_of_DAY);
			RenderSettings.ambientIntensity    = AmbientIntensity;

			break;

		case 1:
			AmbientIntensity                   = AmbientIntensityCurve[DAY_of_WEEK].Evaluate (TIME_of_DAY);
			RenderSettings.ambientIntensity    = AmbientIntensity;
			RenderSettings.ambientSkyColor     = SkyAmbientColorGradient[DAY_of_WEEK].Evaluate(TIME_of_DAY/24);
			RenderSettings.ambientEquatorColor = EquatorAmbientColorGradient[DAY_of_WEEK].Evaluate(TIME_of_DAY/24);
			RenderSettings.ambientGroundColor  = GroundAmbientColorGradient[DAY_of_WEEK].Evaluate(TIME_of_DAY/24);

			break;

		case 2:
			AmbientIntensity                   = AmbientIntensityCurve[DAY_of_WEEK].Evaluate (TIME_of_DAY);
			RenderSettings.ambientIntensity    = AmbientIntensity;
			RenderSettings.ambientSkyColor     = AmbientColorGradient[DAY_of_WEEK].Evaluate(TIME_of_DAY/24);
			break;
		}

		ReflectionIntensity = ReflectionIntensityCurve[DAY_of_WEEK].Evaluate (TIME_of_DAY);
		RenderSettings.reflectionIntensity = ReflectionIntensity;
		ReflectionBounces = (int)ReflectionBouncesCurve[DAY_of_WEEK].Evaluate (TIME_of_DAY);
		RenderSettings.reflectionBounces = ReflectionBounces;
	}
	//-------------------------------------------------------------------------------------------------------
	private void SetUnityFog()
	{
		RenderSettings.fogColor = UnityFogGradientColor[DAY_of_WEEK].Evaluate (getGradientTime);
		UnityFogDensity = UnityFogDensityCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		UnityFogStart   = UnityFogStartCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		UnityFogEnd     = UnityFogEndCurve[DAY_of_WEEK].Evaluate (getCurveTime);


		RenderSettings.fogDensity       = UnityFogDensity * 1e-4f;
		RenderSettings.fogStartDistance = UnityFogStart;
		RenderSettings.fogEndDistance   = UnityFogEnd;
	}
	//-------------------------------------------------------------------------------------------------------
	private void UpdateReflections()
	{
		timeSinceLastUpdate += Time.deltaTime;
		if (ReflectionProbeRefreshMode == 2) {
			if (timeSinceLastUpdate >= ReflectionProbeTimeToUpdate) {
				AzureReflectionProbe.RenderProbe (null);
				timeSinceLastUpdate = 0;
			}
		}
		ReflectionProbeIntensity = ReflectionProbeIntensityCurve [DAY_of_WEEK].Evaluate (TIME_of_DAY);
		AzureReflectionProbe.intensity = ReflectionProbeIntensity;
	}
	//-------------------------------------------------------------------------------------------------------
	public void ChangeShader(int shader)
	{
		AzureSkyCloudAnimation animComponent = GetComponent<AzureSkyCloudAnimation> ();
		switch (shader) {
		case 0:
			noCloudsShader = Shader.Find ("azure[Sky]/azure[Sky]_NoClouds");
			if (animComponent) {
				animComponent.enabled = false;
			}
			if (Sky_Material.shader != noCloudsShader)
				Sky_Material.shader = noCloudsShader;
			break;
		case 1:
			preRenderedShader = Shader.Find ("azure[Sky]/azure[Sky]_PreRenderedClouds");
			if (animComponent) {
				animComponent.enabled = true;
			}
			if(Sky_Material.shader != preRenderedShader)
				Sky_Material.shader = preRenderedShader;
			break;
		case 2:
			proceduralCloudShader = Shader.Find ("azure[Sky]/azure[Sky]_ProceduralClouds");
			if (animComponent) {
				animComponent.enabled = false;
			}
			if(Sky_Material.shader != proceduralCloudShader)
				Sky_Material.shader = proceduralCloudShader;
			break;
		}
	}
	//-------------------------------------------------------------------------------------------------------
	//-------------------------------------------------------------------------------------------------------
	/// <summary>
	/// Get Azure[Sky] curve output based on value of "time of day".
	/// </summary>
	/// <returns>float.</returns>
	/// <param name="index">Index.</param>
	public float AzureSkyGetCurveOutput(int index)
	{
		if (SetTime_By_Curve) {
			return OutputCurveList [index].Evaluate (TIME_of_DAY_by_CURVE);
		} else { return OutputCurveList [index].Evaluate (TIME_of_DAY); }
	}
	/// <summary>
	/// Get Azure[Sky] gradient output based on value of "time of day"
	/// </summary>
	/// <returns>Color.</returns>
	/// <param name="index">Index.</param>
	public Color AzureSkyGetGradientOutput(int index)
	{
		float getColorKey;
		if (SetTime_By_Curve) {
			getColorKey = TIME_of_DAY_by_CURVE / 24;
		} else { getColorKey = TIME_of_DAY / 24; }
		return OutputGradientList[index].Evaluate(getColorKey);
	}
	//-------------------------------------------------------------------------------------------------------
	//=======================================================================================================
	//-------------------------------------------------------------------------------------------------------
	// Update the sky properties
	void SkyUpdate()
	{
		//Get Curves Value
		RayCoeff      = RayCoeffCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		MieCoeff      = MieCoeffCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		Turbidity     = TurbidityCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		g             = gCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		lambda.x      = LambdaCurveR[DAY_of_WEEK].Evaluate (getCurveTime);
		lambda.y      = LambdaCurveG[DAY_of_WEEK].Evaluate (getCurveTime);
		lambda.z      = LambdaCurveB[DAY_of_WEEK].Evaluate (getCurveTime);
		SkyCoeff      = SkyCoeffCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		SunIntensity  = SunIntensityCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		MoonIntensity = MoonIntensityCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		Kr            = KrCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		Km            = KmCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		Altitude      = AltitudeCurve[DAY_of_WEEK].Evaluate (getCurveTime);

		SkyLuminance      = SkyLuminanceCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		SkyDarkness       = SkyDarknessCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		SunsetPower       = SunsetPowerCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		SunDiskSize       = SunDiskSizeCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		SunDiskIntensity  = SunDiskIntensityCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		SunDiskPropagation= SunDiskPropagationCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		MoonSize          = MoonSizeCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		StarsIntensity    = StarsIntensityCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		StarsExtinction   = StarsExtinctionCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		MoonExtinction    = MoonExtinctionCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		MilkyWayIntensity = MilkyWayIntensityCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		MilkyWayPower     = MilkyWayPowerCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		Exposure          = ExposureCurve[DAY_of_WEEK].Evaluate (getCurveTime);

		MoonColorPower = MoonColorPowerCurve [DAY_of_WEEK].Evaluate (getCurveTime);

		NightSkyFarColorDistance = NightSkyFarColorDistanceCurve [DAY_of_WEEK].Evaluate (getCurveTime);
		NightSkyFarColorPower    = NightSkyFarColorPowerCurve [DAY_of_WEEK].Evaluate (getCurveTime);

		ScatteringFogDistance = ScatteringFogDistanceCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		FogBlendPoint         = FogBlendPointCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		NormalFogDistance     = NormalFogDistanceCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		DenseFogIntensity     = DenseFogIntensityCurve[DAY_of_WEEK].Evaluate (getCurveTime);
		DenseFogAltitude      = DenseFogAltitudeCurve[DAY_of_WEEK].Evaluate (getCurveTime);

		//////////////////
		// SKY MATERIAL //
		Sky_Material.SetVector ("_Br",           BetaRay () * RayCoeff);
		Sky_Material.SetVector ("_Br2",          BetaRay () * 3.0f);
		Sky_Material.SetVector ("_Bm",           BetaMie () * MieCoeff);
		Sky_Material.SetVector ("_Brm",          BetaRay () * RayCoeff + BetaMie () * MieCoeff);
		Sky_Material.SetVector ("_mieG",         GetMieG ());
		Sky_Material.SetFloat ("_SunIntensity",  SunIntensity);
		Sky_Material.SetFloat ("_MoonIntensity", MoonIntensity);
		Sky_Material.SetFloat ("_Kr",            Kr);
		Sky_Material.SetFloat ("_Km",            Km);
		Sky_Material.SetFloat ("_Altitude",      Altitude);
		Sky_Material.SetFloat ("_pi316",         pi316 ());
		Sky_Material.SetFloat ("_pi14",          pi14 ());
		Sky_Material.SetFloat ("_pi",            pi);

		Sky_Material.SetFloat ("_Exposure",           Exposure);
		Sky_Material.SetFloat ("_SkyLuminance",       SkyLuminance);
		Sky_Material.SetFloat ("_SkyDarkness",        SkyDarkness);
		Sky_Material.SetFloat ("_SunsetPower",        SunsetPower);
		Sky_Material.SetFloat ("_SunDiskSize",        SunDiskSize);
		Sky_Material.SetFloat ("_SunDiskIntensity",   SunDiskIntensity);
		Sky_Material.SetFloat ("_SunDiskPropagation", SunDiskPropagation);
		Sky_Material.SetFloat ("_MoonSize",           MoonSize);
		Sky_Material.SetFloat ("_StarsIntensity",     StarsIntensity);
		Sky_Material.SetFloat ("_StarsExtinction",    StarsExtinction);
		Sky_Material.SetFloat ("_MoonExtinction",     MoonExtinction);
		Sky_Material.SetFloat ("_MilkyWayIntensity",  MilkyWayIntensity);
		Sky_Material.SetFloat ("_MilkyWayPower",      MilkyWayPower);
		Sky_Material.SetFloat ("_MoonEclipseShadow",  MoonEclipseShadow);

//		Sky_Material.SetColor ("_SunColor",           SunGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));
		Sky_Material.SetColor ("_SunsetColor",        SunsetGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));
		Sky_Material.SetColor ("_MoonBrightColor",    MoonBrightGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));
		Sky_Material.SetColor ("_GroundCloseColor",   NightGroundCloseGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));
		Sky_Material.SetColor ("_GroundFarColor",     NightGroundFarGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));

		Sky_Material.SetFloat ("_FarColorDistance",   NightSkyFarColorDistance);
		Sky_Material.SetFloat ("_FarColorIntensity",  NightSkyFarColorPower);

		Sky_Material.SetTexture ("_MoonSampler", MoonTexture);
		Sky_Material.SetInt   ("_MoonEclipseShadow", MoonEclipseShadow);
		Sky_Material.SetFloat ("_Umbra", Umbra);
		Sky_Material.SetFloat ("_UmbraSize", UmbraSize);
		Sky_Material.SetFloat ("_Penumbra", Penumbra);
		Sky_Material.SetFloat ("_PenumbraSize", PenumbraSize);
		Sky_Material.SetColor ("_PenumbraColor", PenumbraColor);


		Sky_Material.SetTexture ("_StarField",   StarField);
		Sky_Material.SetTexture ("_StarNoise",   StarNoise);
		Sky_Material.SetTexture ("_MilkyWay",    MilkyWay);

		Sky_Material.SetFloat ("_ColorCorrection",  ColorCorrection);


		//////////////////
		// FOG MATERIAL //
		Fog_Material.SetVector ("_Br",           BetaRay () * RayCoeff);
		Fog_Material.SetVector ("_Br2",          BetaRay () * 3.0f);
		Fog_Material.SetVector ("_Bm",           BetaMie () * MieCoeff);
		Fog_Material.SetVector ("_Brm",          BetaRay () * RayCoeff + BetaMie () * MieCoeff);
		Fog_Material.SetVector ("_mieG",         GetMieG ());
		Fog_Material.SetFloat ("_SunIntensity",  SunIntensity);
		Fog_Material.SetFloat ("_MoonIntensity", MoonIntensity);
		Fog_Material.SetFloat ("_Kr",            Kr);
		Fog_Material.SetFloat ("_Km",            Km);
		Fog_Material.SetFloat ("_Altitude",      Altitude);
		Fog_Material.SetFloat ("_pi316",         pi316 ());
		Fog_Material.SetFloat ("_pi14",          pi14 ());
		Fog_Material.SetFloat ("_pi",            pi);

		Fog_Material.SetFloat ("_Exposure",           Exposure);
		Fog_Material.SetFloat ("_SkyLuminance",       SkyLuminance);
		Fog_Material.SetFloat ("_SkyDarkness",        SkyDarkness);
		Fog_Material.SetFloat ("_SunsetPower",        SunsetPower);

		Fog_Material.SetFloat ("_ColorCorrection",  ColorCorrection);

		Fog_Material.SetColor ("_SunsetColor",        SunsetGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));
		Fog_Material.SetColor ("_MoonBrightColor",    MoonBrightGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));
		Fog_Material.SetColor ("_GroundCloseColor",   NightGroundCloseGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));
		Fog_Material.SetColor ("_GroundFarColor",     NightGroundFarGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));

		Fog_Material.SetFloat ("_FarColorDistance",   NightSkyFarColorDistance);
		Fog_Material.SetFloat ("_FarColorIntensity",  NightSkyFarColorPower);

		Fog_Material.SetFloat ("_ScatteringFogDistance",  ScatteringFogDistance);
		Fog_Material.SetFloat ("_BlendFogDistance",  FogBlendPoint);
		Fog_Material.SetFloat ("_NormalFogDistance",  NormalFogDistance);
		Fog_Material.SetFloat ("_DenseFogIntensity",  DenseFogIntensity);
		Fog_Material.SetFloat ("_DenseFogAltitude",  DenseFogAltitude);
		Fog_Material.SetColor ("_DenseFogColor",     DenseFogGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));

		Fog_Material.SetColor ("_NormalFogColor",     NormalFogGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));
		Fog_Material.SetColor ("_GlobalColor",        GlobalFogGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));
		if (LinearFog) {
			Fog_Material.SetFloat ("_LinearFog", 0.45f);
		} else {
			Fog_Material.SetFloat ("_LinearFog", 1.0f);
		}


		/////////
		//Cloud//
		switch (cloudModeIndex) {
		case 0:
			
			break;
		case 1://Pre Rendered Clouds
			Sky_Material.SetColor ("_EdgeColor", EdgeColorGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));
			Sky_Material.SetColor ("_DarkColor", DarkColorGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));

			CloudExtinction = CloudExtinctionCurve[DAY_of_WEEK].Evaluate (getCurveTime);
			AlphaSaturation = AlphaSaturationCurve[DAY_of_WEEK].Evaluate (getCurveTime);
			CloudDensity    = CloudDensityCurve[DAY_of_WEEK].Evaluate (getCurveTime);
			MoonBrightIntensity = MoonBrightIntensityCurve[DAY_of_WEEK].Evaluate (getCurveTime);
			MoonBrightRange = MoonBrightRangeCurve[DAY_of_WEEK].Evaluate (getCurveTime);
			PreRenderedCloudAltitude = PreRenderedCloudAltitudeCurve[DAY_of_WEEK].Evaluate (getCurveTime);

			Sky_Material.SetFloat ("_CloudExtinction",  CloudExtinction);
			Sky_Material.SetFloat ("_AlphaSaturation",  AlphaSaturation);
			Sky_Material.SetFloat ("_CloudDensity",     CloudDensity);
			Sky_Material.SetFloat ("_MoonBrightIntensity", MoonBrightIntensity);
			Sky_Material.SetFloat ("_MoonBrightRange", MoonBrightRange);
			Sky_Material.SetFloat ("_CloudAltitude", PreRenderedCloudAltitude);
			break;
		case 2://Procedural Wispy Clouds
			Sky_Material.SetColor ("_WispyDarkness", WispyDarknessGradientColor [DAY_of_WEEK].Evaluate (getGradientTime));
			Sky_Material.SetColor ("_WispyBright", WispyBrightGradientColor [DAY_of_WEEK].Evaluate (getGradientTime));
			Sky_Material.SetColor ("_WispyColor", WispyColorGradientColor [DAY_of_WEEK].Evaluate (getGradientTime));

			WispyCovarage = WispyCovarageCurve [DAY_of_WEEK].Evaluate (getCurveTime);
			WispyCloudSpeed = WispyCloudSpeedCurve [DAY_of_WEEK].Evaluate (getCurveTime);

			Sky_Material.SetFloat ("_WispyCovarage", WispyCovarage);
			Sky_Material.SetTexture ("_WispyCloudTexture", WispyCloudTexture);

			WispyCloudPosition -= WispyCloudSpeed * (1e-3f * Time.deltaTime);
			Sky_Material.SetFloat ("_ProceduralCloudSpeed", WispyCloudPosition);

			Sky_Material.SetFloat ("_WispyCloudDirection", WispyCloudDirection);
			Sky_Material.SetFloat ("_WispyColorCorrection",  WispyColorCorrection);

			break;
		}


		/////////////////
		//Moon Material//
		Moon_Material.SetColor ("_MoonColor",      MoonGradientColor[DAY_of_WEEK].Evaluate(getGradientTime));
		Moon_Material.SetFloat ("_LightIntensity", MoonColorPower);

		// General Settings //
		if (SkyHDR)
		{
			Sky_Material.DisableKeyword ("HDR_OFF");
			Sky_Material.EnableKeyword ("HDR_ON");
			Fog_Material.DisableKeyword ("HDR_OFF");
			Fog_Material.EnableKeyword ("HDR_ON");
		}
		else
		{
			Sky_Material.EnableKeyword ("HDR_OFF");
			Sky_Material.DisableKeyword ("HDR_ON");
			Fog_Material.EnableKeyword ("HDR_OFF");
			Fog_Material.DisableKeyword ("HDR_ON");
		}
	}
	void ClearList()
	{
		for (int i = NUMBER_of_DAYS; i < 7; i++) {
			//Scattering
			LambdaCurveR.RemoveAt (NUMBER_of_DAYS);
			LambdaCurveG.RemoveAt (NUMBER_of_DAYS);
			LambdaCurveB.RemoveAt (NUMBER_of_DAYS);
			RayCoeffCurve.RemoveAt (NUMBER_of_DAYS);
			MieCoeffCurve.RemoveAt (NUMBER_of_DAYS);
			TurbidityCurve.RemoveAt (NUMBER_of_DAYS);
			gCurve.RemoveAt (NUMBER_of_DAYS);
			SkyCoeffCurve.RemoveAt (NUMBER_of_DAYS);
			SunIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			MoonIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			KrCurve.RemoveAt (NUMBER_of_DAYS);
			KmCurve.RemoveAt (NUMBER_of_DAYS);
			AltitudeCurve.RemoveAt (NUMBER_of_DAYS);
			//Sky Settings
			SkyLuminanceCurve.RemoveAt (NUMBER_of_DAYS);
			SkyDarknessCurve.RemoveAt (NUMBER_of_DAYS);
			SunsetPowerCurve.RemoveAt (NUMBER_of_DAYS);
			SunDiskSizeCurve.RemoveAt (NUMBER_of_DAYS);
			SunDiskIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			SunDiskPropagationCurve.RemoveAt (NUMBER_of_DAYS);
			MoonSizeCurve.RemoveAt (NUMBER_of_DAYS);
			MoonColorPowerCurve.RemoveAt (NUMBER_of_DAYS);
			StarsIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			StarsExtinctionCurve.RemoveAt (NUMBER_of_DAYS);
			MilkyWayIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			MilkyWayPowerCurve.RemoveAt (NUMBER_of_DAYS);
			ExposureCurve.RemoveAt (NUMBER_of_DAYS);
			NightSkyFarColorDistanceCurve.RemoveAt (NUMBER_of_DAYS);
			NightSkyFarColorPowerCurve.RemoveAt (NUMBER_of_DAYS);
			SunsetGradientColor.RemoveAt (NUMBER_of_DAYS);
			MoonGradientColor.RemoveAt (NUMBER_of_DAYS);
			MoonBrightGradientColor.RemoveAt (NUMBER_of_DAYS);
			NightGroundCloseGradientColor.RemoveAt (NUMBER_of_DAYS);
			NightGroundFarGradientColor.RemoveAt (NUMBER_of_DAYS);
			//Fog
			NormalFogGradientColor.RemoveAt (NUMBER_of_DAYS);
			GlobalFogGradientColor.RemoveAt (NUMBER_of_DAYS);
			ScatteringFogDistanceCurve.RemoveAt (NUMBER_of_DAYS);
			FogBlendPointCurve.RemoveAt (NUMBER_of_DAYS);
			NormalFogDistanceCurve.RemoveAt (NUMBER_of_DAYS);
			//Pre-Rendered Clouds
			EdgeColorGradientColor.RemoveAt (NUMBER_of_DAYS);
			DarkColorGradientColor.RemoveAt (NUMBER_of_DAYS);
			PreRenderedCloudAltitudeCurve.RemoveAt (NUMBER_of_DAYS);
			CloudExtinctionCurve.RemoveAt (NUMBER_of_DAYS);
			AlphaSaturationCurve.RemoveAt (NUMBER_of_DAYS);
			CloudDensityCurve.RemoveAt (NUMBER_of_DAYS);
			MoonBrightIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			MoonBrightRangeCurve.RemoveAt (NUMBER_of_DAYS);
			//Procedural Clouds
			WispyCovarageCurve.RemoveAt (NUMBER_of_DAYS);
			WispyCloudSpeedCurve.RemoveAt (NUMBER_of_DAYS);
			WispyColorGradientColor.RemoveAt (NUMBER_of_DAYS);
			WispyBrightGradientColor.RemoveAt (NUMBER_of_DAYS);
			WispyDarknessGradientColor.RemoveAt (NUMBER_of_DAYS);
			//Ambient
			AmbientIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			ReflectionIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			ReflectionBouncesCurve.RemoveAt (NUMBER_of_DAYS);
			ReflectionProbeIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			AmbientColorGradient.RemoveAt (NUMBER_of_DAYS);
			SkyAmbientColorGradient.RemoveAt (NUMBER_of_DAYS);
			EquatorAmbientColorGradient.RemoveAt (NUMBER_of_DAYS);
			GroundAmbientColorGradient.RemoveAt (NUMBER_of_DAYS);
			//Lighting
			SunDirLightIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			MoonDirLightIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			SunFlareIntensityCurve.RemoveAt (NUMBER_of_DAYS);
			SunDirLightColorGradient.RemoveAt (NUMBER_of_DAYS);
			MoonDirLightColorGradient.RemoveAt (NUMBER_of_DAYS);
		}
	}
	void OnEnable()
	{
		#if UNITY_EDITOR
		if (RenderSettings.skybox != Sky_Material) {
			RenderSettings.skybox = Sky_Material;
		}
		switch (ambientSourceIndex){
		case 0:
			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
			break;
		case 1:
			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
			break;
		case 2:
			RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
			break;
		}
		#endif
	}
}