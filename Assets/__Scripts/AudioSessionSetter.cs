using System.Runtime.InteropServices;
using UnityEngine;

namespace _Scripts
{
    public class AudioSessionSetter
    {
        // -------------------------------------------------------------------------
        // Native Code Calls
        // -------------------------------------------------------------------------
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void _SetAudioSession();

    // -------------------------------------------------------------------------
    public static void SetAudioSession()
    {
        Debug.Log("Call set audio session from Unity");
        _SetAudioSession();
    }
#else
        // -------------------------------------------------------------------------
        public static void SetAudioSession()
        {
            //not implemented --> fallback
        }
#endif
    }
}