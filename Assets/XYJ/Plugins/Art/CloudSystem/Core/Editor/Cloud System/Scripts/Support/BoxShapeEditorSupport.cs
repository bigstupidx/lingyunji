//
// Edelweiss.CloudSystemEditor.BoxShapeEditorSupport.cs: Produces arrays of box shape coordinates and modifies them.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Edelweiss.CloudSystem;

namespace Edelweiss.CloudSystemEditor {

	public class BoxShapeEditorSupport {
		
		public static void MoveBoxFace (CS_Shape a_BoxShape, int a_Index, Vector3 a_NewPosition) {
			
			List <BoxFace> l_BoxFaces = BoxShapeEditorSupport.BoxFaces (a_BoxShape);
			BoxFace l_BoxFace = l_BoxFaces [a_Index];
			Vector3 l_OldPosition = l_BoxFace.center;
			BoxFaceType l_OppositeBoxFaceType = OppositeFaceType (l_BoxFace.boxFaceType);
			Vector3 l_OppositePosition = Vector3.zero;
			foreach (BoxFace l_OtherBoxFace in l_BoxFaces) {
				if (l_OtherBoxFace.boxFaceType == l_OppositeBoxFaceType) {
					l_OppositePosition = l_OtherBoxFace.center;
					break;
				}
			}
			
			Vector3 l_DeltaMove = a_NewPosition - l_OldPosition;
			if (l_BoxFace.boxFaceType == BoxFaceType.Left || l_BoxFace.boxFaceType == BoxFaceType.Right) {
				l_DeltaMove.y = 0.0f;
				l_DeltaMove.z = 0.0f;
			} else if (l_BoxFace.boxFaceType == BoxFaceType.Top || l_BoxFace.boxFaceType == BoxFaceType.Bottom) {
				l_DeltaMove.x = 0.0f;
				l_DeltaMove.z = 0.0f;
			} else {
				l_DeltaMove.x = 0.0f;
				l_DeltaMove.y = 0.0f;
			}
			a_BoxShape.position = a_BoxShape.position + a_BoxShape.LocalTransformMatrix ().MultiplyVector (0.5f * l_DeltaMove);
			
			Vector3 l_NewScale = a_NewPosition - l_OppositePosition;
			l_NewScale.x = Mathf.Abs (l_NewScale.x);
			l_NewScale.y = Mathf.Abs (l_NewScale.y);
			l_NewScale.z = Mathf.Abs (l_NewScale.z);
			if (l_BoxFace.boxFaceType == BoxFaceType.Left || l_BoxFace.boxFaceType == BoxFaceType.Right) {
				l_NewScale.y = a_BoxShape.scale.y;
				l_NewScale.z = a_BoxShape.scale.z;
			} else if (l_BoxFace.boxFaceType == BoxFaceType.Top || l_BoxFace.boxFaceType == BoxFaceType.Bottom) {
				l_NewScale.x = a_BoxShape.scale.x;
				l_NewScale.z = a_BoxShape.scale.z;
			} else {
				l_NewScale.x = a_BoxShape.scale.x;
				l_NewScale.y = a_BoxShape.scale.y;
			}
			a_BoxShape.scale = l_NewScale;
		}
		
		
			// Vertices for Editor GUI visualizations
		
