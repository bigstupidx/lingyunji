//
// Edelweiss.CloudSystemEditor.BoxFace.cs: Box face.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

namespace Edelweiss.CloudSystemEditor {
	
	public struct BoxFace : System.IComparable <BoxFace> {
		
		public BoxFace (BoxFaceType a_BoxFaceType, Vector3[] a_FaceVertices, Vector3 a_Center, Vector3 a_Normal) {
			boxFaceType = a_BoxFaceType;
			faceVertices = a_FaceVertices;
			center = a_Center;
			normal = a_Normal;
			dotProduct = 0.0f;
		}
		
		public BoxFaceType boxFaceType;
		public Vector3[] faceVertices;
		public Vector3 center;
		public Vector3 normal;
		public float dotProduct;

		public int CompareTo (BoxFace a_Other) {
			return (- dotProduct.CompareTo (a_Other.dotProduct));
		}

		public static BoxFace TransformBoxFace (Matrix4x4 a_TransformMatrix, Matrix4x4 a_NormalTransformMatrix, BoxFace a_BoxFace) {
			Vector3[] l_FaceVertices = a_BoxFace.faceVertices;
			for (int i = 0; i < l_FaceVertices.Length; i = i + 1) {
				l_FaceVertices [i] = a_TransformMatrix.MultiplyPoint3x4 (l_FaceVertices [i]);
			}
			Vector3 l_Normal = a_NormalTransformMatrix.MultiplyVector (a_BoxFace.normal);
			Vector3 l_Center = a_TransformMatrix.MultiplyPoint3x4 (a_BoxFace.center);
			BoxFace l_Result = new BoxFace (a_BoxFace.boxFaceType, l_FaceVertices, l_Center, l_Normal);
			return (l_Result);
		}
	}
}