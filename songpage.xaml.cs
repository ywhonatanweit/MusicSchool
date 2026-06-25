using Model;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MusicSchoolWpf
{
    /// <summary>
    /// Interaction logic for songpage.xaml
    /// </summary>
    public partial class songpage : Page
    {
        ApiService api = new ApiService();
        SongList songlist = new SongList();

        public songpage()
        {
            InitializeComponent();

            LoadPageData();
        }

        public songpage(song s)
        {
            InitializeComponent();

            HideSearchAndFilterArea();

            sp.Children.Clear();

            SongUserControl sUC = new SongUserControl(s);
            sp.Children.Add(sUC);
        }

        private async void LoadPageData()
        {
            try
            {
                songlist = await api.SelectAllSongs();

                await LoadFilterOptions();

                UpdateView(songlist);
            }
            catch (Exception ex)
            {
                MessageBox.Show("בעיה בטעינת השירים:\n" + ex.Message);
            }
        }

        private async Task LoadFilterOptions()
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

            List<string> diffOptions = di
                .OrderBy(d => d.Diff)
                .Select(d => d.Diff.ToString())
                .ToList();

            diffOptions.Insert(0, "הכל");
            cmbDifficulty.ItemsSource = diffOptions;
            cmbDifficulty.SelectedIndex = 0;
        }

        private void HideSearchAndFilterArea()
        {
            SearchHeader.Visibility = Visibility.Collapsed;
            SortFilterRow.Visibility = Visibility.Collapsed;
            filterPanel.Visibility = Visibility.Collapsed;

            MainGrid.RowDefinitions[0].Height = new GridLength(0);
            MainGrid.RowDefinitions[1].Height = new GridLength(0);
        }

        private void UpdateView(IEnumerable<song> songsToDisplay)
        {
            sp.Children.Clear();

            foreach (song s in songsToDisplay)
            {
                sp.Children.Add(new SongCardUC(s));
            }
        }

        private void ApplyFilters()
        {
            if (cmbGenre == null ||
                cmbLanguage == null ||
                cmbDifficulty == null ||
                cmbSort == null ||
                txtSearch == null ||
                songlist == null)
            {
                return;
            }

            IEnumerable<song> filtered = songlist.AsEnumerable();

            string search = txtSearch.Text.Trim().ToLower();

            if (!string.IsNullOrWhiteSpace(search))
            {
                filtered = filtered.Where(s =>
                    (s.Name != null && s.Name.ToLower().Contains(search)) ||
                    (s.Artistid != null &&
                     s.Artistid.Name != null &&
                     s.Artistid.Name.ToLower().Contains(search)));
            }

            if (cmbGenre.SelectedItem != null &&
                cmbGenre.SelectedItem.ToString() != "הכל")
            {
                string selectedGenre = cmbGenre.SelectedItem.ToString();

                filtered = filtered.Where(s =>
                    s.Gaenreid != null &&
                    s.Gaenreid.Genrename == selectedGenre);
            }

            if (cmbLanguage.SelectedItem != null &&
                cmbLanguage.SelectedItem.ToString() != "הכל")
            {
                string selectedLanguage = cmbLanguage.SelectedItem.ToString();

                filtered = filtered.Where(s =>
                    s.Languageid != null &&
                    s.Languageid.Languagename == selectedLanguage);
            }

            if (cmbDifficulty.SelectedItem != null &&
                cmbDifficulty.SelectedItem.ToString() != "הכל")
            {
                int selectedDifficulty = int.Parse(cmbDifficulty.SelectedItem.ToString());

                filtered = filtered.Where(s =>
                    s.Difficultyid != null &&
                    s.Difficultyid.Diff == selectedDifficulty);
            }

            string sortText = GetSelectedSortText();

            if (sortText.Contains("א-ב") || sortText.Contains("שם"))
            {
                filtered = filtered.OrderBy(s => s.Name);
            }
            else if (sortText == "קל לקשה")
            {
                filtered = filtered.OrderBy(s =>
                    s.Difficultyid != null ? s.Difficultyid.Diff : 0);
            }
            else if (sortText == "קשה לקל")
            {
                filtered = filtered.OrderByDescending(s =>
                    s.Difficultyid != null ? s.Difficultyid.Diff : 0);
            }

            UpdateView(filtered.ToList());
        }

        private string GetSelectedSortText()
        {
            if (cmbSort.SelectedItem is ComboBoxItem item)
                return item.Content?.ToString() ?? "";

            return cmbSort.SelectedItem?.ToString() ?? "";
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void cmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplyFilters();
        }

        private void btnOpenFilter_Click(object sender, RoutedEventArgs e)
        {
            filterPanel.Visibility = Visibility.Visible;
        }

        private void btnCloseFilter_Click(object sender, RoutedEventArgs e)
        {
            filterPanel.Visibility = Visibility.Collapsed;
        }

        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            ApplyFilters();
            filterPanel.Visibility = Visibility.Collapsed;
        }
    }
}