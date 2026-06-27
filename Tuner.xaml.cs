using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MusicSchoolWpf
{
    public partial class Tuner : Page
    {
        private bool isElectric = true;
        private bool isLeftHanded = false;

        private WaveInEvent waveIn;
        private bool isListening = false;

        private float targetFrequency = 82.4f;
        private string targetStringName = "Low E";

        private float tolerance = 5f;

        private class GuitarStringTarget
        {
            public string Name { get; set; }
            public float Frequency { get; set; }

            public GuitarStringTarget(string name, float frequency)
            {
                Name = name;
                Frequency = frequency;
            }
        }

        private readonly GuitarStringTarget[] guitarStrings =
        {
            new GuitarStringTarget("Low E", 82.4f),
            new GuitarStringTarget("A", 110.0f),
            new GuitarStringTarget("D", 146.8f),
            new GuitarStringTarget("G", 196.0f),
            new GuitarStringTarget("B", 246.9f),
            new GuitarStringTarget("High E", 329.6f)
        };

        public Tuner()
        {
            InitializeComponent();

            UpdateView();

            StartListening();

            this.Unloaded += Tuner_Unloaded;
        }

        private void Tuner_Unloaded(object sender, RoutedEventArgs e)
        {
            StopListening();
        }

        private void SwitchHead_Click(object sender, RoutedEventArgs e)
        {
            isElectric = !isElectric;
            UpdateView();
        }

        private void HandSwitch_Checked(object sender, RoutedEventArgs e)
        {
            isLeftHanded = true;
            HandSwitch.Content = "Left Handed";
            UpdateView();
        }

        private void HandSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            isLeftHanded = false;
            HandSwitch.Content = "Right Handed";
            UpdateView();
        }

        private void UpdateView()
        {
            ElectricRight.Visibility = Visibility.Collapsed;
            ElectricLeft.Visibility = Visibility.Collapsed;
            AcousticRight.Visibility = Visibility.Collapsed;
            AcousticLeft.Visibility = Visibility.Collapsed;

            if (isElectric && !isLeftHanded)
                ElectricRight.Visibility = Visibility.Visible;
            else if (isElectric && isLeftHanded)
                ElectricLeft.Visibility = Visibility.Visible;
            else if (!isElectric && !isLeftHanded)
                AcousticRight.Visibility = Visibility.Visible;
            else
                AcousticLeft.Visibility = Visibility.Visible;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void SetTargetAndPlay(string noteName, string textToShow, double beepFrequency)
        {
            targetStringName = noteName;
            targetFrequency = (float)beepFrequency;

            frequency.Text = noteName + " | Target: " + textToShow + " Hz";

            PlayTone(beepFrequency, 450, 0.65);
        }

        private void PlayTone(double freq, int durationMs, double volume)
        {
            Task.Run(() =>
            {
                try
                {
                    using (WaveOutEvent output = new WaveOutEvent())
                    {
                        SignalGenerator signal = new SignalGenerator()
                        {
                            Frequency = freq,
                            Gain = volume,
                            Type = SignalGeneratorType.SawTooth
                        };

                        output.Init(signal);
                        output.Play();

                        Thread.Sleep(durationMs);

                        output.Stop();
                    }
                }
                catch
                {
                }
            });
        }

        private void he(object sender, RoutedEventArgs e)
        {
            SetTargetAndPlay("High E", "329.6", 329.6);
        }

        private void b(object sender, RoutedEventArgs e)
        {
            SetTargetAndPlay("B", "246.9", 246.9);
        }

        private void g(object sender, RoutedEventArgs e)
        {
            SetTargetAndPlay("G", "196.0", 196.0);
        }

        private void d(object sender, RoutedEventArgs e)
        {
            SetTargetAndPlay("D", "146.8", 146.8);
        }

        private void a(object sender, RoutedEventArgs e)
        {
            SetTargetAndPlay("A", "110.0", 110.0);
        }

        private void le(object sender, RoutedEventArgs e)
        {
            SetTargetAndPlay("Low E", "82.4", 82.4);
        }

        private void StartListening()
        {
            try
            {
                if (isListening)
                    return;

                waveIn = new WaveInEvent();

                // 0 = התקן הקלט הראשי שמוגדר ב-Windows
                waveIn.DeviceNumber = 0;

                waveIn.WaveFormat = new WaveFormat(44100, 16, 1);
                waveIn.BufferMilliseconds = 80;
                waveIn.DataAvailable += OnDataAvailable;

                waveIn.StartRecording();

                isListening = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("לא הצלחתי להתחיל להאזין לקלט:\n" + ex.Message);
            }
        }

        private void StopListening()
        {
            try
            {
                if (waveIn != null)
                {
                    waveIn.DataAvailable -= OnDataAvailable;
                    waveIn.StopRecording();
                    waveIn.Dispose();
                    waveIn = null;
                }

                isListening = false;
            }
            catch
            {
            }
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            try
            {
                int sampleCount = e.BytesRecorded / 2;

                if (sampleCount <= 0)
                    return;

                float[] samples = new float[sampleCount];

                for (int i = 0; i < sampleCount; i++)
                {
                    short sample = BitConverter.ToInt16(e.Buffer, i * 2);
                    samples[i] = sample / 32768f;
                }

                float inputLevel = GetInputLevel(samples);

                // רק אם באמת יש סאונד מהגיטרה/קלט
                if (inputLevel < 0.015f)
                    return;

                float detectedFreq = DetectFrequency(samples, 44100);

                // טווח רחב כדי לא לפספס מיתרים גבוהים או קפיצות
                if (detectedFreq < 40 || detectedFreq > 1000)
                    return;

                Dispatcher.Invoke(() =>
                {
                    UpdateUI(detectedFreq);
                });
            }
            catch
            {
            }
        }

        private float GetInputLevel(float[] samples)
        {
            float max = 0;

            for (int i = 0; i < samples.Length; i++)
            {
                float abs = Math.Abs(samples[i]);

                if (abs > max)
                    max = abs;
            }

            return max;
        }

        private float DetectFrequency(float[] samples, int sampleRate)
        {
            int crossings = 0;

            for (int i = 1; i < samples.Length; i++)
            {
                if ((samples[i - 1] < 0 && samples[i] >= 0) ||
                    (samples[i - 1] > 0 && samples[i] <= 0))
                {
                    crossings++;
                }
            }

            double seconds = (double)samples.Length / sampleRate;

            if (seconds <= 0)
                return 0;

            double frequency = crossings / 2.0 / seconds;

            return (float)frequency;
        }

        private GuitarStringTarget FindClosestString(float detectedFrequency)
        {
            GuitarStringTarget closest = guitarStrings[0];
            float smallestDiff = Math.Abs(detectedFrequency - closest.Frequency);

            foreach (GuitarStringTarget guitarString in guitarStrings)
            {
                float diff = Math.Abs(detectedFrequency - guitarString.Frequency);

                if (diff < smallestDiff)
                {
                    smallestDiff = diff;
                    closest = guitarString;
                }
            }

            return closest;
        }

        private void UpdateUI(float detectedFrequency)
        {
            GuitarStringTarget closestString = FindClosestString(detectedFrequency);

            targetStringName = closestString.Name;
            targetFrequency = closestString.Frequency;

            double realDiff = detectedFrequency - targetFrequency;

            frequency.Text =
                "Detected: " + detectedFrequency.ToString("0.0") +
                " Hz | String: " + targetStringName +
                " | Target: " + targetFrequency.ToString("0.0") + " Hz";

            double maxRange = 20;
            double displayDiff = Math.Max(-maxRange, Math.Min(maxRange, realDiff));

            double pixelsPerHz = 5;
            double offset = displayDiff * pixelsPerHz;

            FrequencyLine.Margin = new Thickness(offset, 0, 0, 0);

            GoodRange.Width = tolerance * pixelsPerHz * 2;

            if (Math.Abs(realDiff) <= tolerance)
            {
                GoodRange.Background = System.Windows.Media.Brushes.LimeGreen;
            }
            else
            {
                GoodRange.Background = System.Windows.Media.Brushes.DarkGreen;
            }
        }
    }
}