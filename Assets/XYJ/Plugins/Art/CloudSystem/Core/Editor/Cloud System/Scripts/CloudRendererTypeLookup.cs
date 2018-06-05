//
// Edelweiss.CloudSystem.CloudRendererTypeLookup.cs:
//   Originally used as direct mapping from the enum to the actual type.
//   The inspector is not anymore using the CloudRendererTypeEnum directly,
//   but CloudRendererEnum and CloudRenderingMethodEnum. It could not be
//   remove as the enum is still used in the clouds directly.
//
// Author:
//   Andreas Suter (andy@edelweissinteractive.com)
//
// Copyright (C) 2012 Edelweiss Interactive (http://www.edelweissinteractive.com)
//

using UnityEngine;
using System;
using System.Collections;
using Edelweiss.CloudSystem;

namespace Edelweiss.CloudSystemEditor {
	
	public abstract class CloudRendererTypeLookup {
		
		public CloudRendererEnum CloudRendererFromType (CloudRendererTypeEnum a_CloudRendererType) {
			CloudRendererEnum l_Result = CloudRendererEnum.Custom;
			
			if (0 <= (int) a_CloudRendererType && (int) a_CloudRendererType <= 3) {
				l_Result = CloudRendererEnum.Custom;
			} else if (4 <= (int) a_CloudRendererType && (int) a_CloudRendererType <= 7) {
				l_Result = CloudRendererEnum.SimpleCustom;
			} else if (8 <= (int) a_CloudRendererType && (int) a_CloudRendererType <= 11) {
				l_Result = CloudRendererEnum.Legacy;
			} else if (12 <= (int) a_CloudRendererType && (int) a_CloudRendererType <= 15) {
				l_Result = CloudRendererEnum.Shuriken;
			} else {
				throw (new ArgumentException ("Invalid CloudRendererTypeEnum parameter."));
			}
			
			return (l_Result);
		}
		
		public CloudRenderingMethodEnum CloudRenderingMethodFromType (CloudRendererTypeEnum a_CloudRendererType) {
			CloudRenderingMethodEnum l_Result;
			int l_Method = (int) a_CloudRendererType % 4;
			l_Result = (CloudRenderingMethodEnum) l_Method;
			return (l_Result);
		}
		
		public CloudRendererTypeEnum CloudRendererTypeFromRendererAndMethod (CloudRendererEnum a_CloudRenderer, CloudRenderingMethodEnum a_CloudRenderingMethod) {
			CloudRendererTypeEnum l_Result;
			
			int l_Index = 0;
			if (a_CloudRenderer == CloudRendererEnum.Custom) {
				l_Index = 0 * 4;
			} else if (a_CloudRenderer == CloudRendererEnum.SimpleCustom) {
				l_Index = 1 * 4;
			} else if (a_CloudRenderer == CloudRendererEnum.Legacy) {
				l_Index = 2 * 4;
			} else if (a_CloudRenderer == CloudRendererEnum.Shuriken) {
				l_Index = 3 * 4;
			} else {
				throw (new ArgumentException ("Invalid CloudRenderer parameter."));
			}
			
			l_Index = l_Index + (int) a_CloudRenderingMethod;
			
			l_Result = (CloudRendererTypeEnum) l_Index;
			return (l_Result);
		}
		
		public abstract Type TypeForCloudSystemRendererEnum (CloudRendererTypeEnum a_CloudSystemRendererEnum);
	}
}