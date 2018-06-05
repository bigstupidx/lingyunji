#if (UNITY_STANDALONE_WIN || UNITY_EDITOR) && USER_IFLY
using System;

namespace NAudio.SoundFont
{
	/// <summary>
	/// SoundFont sample modes
	/// </summary>
	public enum SampleMode
	{
        /// <summary>
        /// No loop
        /// </summary>
		NoLoop,
        /// <summary>
        /// Loop Continuously
        /// </summary>
		LoopContinuously,
        /// <summary>
        /// Reserved no loop
        /// </summary>
		ReservedNoLoop,
        /// <summary>
        /// Loop and continue
        /// </summary>
		LoopAndContinue
	}
}

#endif