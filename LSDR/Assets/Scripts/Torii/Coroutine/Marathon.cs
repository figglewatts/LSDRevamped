using System;
using System.Diagnostics;

namespace Torii.Coroutine
{
    public class Marathon : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly float _timeThresholdMs;

        public Marathon(float timeThresholdMs)
        {
            _timeThresholdMs = timeThresholdMs;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
        }

        public bool Run(Action action, bool lastResult)
        {
            if (lastResult) _stopwatch.Start();

            action();

            if (_stopwatch.ElapsedMilliseconds > _timeThresholdMs)
            {
                _stopwatch.Reset();
                return true;
            }

            return false;
        }
    }
}
