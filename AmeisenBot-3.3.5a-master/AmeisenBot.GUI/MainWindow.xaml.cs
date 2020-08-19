using AmeisenBotManager;
using System.Collections.Generic;
using System.Windows;

namespace AmeisenBotGUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<BotManager> BotManagers { get; set; }
        private List<BotView> BotViews { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            BotView DefaultBotView = new BotView();
            DefaultBotView.botName.Content = "Click to add bot...";
            DefaultBotView.botImage.MouseLeftButtonUp += DefaultBotImage_MouseLeftButtonUp;

            BotManagers = new List<BotManager>();
            BotViews = new List<BotView>
            {
                DefaultBotView
            };

            botPanel.Children.Add(DefaultBotView);
        }

        private void DefaultBotImage_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            AddNewBot();
        }

        private void AddNewBot()
        {
            BotView BotView = new BotView();
            BotView.botName.Content = "Schisch";
            BotViews.Add(BotView);
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e) => Close();
        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void BotPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        { try { DragMove(); } catch { } }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        { try { DragMove(); } catch { } }
    }
}
