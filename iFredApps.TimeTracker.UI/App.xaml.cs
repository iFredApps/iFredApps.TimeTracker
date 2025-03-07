using ControlzEx.Theming;
using iFredApps.TimeTracker.UI.Models;
using MahApps.Metro.Theming;
using System;
using System.Windows;

namespace iFredApps.TimeTracker.UI
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
      public App()
      {
         InitializeComponent();
      }

      protected override void OnStartup(StartupEventArgs e)
      {
         base.OnStartup(e);

         var theme = ThemeManager.Current.AddLibraryTheme(
            new LibraryTheme(
                new Uri("pack://application:,,,/Themes/Theme.Dark.BluePurple.xaml"),
                MahAppsLibraryThemeProvider.DefaultInstance
            )
         );


         ThemeManager.Current.ChangeTheme(this, theme);

         if (AppWebClient.Instance.GetLoggedUserData() != null)
         {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
         }
         else
         {
            wLogin loginWindow = new wLogin();
            loginWindow.Show();
         }
      }
   }
}
