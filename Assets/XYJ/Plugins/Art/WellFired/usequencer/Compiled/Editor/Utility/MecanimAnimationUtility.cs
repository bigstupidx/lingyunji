using UnityEngine;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WellFired
{
	public static class MecanimAnimationUtility 
	{
       
		public static List<string> GetAllStateNames(GameObject gameObject, int layer)
		{
            var animator = USTimelineAnimation.GetAnimator(gameObject);
			var availableStateNames = new List<string>();
            if (animator == null)
                return availableStateNames;
			if(animator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController)
			{
				var ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
				var sm = ac.layers[layer].stateMachine;
				for (int i = 0; i < sm.states.Length; i++) 
				{
					UnityEditor.Animations.AnimatorState state = sm.states[i].state;
					var stateName = state.name;
					availableStateNames.Add(stateName);
				}
			}
			else if(animator.runtimeAnimatorController is UnityEngine.AnimatorOverrideController)
			{
				var aoc = animator.runtimeAnimatorController as UnityEngine.AnimatorOverrideController;
				aoc.animationClips.ToList().ForEach(animationClip => availableStateNames.Add(animationClip.name));
			}
			else
			{
				Debug.LogError("AnimatorController Type not supported");
			}

			return availableStateNames;
		}

		public static List<string> GetAllLayerNames(GameObject gameObject)
		{
            var animator = USTimelineAnimation.GetAnimator(gameObject);
			var availableLayerNames = new List<string>();
            if (animator == null)
                return availableLayerNames;
			for(int layerIndex = 0; layerIndex < animator.layerCount; layerIndex++)
				availableLayerNames.Add(animator.GetLayerName(layerIndex));

			return availableLayerNames;
		}

		public static int LayerNameToIndex(GameObject gameObject, string layerName)
		{
            var animator = USTimelineAnimation.GetAnimator(gameObject);

            if (animator == null)
                return 0;
			
			for(int layerIndex = 0; layerIndex < animator.layerCount; layerIndex++)
			{
				if(animator.GetLayerName(layerIndex) == layerName)
					return layerIndex;
			}

			return 0;
		}
		
		public static string LayerIndexToName(GameObject gameObject, int requestedLayerIndex)
		{
            var animator = USTimelineAnimation.GetAnimator(gameObject);
            if (animator == null)
                return "";
			return animator.GetLayerName(requestedLayerIndex);
		}

		public static float GetStateDuration(string stateName, GameObject gameObject)
		{
            var animator = USTimelineAnimation.GetAnimator(gameObject);
            if (animator == null)
                return 0;
			if(animator.runtimeAnimatorController is UnityEditor.Animations.AnimatorController)
			{
				var ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
				for(var layerIndex = 0; layerIndex < animator.layerCount; layerIndex++)
				{
					var sm = ac.layers[layerIndex].stateMachine;
					for(int i = 0; i < sm.states.Length; i++) 
					{
						var state = sm.states[i].state;
						if(state.name == stateName)
							return state.motion.averageDuration;
					}
				}
			}
			else if(animator.runtimeAnimatorController is UnityEngine.AnimatorOverrideController)
			{
				var aoc = animator.runtimeAnimatorController as UnityEngine.AnimatorOverrideController;
				for(int i = 0; i < aoc.animationClips.Count(); i++) 
				{
					var clipName = aoc.animationClips[i].name;
					if(clipName == stateName)
						return aoc.animationClips[i].averageDuration;
				}
			}
			else
			{
				Debug.LogError("AnimatorController Type not supported");
			}

			throw new System.Exception(string.Format("StateName {0} not found", stateName));
		}
		
		public static UnityEditor.Animations.AnimatorState GetState(string stateName, GameObject gameObject)
		{
            var animator = USTimelineAnimation.GetAnimator(gameObject);
            if (animator == null)
                return null;
			UnityEditor.Animations.AnimatorController ac = animator.runtimeAnimatorController as UnityEditor.Animations.AnimatorController;
			var sm = ac.layers[0].stateMachine;
			for (int i = 0; i < sm.states.Length; i++) 
			{
				var state = sm.states[i].state;
				if(state.name == stateName)
					return state;
			}
			
			throw new System.Exception(string.Format("StateName {0} not found", stateName));
		}
	}
}