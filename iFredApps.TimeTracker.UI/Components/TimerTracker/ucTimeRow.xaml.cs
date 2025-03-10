﻿using iFredApps.Lib;
using iFredApps.Lib.Wpf.Execption;
using iFredApps.Lib.Wpf.Messages;
using iFredApps.TimeTracker.UI.Models;
using MahApps.Metro.Controls;
using System;
using System.Windows;
using System.Windows.Controls;

namespace iFredApps.TimeTracker.UI.Components
{
   /// <summary>
   /// Interaction logic for ucTimeRow.xaml
   /// </summary>
   public partial class ucTimeRow : UserControl
   {
      public event EventHandler<TimeTaskContinueEventArgs> OnTaskContinue;
      public event EventHandler<TimeTaskRemoveEventArgs> OnTaskRemove;
      public event EventHandler<TimeTaskEditEventArgs> OnTaskChanged;
      public event EventHandler<TimeTaskSessionEditEventArgs> OnSessionChanged;
      public event EventHandler<TimeTaskSessionEditEventArgs> OnSessionRemove;

      public ucTimeRow()
      {
         InitializeComponent();
      }

      #region Events

      private void OnTaskContinueClick(object sender, RoutedEventArgs e)
      {
         try
         {
            if (DataContext is TimeManagerTask taskData)
            {
               OnTaskContinue?.Invoke(this, new TimeTaskContinueEventArgs { TaskData = taskData });
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnTaskRemoveClick(object sender, RoutedEventArgs e)
      {
         try
         {
            if (DataContext is TimeManagerTask taskData)
            {
               OnTaskRemove?.Invoke(this, new TimeTaskRemoveEventArgs { TaskData = taskData });
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnDescriptionLostFocus(object sender, RoutedEventArgs e)
      {
         try
         {
            if (DataContext is TimeManagerTask taskData)
            {
               string oldDescription = taskData.description.ToString();
               string newDescription = ((TextBox)e.Source).Text.Trim();
               if (oldDescription == newDescription)
                  return;

               taskData.description = newDescription;
               OnTaskChanged?.Invoke(this, new TimeTaskEditEventArgs { oldDescription = oldDescription, TaskData = taskData });
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnDetailButtonClick(object sender, RoutedEventArgs e)
      {
         try
         {
            if (DataContext is TimeManagerTask taskData)
            {
               if (taskData.is_detail_session_open) //Close
               {
                  detailButtonIcon.Kind = MahApps.Metro.IconPacks.PackIconBootstrapIconsKind.ChevronDown;
                  sessionDetail.Visibility = Visibility.Collapsed;
               }
               else //Open
               {
                  detailButtonIcon.Kind = MahApps.Metro.IconPacks.PackIconBootstrapIconsKind.ChevronUp;
                  sessionDetail.Visibility = Visibility.Visible;
               }

               taskData.NotifyValue(nameof(taskData.is_detail_session_open), !taskData.is_detail_session_open);
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnEditSessionRow(object sender, RoutedEventArgs e)
      {
         try
         {
            if (e.Source is Button btn)
            {
               if (btn.DataContext is TimeManagerTaskSession session)
               {
                  session.NotifyValue(nameof(session.is_editing), true);
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnDeleteSessionRow(object sender, RoutedEventArgs e)
      {
         try
         {
            if (Message.Confirmation("Are you sure you want to remove this session?") == MessageBoxResult.Yes)
            {
               if (e.Source is Button btn)
               {
                  if (btn.DataContext is TimeManagerTaskSession session)
                  {
                     OnSessionRemove?.Invoke(this, new TimeTaskSessionEditEventArgs { Session = session });
                  }
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnSaveSessionRowCancel(object sender, RoutedEventArgs e)
      {
         try
         {
            if (e.Source is Button btn)
            {
               if (btn.DataContext is TimeManagerTaskSession session)
               {
                  session.NotifyValue(nameof(session.is_editing), false);
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnSaveSessionRowChanges(object sender, RoutedEventArgs e)
      {
         try
         {
            if (e.Source is Button btn)
            {
               if (btn.DataContext is TimeManagerTaskSession session)
               {
                  SaveSessionData(session);
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      private void OnKeyUpDateInput(object sender, System.Windows.Input.KeyEventArgs e)
      {
         try
         {
            if (e.Key != System.Windows.Input.Key.Enter)
               return;

            if (e.Source is DateTimePicker datePicker)
            {
               if (datePicker.DataContext is TimeManagerTaskSession session)
               {
                  SaveSessionData(session);
               }
            }
         }
         catch (Exception ex)
         {
            ex.ShowException();
         }
      }

      #endregion

      #region Private Methods

      private void SaveSessionData(TimeManagerTaskSession session)
      {
         if (session.start_date >= session.end_date.Value)
         {
            Message.Error("The start date time cannot be greater than the end date time!");
            return;
         }

         OnSessionChanged?.Invoke(this, new TimeTaskSessionEditEventArgs { Session = session });
         session.NotifyValue(nameof(session.is_editing), false);
         session.NotifyValue(nameof(session.total_time), session.end_date.Value - session.start_date);
      }

      #endregion
   }
}
