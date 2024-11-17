using Time = UnityEngine.Time;
using Audio = UnityEngine.AudioListener;

namespace MultiPlatformAds
{
    /// <summary>
    /// Pause/Resume game state
    /// </summary>
    public static class GamePauser
    {
        /// <summary>
        /// Paused game
        /// </summary>
        public static void Pause()
        {
            Time.timeScale = 0;
            Audio.pause = true;
        }

        /// <summary>
        /// Resumed game
        /// </summary>
        public static void Resume()
        {
            Time.timeScale = 1;
            Audio.pause = false;
        }
    }
}