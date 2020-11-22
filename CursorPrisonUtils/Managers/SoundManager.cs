using CSCore.CoreAudioAPI;
using CursorPrisonUtils.Config;
using CursorPrisonUtils.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursorPrisonUtils.Managers
{
    public class SoundManager : IChangeManager
    {
        public void HandleForegroundWindowChange(string processName, IntPtr hwnd)
        {
            var dict = PlaceholderConfig.Value.ProcessConfigs.ToDictionary(c => c.ProcessName, c => c.BackgroundMute);

            using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
            {
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                    foreach (var session in sessionEnumerator)
                    {
                        using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                        using (var sessionControl = session.QueryInterface<AudioSessionControl2>())
                        {
                            if (dict.ContainsKey(sessionControl.Process.ProcessName) && dict[sessionControl.Process.ProcessName])
                            {
                                simpleVolume.IsMuted = sessionControl.Process.ProcessName != processName;
                            }
                        }
                    }
                }
            }
        }

        private AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
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
