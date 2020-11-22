using CSCore.CoreAudioAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CursorPrison
{
    public class SoundManager
    {
        private SoundManager()
        {

        }

        private static readonly Lazy<SoundManager> _instance = new Lazy<SoundManager>(() => new SoundManager());
        public static SoundManager Instance => _instance.Value;

        public void Initialize()
        {
            const string app = "GenshinImpact";

            using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
            {
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                    foreach (var session in sessionEnumerator)
                    {
                        using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                        using (var sessionControl = session.QueryInterface<AudioSessionControl2>())
                        {
                            if (sessionControl.Process.ProcessName == app)
                            {
                                simpleVolume.IsMuted = true;
                            }
                        }
                    }
                }
            }

            //hook to window change event
            //dele = WinEventProc;
            //SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, dele, 0, 0, WINEVENT_OUTOFCONTEXT);
        }

        private static AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    Debug.WriteLine("DefaultDevice: " + device.FriendlyName);
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }
        }
    }
}
