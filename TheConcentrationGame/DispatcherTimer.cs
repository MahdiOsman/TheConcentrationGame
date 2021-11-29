using System;

namespace TheConcentrationGame
{
    internal class DispatcherTimer
    {
        public DispatcherTimer()
        {
        }

        public Action Tick { get; internal set; }
        public TimeSpan Interval { get; internal set; }

        internal void Start()
        {
            throw new NotImplementedException();
        }
    }
}