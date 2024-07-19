﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace TimeTracker.UI.Models
{
   public class TimeManager : INotifyPropertyChanged
   {
      internal string databaseFileDir = "database.json";
      internal bool inicializeDummyData = false;

      public event PropertyChangedEventHandler PropertyChanged;
      private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
      {
         if (PropertyChanged != null)
         {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
         }
      }

      private TimeManagerTaskSession _current_session;
      public TimeManagerTaskSession current_session
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
      public ObservableCollection<TimeManagerGroup> task_groups { get; set; }
      public TimeManager()
      {
         current_session = new TimeManagerTaskSession();
         tasks = new ObservableCollection<TimeManagerTask>();
         task_groups = new ObservableCollection<TimeManagerGroup>();
      }

      public void LoadTasks()
      {
         tasks.Clear();

         if(inicializeDummyData)
            InicilizeDummyData();

         if (File.Exists(databaseFileDir))
         {
            string tasksJSON = File.ReadAllText(databaseFileDir);
            List<TimeManagerTask> dbTasks = JsonConvert.DeserializeObject<List<TimeManagerTask>>(tasksJSON);
            if (dbTasks != null && dbTasks.Count > 0)
            {
               dbTasks.ForEach(x => tasks.Add(x));
            }
         }

         RefreshTasks();
      }

      public void RefreshTasks()
      {
         task_groups.Clear();

         if (tasks != null)
         {
            Dictionary<string, List<TimeManagerTask>> dicTasksByGroupName = new Dictionary<string, List<TimeManagerTask>>();

            //Default Group
            dicTasksByGroupName.Add("Today", new List<TimeManagerTask>());

            foreach (var task in tasks)
            {
               TimeManagerTaskSession lastSession = task.sessions?.OrderByDescending(x => x.end_date)?.FirstOrDefault();
               if (lastSession != null)
               {
                  string groupKey = "";
                  if (lastSession.end_date.HasValue)
                  {
                     if (lastSession.end_date.Value.Date == DateTime.Now.Date)
                        groupKey = "Today";
                     else
                        groupKey = lastSession.end_date.Value.ToString("dd/MM/yyyy");
                  }
                  else
                  {
                     groupKey = "Unrecorded data";
                  }

                  if (dicTasksByGroupName.ContainsKey(groupKey))
                  {
                     List<TimeManagerTask> taskList = null;
                     if (dicTasksByGroupName.TryGetValue(groupKey, out taskList))
                     {
                        taskList.Add(task);
                     }
                  }
                  else
                  {
                     dicTasksByGroupName.Add(groupKey, new List<TimeManagerTask> { task });
                  }
               }
            }

            foreach (var dicRow in dicTasksByGroupName.OrderByDescending(x => x.Key))
            {
               ObservableCollection<TimeManagerTask> taskList = new ObservableCollection<TimeManagerTask>();
               dicRow.Value.ForEach(x => taskList.Add(x));

               TimeSpan totalTime = TimeSpan.Zero;
               if (taskList != null)
               {
                  //TODO: Calc total time
                  //totalTime = taskList.SelectMany(x => x.sessions.Where(l => l.end_date.HasValue)).Sum(x => x.end_date - x.start_date);
               }
               task_groups.Add(new TimeManagerGroup { description = dicRow.Key, tasks = taskList, total_time = TimeSpan.Zero });
            }
         }
      }

      public void SaveTasks()
      {
         string tasksJSON = JsonConvert.SerializeObject(tasks);
         File.WriteAllText(databaseFileDir, tasksJSON);
      }

      internal void InicilizeDummyData()
      {
         tasks.Add(new TimeManagerTask
         {
            id_task = 1,
            description = "Enterprise Website X",
            sessions = new ObservableCollection<TimeManagerTaskSession>
            {
               new TimeManagerTaskSession { id_task = 1, id_session = 1, start_date = new DateTime(2024, 7, 15, 9, 15, 0), end_date = new DateTime(2024, 7, 15, 11, 15, 0) },
               new TimeManagerTaskSession { id_task = 1, id_session = 2, start_date = new DateTime(2024, 7, 16, 9, 12, 0), end_date = new DateTime(2024, 7, 16, 11, 5, 0) },
               new TimeManagerTaskSession { id_task = 1, id_session = 3, start_date = new DateTime(2024, 7, 16, 11, 25, 0), end_date = new DateTime(2024, 7, 16, 13, 32, 0) },
            }
         });

         tasks.Add(new TimeManagerTask
         {
            id_task = 2,
            description = "CRM - enterprise x",
            sessions = new ObservableCollection<TimeManagerTaskSession>
            {
               new TimeManagerTaskSession { id_task = 2, id_session = 1, start_date = new DateTime(2024, 7, 17, 9, 47, 0), end_date = new DateTime(2024, 7, 17, 11, 2, 0) },
            }
         });

         tasks.Add(new TimeManagerTask
         {
            id_task = 3,
            description = "CRM - enterprise y",
            sessions = new ObservableCollection<TimeManagerTaskSession>
            {
               new TimeManagerTaskSession { id_task = 2, id_session = 1, start_date = new DateTime(2024, 7, 17, 12, 0, 0), end_date = new DateTime(2024, 7, 17, 13, 0, 0) },
            }
         });

         tasks.Add(new TimeManagerTask
         {
            id_task = 4,
            description = "ERP",
            sessions = new ObservableCollection<TimeManagerTaskSession>
            {
               new TimeManagerTaskSession { id_task = 3, id_session = 1, start_date = new DateTime(2024, 7, 13, 9, 47, 0), end_date = new DateTime(2024, 7, 13, 11, 2, 0) },
            }
         });
      }
   }

   public class TimeManagerGroup
   {
      public string description { get; set; }
      public TimeSpan total_time { get; set; }
      public ObservableCollection<TimeManagerTask> tasks { get; set; }
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
   }

   public class TimeManagerTask
   {
      public long id_task { get; set; }
      public string description { get; set; }
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
      public ObservableCollection<TimeManagerTaskSession> sessions { get; set; }

      public TimeManagerTask()
      {
         sessions = new ObservableCollection<TimeManagerTaskSession>();
      }
   }

   public class TimeManagerTaskSession
   {
      public long id_task { get; set; }
      public long id_session { get; set; }
      public DateTime start_date { get; set; }
      public DateTime? end_date { get; set; }
      public string description { get; set; }
      public string observation { get; set; }
   }
}
