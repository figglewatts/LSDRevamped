using System;
using System.Diagnostics;

namespace Torii.Coroutine
{
    public class Marathon : IDisposable
    {
        private readonly float _timeThresholdMs;
        private readonly Stopwatch _stopwatch;
        
        public Marathon(float timeThresholdMs)
        {
            _timeThresholdMs = timeThresholdMs;
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
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
        
        public void Dispose()
        {
            _stopwatch.Stop();
        }
    }
}