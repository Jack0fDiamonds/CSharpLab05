using System;

namespace Lab05.Models
{
    class CustomThread
    {
        private int _id;
        private string _state;
        private string _startTime;

        public int ID { get => _id; }
        public string State { get => _state; }
        public string StartTime { get => _startTime; }

        public CustomThread(int id, string state, DateTime startTime)
        {
            _id = id;
            _state = state;
            _startTime = startTime.ToShortDateString() + " " + startTime.ToShortTimeString();
        }
    }
}
