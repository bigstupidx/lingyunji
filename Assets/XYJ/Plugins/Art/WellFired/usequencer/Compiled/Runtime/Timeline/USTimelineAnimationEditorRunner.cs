using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WellFired
{
	[Serializable]
	public class USTimelineAnimationEditorRunner : ScriptableObject
	{
		[SerializeField]
		private Animator animator;

		public Animator Animator
		{
			private get 
            {
                if (animator == null)
                    animator = animationTimeline.Animator;
                return animator; 
            }
			set { animator = value; }
		}
		
		[SerializeField]
		private USTimelineAnimation animationTimeline;
		public USTimelineAnimation AnimationTimeline
		{
			private get { return animationTimeline; }
			set { animationTimeline = value; }
		}
		
		[SerializeField]
		private List<AnimationClipData> allClips = new List<AnimationClipData>();

		private List<AnimationClipData> cachedRunningClips = new List<AnimationClipData>();
	
		private float previousTime = 0.0f;
	
		public void Stop()
		{
			previousTime = 0.0f;
		}

        public void Process(float sequenceTime, float playbackRate)
        {
            allClips.Clear();
            for (int index = 0; index < AnimationTimeline.AnimationTracks.Count; index++)
            {
                var track = AnimationTimeline.AnimationTracks[index];
                if (track == null || track.TrackClips == null)
                    continue;

				for(int trackClipIndex = 0; trackClipIndex < track.TrackClips.Count; trackClipIndex++)
				{
					var trackClip = track.TrackClips[trackClipIndex];
					allClips.Add(trackClip);
					trackClip.RunningLayer = track.Layer;
				}
			}

			var totalDeltaTime = sequenceTime - previousTime;
			var absDeltaTime = Mathf.Abs(totalDeltaTime);
			var timelinePlayingInReverse = totalDeltaTime < 0.0f;
			var runningTime = USSequencer.SequenceUpdateRate;
			var runningTotalTime = previousTime + runningTime;

			if(timelinePlayingInReverse)
			{
				AnimationTimeline.ResetAnimation();
				previousTime = 0.0f;
				AnimationTimeline.Process(sequenceTime, playbackRate);
			}
			else
			{
				while(absDeltaTime > 0.0f)
				{	
					cachedRunningClips.Clear();
					for(int allClipIndex = 0; allClipIndex < allClips.Count; allClipIndex++)
					{
						var clip = allClips[allClipIndex];
						if(!AnimationClipData.IsClipRunning(runningTotalTime, clip))
							continue;

						cachedRunningClips.Add(clip);
					}

					cachedRunningClips.Sort((x, y) => x.StartTime.CompareTo(y.StartTime));
				
					for(int runningClipIndex = 0; runningClipIndex < cachedRunningClips.Count; runningClipIndex++)
					{
						var clip = cachedRunningClips[runningClipIndex];
						PlayClip(clip, clip.RunningLayer, runningTotalTime);
					}

                    if (Animator.gameObject.activeSelf)
                        Animator.Update(runningTime);
					
					absDeltaTime -= USSequencer.SequenceUpdateRate;
					if(!Mathf.Approximately(absDeltaTime, Mathf.Epsilon) && absDeltaTime < USSequencer.SequenceUpdateRate)
						runningTime = absDeltaTime;
					
					runningTotalTime += runningTime;
				}
			}

			previousTime = sequenceTime;
		}
		
		public void PauseTimeline()
		{
			Animator.enabled = false;
		}
		
		private void PlayClip(AnimationClipData clipToPlay, int layer, float sequenceTime)
		{
			float normalizedTime = (sequenceTime - clipToPlay.StartTime) / clipToPlay.StateDuration;
			
			if(clipToPlay.CrossFade)
			{
				// The calculation and clamp are here, to resolve issues with big timesteps.
				// crossFadeTime will not always be equal to clipToPlay.transitionDuration, for insance
				// if the timeStep allows for a step of 0.5, we'll be 0.5s into the crossfade.
				var crossFadeTime = clipToPlay.TransitionDuration - (sequenceTime - clipToPlay.StartTime);
				crossFadeTime = Mathf.Clamp(crossFadeTime, 0.0f, Mathf.Infinity);

                animationTimeline.CrossFade(clipToPlay.StateName, crossFadeTime, layer, normalizedTime);
			}
			else
                animationTimeline.Play(clipToPlay.StateName, layer, normalizedTime);
			
			//var message = string.Format("state: {0}, nt: {1}, cf: {2}", clipToPlay.StateName, normalizedTime, clipToPlay.CrossFade);
			//Debug.LogError(message);
		}
	}
}