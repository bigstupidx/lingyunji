//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2011-2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System.Collections;

namespace Edelweiss.CloudSystem {
	
	/// <summary>
	/// All cloud data that is needed by the renderers for multithreading.
	/// </summary>
	internal class CloudRendererData {
		
		public Color m_Tint;
		public Color Tint {
			get {
				return (m_Tint);
			}
		}
		
		public float m_CoreParticleTransparency;
		public float CoreParticleTransparency {
			get {
				return (m_CoreParticleTransparency);
			}
		}
		
		public float m_NonCoreParticleTransparency;
		public float NonCoreParticleTransparency {
			get {
				return (m_NonCoreParticleTransparency);
			}
		}
		
		public float m_VerticalShadingInfluence;
		public float VerticalShadingInfluence {
			get {
				return (m_VerticalShadingInfluence);
			}
		}
	
		public float m_ShadingGroupInfluence;
		public float ShadingGroupInfluence {
			get {
				return (m_ShadingGroupInfluence);
			}
		}
		
		public CS_VerticalColor[] verticalColors;
		public float[] m_VerticalColorsInverseHeightFactors;
		public float m_VerticalColorBottom;
		public float m_VerticalColorInverseHeight;
		
		public float HeightToVerticalFactor (float a_Height) {
			float l_Result = (a_Height - m_VerticalColorBottom) * (m_VerticalColorInverseHeight);
			return (l_Result);
		}
		
		public Color VerticalColorAt (float a_VerticalFactor) {		
			int i;
			for (i = 0; i < verticalColors.Length - 2; i = i + 1) {
				if
					(verticalColors [i].verticalFactor <= a_VerticalFactor &&
				     a_VerticalFactor <= verticalColors [i + 1].verticalFactor)
				{	
					break;
				}
			}
			float l_Factor = (a_VerticalFactor - verticalColors [i].verticalFactor) * m_VerticalColorsInverseHeightFactors [i];
			Color l_Result = Color.Lerp (verticalColors [i].color, verticalColors [i + 1].color, l_Factor);
			return (l_Result);
		}
		
		
		public CS_ShadingGroup[] shadingGroups;
		public CS_ShadingColor[] shadingColors;
		public float [] m_ShadingColorsInverseShadingFactors;
		public Color ShadingColorAt (float a_ShadingFactor) {
			int i;
			for (i = 0; i < shadingColors.Length - 2; i = i + 1) {
				if
					(shadingColors [i].shadingFactor <= a_ShadingFactor &&
					 a_ShadingFactor <= shadingColors [i + 1].shadingFactor)
				{
					break;
				}
			}
			float l_Factor = (a_ShadingFactor - shadingColors [i].shadingFactor) * m_ShadingColorsInverseShadingFactors [i];
			Color l_Result = Color.Lerp (shadingColors [i].color, shadingColors [i + 1].color, l_Factor);
			return (l_Result);
		}
	}
}
