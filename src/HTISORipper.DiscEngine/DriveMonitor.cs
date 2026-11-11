using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HTISORipper.Core;
using HTISORipper.Logging;

namespace HTISORipper.DiscEngine
{
    public sealed class DriveStateChangedEventArgs : EventArgs
    {
        public DriveStateChangedEventArgs(string driveLetter, DriveMediaState oldState, DriveMediaState newState)
        {
            DriveLetter = driveLetter;
            OldState = oldState;
            NewState = newState;
        }

        public string DriveLetter { get; private set; }
        public DriveMediaState OldState { get; private set; }
        public DriveMediaState NewState { get; private set; }
    }

    /// <summary>
    /// Detects disc insertion/removal. Uses a lightweight poll (IOCTL_STORAGE_CHECK_VERIFY2 on a query-only handle,
    /// which works without media and without administrator rights) and can be nudged immediately by the UI when
    /// Windows broadcasts WM_DEVICECHANGE. Drives that are being imaged are skipped.
    /// Events are raised on a thread-pool thread.
    /// </summary>
    public sealed class DriveMonitor : IDisposable
    {
        private readonly ILogger _log;
        private readonly object _sync = new object();
        private readonly Dictionary<string, DriveMediaState> _states = new Dictionary<string, DriveMediaState>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> _busy = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly Timer _timer;
        private readonly TimeSpan _interval;
        private int _polling;
        private bool _disposed;

        public event EventHandler<DriveStateChangedEventArgs> DriveStateChanged;
        public event EventHandler DriveListChanged;

        public DriveMonitor(ILogger log, TimeSpan interval)
        {
            _log = log ?? NullLogger.Instance;
            _interval = interval;
            _timer = new Timer(_ => Poll(), null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start(IEnumerable<string> driveLetters)
        {
            lock (_sync)
            {
                _states.Clear();
                foreach (string d in driveLetters) _states[d] = DriveMediaState.Unknown;
            }
            _timer.Change(TimeSpan.Zero, _interval);
        }

        public DriveMediaState GetState(string driveLetter)
        {
            lock (_sync)
            {
                DriveMediaState s;
                return _states.TryGetValue(driveLetter, out s) ? s : DriveMediaState.Unknown;
            }
        }

        public void SetBusy(string driveLetter, bool busy)
        {
            lock (_sync)
            {
                if (busy) _busy.Add(driveLetter); else _busy.Remove(driveLetter);
            }
        }

        /// <summary>Poll now (call from WM_DEVICECHANGE).</summary>
        public void RequestRefresh()
        {
            if (!_disposed) ThreadPool.QueueUserWorkItem(_ => Poll());
        }

        private void Poll()
        {
            if (_disposed || Interlocked.Exchange(ref _polling, 1) == 1) return;
            try
            {
                CheckDriveList();
                string[] letters;
                lock (_sync) letters = _states.Keys.Where(k => !_busy.Contains(k)).ToArray();

                foreach (string letter in letters)
                {
                    DriveMediaState now = DriveEnumerator.GetMediaState(letter);
                    DriveMediaState before;
                    lock (_sync)
                    {
                        if (!_states.TryGetValue(letter, out before) || _busy.Contains(letter)) continue;
                        _states[letter] = now;
                    }
                    if (now != before)
                    {
                        _log.Info("Drive " + letter + ":\\ state " + before + " -> " + now);
                        var handler = DriveStateChanged;
                        if (handler != null) handler(this, new DriveStateChangedEventArgs(letter, before, now));
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error("Drive polling failed", ex);
            }
            finally
            {
                Interlocked.Exchange(ref _polling, 0);
            }
        }

        private void CheckDriveList()
        {
            var current = new HashSet<string>(
                System.IO.DriveInfo.GetDrives().Where(d => d.DriveType == System.IO.DriveType.CDRom)
                    .Select(d => d.Name.Substring(0, 1).ToUpperInvariant()), StringComparer.OrdinalIgnoreCase);
            bool changed;
            lock (_sync)
            {
                changed = !current.SetEquals(_states.Keys);
                if (changed)
                {
                    foreach (string gone in _states.Keys.Where(k => !current.Contains(k)).ToList()) _states.Remove(gone);
                    foreach (string added in current.Where(k => !_states.ContainsKey(k))) _states[added] = DriveMediaState.Unknown;
                }
            }
            if (changed)
            {
                _log.Info("Optical drive list changed: " + string.Join(", ", current.ToArray()));
                var handler = DriveListChanged;
                if (handler != null) handler(this, EventArgs.Empty);
            }
        }

        public void Dispose()
        {
            _disposed = true;
            _timer.Dispose();
        }
    }

    public static class SpeedControl
    {
        /// <summary>Maps a preference to a CDROM_SET_SPEED value in KB/s (1x CD = 176 KB/s raw, 1x DVD = 1,385 KB/s). Null = do not change.</summary>
        public static ushort? KilobytesPerSecond(ReadSpeedPreference preference, MediaFamily family)
        {
            bool dvd = family == MediaFamily.DVD;
            switch (preference)
            {
                case ReadSpeedPreference.Quiet: return dvd ? (ushort)(1385 * 4) : (ushort)(176 * 8);       // DVD 4x / CD 8x
                case ReadSpeedPreference.Balanced: return dvd ? (ushort)(1385 * 8) : (ushort)(176 * 24);   // DVD 8x / CD 24x
                case ReadSpeedPreference.Maximum: return 0xFFFF;
                default: return null;
            }
        }
    }
}
