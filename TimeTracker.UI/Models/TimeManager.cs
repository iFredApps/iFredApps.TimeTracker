﻿using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeTracker.UI.Models
{
    public class TimeManager : INotifyPropertyChanged
    {
        private TimeManagerTaskCurrentSession _current_session;
        [JsonIgnore]
        public TimeManagerTaskCurrentSession current_session
        {
            get => _current_session;
            set
            {
                if (value == _current_session) return;
                _current_session = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<TimeManagerTask> tasks { get; set; }
        [JsonIgnore]
        public ObservableCollection<TimeManagerGroup> task_groups { get; set; }
        public TimeManager()
        {
            current_session = new TimeManagerTaskCurrentSession();
            tasks = new ObservableCollection<TimeManagerTask>();
            task_groups = new ObservableCollection<TimeManagerGroup>();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TimeManagerGroup
    {
        public string description { get; set; }
        public TimeSpan total_time { get; set; }
        public ObservableCollection<TimeManagerTask> tasks { get; set; }
        public DateTime date_group_reference { get; set; }
        public TimeSpan tasks_total_time
        {
            get
            {
                TimeSpan result = TimeSpan.Zero;

                if (tasks != null)
                {
                    foreach (var task in tasks)
                    {
                        result += task.session_total_time;
                    }
                }

                return result;
            }
        }
        public TimeSpan tasks_total_time_by_reference
        {
            get
            {
                TimeSpan result = TimeSpan.Zero;

                if (tasks != null)
                {
                    foreach (var task in tasks)
                    {
                        if (task.sessions != null && task.sessions.Count > 0)
                        {
                            foreach (var session in task.sessions)
                            {
                                if (session.end_date.HasValue && session.end_date.Value.Date == date_group_reference.Date)
                                    result += session.total_time;
                            }
                        }
                    }
                }

                return result;
            }
        }
    }

    public class TimeManagerTask : TimeManagerTaskBase, INotifyPropertyChanged
    {
        public new ObservableCollection<TimeManagerTaskSession> sessions { get; set; }

        [JsonIgnore]
        public TimeSpan session_total_time
        {
            get
            {
                TimeSpan result = TimeSpan.Zero;

                if (sessions != null)
                {
                    foreach (var session in sessions)
                    {
                        if (session.end_date.HasValue)
                        {
                            result += (session.end_date - session.start_date).Value;
                        }
                    }
                }

                return result;
            }
        }
        private bool _is_detail_session_open;
        [JsonIgnore]
        public bool is_detail_session_open
        {
            get => _is_detail_session_open;
            set
            {
                if (value == _is_detail_session_open) return;
                _is_detail_session_open = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TimeManagerTaskSession : TimeManagerTaskSessionBase, INotifyPropertyChanged
    {
        private TimeSpan _total_time;
        [JsonIgnore]
        public TimeSpan total_time
        {
            get
            {
                if (_total_time == TimeSpan.Zero && end_date.HasValue)
                    _total_time = end_date.Value - start_date;
                return _total_time;
            }
            set
            {
                if (value == _total_time) return;
                _total_time = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TimeManagerTaskCurrentSession : TimeManagerTaskSession, INotifyPropertyChanged
    {
        private bool _is_working;
        [JsonIgnore]
        public bool is_working
        {
            get => _is_working;
            set
            {
                if (value == _is_working) return;
                _is_working = value;
                NotifyPropertyChanged();
            }
        }
    }
}
