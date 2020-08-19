using AmeisenBot.Character.Enums;
using AmeisenBot.Character.Objects;
using AmeisenBotManager;
using AmeisenBotUtilities.Objects;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace AmeisenBotGUI
{
    /// <summary>
    /// Interaktionslogik für GearWindow.xaml
    /// </summary>
    public partial class GearWindow : Window
    {
        public GearWindow(BotManager botManager)
        {
            InitializeComponent();
            BotManager = botManager;
        }

        private BotManager BotManager { get; set; }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private Brush GetBrushByItemQuality(ItemQuality quality)
        {
            switch (quality)
            {
                case ItemQuality.ARTIFACT: return new SolidColorBrush((Color)ColorConverter.ConvertFromString(WowColor.ItemArtifact));
                case ItemQuality.LEGENDARY: return new SolidColorBrush((Color)ColorConverter.ConvertFromString(WowColor.ItemLegendary));
                case ItemQuality.EPIC: return new SolidColorBrush((Color)ColorConverter.ConvertFromString(WowColor.ItemEpic));
                case ItemQuality.RARE: return new SolidColorBrush((Color)ColorConverter.ConvertFromString(WowColor.ItemRare));
                case ItemQuality.UNCOMMON: return new SolidColorBrush((Color)ColorConverter.ConvertFromString(WowColor.ItemUncommon));
                case ItemQuality.POOR: return new SolidColorBrush((Color)ColorConverter.ConvertFromString(WowColor.ItemPoor));
                default: return new SolidColorBrush((Color)ColorConverter.ConvertFromString(WowColor.ItemCommon));
            }
        }

        private Brush GetBrushForSpellbook(string spellbook)
        {
            switch (spellbook)
            {
                default: return new SolidColorBrush((Color)ColorConverter.ConvertFromString(WowColor.ItemCommon));
            }
        }

        private void Mainscreen_Loaded(object sender, RoutedEventArgs e)
        {
            while (!BotManager.Character.FullyLoaded)
            {
                Thread.Sleep(250);
            }

            foreach (Item item in BotManager.Character.Equipment.AsList())
            {
                listboxEquipped.Items.Add(
                    new DataItem(item.ToString(),
                    GetBrushByItemQuality(item.Quality)));
            }

            foreach (Item item in BotManager.Character.InventoryItems)
            {
                listboxInventory.Items.Add(
                    new DataItem(item.ToString(),
                    GetBrushByItemQuality(item.Quality)));
            }

            foreach (Spell spell in BotManager.Character.Spells)
            {
                listboxSpells.Items.Add(
                    new DataItem(spell.ToString(),
                    GetBrushForSpellbook(spell.SpellbookName)));
            }
        }

        private void Mainscreen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();
    }
}