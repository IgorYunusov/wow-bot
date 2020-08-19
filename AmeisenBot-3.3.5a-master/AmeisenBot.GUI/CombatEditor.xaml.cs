using AmeisenBotManager;
using System.Windows;

namespace AmeisenBotGUI
{
    /// <summary>
    /// Interaktionslogik für CombatEditor.xaml
    /// </summary>
    public partial class CombatEditor : Window
    {
        public CombatEditor(BotManager botManager)
        {
            InitializeComponent();
            BotManager = botManager;
        }

        private BotManager BotManager { get; set; }

        private void ButtonExit_Click(object sender, RoutedEventArgs e) => Close();

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    }
}