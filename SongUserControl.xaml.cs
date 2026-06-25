using Model;
using NoaMedia;
using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MusicSchoolWpf
{
    public partial class SongUserControl : UserControl
    {
        ApiService apiService = new ApiService();

        public SongUserControl(song s)
        {
            InitializeComponent();
            GetSong(s);
        }

        private async void GetSong(song s)
        {
            try
            {
                LyricsList ly = await apiService.SelectAllLyrics();

                List<lyrics> currentLy = ly
                    .Where(x => x.Songid != null && x.Songid.Id == s.Id)
                    .OrderBy(x => x.Placment)
                    .ToList();

                diffRating.Value = s.Difficultyid != null ? s.Difficultyid.Id : 0;
                genrebox.Text = "genre " + (s.Gaenreid != null ? s.Gaenreid.Genrename : "");
                artistbox.Text = "artist " + (s.Artistid != null ? s.Artistid.Name : "");
                namebox.Text = s.Name ?? "";
                lanbox.Text = s.Languageid != null ? s.Languageid.Languagename : "";

                pic.Source = ImageExtensions.ToBitmapImage(s.SongPic, s.Songpath);

                songData.Children.Clear();

                if (currentLy.Count == 0)
                {
                    TextBlock emptyText = new TextBlock();
                    emptyText.Text = "אין מילים לשיר הזה";
                    emptyText.Foreground = Brushes.Gray;
                    emptyText.FontSize = 16;
                    emptyText.Margin = new Thickness(10);
                    songData.Children.Add(emptyText);
                    return;
                }

                foreach (lyrics lyr in currentLy)
                {
                    StackPanel lyPanel = new StackPanel
                    {
                        Margin = new Thickness(10, 0, 10, 10)
                    };

                    if (lyr.Chordid != null &&
                        !string.IsNullOrWhiteSpace(lyr.Chordid.Name) &&
                        lyr.Chordid.Name != "N.C.")
                    {
                        TextBlock chordText = new TextBlock();
                        chordText.Text = lyr.Chordid.Name;
                        chordText.Foreground = Brushes.DarkRed;
                        chordText.FontWeight = FontWeights.Bold;
                        chordText.FontSize = 14;

                        lyPanel.Children.Add(chordText);
                    }

                    TextBlock lyText = new TextBlock();
                    lyText.Text = lyr.Lyricsname ?? "";
                    lyText.Foreground = Brushes.White;
                    lyText.FontSize = 16;
                    lyText.TextWrapping = TextWrapping.Wrap;

                    lyPanel.Children.Add(lyText);
                    songData.Children.Add(lyPanel);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("בעיה בטעינת השיר:\n" + ex.Message);
            }
        }
        private FlowDirection GetSongFlowDirection(song s)
        {
            string language = s.Languageid?.Languagename?.Trim().ToLower() ?? "";

            if (language == "english" || language == "אנגלית")
                return FlowDirection.LeftToRight;

            return FlowDirection.RightToLeft;
        }

        private TextAlignment GetSongTextAlignment(song s)
        {
            string language = s.Languageid?.Languagename?.Trim().ToLower() ?? "";

            if (language == "english" || language == "אנגלית" )
                return TextAlignment.Left;

            return TextAlignment.Right;
        }
    }
}
    