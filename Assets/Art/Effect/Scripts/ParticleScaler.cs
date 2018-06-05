//This script will only work in editor mode. You cannot adjust the scale dynamically in-game!
using UnityEngine;
using System.Collections;

#if UNITY_EDITOR 
using UnityEditor;
#endif
#pragma warning disable

[ExecuteInEditMode]
public class ParticleScaler : MonoBehaviour 
{
	public float particleScale = 1.0f;
	public bool alsoScaleGameobject = true;
    public bool testRoleScale = false;  //测试角色放缩，会省略一些放缩属性

	float prevScale;

    public static void SetScale( GameObject go,float scale )
    {
        ParticleScaler par = go.GetComponent<ParticleScaler>();
        if (par == null)
            par = go.AddComponent<ParticleScaler>();
        par.SetScaleEffect(scale);
        //par.particleScale = scale;
    }

	void Awake()
	{
		prevScale = particleScale;
	}


    void Update()
    {
#if UNITY_EDITOR
        SetScaleEffect(particleScale);
#endif

	}

    //设置特效大小，现在可以动态修改
    public void SetScaleEffect( float scale )
    {
        //check if we need to update
        if (prevScale != scale && scale > 0)
        {
            if (alsoScaleGameobject)
                transform.localScale = new Vector3(scale, scale, scale);

            float scaleFactor = scale / prevScale;

            //scale legacy particle systems
            ScaleLegacySystems(scaleFactor);

            //scale shuriken particle systems
            ScaleShurikenSystems(scaleFactor);

            //scale trail renders
            ScaleTrailRenderers(scaleFactor);

            prevScale = scale;
            particleScale = scale;
        }
    }

	void ScaleShurikenSystems(float scaleFactor)
	{
		//get all shuriken systems we need to do scaling on
		ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();

		foreach (ParticleSystem system in systems)
		{
			system.startSpeed *= scaleFactor;
			system.startSize *= scaleFactor;
			system.gravityModifier *= scaleFactor;

#if UNITY_EDITOR
            if(!testRoleScale)
            {
                //some variables cannot be accessed through regular script, we will acces them through a serialized object
                SerializedObject so = new SerializedObject(system);

                //unity 4.0 and onwards will already do this one for us
#if UNITY_3_5 

			so.FindProperty("ShapeModule.radius").floatValue *= scaleFactor;
			so.FindProperty("ShapeModule.boxX").floatValue *= scaleFactor;
			so.FindProperty("ShapeModule.boxY").floatValue *= scaleFactor;
			so.FindProperty("ShapeModule.boxZ").floatValue *= scaleFactor;
#endif
                so.FindProperty("VelocityModule.x.scalar").floatValue *= scaleFactor;
                so.FindProperty("VelocityModule.y.scalar").floatValue *= scaleFactor;
                so.FindProperty("VelocityModule.z.scalar").floatValue *= scaleFactor;
                so.FindProperty("ClampVelocityModule.magnitude.scalar").floatValue *= scaleFactor;
                so.FindProperty("ClampVelocityModule.x.scalar").floatValue *= scaleFactor;
                so.FindProperty("ClampVelocityModule.y.scalar").floatValue *= scaleFactor;
                so.FindProperty("ClampVelocityModule.z.scalar").floatValue *= scaleFactor;
                so.FindProperty("ForceModule.x.scalar").floatValue *= scaleFactor;
                so.FindProperty("ForceModule.y.scalar").floatValue *= scaleFactor;
                so.FindProperty("ForceModule.z.scalar").floatValue *= scaleFactor;
                so.FindProperty("ColorBySpeedModule.range").vector2Value *= scaleFactor;
                so.FindProperty("SizeBySpeedModule.range").vector2Value *= scaleFactor;
                so.FindProperty("RotationBySpeedModule.range").vector2Value *= scaleFactor;

                so.ApplyModifiedProperties();

            }
#endif
		}
	}

	void ScaleLegacySystems(float scaleFactor)
	{
		//get all emitters we need to do scaling on
		ParticleEmitter[] emitters = GetComponentsInChildren<ParticleEmitter>();

		//get all animators we need to do scaling on
		ParticleAnimator[] animators = GetComponentsInChildren<ParticleAnimator>();

		//apply scaling to emitters
		foreach (ParticleEmitter emitter in emitters)
		{
			emitter.minSize *= scaleFactor;
			emitter.maxSize *= scaleFactor;
			emitter.worldVelocity *= scaleFactor;
			emitter.localVelocity *= scaleFactor;
			emitter.rndVelocity *= scaleFactor;

#if UNITY_EDITOR
            if(!testRoleScale)
            {
                //some variables cannot be accessed through regular script, we will acces them through a serialized object
                SerializedObject so = new SerializedObject(emitter);

                so.FindProperty("m_Ellipsoid").vector3Value *= scaleFactor;
                so.FindProperty("tangentVelocity").vector3Value *= scaleFactor;
                so.ApplyModifiedProperties();
            }
#endif
		}

		//apply scaling to animators
		foreach (ParticleAnimator animator in animators)
		{
			animator.force *= scaleFactor;
			animator.rndForce *= scaleFactor;
		}
	}

	void ScaleTrailRenderers(float scaleFactor)
	{
		//get all animators we need to do scaling on
		TrailRenderer[] trails = GetComponentsInChildren<TrailRenderer>();

		//apply scaling to animators
		foreach (TrailRenderer trail in trails)
		{
			trail.startWidth *= scaleFactor;
			trail.endWidth *= scaleFactor;
		}
    }
}
