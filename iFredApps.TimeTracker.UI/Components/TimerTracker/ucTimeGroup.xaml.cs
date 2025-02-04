﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using iFredApps.TimeTracker.UI.Models;

namespace iFredApps.TimeTracker.UI.Components
{
   /// <summary>
   /// Interaction logic for ucTimeGroup.xaml
   /// </summary>
   public partial class ucTimeGroup : UserControl
   {
      public event EventHandler<TimeTaskContinueEventArgs> OnTaskContinue;
      public event EventHandler<TimeTaskRemoveEventArgs> OnTaskRemove;
      public event EventHandler<TimeTaskEditEventArgs> OnTaskChanged;
      public event EventHandler<TimeTaskSessionEditEventArgs> OnSessionChanged;
      public event EventHandler<TimeTaskSessionEditEventArgs> OnSessionRemoved;
      public event EventHandler<TimeTaskGroupArgs> OnSendReportRequest;

      public ucTimeGroup()
      {
         InitializeComponent();
      }

      #region Events

      private void lstView_PreviewMouseWheel(object sender, MouseWheelEventArgs e) //Disable scroll on list view
      {
         e.Handled = true;
         MouseWheelEventArgs e2 = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
         e2.RoutedEvent = UIElement.MouseWheelEvent;
         lstView.RaiseEvent(e2);
      }

      private void OnTaskContinueClick(object sender, TimeTaskContinueEventArgs e)
      {
         OnTaskContinue?.Invoke(this, e);
      }

      private void OnTaskRemoveClick(object sender, TimeTaskRemoveEventArgs e)
      {
         OnTaskRemove?.Invoke(this, e);
      }

      private void OnTaskChange(object sender, TimeTaskEditEventArgs e)
      {
         OnTaskChanged?.Invoke(this, e);
      }

      private void OnSessionChange(object sender, TimeTaskSessionEditEventArgs e)
      {
         OnSessionChanged?.Invoke(this, e);
      }

      private void OnSessionRemove(object sender, TimeTaskSessionEditEventArgs e)
      {
         OnSessionRemoved?.Invoke(this, e);
      }

      private void OnSendReport(object sender, RoutedEventArgs e)
      {
         if (DataContext is TimeManagerGroup group)
         {
            OnSendReportRequest?.Invoke(this, new TimeTaskGroupArgs { Group = group });
         }
      }

      #endregion
   }
}