		public static List <BoxFace> BoxFaces (CS_Shape a_BoxShape) {
			List <BoxFace> l_Result = new List<BoxFace> ();
			Vector3 l_Scale = a_BoxShape.scale;
			BoxFace l_BoxFace;
			Vector3[] l_Face;
			Vector3 l_Center;
			Vector3 l_Normal;
			
			float x = 0.5f * l_Scale.x;
			float y = 0.5f * l_Scale.y;
			float z = 0.5f * l_Scale.z;
			
				// Front
			l_Face = new Vector3 [4];
			l_Face [0] = new Vector3 (+x, +y, +z);
			l_Face [1] = new Vector3 (+x, -y, +z);
			l_Face [2] = new Vector3 (-x, -y, +z);
			l_Face [3] = new Vector3 (-x, +y, +z);
			l_Normal = new Vector3 (0.0f, 0.0f, z);
			l_Center = l_Normal;
			l_BoxFace = new BoxFace (BoxFaceType.Front, l_Face, l_Center, l_Normal);
			l_Result.Add (l_BoxFace);
			
				// Back
			l_Face = new Vector3 [4];
			l_Face [0] = new Vector3 (+x, +y, -z);
			l_Face [1] = new Vector3 (+x, -y, -z);
			l_Face [2] = new Vector3 (-x, -y, -z);
			l_Face [3] = new Vector3 (-x, +y, -z);
			l_Normal = new Vector3 (0.0f, 0.0f, -z);
			l_Center = l_Normal;
			l_BoxFace = new BoxFace (BoxFaceType.Back, l_Face, l_Center, l_Normal);
			l_Result.Add (l_BoxFace);
			
				// Left
			l_Face = new Vector3 [4];
			l_Face [0] = new Vector3 (-x, +y, +z);
			l_Face [1] = new Vector3 (-x, +y, -z);
			l_Face [2] = new Vector3 (-x, -y, -z);
			l_Face [3] = new Vector3 (-x, -y, +z);
			l_Normal = new Vector3 (-x, 0.0f, 0.0f);
			l_Center = l_Normal;
			l_BoxFace = new BoxFace (BoxFaceType.Left, l_Face, l_Center, l_Normal);
			l_Result.Add (l_BoxFace);
			
				// Right
			l_Face = new Vector3 [4];
			l_Face [0] = new Vector3 (+x, +y, +z);
			l_Face [1] = new Vector3 (+x, +y, -z);
			l_Face [2] = new Vector3 (+x, -y, -z);
			l_Face [3] = new Vector3 (+x, -y, +z);
			l_Normal = new Vector3 (x, 0.0f, 0.0f);
			l_Center = l_Normal;
			l_BoxFace = new BoxFace (BoxFaceType.Right, l_Face, l_Center, l_Normal);
			l_Result.Add (l_BoxFace);
			
				// Top
			l_Face = new Vector3 [4];
			l_Face [0] = new Vector3 (+x, +y, +z);
			l_Face [1] = new Vector3 (+x, +y, -z);
			l_Face [2] = new Vector3 (-x, +y, -z);
			l_Face [3] = new Vector3 (-x, +y, +z);
			l_Normal = new Vector3 (0.0f, y, 0.0f);
			l_Center = l_Normal;
			l_BoxFace = new BoxFace (BoxFaceType.Top, l_Face, l_Center, l_Normal);
			l_Result.Add (l_BoxFace);
			
				// Bottom
			l_Face = new Vector3 [4];
			l_Face [0] = new Vector3 (+x, -y, +z);
			l_Face [1] = new Vector3 (+x, -y, -z);
			l_Face [2] = new Vector3 (-x, -y, -z);
			l_Face [3] = new Vector3 (-x, -y, +z);
			l_Normal = new Vector3 (0.0f, -y, 0.0f);
			l_Center = l_Normal;
			l_BoxFace = new BoxFace (BoxFaceType.Bottom, l_Face, l_Center, l_Normal);
			l_Result.Add (l_BoxFace);
			
			return (l_Result);
		}
		
		public static BoxFaceType OppositeFaceType (BoxFaceType a_BoxFaceType) {
			BoxFaceType l_Result;
			if (a_BoxFaceType == BoxFaceType.Front) {
				l_Result = BoxFaceType.Back;
			} else if (a_BoxFaceType == BoxFaceType.Back) {
				l_Result = BoxFaceType.Front;
			} else if (a_BoxFaceType == BoxFaceType.Left) {
				l_Result = BoxFaceType.Right;
			} else if (a_BoxFaceType == BoxFaceType.Right) {
				l_Result = BoxFaceType.Left;
			} else if (a_BoxFaceType == BoxFaceType.Top) {
				l_Result = BoxFaceType.Bottom;
			} else {
				l_Result = BoxFaceType.Top;
			}
			return (l_Result);
		}
	}
}
