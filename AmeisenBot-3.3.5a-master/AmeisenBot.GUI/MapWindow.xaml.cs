using AmeisenBot.Map;
using AmeisenBotManager;
using AmeisenBotUtilities;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace AmeisenBotGUI
{
    /// <summary>
    /// Interaction logic for GroupWindow.xaml
    /// </summary>
    public partial class GroupWindow : Window
    {
        public GroupWindow(BotManager botManager)
        {
            InitializeComponent();
            BotManager = botManager;
            AmeisenMap = new AmeisenMap(220, 216);
            Topmost = BotManager.Settings.topMost;
        }
        
        private DispatcherTimer uiUpdateTimer;
        private BotManager BotManager { get; set; }
        private AmeisenMap AmeisenMap { get; set; }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
            => Close();

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void StartUIUpdateTimer()
        {
            uiUpdateTimer = new DispatcherTimer();
            uiUpdateTimer.Tick += new EventHandler(UIUpdateTimer_Tick);
            uiUpdateTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            uiUpdateTimer.Start();
        }

        private void UIUpdateTimer_Tick(object sender, EventArgs e)
        {
            if(mapImage.ActualHeight > 0 && mapImage.ActualWidth > 0)
            {
                AmeisenMap.SizeX = (int)mapImage.ActualHeight;
                AmeisenMap.SizeY = (int)mapImage.ActualWidth;
            }

            AmeisenMap.ActiveUnits = BotManager.ActiveWoWObjects;
            mapImage.Source = AmeisenMap.GenerateBitmap(BotManager.Me);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
            => uiUpdateTimer.Stop();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => StartUIUpdateTimer();

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try { DragMove(); }
            catch { }
        }
    }
}