using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading.Tasks;
using MaterialDesignThemes.Wpf; // וודא שהספרייה מותקנת
using System.Windows.Input;

namespace MusicSchoolWpf
{
    public partial class metronome : Page
    {
        private int bpm = 60;
        private DispatcherTimer metronomeTimer;
        private bool isPlaying = false;
        private bool pendulumDirection = true; // true = ימין, false = שמאל

        private const double CenterX = 80;
        private const double CenterY = 80;
        private const double ArmLength = 68;
        private const double BobRadius = 10;
        private const double SwingAngle = 35;

        public metronome()
        {
            InitializeComponent();

            metronomeTimer = new DispatcherTimer();
            metronomeTimer.Tick += MetronomeTimer_Tick;

            BpmSlider.Value = bpm;
            UpdateBpmText();
            SetPendulumPosition(0);
        }

        private void IncreaseBpm(object sender, RoutedEventArgs e)
        {
            bpm = Math.Min(bpm + 1, 240); // העלאה ב-1 לדיוק, אפשר להשאיר 5
            BpmSlider.Value = bpm;
        }

        private void DecreaseBpm(object sender, RoutedEventArgs e)
        {
            bpm = Math.Max(bpm - 1, 40);
            BpmSlider.Value = bpm;
        }

        private void UpdateBpmText()
        {
            if (BpmTextBox != null)
                BpmTextBox.Text = bpm.ToString();
        }

        private void UpdateTimerInterval()
        {
            if (bpm > 0)
            {
                // התיקון הקריטי: 60,000 חלקי BPM
                double intervalMs = 60000.0 / bpm;
                metronomeTimer.Interval = TimeSpan.FromMilliseconds(intervalMs);
            }
        }

        private void BpmSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bpm = (int)e.NewValue;
            UpdateBpmText();
            if (isPlaying) UpdateTimerInterval();
        }

        private void playbpm(object sender, RoutedEventArgs e)
        {
            if (!isPlaying) StartMetronome();
            else StopMetronome();
        }

        private void StartMetronome()
        {
            isPlaying = true;
            UpdatePlayIcon(true);
            UpdateTimerInterval();
            metronomeTimer.Start();
            Tick(); // הפעלה מיידית של הפעימה הראשונה
        }

        private void StopMetronome()
        {
            isPlaying = false;
            metronomeTimer.Stop();
            UpdatePlayIcon(false);
            AnimatePendulumToCenter();
        }

        private void MetronomeTimer_Tick(object sender, EventArgs e)
        {
            Tick();
        }

        private void Tick()
        {
            // השמעת צליל (בנפרד כדי לא לתקוע את ה-UI)
            Task.Run(() => Console.Beep(pendulumDirection ? 1000 : 800, 50));

            double targetAngle = pendulumDirection ? SwingAngle : -SwingAngle;
            AnimatePendulum(targetAngle);

            pendulumDirection = !pendulumDirection;
        }

        private void SetPendulumPosition(double angleDegrees)
        {
            double rad = angleDegrees * Math.PI / 180;
            double endX = CenterX + ArmLength * Math.Sin(rad);
            double endY = CenterY + ArmLength * Math.Cos(rad);

            PendulumArm.X2 = endX;
            PendulumArm.Y2 = endY;

            Canvas.SetLeft(PendulumBob, endX - BobRadius);
            Canvas.SetTop(PendulumBob, endY - BobRadius);
        }

        private void AnimatePendulum(double targetAngleDegrees)
        {
            double intervalMs = 60000.0 / bpm;

            double rad = targetAngleDegrees * Math.PI / 180;
            double endX = CenterX + ArmLength * Math.Sin(rad);
            double endY = CenterY + ArmLength * Math.Cos(rad);

            // האנימציה צריכה לקחת בדיוק את זמן ה-Interval
            var duration = new Duration(TimeSpan.FromMilliseconds(intervalMs));
            var easing = new SineEase { EasingMode = EasingMode.EaseInOut };

            var armX = new DoubleAnimation(endX, duration) { EasingFunction = easing };
            var armY = new DoubleAnimation(endY, duration) { EasingFunction = easing };
            PendulumArm.BeginAnimation(System.Windows.Shapes.Line.X2Property, armX);
            PendulumArm.BeginAnimation(System.Windows.Shapes.Line.Y2Property, armY);

            var bobX = new DoubleAnimation(endX - BobRadius, duration) { EasingFunction = easing };
            var bobY = new DoubleAnimation(endY - BobRadius, duration) { EasingFunction = easing };
            PendulumBob.BeginAnimation(Canvas.LeftProperty, bobX);
            PendulumBob.BeginAnimation(Canvas.TopProperty, bobY);
        }

        private void AnimatePendulumToCenter()
        {
            var duration = new Duration(TimeSpan.FromMilliseconds(300));
            var easing = new SineEase { EasingMode = EasingMode.EaseOut };

            PendulumArm.BeginAnimation(System.Windows.Shapes.Line.X2Property, new DoubleAnimation(CenterX, duration) { EasingFunction = easing });
            PendulumArm.BeginAnimation(System.Windows.Shapes.Line.Y2Property, new DoubleAnimation(CenterY + ArmLength, duration) { EasingFunction = easing });
            PendulumBob.BeginAnimation(Canvas.LeftProperty, new DoubleAnimation(CenterX - BobRadius, duration) { EasingFunction = easing });
            PendulumBob.BeginAnimation(Canvas.TopProperty, new DoubleAnimation(CenterY + ArmLength - BobRadius, duration) { EasingFunction = easing });
        }

        private void UpdatePlayIcon(bool playing)
        {
            // הגישה הישירה והבטוחה ביותר
            ButtonIcon.Kind = playing ? PackIconKind.Stop : PackIconKind.Play;
            PlayPauseButton.ToolTip = playing ? "Stop" : "Play";
        }
        private void BpmTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // מאפשר הקלדה רק של מספרים
            e.Handled = !int.TryParse(e.Text, out _);
        }

        private void BpmTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyBpmFromTextBox();
                Keyboard.ClearFocus();
            }
        }

        private void BpmTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ApplyBpmFromTextBox();
        }

        private void ApplyBpmFromTextBox()
        {
            if (int.TryParse(BpmTextBox.Text, out int newBpm))
            {
                if (newBpm < 40)
                    newBpm = 40;

                if (newBpm > 240)
                    newBpm = 240;

                bpm = newBpm;
                BpmSlider.Value = bpm;
                UpdateBpmText();

                if (isPlaying)
                    UpdateTimerInterval();
            }
            else
            {
                UpdateBpmText();
            }
        }
    }
}