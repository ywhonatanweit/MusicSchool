using Microsoft.Win32;
using Model;
using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MusicSchoolWpf
{
    public partial class AdminSongsPage : Page
    {
        private readonly ApiService api = new ApiService();
        private readonly ItunesMusicService itunesService = new ItunesMusicService();

        private string adminName;

        public string AdminName => adminName;

        private string? selectedSongPicBase64 = null;
        private string? selectedSongPicPath = null;

        private song? editingSong = null;

        private SongList songs = new SongList();
        private ArtistList artists = new ArtistList();
        private GenreList genres = new GenreList();
        private LanguageList languages = new LanguageList();
        private DifficultyList difficulties = new DifficultyList();
        private ChordList chords = new ChordList();

        public AdminSongsPage() : this("Admin")
        {
        }

        public AdminSongsPage(string name)
        {
            InitializeComponent();

            adminName = name;

            LoadAllData();
        }

        private async void LoadAllData()
        {
            try
            {
                artists = await api.SelectAllArtists();
                genres = await api.SelectAllGenres();
                languages = await api.SelectAllLanguages();
                difficulties = await api.SelectAllDifficulties();
                chords = await api.SelectAllChords();

                RefreshArtistsCombo();
                RefreshGenresCombo();

                cmbLanguage.ItemsSource = null;
                cmbLanguage.ItemsSource = languages;
                cmbLanguage.DisplayMemberPath = "Languagename";

                await LoadSongsOnlyAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("בעיה בטעינת הנתונים מהשרת:\n" + ex.Message);
            }
        }

        private async Task LoadSongsOnlyAsync()
        {
            songs = await api.SelectAllSongs();

            lstExistingSongs.ItemsSource = null;
            lstExistingSongs.ItemsSource = songs;
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AdminDashboardPage(adminName));
        }

        private async void BtnSearchApi_Click(object sender, RoutedEventArgs e)
        {
            string query = txtApiSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(query))
            {
                MessageBox.Show("כתוב שם שיר או אמן לחיפוש");
                return;
            }

            try
            {
                lblApiStatus.Text = "מחפש מידע מ-iTunes...";

                List<ItunesTrack> results = await itunesService.SearchSongsAsync(query, 12);

                lstApiResults.ItemsSource = null;
                lstApiResults.ItemsSource = results;

                lblApiStatus.Text = "נמצאו " + results.Count + " תוצאות";

                if (results.Count == 0)
                    MessageBox.Show("לא נמצאו תוצאות");
            }
            catch (Exception ex)
            {
                lblApiStatus.Text = "";
                MessageBox.Show("בעיה בחיפוש דרך iTunes API:\n" + ex.Message);
            }
        }

        private async void LstApiResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItunesTrack? track = lstApiResults.SelectedItem as ItunesTrack;

            if (track == null)
                return;

            txtSongName.Text = track.TrackName ?? "";

            try
            {
                Artist? selectedArtist = await GetOrCreateArtistFromApi(track.ArtistName);

                if (selectedArtist != null)
                    cmbArtist.SelectedItem = selectedArtist;
            }
            catch
            {
                lblApiStatus.Text = "פרטי השיר נטענו, אבל האמן לא נוסף אוטומטית.";
            }

            try
            {
                genre? selectedGenre = await GetOrCreateGenreFromApi(track.PrimaryGenreName);

                if (selectedGenre != null)
                    cmbGenre.SelectedItem = selectedGenre;
            }
            catch
            {
                lblApiStatus.Text = "פרטי השיר נטענו, אבל הז׳אנר לא נוסף אוטומטית.";
            }

            TrySelectDefaultLanguage();

            selectedSongPicBase64 = null;
            selectedSongPicPath = null;
            lblPicStatus.Text = "לא נבחרה תמונה";

            if (!string.IsNullOrWhiteSpace(track.BestArtworkUrl))
            {
                string? imageBase64 = await itunesService.DownloadImageAsBase64Async(track.BestArtworkUrl);

                if (!string.IsNullOrWhiteSpace(imageBase64))
                {
                    selectedSongPicBase64 = imageBase64;
                    selectedSongPicPath = SaveBase64ImageToLocalFile(imageBase64, txtSongName.Text);
                    lblPicStatus.Text = "תמונה נטענה אוטומטית מ-iTunes";
                }
                else
                {
                    selectedSongPicBase64 = null;
                    selectedSongPicPath = null;
                    lblPicStatus.Text = "לא נטענה תמונה. אפשר להמשיך בלי תמונה או לבחור ידנית.";
                }
            }

            lblApiStatus.Text = "פרטי השיר נטענו. תמונה אינה חובה.";

            lstApiResults.SelectedItem = null;
        }

        private void LstExistingSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            song? selected = lstExistingSongs.SelectedItem as song;

            if (selected == null)
                return;

            editingSong = selected;

            txtSongName.Text = selected.Name ?? "";
            selectedSongPicBase64 = selected.SongPic;
            selectedSongPicPath = selected.Songpath;

            SelectComboById(cmbArtist, selected.Artistid?.Id ?? 0);
            SelectComboById(cmbGenre, selected.Gaenreid?.Id ?? 0);
            SelectComboById(cmbLanguage, selected.Languageid?.Id ?? 0);

            if (selected.Difficultyid != null)
                songRating.Value = selected.Difficultyid.Diff;
            else
                songRating.Value = 3;

            lblPicStatus.Text =
                string.IsNullOrWhiteSpace(selectedSongPicBase64) &&
                string.IsNullOrWhiteSpace(selectedSongPicPath)
                    ? "לשיר אין תמונה שמורה"
                    : "קיימת תמונה שמורה לשיר";

            LoadLyricsOfSong(selected);

            btnSave.Content = "עדכן שיר במערכת";
        }

        private async void LoadLyricsOfSong(song selectedSong)
        {
            try
            {
                LyricsList allLyrics = await api.SelectAllLyrics();

                List<lyrics> songLyrics = allLyrics
                    .Where(x => x.Songid != null && x.Songid.Id == selectedSong.Id)
                    .OrderBy(x => x.Placment)
                    .ToList();

                txtLyricsAndChords.Text = string.Join(
                    Environment.NewLine,
                    songLyrics.Select(x => FormatLyricForEdit(x))
                );
            }
            catch
            {
                txtLyricsAndChords.Text = "";
            }
        }

        private void BtnUploadPic_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "בחר תמונת שיר";
            op.Filter = "Image files|*.jpg;*.jpeg;*.png";

            if (op.ShowDialog() == true)
            {
                byte[] imageBytes = File.ReadAllBytes(op.FileName);

                selectedSongPicBase64 = Convert.ToBase64String(imageBytes);
                selectedSongPicPath = SaveImageBytesToLocalFile(
                    imageBytes,
                    txtSongName.Text,
                    Path.GetExtension(op.FileName)
                );

                lblPicStatus.Text = "תמונה נטענה מהמחשב";
            }
        }

        private void BtnImportTxt_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "בחר קובץ מילים ואקורדים";
            op.Filter = "Text files|*.txt";

            if (op.ShowDialog() == true)
            {
                txtLyricsAndChords.Text = File.ReadAllText(op.FileName);
                MessageBox.Show("הקובץ נטען בהצלחה");
            }
        }

        private async void BtnSaveSong_Click(object sender, RoutedEventArgs e)
        {
            Artist? artist = cmbArtist.SelectedItem as Artist;
            genre? selectedGenre = cmbGenre.SelectedItem as genre;
            language? selectedLanguage = cmbLanguage.SelectedItem as language;

            if (string.IsNullOrWhiteSpace(txtSongName.Text))
            {
                MessageBox.Show("יש למלא שם שיר");
                return;
            }

            if (artist == null)
            {
                MessageBox.Show("יש לבחור אמן");
                return;
            }

            if (selectedGenre == null)
            {
                MessageBox.Show("יש לבחור סגנון");
                return;
            }

            if (selectedLanguage == null)
            {
                MessageBox.Show("יש לבחור שפה");
                return;
            }

            List<ParsedLyricLine> parsedLines = SongTextParser.Parse(txtLyricsAndChords.Text);

            difficulty selectedDifficulty = FindDifficulty((int)Math.Round(songRating.Value));

            song songToSave = new song
            {
                Name = txtSongName.Text.Trim(),
                Artistid = artist,
                Gaenreid = selectedGenre,
                Languageid = selectedLanguage,
                Difficultyid = selectedDifficulty,
                SongPic = string.IsNullOrWhiteSpace(selectedSongPicPath) ? (selectedSongPicBase64 ?? "") : "",
                Songpath = selectedSongPicPath ?? ""
            };

            if (editingSong != null)
                songToSave.Id = editingSong.Id;

            try
            {
                int result;

                if (editingSong == null)
                    result = await api.InsertASong(songToSave);
                else
                    result = await api.UpdateASong(songToSave);

                if (result <= 0)
                {
                    MessageBox.Show("השיר לא נשמר. בדוק את InsertASong / UpdateASong בשרת.");
                    return;
                }

                await LoadSongsOnlyAsync();

                song? savedSong;

                if (editingSong == null)
                {
                    savedSong = songs
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault(x =>
                            Normalize(x.Name) == Normalize(songToSave.Name) &&
                            x.Artistid != null &&
                            x.Artistid.Id == songToSave.Artistid.Id);
                }
                else
                {
                    savedSong = songs.FirstOrDefault(x => x.Id == songToSave.Id);
                }

                if (savedSong == null)
                {
                    MessageBox.Show("השיר נשמר, אבל לא נמצא מחדש לצורך שמירת המילים.");
                    return;
                }

                bool lyricsSaved = await SaveLyricsForSong(savedSong, parsedLines);

                if (!lyricsSaved)
                    return;

                MessageBox.Show(editingSong == null
                    ? "השיר נוסף בהצלחה"
                    : "השיר עודכן בהצלחה");

                ClearForm();

                await LoadSongsOnlyAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("בעיה בשמירת השיר:\n" + ex.Message);
            }
        }

        private async Task<bool> SaveLyricsForSong(song savedSong, List<ParsedLyricLine> parsedLines)
        {
            try
            {
                if (editingSong != null)
                {
                    LyricsList allLyrics = await api.SelectAllLyrics();

                    List<lyrics> oldLyrics = allLyrics
                        .Where(x => x.Songid != null && x.Songid.Id == savedSong.Id)
                        .ToList();

                    foreach (lyrics oldLine in oldLyrics)
                    {
                        await api.DeleteALyrics(oldLine);
                    }
                }

                if (parsedLines.Count == 0)
                    return true;

                int placement = 1;

                foreach (ParsedLyricLine parsed in parsedLines)
                {
                    string chordName = string.IsNullOrWhiteSpace(parsed.ChordName)
                        ? "N.C."
                        : parsed.ChordName;

                    chord? matchingChord = await GetOrCreateChordByName(chordName);

                    if (matchingChord == null)
                        continue;

                    lyrics newLine = new lyrics
                    {
                        Songid = savedSong,
                        Chordid = matchingChord,
                        Placment = placement++,
                        Lyricsname = parsed.Text ?? ""
                    };

                    await api.InsertALyrics(newLine);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("השיר נשמר, אבל הייתה בעיה בשמירת המילים והאקורדים:\n" + ex.Message);
                return false;
            }
        }

        private async void BtnDeleteSong_Click(object sender, RoutedEventArgs e)
        {
            if (editingSong == null)
            {
                MessageBox.Show("בחר שיר למחיקה מהרשימה");
                return;
            }

            MessageBoxResult confirm = MessageBox.Show(
                "האם אתה בטוח שברצונך למחוק את השיר?",
                "אישור מחיקה",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes)
                return;

            try
            {
                LyricsList allLyrics = await api.SelectAllLyrics();

                List<lyrics> lyricsToDelete = allLyrics
                    .Where(x => x.Songid != null && x.Songid.Id == editingSong.Id)
                    .ToList();

                foreach (lyrics line in lyricsToDelete)
                {
                    await api.DeleteALyrics(line);
                }

                int result = await api.DeleteASong(editingSong);

                if (result > 0)
                {
                    MessageBox.Show("השיר נמחק בהצלחה");
                    ClearForm();
                    await LoadSongsOnlyAsync();
                }
                else
                {
                    MessageBox.Show("השיר לא נמחק");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("בעיה במחיקת השיר:\n" + ex.Message);
            }
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearForm();
        }

        private void ClearForm()
        {
            editingSong = null;
            selectedSongPicBase64 = null;
            selectedSongPicPath = null;

            txtSongName.Clear();
            txtLyricsAndChords.Clear();
            txtApiSearch.Clear();

            cmbArtist.SelectedItem = null;
            cmbGenre.SelectedItem = null;
            cmbLanguage.SelectedItem = null;

            songRating.Value = 3;

            lblPicStatus.Text = "לא נבחרה תמונה";
            lblApiStatus.Text = "";

            lstApiResults.ItemsSource = null;
            lstExistingSongs.SelectedItem = null;

            btnSave.Content = "שמור שיר במערכת";
        }

        private async Task<Artist?> GetOrCreateArtistFromApi(string? artistName)
        {
            if (string.IsNullOrWhiteSpace(artistName))
                return null;

            try
            {
                artists = await api.SelectAllArtists();

                Artist? existingArtistByName = artists.FirstOrDefault(x =>
                    Normalize(x.Name) == Normalize(artistName));

                if (existingArtistByName != null)
                {
                    RefreshArtistsCombo();
                    return existingArtistByName;
                }

                PersonList persons = await api.SelectAllPersons();

                person? existingPerson = persons.FirstOrDefault(x =>
                    Normalize(x.Name) == Normalize(artistName));

                if (existingPerson == null)
                {
                    person newPerson = new person
                    {
                        Name = artistName.Trim(),
                        Code = "artist"
                    };

                    int personResult = await api.InsertAPerson(newPerson);

                    if (personResult <= 0)
                        return null;

                    persons = await api.SelectAllPersons();

                    existingPerson = persons
                        .OrderByDescending(x => x.Id)
                        .FirstOrDefault(x => Normalize(x.Name) == Normalize(artistName));
                }

                if (existingPerson == null)
                    return null;

                artists = await api.SelectAllArtists();

                Artist? existingArtistById = artists.FirstOrDefault(x =>
                    x.Id == existingPerson.Id);

                if (existingArtistById != null)
                {
                    RefreshArtistsCombo();
                    return existingArtistById;
                }

                Artist newArtist = new Artist
                {
                    Id = existingPerson.Id,
                    Name = existingPerson.Name,
                    Code = existingPerson.Code
                };

                int artistResult = await api.InsertAArtist(newArtist);

                if (artistResult <= 0)
                    return null;

                artists = await api.SelectAllArtists();

                RefreshArtistsCombo();

                Artist? finalArtist = artists.FirstOrDefault(x => x.Id == existingPerson.Id);

                return finalArtist;
            }
            catch
            {
                try
                {
                    artists = await api.SelectAllArtists();

                    RefreshArtistsCombo();

                    Artist? fallbackArtist = artists.FirstOrDefault(x =>
                        Normalize(x.Name) == Normalize(artistName));

                    return fallbackArtist;
                }
                catch
                {
                    return null;
                }
            }
        }

        private async Task<genre?> GetOrCreateGenreFromApi(string? genreName)
        {
            if (string.IsNullOrWhiteSpace(genreName))
                return null;

            try
            {
                genres = await api.SelectAllGenres();

                genre? existingGenre = genres.FirstOrDefault(x =>
                    Normalize(x.Genrename) == Normalize(genreName));

                if (existingGenre != null)
                {
                    RefreshGenresCombo();
                    return existingGenre;
                }

                genre newGenre = new genre
                {
                    Genrename = genreName.Trim()
                };

                int genreResult = await api.InsertAGenre(newGenre);

                if (genreResult <= 0)
                    return null;

                genres = await api.SelectAllGenres();

                RefreshGenresCombo();

                genre? finalGenre = genres
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault(x => Normalize(x.Genrename) == Normalize(genreName));

                return finalGenre;
            }
            catch
            {
                try
                {
                    genres = await api.SelectAllGenres();

                    RefreshGenresCombo();

                    genre? fallbackGenre = genres.FirstOrDefault(x =>
                        Normalize(x.Genrename) == Normalize(genreName));

                    return fallbackGenre;
                }
                catch
                {
                    return null;
                }
            }
        }

        private async Task<chord?> GetOrCreateChordByName(string? chordName)
        {
            if (string.IsNullOrWhiteSpace(chordName))
                return null;

            try
            {
                chords = await api.SelectAllChords();

                chord? existingChord = chords.FirstOrDefault(x =>
                    Normalize(x.Name) == Normalize(chordName));

                if (existingChord != null)
                    return existingChord;

                difficulty easyDifficulty = FindDifficulty(1);

                chord newChord = new chord
                {
                    Name = chordName.Trim(),
                    Difficulty = easyDifficulty,
                    Chordpic = "",
                    Chordpath = ""
                };

                int insertResult = await api.InsertAChord(newChord);

                if (insertResult <= 0)
                    return null;

                chords = await api.SelectAllChords();

                chord? finalChord = chords.FirstOrDefault(x =>
                    Normalize(x.Name) == Normalize(chordName));

                return finalChord;
            }
            catch
            {
                return null;
            }
        }

        private void RefreshArtistsCombo()
        {
            cmbArtist.ItemsSource = null;
            cmbArtist.ItemsSource = artists;
            cmbArtist.DisplayMemberPath = "Name";
        }

        private void RefreshGenresCombo()
        {
            cmbGenre.ItemsSource = null;
            cmbGenre.ItemsSource = genres;
            cmbGenre.DisplayMemberPath = "Genrename";
        }

        private void TrySelectDefaultLanguage()
        {
            if (cmbLanguage.SelectedItem != null)
                return;

            language? english = languages.FirstOrDefault(x =>
                Normalize(x.Languagename) == Normalize("English") ||
                Normalize(x.Languagename) == Normalize("אנגלית"));

            if (english != null)
                cmbLanguage.SelectedItem = english;
        }

        private difficulty FindDifficulty(int diffValue)
        {
            difficulty? d = difficulties.FirstOrDefault(x =>
                x.Diff == diffValue || x.Id == diffValue);

            if (d != null)
                return d;

            return new difficulty
            {
                Id = diffValue,
                Diff = diffValue
            };
        }

        private void SelectComboById(ComboBox combo, int id)
        {
            if (id <= 0)
                return;

            foreach (object item in combo.Items)
            {
                BaseEntity? entity = item as BaseEntity;

                if (entity != null && entity.Id == id)
                {
                    combo.SelectedItem = item;
                    return;
                }
            }
        }

        private string? SaveBase64ImageToLocalFile(string imageBase64, string songName)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(imageBase64);
                return SaveImageBytesToLocalFile(bytes, songName, ".jpg");
            }
            catch
            {
                return null;
            }
        }

        private string? SaveImageBytesToLocalFile(byte[] bytes, string songName, string extension)
        {
            try
            {
                string cleanName = string.IsNullOrWhiteSpace(songName)
                    ? "song"
                    : Normalize(songName);

                if (string.IsNullOrWhiteSpace(cleanName))
                    cleanName = "song";

                string safeExtension = string.IsNullOrWhiteSpace(extension)
                    ? ".jpg"
                    : extension;

                string folder = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "IMAGES",
                    "song_pics"
                );

                Directory.CreateDirectory(folder);

                string fileName =
                    cleanName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + safeExtension;

                string fullPath = Path.Combine(folder, fileName);

                File.WriteAllBytes(fullPath, bytes);

                return Path.Combine("IMAGES", "song_pics", fileName);
            }
            catch
            {
                return null;
            }
        }

        private string FormatLyricForEdit(lyrics line)
        {
            string chordName = line.Chordid?.Name ?? "";
            string text = line.Lyricsname ?? "";

            if (string.IsNullOrWhiteSpace(chordName) || chordName == "N.C.")
                return text;

            return "[" + chordName + "] " + text;
        }

        private string Normalize(string? text)
        {
            return (text ?? "")
                .Trim()
                .ToLower()
                .Replace(" ", "")
                .Replace("-", "")
                .Replace("_", "")
                .Replace("׳", "")
                .Replace("'", "")
                .Replace("\"", "");
        }
    }
}