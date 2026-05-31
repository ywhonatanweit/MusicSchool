using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Model;
using Service;

namespace MusicSchoolWpf
{
    public partial class AdminDashboardPage : Page
    {
        private ApiService api = new ApiService();
        private string selectedSongPicBase64 = null;

        public AdminDashboardPage()
        {
            InitializeComponent();
            LoadFormComboboxes();
            LoadExistingSongs();
        }

        // טעינת רשימות הבחירה לטופס מה-API (כדי שלא יהיה שדה חופשי)
        private async void LoadFormComboboxes()
        {
            try
            {
                cmbArtist.ItemsSource = await api.SelectAllArtists();
                cmbArtist.DisplayMemberPath = "Name";

                cmbGenre.ItemsSource = await api.SelectAllGenres();
                cmbGenre.DisplayMemberPath = "Genrename";

                cmbLanguage.ItemsSource = await api.SelectAllLanguages();
                cmbLanguage.DisplayMemberPath = "Languagename";
            }
            catch { }
        }

        // טעינת השירים הקיימים (תצוגה שדומה לדף 2)
        private async void LoadExistingSongs()
        {
            try
            {
                SongList songs = await api.SelectAllSongs();
                wpItems.Children.Clear();
                foreach (song s in songs)
                {
                    // שימוש ביוזר קונטרול הקיים שלך כדי להציג את רשימת השירים
                    wpItems.Children.Add(new SongUserControl(s));
                }
            }
            catch { }
        }

        // העלאת תמונה והמרתה ל-Base64
        private void BtnUploadPic_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "בחר תמונת שיר";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";

            if (op.ShowDialog() == true)
            {
                byte[] imageBytes = File.ReadAllBytes(op.FileName);
                selectedSongPicBase64 = Convert.ToBase64String(imageBytes);
                lblPicStatus.Text = "תמונה נטענה בהצלחה!";
            }
        }

        // כפתור שמירה שמפעיל את אלגוריתם הפיצול
        private async void BtnSaveSong_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSongName.Text) || cmbArtist.SelectedItem == null || cmbGenre.SelectedItem == null)
            {
                MessageBox.Show("נא למלא שדות חובה: שם שיר, אמן וסגנון.");
                return;
            }

            // 1. יצירת אובייקט השיר הבסיסי
            song newSong = new song
            {
                Name = txtSongName.Text,
                Artistid = (Artist)cmbArtist.SelectedItem,
                Gaenreid = (genre)cmbGenre.SelectedItem,
                Languageid = (language)cmbLanguage.SelectedItem,
                SongPic = selectedSongPicBase64,
                Difficultyid = new difficulty { Diff = (int)songRating.Value } // השמת דירוג הכוכבים
            };

            // שמירת השיר בשרת לקבלת מזהה (ID)
            int songInsertResult = await api.InsertASong(newSong);

            // שליפת השיר מחדש כדי לקבל את ה-ID העדכני שלו לצורך קישור המילים
            SongList allSongs = await api.SelectAllSongs();
            song savedSong = allSongs.FirstOrDefault(x => x.Name == newSong.Name && x.Artistid.Id == newSong.Artistid.Id);

            if (savedSong == null)
            {
                MessageBox.Show("שגיאה בשמירת השיר, לא ניתן להמשיך להכנסת מילים.");
                return;
            }

            // 2. הפעלת מנגנון פירוק הטקסט והאקורדים (האלגוריתם שביקשת)
            string rawContent = txtLyricsAndChords.Text;

            // פיצול המחרוזת לפי הסוגריים המרובעים. כולל את הסוגריים במערך התוצאה.
            string[] tokens = Regex.Split(rawContent, @"(\[.*?\])");

            int placementCounter = 1;
            ChordList allChords = await api.SelectAllChords();

            // רצים על המערך המפוצל
            for (int i = 0; i < tokens.Length; i++)
            {
                string currentToken = tokens[i];

                // אם הגענו לטקסט שהוא לא אקורד (כלומר משפט/מילים)
                if (!currentToken.StartsWith("[") && !string.IsNullOrWhiteSpace(currentToken))
                {
                    string textSegment = currentToken;
                    string associatedChordName = null;

                    // בודקים אם האיבר הבא מיד אחריו הוא האקורד בסוגריים שמקושר אליו
                    if (i + 1 < tokens.Length && tokens[i + 1].StartsWith("["))
                    {
                        // שליפת שם האקורד מתוך הסוגריים הריבועיים [Am] -> Am
                        associatedChordName = tokens[i + 1].Trim('[', ']');
                    }

                    // מציאת האובייקט של האקורד מתוך מסד הנתונים לפי השם שלו
                    chord matchingChord = allChords.FirstOrDefault(c => c.Name.ToLower() == associatedChordName?.ToLower());

                    // יצירת אובייקט הליריקה החדש וקישורו
                    lyrics newLine = new lyrics
                    {
                        Lyricsname = textSegment,
                        Songid = savedSong,
                        Placment = placementCounter++,
                        Chordid = matchingChord ?? new chord { Name = associatedChordName ?? "N/A" }
                    };

                    // שליחה לשרת/API
                    await api.InsertALyrics(newLine);
                }
            }

            MessageBox.Show("השיר, המילים והאקורדים פורקו ונשמרו בהצלחה!");
            LoadExistingSongs(); // רענון רשימת השירים בדף
        }
    }
}