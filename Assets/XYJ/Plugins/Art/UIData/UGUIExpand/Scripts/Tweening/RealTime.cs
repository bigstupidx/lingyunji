using UnityEngine;

namespace UI
{
    /// <summary>
    /// Time class has no timeScale-independent time. This class fixes that.
    /// </summary>

    public class RealTime
    {
        /// <summary>
        /// Real time since startup.
        /// </summary>
        static public float time { get { return Time.unscaledTime; } }

        /// <summary>
        /// Real delta time.
        /// </summary>
        static public float deltaTime { get { return Time.unscaledDeltaTime; } }
    }
}