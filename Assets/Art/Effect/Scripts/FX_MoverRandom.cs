using UnityEngine;
using System.Collections;

namespace MagicalFX
{
	public class FX_MoverRandom : MonoBehaviour
	{
	
		public float Speed = 1;
		public Vector3 Noise = Vector3.zero;

        Transform trans;

		void Start ()
		{
            trans = transform;
		}
	
		void FixedUpdate ()
		{
            Vector3 position = trans.position;
            position += this.transform.forward * Speed * Time.fixedDeltaTime;
            position += new Vector3(Random.Range(-Noise.x, Noise.x), Random.Range(-Noise.y, Noise.y), Random.Range(-Noise.z, Noise.z)) * Time.fixedDeltaTime;

            trans.position = position;
		}
	}
}