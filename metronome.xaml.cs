using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Threading.Tasks;


namespace MusicSchoolWpf
{
    public partial class metronome : Page
    {
        private int bpm = 60;
        private DispatcherTimer metronomeTimer;
        private bool isPlaying = false;
        private bool pendulumDirection = true; // true = ימין, false = שמאל

        // נקודת המרכז של המטוטלת
        private const double CenterX = 80;
        private const double CenterY = 80;
        private const double ArmLength = 68;
        private const double BobRadius = 10;
        private const double SwingAngle = 35; // מעלות

        public metronome()
        {
            InitializeComponent();

            metronomeTimer = new DispatcherTimer();
            metronomeTimer.Tick += MetronomeTimer_Tick;

            // סנכרון ה-Slider עם ה-BPM ההתחלתי
            BpmSlider.Value = bpm;
            UpdateBpmText();
            SetPendulumPosition(0); // מיקום התחלתי - אמצע
        }

        // ==================== BPM Logic ====================

        private void IncreaseBpm(object sender, RoutedEventArgs e)
        {
            bpm = Math.Min(bpm + 5, 240);
            BpmSlider.Value = bpm;
            UpdateBpmText();
            UpdateTimerInterval();
        }

        private void DecreaseBpm(object sender, RoutedEventArgs e)
        {
            bpm = Math.Max(bpm - 5, 40);
            BpmSlider.Value = bpm;
            UpdateBpmText();
            UpdateTimerInterval();
        }

        private void UpdateBpmText()
        {
            BpmTextBox.Text = bpm.ToString();
        }

        private void UpdateTimerInterval()
        {
            if (bpm > 0)
                metronomeTimer.Interval = TimeSpan.FromMilliseconds(60000.0 / bpm);

        }

        private void BpmSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            bpm = (int)e.NewValue;
            UpdateBpmText();
            UpdateTimerInterval();
        }

        // ==================== Play / Stop ====================

        private void playbpm(object sender, RoutedEventArgs e)
        {
            if (!isPlaying)
            {
                StartMetronome();
            }
            else
            {
                StopMetronome();
            }
        }

        private void StartMetronome()
        {
            if (bpm <= 0) return;

            isPlaying = true;
            pendulumDirection = true;

            // החלף אייקון ל-Stop
            var icon = ((Button)FindName("") ?? null);
            UpdatePlayIcon(true);

            UpdateTimerInterval();
            metronomeTimer.Start();

            // פעם ראשונה מיידית
            Tick();
        }

        private void StopMetronome()
        {
            isPlaying = false;
            metronomeTimer.Stop();

            // החזר מטוטלת למרכז בהדרגה
            AnimatePendulumToCenter();
            UpdatePlayIcon(false);
        }

        // ==================== Timer Tick ====================

        private void MetronomeTimer_Tick(object sender, EventArgs e)
        {
            Tick();
        }

        private void Tick()
        {
            // צליל
            Task.Run(() => Console.Beep(pendulumDirection ? 1200 : 900, 60));

            // אנימציית מטוטלת
            double targetAngle = pendulumDirection ? SwingAngle : -SwingAngle;
            AnimatePendulum(targetAngle);

            pendulumDirection = !pendulumDirection;
        }

        // ==================== Pendulum Animation ====================

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

            // חישוב מיקום יעד
            double rad = targetAngleDegrees * Math.PI / 180;
            double endX = CenterX + ArmLength * Math.Sin(rad);
            double endY = CenterY + ArmLength * Math.Cos(rad);

            var duration = new Duration(TimeSpan.FromMilliseconds(intervalMs * 0.9));
            var easing = new SineEase { EasingMode = EasingMode.EaseInOut };

            // אנימציית X2 של הקו
            var armX = new DoubleAnimation(endX, duration) { EasingFunction = easing };
            var armY = new DoubleAnimation(endY, duration) { EasingFunction = easing };
            PendulumArm.BeginAnimation(System.Windows.Shapes.Line.X2Property, armX);
            PendulumArm.BeginAnimation(System.Windows.Shapes.Line.Y2Property, armY);

            // אנימציית הכדור
            var bobX = new DoubleAnimation(endX - BobRadius, duration) { EasingFunction = easing };
            var bobY = new DoubleAnimation(endY - BobRadius, duration) { EasingFunction = easing };
            PendulumBob.BeginAnimation(Canvas.LeftProperty, bobX);
            PendulumBob.BeginAnimation(Canvas.TopProperty, bobY);
        }

        private void AnimatePendulumToCenter()
        {
            var duration = new Duration(TimeSpan.FromMilliseconds(400));
            var easing = new SineEase { EasingMode = EasingMode.EaseOut };

            var armX = new DoubleAnimation(CenterX, duration) { EasingFunction = easing };
            var armY = new DoubleAnimation(CenterY + ArmLength, duration) { EasingFunction = easing };
            PendulumArm.BeginAnimation(System.Windows.Shapes.Line.X2Property, armX);
            PendulumArm.BeginAnimation(System.Windows.Shapes.Line.Y2Property, armY);

            var bobX = new DoubleAnimation(CenterX - BobRadius, duration) { EasingFunction = easing };
            var bobY = new DoubleAnimation(CenterY + ArmLength - BobRadius, duration) { EasingFunction = easing };
            PendulumBob.BeginAnimation(Canvas.LeftProperty, bobX);
            PendulumBob.BeginAnimation(Canvas.TopProperty, bobY);
        }

        // ==================== Icon Toggle ====================

        private void UpdatePlayIcon(bool playing)
        {
            // מחפש את האייקון בתוך הכפתור
            var playButton = FindVisualChild<Button>(this);
            // אם רוצים לשנות את האייקון - אפשר לעשות זאת ידנית ב-XAML עם Trigger
            // כאן פשוט משנים את ה-ToolTip
        }

        private static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T result) return result;
                var found = FindVisualChild<T>(child);
                if (found != null) return found;
            }
            return null;
        }
    }
}