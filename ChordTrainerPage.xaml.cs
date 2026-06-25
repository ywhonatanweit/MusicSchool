using Model;
using NoaMedia;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MusicSchoolWpf
{
    public partial class ChordTrainerPage : Page
    {
        private readonly ApiService api = new ApiService();
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private readonly Random random = new Random();

        private List<chord> chords = new List<chord>();
        private chord? currentChord;

        private int bpm = 80;
        private int beatsPerChord = 4;
        private int currentBeat = 0;
        private bool isPlaying = false;
        private bool chordSoundEnabled = true;

        public ChordTrainerPage()
        {
            InitializeComponent();

            timer.Tick += Timer_Tick;

            UpdateTimerInterval();
            LoadChords();
        }

        private async void LoadChords()
        {
            try
            {
                ChordList loaded = await api.SelectAllChords();

                chords = loaded
                    .Where(x => !string.IsNullOrWhiteSpace(x.Name) && x.Name != "N.C.")
                    .ToList();

                if (chords.Count == 0)
                {
                    txtChordName.Text = "אין אקורדים";
                    txtBeatInfo.Text = "צריך להוסיף אקורדים למסד הנתונים";
                    btnStartStop.IsEnabled = false;
                    btnNext.IsEnabled = false;
                    return;
                }

                ShowRandomChord();
            }
            catch (Exception ex)
            {
                txtChordName.Text = "שגיאה";
                txtBeatInfo.Text = "לא הצלחתי לטעון אקורדים: " + ex.Message;
                btnStartStop.IsEnabled = false;
            }
        }

        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (isPlaying)
                Stop();
            else
                Start();
        }

        private void Start()
        {
            if (chords.Count == 0)
                return;

            isPlaying = true;
            btnStartStop.Content = "Stop";

            currentBeat = 0;

            UpdateTimerInterval();
            timer.Start();

            Tick();
        }

        private void Stop()
        {
            isPlaying = false;
            timer.Stop();

            btnStartStop.Content = "Start";
            txtBeatInfo.Text = "עצור";
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            Tick();
        }

        private void Tick()
        {
            PlayClick();

            currentBeat++;

            if (currentBeat > beatsPerChord)
            {
                currentBeat = 1;
                ShowRandomChord();
            }

            txtBeatInfo.Text = "פעימה " + currentBeat + " מתוך " + beatsPerChord + "  |  " + bpm + " BPM";
        }

        private void ShowRandomChord()
        {
            if (chords.Count == 0)
                return;

            chord next;

            do
            {
                int index = random.Next(chords.Count);
                next = chords[index];
            }
            while (chords.Count > 1 && currentChord != null && next.Id == currentChord.Id);

            currentChord = next;

            txtChordName.Text = currentChord.Name;
            imgChord.Source = ImageExtensions.ToBitmapImage(currentChord.Chordpic, currentChord.Chordpath);

            if (chordSoundEnabled)
            {
                ChordSoundPlayer.PlayChordById(currentChord.Id);
            }
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            currentBeat = 0;
            ShowRandomChord();
            txtBeatInfo.Text = "אקורד הוחלף ידנית";
        }

        private void BpmSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bpm = (int)Math.Round(e.NewValue);

            if (txtBpm != null)
                txtBpm.Text = bpm.ToString();

            UpdateTimerInterval();
        }

        private void TxtBpm_LostFocus(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(txtBpm.Text, out int parsed))
            {
                bpm = Math.Max(40, Math.Min(220, parsed));
                bpmSlider.Value = bpm;
                txtBpm.Text = bpm.ToString();

                UpdateTimerInterval();
            }
        }

        private void BeatsSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            beatsPerChord = Math.Max(1, (int)Math.Round(e.NewValue));

            if (txtBeatsPerChord != null)
                txtBeatsPerChord.Text = beatsPerChord.ToString();
        }

        private void UpdateTimerInterval()
        {
            timer.Interval = TimeSpan.FromMilliseconds(60000.0 / bpm);
        }

        private void PlayClick()
        {
            int frequency = currentBeat == 0 ? 1100 : 850;

            Task.Run(() =>
            {
                try
                {
                    Console.Beep(frequency, 45);
                }
                catch
                {
                }
            });
        }
        private void BtnSound_Click(object sender, RoutedEventArgs e)
        {
            chordSoundEnabled = !chordSoundEnabled;

            btnSound.Content = chordSoundEnabled ? "Sound: ON" : "Sound: OFF";
            btnSound.Background = chordSoundEnabled
                ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(204, 34, 0))
                : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(51, 51, 51));
        }
    }
}