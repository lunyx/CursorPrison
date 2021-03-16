using CSCore.CoreAudioAPI;
using CursorPrisonUtils.Config;
using CursorPrisonUtils.Contracts;
using Serilog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace CursorPrisonUtils.Managers
{
    public class SoundManager : IChangeManager
    {
        private readonly object locker = new object();
        private readonly ManualResetEvent blocker = new ManualResetEvent(true);
        private volatile int registeredEvents = 0;

        public void HandleForegroundWindowChange(string processName, IntPtr hwnd)
        {
            // register event so we know if we should skip processing later when new event comes in
            lock (locker)
            {
                registeredEvents++;
                Log.Debug($"{nameof(SoundManager)}.{nameof(HandleForegroundWindowChange)}:[{processName}]:{registeredEvents}");
            }

            blocker.WaitOne();

            try
            {
                blocker.Reset();
                HandleForegroundWindowChangeImpl(processName, hwnd, 0);
            }
            finally
            {
                lock (locker)
                {
                    registeredEvents--;
                    blocker.Set();
                }
            }
        }

        private void HandleForegroundWindowChangeImpl(string processName, IntPtr hwd, int attemptNum)
        {
            

            var backgroundMuteProcesses = ConfigManager.Instance.Config.ProcessConfigs.Where(c => c.BackgroundMute).Select(c => c.ProcessName).ToHashSet();
            var updatedFocusedApplication = false;

            using (var sessionManager = GetDefaultAudioSessionManager2(DataFlow.Render))
            {
                using (var sessionEnumerator = sessionManager.GetSessionEnumerator())
                {
                    foreach (var session in sessionEnumerator)
                    {
                        lock (locker) // if another event has come in, just skip this and process next one
                        {
                            if (registeredEvents > 1)
                            {
                                return;
                            }
                        }

                        using (var simpleVolume = session.QueryInterface<SimpleAudioVolume>())
                        using (var sessionControl = session.QueryInterface<AudioSessionControl2>())
                        {
                            string sessionProcessName = null;
                            try
                            {
                                sessionProcessName = sessionControl?.Process?.ProcessName;
                            }
                            catch (InvalidOperationException)
                            {   // can occur if process has exited, just move on
                                continue;
                            }
                            catch (Win32Exception)
                            {   // (0x80004005): Not enough memory resources are available to process this command
                                sessionProcessName = sessionControl?.Process?.ProcessName; //try again
                            }

                            if (sessionProcessName == null)
                                continue;

                            if (backgroundMuteProcesses.Contains(sessionProcessName))
                            {
                                simpleVolume.IsMuted = sessionProcessName != processName;
                                // sometimes, focused application doesn't immediately become available for processing on initial launch
                                // so we set this flag to indicate that we should retry after short delay when it is available
                                if (!updatedFocusedApplication && sessionProcessName == processName)
                                    updatedFocusedApplication = true;
                            }
                        }
                    }
                }
            }

            if (attemptNum < 10 && !updatedFocusedApplication && backgroundMuteProcesses.Contains(processName))
            {
                Log.Debug($"Unable to find target process {processName} for sound update, retrying ({attemptNum})...");
                Thread.Sleep(TimeSpan.FromSeconds(1));
                HandleForegroundWindowChangeImpl(processName, hwd, attemptNum + 1);
            }
        }

        private AudioSessionManager2 GetDefaultAudioSessionManager2(DataFlow dataFlow)
        {
            using (var enumerator = new MMDeviceEnumerator())
            {
                using (var device = enumerator.GetDefaultAudioEndpoint(dataFlow, Role.Multimedia))
                {
                    var sessionManager = AudioSessionManager2.FromMMDevice(device);
                    return sessionManager;
                }
            }
        }
    }
}
