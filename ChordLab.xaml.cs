using Model;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Input;

namespace MusicSchoolWpf
{
    public partial class chordlab : Page
    {
        ApiService api = new ApiService();
        ChordList chordlist = new ChordList();    
        public chordlab()
        {
            InitializeComponent();
            LoadData();
            
        }

        private async void LoadData()
        {
            // שליפת נתונים (תוודא שיש לך פונקציה כזו ב-ApiService)
            chordlist = await api.SelectAllChords();

            // מילוי קומבובוקס סינון
            var diffs = chordlist.Select(c => c.Difficulty.ToString()).Distinct().ToList();
            diffs.Insert(0, "הכל");
            cmbDifficulty.ItemsSource = diffs;
            cmbDifficulty.SelectedIndex = 0;

            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (chordlist == null) return;
            if(!chordlist.Any()) return;

            var filtered = chordlist.AsEnumerable();

            // חיפוש בשם
            string search = txtSearch.Text.ToLower();
            if (!string.IsNullOrEmpty(search))
                filtered = filtered.Where(c => c.Name.ToLower().Contains(search));

            // סינון קושי
            if (cmbDifficulty.SelectedItem?.ToString() != "הכל")
                filtered = filtered.Where(c => c.Difficulty.ToString() == cmbDifficulty.SelectedItem.ToString());

            // מיון
            filtered = cmbSort.SelectedIndex switch
            {
                1 => filtered.OrderBy(c => c.Name),
                2 => filtered.OrderBy(c => c.Difficulty.ToString()),
                3 => filtered.OrderByDescending(c => c.Difficulty.ToString()),
                _ => filtered
            };

            UpdateView(filtered);
        }

        private void UpdateView(IEnumerable<chord> chords)
        {
            wpChords.Children.Clear();

            foreach (var c in chords)
            {
                ChordCardUC card = new ChordCardUC(c);

                card.Cursor = Cursors.Hand;

                card.MouseLeftButtonDown += (sender, e) =>
                {
                    ChordSoundPlayer.PlayChordById(c.Id);
                };

                wpChords.Children.Add(card);
            }
        }

        // אירועים
        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilters();
        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => ApplyFilters();
        private void btnOpenFilter_Click(object sender, RoutedEventArgs e) => filterPanel.Visibility = Visibility.Visible;
        private void btnCloseFilter_Click(object sender, RoutedEventArgs e) => filterPanel.Visibility = Visibility.Collapsed;
        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
            filterPanel.Visibility = Visibility.Collapsed;
        }
    }
}
