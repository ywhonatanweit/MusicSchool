//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using System.Windows.Documents;
//using System.Windows.Input;
//using System.Windows.Media;
//using System.Windows.Media.Imaging;
//using System.Windows.Navigation;
//using System.Windows.Shapes;

//namespace MusicSchoolWpf
//{
//    /// <summary>
//    /// Interaction logic for Tuner.xaml
//    /// </summary>
//    public partial class Tuner : Page
//    {
//        public Tuner()
//        {
//            InitializeComponent();
//        }
//        //private bool isElectric = true;

//        //private void SwitchHead_Click(object sender, RoutedEventArgs e)
//        //{
//        //    if (isElectric)
//        //    {
//        //        ElectricHead.Visibility = Visibility.Collapsed;
//        //        AcousticHead.Visibility = Visibility.Visible;
//        //    }
//        //    else
//        //    {
//        //        ElectricHead.Visibility = Visibility.Visible;
//        //        AcousticHead.Visibility = Visibility.Collapsed;
//        //    }

//        //    isElectric = !isElectric;
//        //}
//        private bool isElectric = true;



//        // החלפת חשמלית / אקוסטית
//        private void SwitchHead_Click(object sender, RoutedEventArgs e)
//        {
//            if (isElectric)
//            {
//                ElectricHead.Visibility = Visibility.Collapsed;
//                AcousticHead.Visibility = Visibility.Visible;
//            }
//            else
//            {
//                ElectricHead.Visibility = Visibility.Visible;
//                AcousticHead.Visibility = Visibility.Collapsed;
//            }

//            isElectric = !isElectric;
//        }

//        // מעבר לימני
//        private void HandSwitch_Unchecked(object sender, RoutedEventArgs e)
//        {
//            HeadTransform.ScaleX = 1;
//            HandSwitch.Content = "Right Handed";
//        }

//        // מעבר לשמאלי
//        private void HandSwitch_Checked(object sender, RoutedEventArgs e)
//        {
//            HeadTransform.ScaleX = -1;
//            HandSwitch.Content = "Left Handed";
//        }
//    }
//}
using System.Windows;
using System.Windows.Controls;
using NAudio.Wave;


namespace MusicSchoolWpf
{
    public partial class Tuner : Page
    {
        private bool isElectric = true;
        private bool isLeftHanded = false;

        public Tuner()
        {
            InitializeComponent();
            UpdateView();
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
            // להסתיר הכל
            ElectricRight.Visibility = Visibility.Collapsed;
            ElectricLeft.Visibility = Visibility.Collapsed;
            AcousticRight.Visibility = Visibility.Collapsed;
            AcousticLeft.Visibility = Visibility.Collapsed;

            // להציג רק את המתאים
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

        private void he(object sender, RoutedEventArgs e)
        {
            frequency.Text = "329.6";
        }

        private void d(object sender, RoutedEventArgs e)
        {
            frequency.Text = "146.8";

        }

        private void g(object sender, RoutedEventArgs e)
        {
            frequency.Text = "196.0";

        }

        private void a(object sender, RoutedEventArgs e)
        {
            frequency.Text = "110.0";

        }

        private void b(object sender, RoutedEventArgs e)
        {
            frequency.Text = "246.9";
        }

        private void le(object sender, RoutedEventArgs e)
        {
            frequency.Text = "82.4";

        }

        private WaveInEvent waveIn;
        private float[] buffer = new float[2048];

        private float targetFrequency = 82.4f; // אפשר לשנות בעתיד
        private float tolerance = 5f; // טווח ירוק (Hz)

        

        private void StartListening()
        {
            waveIn = new WaveInEvent();
            waveIn.WaveFormat = new WaveFormat(44100, 1);
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.StartRecording();
            frequency.Text = float.Parse(DetectFrequency);
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = BitConverter.ToInt16(e.Buffer, i * 2) / 32768f;
            }

            float freq = DetectFrequency(buffer, 44100);

            Dispatcher.Invoke(() => UpdateUI(freq));
        }

        private float DetectFrequency(float[] samples, int sampleRate)
        {
            int crossings = 0;

            for (int i = 1; i < samples.Length; i++)
            {
                if (samples[i - 1] < 0 && samples[i] >= 0)
                    crossings++;
            }

            return crossings * sampleRate / (2f * samples.Length);
        }

        private void UpdateUI(float frequency)
        {
            // כמה רחוק מהתדר הרצוי
            double diff = frequency - targetFrequency;

            // הגבלת טווח תצוגה
            double maxRange = 20; // Hz
            diff = Math.Max(-maxRange, Math.Min(maxRange, diff));

            // המרה למיקום (פיקסלים)
            double pixelsPerHz = 5;
            double offset = diff * pixelsPerHz;

            // הזזת הקו הלבן
            FrequencyLine.Margin = new Thickness(offset, 0, 0, 0);

            // עדכון רוחב האזור הירוק לפי tolerance
            GoodRange.Width = tolerance * pixelsPerHz * 2;
        }
    }
}
