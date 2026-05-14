using Model;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Text;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicSchoolWpf
{
    /// <summary>
    /// Interaction logic for songpage.xaml
    /// </summary>
    public partial class songpage : Page
    {

        ApiService api=new ApiService();
        SongList songlist = new SongList();
        public songpage()
        {
            InitializeComponent();
            GetAllSongs();
            LoadFilterOptions();


        }



        public async void GetAllSongs()
        {
            SongCardUC song;
            songlist =await api.SelectAllSongs();

            foreach (song s in songlist)
            {
                song = new SongCardUC(s);
                sp.Children.Add(song);
            }
        }
        public songpage(song s)
        {
            InitializeComponent();
            SongUserControl sUC = new SongUserControl(s);
            sp.Children.Add(sUC);
        }
        private async void LoadFilterOptions()
        {
            GenreList gr = await api.SelectAllGenres();
            LanguageList ln = await api.SelectAllLanguages();
            DifficultyList di = await api.SelectAllDifficulties();

            List<string> styleOptions = gr.Select(g => g.Genrename).ToList();
            styleOptions.Insert(0, "הכל");
            cmbGenre.ItemsSource = styleOptions;
            cmbGenre.SelectedIndex = 0;
           

            List<string> langOptions = ln.Select(l => l.Languagename).ToList();
            langOptions.Insert(0, "הכל");
            cmbLanguage.ItemsSource = langOptions;
            cmbLanguage.SelectedIndex = 0;

            List<string> diffOptions = di.Select(d => d.Diff.ToString()).ToList();
            diffOptions.Insert(0, "הכל");
            cmbDifficulty.ItemsSource = diffOptions;
            cmbDifficulty.SelectedIndex = 0;
        }
        private void UpdateView(IEnumerable<song> songsToDisplay)
        {
            sp.Children.Clear();
            foreach (var s in songsToDisplay)
            {
                sp.Children.Add(new SongCardUC(s));
            }
        }

        // לוגיקת סינון ומיון משולבת
        private void ApplyFilters()
        {
            if (cmbGenre == null || cmbLanguage == null || songlist == null || cmbDifficulty == null)
                return;

            var filtered = songlist.AsEnumerable();

            // 1. חיפוש טקסטואלי (שם שיר או אמן)
            string search = txtSearch.Text.ToLower();
            if (!string.IsNullOrEmpty(search))
            {
                filtered = filtered.Where(s => s.Name.ToLower().Contains(search) ||
                                             s.Artistid.Name.ToLower().Contains(search));
            }

            // 2. סינוני חלונית (רק אם הם לא על "הכל")
            if (cmbGenre.SelectedItem?.ToString() != "הכל")
                filtered = filtered.Where(s => s.Gaenreid.Genrename == cmbGenre.SelectedItem.ToString());

            if (cmbLanguage.SelectedItem?.ToString() != "הכל")
                filtered = filtered.Where(s => s.Languageid.Languagename == cmbLanguage.SelectedItem.ToString());

            if (cmbDifficulty.SelectedItem?.ToString() != "הכל")
                filtered = filtered.Where(s => s.Difficultyid.Diff.ToString() == cmbDifficulty.SelectedItem.ToString());

            // 3. מיון
            if (cmbSort.SelectedIndex == 2) // א-ב
                filtered = filtered.OrderBy(s => s.Name);
            else if (cmbSort.SelectedIndex == 3) // קל לקשה
                filtered = filtered.OrderBy(s => s.Difficultyid.Diff.ToString());
            else if (cmbSort.SelectedIndex == 4) // קשה לקל
                filtered = filtered.OrderByDescending(s => s.Difficultyid.Diff.ToString());

            UpdateView(filtered.ToList());
        }

        private int GetDifficultyValue(string level)
        {
            return level switch { "1" => 1, "2" => 2, "3" => 3, "4" => 4, "5" => 5, _ => 0 };
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
