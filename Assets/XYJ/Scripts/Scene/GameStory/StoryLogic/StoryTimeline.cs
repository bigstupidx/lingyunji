using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public enum TimelineState
    {
        Stop = 0,
        Playing,
        Pause,
    }

    public class StoryTimeline
    {

        protected TimelineState m_state = TimelineState.Stop;

        #region Properties

        public StoryPlayer Player
        {
            get; protected set;
        }

        public bool IsStop { get { return m_state == TimelineState.Stop; } }

        public bool IsPlaying { get { return m_state == TimelineState.Playing; } }

        public bool IsPause { get { return m_state == TimelineState.Pause; } }

        #endregion

        #region Public Methods

        /// <summary>
        /// Optionally implement this method if you need custom behaviour when the Stops.
        /// </summary>
        public virtual void StopTimeline() {; }

        /// <summary>
        /// Optionally implement this method if you need custom behaviour when the Starts.
        /// </summary>
        public virtual void StartTimeline() {; }

        /// <summary>
        /// Optionally implement this method if you need custom behaviour when the Ends.
        /// </summary>
        //public virtual void EndTimeline() {; }

        /// <summary>
        /// Optionally implement this method if you need custom behaviour when the Pauses.
        /// </summary>
        public virtual void PauseTimeline() {; }

        /// <summary>
        /// Optionally implement this method if you need custom behaviour when the is Resumed.
        /// </summary>
        public virtual void ResumeTimeline() {; }

        /// <summary>
        /// Optionally implement this method if you need custom behaviour when the Skips.
        /// </summary>
        //public virtual void Skip() {; }

        /// <summary>
        /// Optionally implement this method if you need custom behaviour when the processes. This should happen during regular playback and when scrubbing
        /// </summary>
        public virtual void Process() {; }

        #endregion

    }
}

