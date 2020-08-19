using AmeisenBotUtilities;
using System.Windows.Controls;

namespace AmeisenBotGUI
{
    /// <summary>
    /// Interaktionslogik für BotView.xaml
    /// </summary>
    public partial class GroupView : UserControl
    {
        public GroupView(Unit unit)
        {
            InitializeComponent();

            labelBotName.Content = unit.Name;
            progressbarBotHealth.Maximum = unit.MaxHealth;
            progressbarBotHealth.Value = unit.Health;

            if (unit.MaxMana > 0)
            {
                progressbarBotEnergy.Maximum = unit.MaxMana;
                progressbarBotEnergy.Value = unit.Mana;
            }
            else if (unit.Energy > 0)
            {
                progressbarBotEnergy.Maximum = unit.MaxEnergy;
                progressbarBotEnergy.Value = unit.Energy;
            }
            else if (unit.Rage > 0)
            {
                progressbarBotEnergy.Maximum = unit.MaxRage;
                progressbarBotEnergy.Value = unit.Rage;
            }
            else if (unit.RuneEnergy > 0)
            {
                progressbarBotEnergy.Maximum = unit.MaxRuneEnergy;
                progressbarBotEnergy.Value = unit.RuneEnergy;
            }
        }
    }
}