using System.Collections.Generic;
using System.Windows;

namespace AmeisenBotGUI
{
    /// <summary>
    /// Interaktionslogik für ChangeRealmlistDialog.xaml
    /// </summary>
    public partial class ChangeRealmlistDialog : Window
    {
        public ChangeRealmlistDialog(List<string> realmlists, int currentSelection)
        {
            InitializeComponent();
            Realmlists = realmlists;
            ApplyChanges = false;
            SelectedRealmlist = currentSelection;
        }

        public bool ApplyChanges { get; set; }
        public List<string> Realmlists { get; set; }
        public int SelectedRealmlist { get; private set; }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (textboxRealmlist.Text.Length > 0)
            {
                listboxRealmlists.Items.Add($"{textboxRealmlist.Text}");
                Realmlists.Add($"set realmlist {textboxRealmlist.Text}");
            }
        }

        private void ButtonAddDeleteRealmlist_Click(object sender, RoutedEventArgs e)
        {
            Realmlists.RemoveAt(listboxRealmlists.SelectedIndex);
            listboxRealmlists.Items.RemoveAt(listboxRealmlists.SelectedIndex);
        }

        private void ButtonApply_Click(object sender, RoutedEventArgs e)
        {
            ApplyChanges = true;

            if (listboxRealmlists.SelectedIndex == -1)
            {
                listboxRealmlists.SelectedIndex = 0;
            }

            SelectedRealmlist = listboxRealmlists.SelectedIndex;
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ListboxRealmlists_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedRealmlist = listboxRealmlists.SelectedIndex;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (string s in Realmlists)
            {
                if (s.Split(' ').Length == 3)
                {
                    listboxRealmlists.Items.Add(s.Split(' ')[2]);
                }
            }
            listboxRealmlists.SelectedIndex = SelectedRealmlist;
        }

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}