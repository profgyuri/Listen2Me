namespace Listen2Me.UI
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media.Animation;

    [TemplatePart(Name = "PART_AniVerticalScrollBar", Type = typeof(ScrollBar))]
    [TemplatePart(Name = "PART_AniHorizontalScrollBar", Type = typeof(ScrollBar))]

    public class SmoothScrollViewer : ScrollViewer
    {
        private ScrollBar _aniVerticalScrollBar;
        private ScrollBar _aniHorizontalScrollBar;

        static SmoothScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SmoothScrollViewer), new FrameworkPropertyMetadata(typeof(SmoothScrollViewer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (GetTemplateChild("PART_AniVerticalScrollBar") is ScrollBar aniVScroll)
            {
                _aniVerticalScrollBar = aniVScroll;
            }
            _aniVerticalScrollBar.ValueChanged += VScrollBar_ValueChanged;

            if (GetTemplateChild("PART_AniHorizontalScrollBar") is ScrollBar aniHScroll)
            {
                _aniHorizontalScrollBar = aniHScroll;
            }
            _aniHorizontalScrollBar.ValueChanged += HScrollBar_ValueChanged;

            PreviewMouseWheel += CustomPreviewMouseWheel;
            PreviewKeyDown += AnimatedScrollViewer_PreviewKeyDown;
        }

        private void AnimatedScrollViewer_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            SmoothScrollViewer thisScroller = (SmoothScrollViewer)sender;

            if (thisScroller.CanKeyboardScroll)
            {
                Key keyPressed = e.Key;
                double newVerticalPos = thisScroller.TargetVerticalOffset;
                double newHorizontalPos = thisScroller.TargetHorizontalOffset;
                bool isKeyHandled = false;

                //Vertical Key Strokes code
                if (keyPressed == Key.Down)
                {
                    newVerticalPos = NormalizeScrollPos(thisScroller, newVerticalPos + 16.0, Orientation.Vertical);
                    isKeyHandled = true;
                }
                else if (keyPressed == Key.PageDown)
                {
                    newVerticalPos = NormalizeScrollPos(thisScroller, newVerticalPos + thisScroller.ViewportHeight, Orientation.Vertical);
                    isKeyHandled = true;
                }
                else if (keyPressed == Key.Up)
                {
                    newVerticalPos = NormalizeScrollPos(thisScroller, newVerticalPos - 16.0, Orientation.Vertical);
                    isKeyHandled = true;
                }
                else if (keyPressed == Key.PageUp)
                {
                    newVerticalPos = NormalizeScrollPos(thisScroller, newVerticalPos - thisScroller.ViewportHeight, Orientation.Vertical);
                    isKeyHandled = true;
                }

                if (newVerticalPos != thisScroller.TargetVerticalOffset)
                {
                    thisScroller.TargetVerticalOffset = newVerticalPos;
                }

                //Horizontal Key Strokes Code

                if (keyPressed == Key.Right)
                {
                    newHorizontalPos = NormalizeScrollPos(thisScroller, newHorizontalPos + 16, Orientation.Horizontal);
                    isKeyHandled = true;
                }
                else if (keyPressed == Key.Left)
                {
                    newHorizontalPos = NormalizeScrollPos(thisScroller, newHorizontalPos - 16, Orientation.Horizontal);
                    isKeyHandled = true;
                }

                if (newHorizontalPos != thisScroller.TargetHorizontalOffset)
                {
                    thisScroller.TargetHorizontalOffset = newHorizontalPos;
                }

                e.Handled = isKeyHandled;
            }
        }

        private double NormalizeScrollPos(SmoothScrollViewer thisScroll, double scrollChange, Orientation o)
        {
            double returnValue = scrollChange;

            if (scrollChange < 0)
            {
                returnValue = 0;
            }

            if (o == Orientation.Vertical && scrollChange > thisScroll.ScrollableHeight)
            {
                return thisScroll.ScrollableHeight;
            }
            else if (o == Orientation.Horizontal && scrollChange > thisScroll.ScrollableWidth)
            {
                return thisScroll.ScrollableWidth;
            }

            return returnValue;
        }

        private void CustomPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double mouseWheelChange = e.Delta;

            SmoothScrollViewer thisScroller = (SmoothScrollViewer)sender;
            double newVOffset = thisScroller.TargetVerticalOffset - (mouseWheelChange / 3);
            thisScroller.TargetVerticalOffset = newVOffset < 0 ? 0 : newVOffset > thisScroller.ScrollableHeight ? thisScroller.ScrollableHeight : newVOffset;
            e.Handled = true;
        }

        private void VScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SmoothScrollViewer thisScroller = this;
            double oldTargetVOffset = e.OldValue;
            double newTargetVOffset = e.NewValue;

            if (newTargetVOffset != thisScroller.TargetVerticalOffset)
            {
                double deltaVOffset = Math.Round(newTargetVOffset - oldTargetVOffset, 3);

                thisScroller.TargetVerticalOffset =
                    deltaVOffset == 1 ? oldTargetVOffset + thisScroller.ViewportHeight :
                    deltaVOffset == -1 ? oldTargetVOffset - thisScroller.ViewportHeight :
                    deltaVOffset == 0.1 ? oldTargetVOffset + 16.0 :
                    deltaVOffset == -0.1 ? oldTargetVOffset - 16.0 :
                    newTargetVOffset;
            }
        }

        private void HScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SmoothScrollViewer thisScroller = this;

            double oldTargetHOffset = e.OldValue;
            double newTargetHOffset = e.NewValue;

            if (newTargetHOffset != thisScroller.TargetHorizontalOffset)
            {
                double deltaVOffset = Math.Round(newTargetHOffset - oldTargetHOffset, 3);

                thisScroller.TargetHorizontalOffset =
                    deltaVOffset == 1 ? oldTargetHOffset + thisScroller.ViewportWidth :
                    deltaVOffset == -1 ? oldTargetHOffset - thisScroller.ViewportWidth :
                    deltaVOffset == 0.1 ? oldTargetHOffset + 16.0 :
                    deltaVOffset == -0.1 ? oldTargetHOffset - 16.0 :
                    newTargetHOffset;
            }
        }

        /// <summary>
        /// This is the VerticalOffset that we'd like to animate to.
        /// </summary>
        public double TargetVerticalOffset
        {
            get { return (double)GetValue(TargetVerticalOffsetProperty); }
            set { SetValue(TargetVerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty TargetVerticalOffsetProperty =
            DependencyProperty.Register("TargetVerticalOffset", typeof(double), typeof(SmoothScrollViewer),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnTargetVerticalOffsetChanged)));

        private static void OnTargetVerticalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SmoothScrollViewer thisScroller = (SmoothScrollViewer)d;

            if ((double)e.NewValue != thisScroller._aniVerticalScrollBar.Value)
            {
                thisScroller._aniVerticalScrollBar.Value = (double)e.NewValue;
            }

            thisScroller.AnimateScroller(thisScroller);
        }

        /// <summary>
        /// This is the HorizontalOffset that we'll be animating to.
        /// </summary>
        public double TargetHorizontalOffset
        {
            get { return (double)GetValue(TargetHorizontalOffsetProperty); }
            set { SetValue(TargetHorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty TargetHorizontalOffsetProperty =
            DependencyProperty.Register("TargetHorizontalOffset", typeof(double), typeof(SmoothScrollViewer),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnTargetHorizontalOffsetChanged)));

        private static void OnTargetHorizontalOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SmoothScrollViewer thisScroller = (SmoothScrollViewer)d;

            if ((double)e.NewValue != thisScroller._aniHorizontalScrollBar.Value)
            {
                thisScroller._aniHorizontalScrollBar.Value = (double)e.NewValue;
            }

            thisScroller.AnimateScroller(thisScroller);
        }

        /// <summary>
        /// This is the actual horizontal offset property we're going use as an animation helper.
        /// </summary>
        public double HorizontalScrollOffset
        {
            get { return (double)GetValue(HorizontalScrollOffsetProperty); }
            set { SetValue(HorizontalScrollOffsetProperty, value); }
        }

        public static readonly DependencyProperty HorizontalScrollOffsetProperty =
            DependencyProperty.Register("HorizontalScrollOffset", typeof(double), typeof(SmoothScrollViewer),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnHorizontalScrollOffsetChanged)));

        private static void OnHorizontalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SmoothScrollViewer thisSViewer = (SmoothScrollViewer)d;
            thisSViewer.ScrollToHorizontalOffset((double)e.NewValue);
        }

        /// <summary>
        /// This is the actual VerticalOffset we're going to use as an animation helper.
        /// </summary>
        public double VerticalScrollOffset
        {
            get { return (double)GetValue(VerticalScrollOffsetProperty); }
            set { SetValue(VerticalScrollOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalScrollOffsetProperty =
            DependencyProperty.Register("VerticalScrollOffset", typeof(double), typeof(SmoothScrollViewer),
            new PropertyMetadata(0.0, new PropertyChangedCallback(OnVerticalScrollOffsetChanged)));

        private static void OnVerticalScrollOffsetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SmoothScrollViewer thisSViewer = (SmoothScrollViewer)d;
            thisSViewer.ScrollToVerticalOffset((double)e.NewValue);
        }

        /// <summary>
        /// A property for changing the time it takes to scroll to a new position. 
        /// </summary>
        public TimeSpan ScrollingTime
        {
            get { return (TimeSpan)GetValue(ScrollingTimeProperty); }
            set { SetValue(ScrollingTimeProperty, value); }
        }

        public static readonly DependencyProperty ScrollingTimeProperty =
            DependencyProperty.Register("ScrollingTime", typeof(TimeSpan), typeof(SmoothScrollViewer),
              new PropertyMetadata(new TimeSpan(0, 0, 0, 0, 500)));

        /// <summary>
        /// A property to allow users to describe a custom spline for the scrolling animation.
        /// </summary>
        public KeySpline ScrollingSpline
        {
            get { return (KeySpline)GetValue(ScrollingSplineProperty); }
            set { SetValue(ScrollingSplineProperty, value); }
        }

        public static readonly DependencyProperty ScrollingSplineProperty =
            DependencyProperty.Register("ScrollingSpline", typeof(KeySpline), typeof(SmoothScrollViewer),
              new PropertyMetadata(new KeySpline(0.024, 0.914, 0.717, 1)));

        public static readonly DependencyProperty CanKeyboardScrollProperty =
            DependencyProperty.Register("CanKeyboardScroll", typeof(bool), typeof(SmoothScrollViewer),
                new FrameworkPropertyMetadata(true));

        public bool CanKeyboardScroll
        {
            get { return (bool)GetValue(CanKeyboardScrollProperty); }
            set { SetValue(CanKeyboardScrollProperty, value); }
        }

        private void AnimateScroller(object objectToScroll)
        {
            SmoothScrollViewer thisScrollViewer = objectToScroll as SmoothScrollViewer;

            KeyTime targetKeyTime = thisScrollViewer.ScrollingTime;
            KeySpline targetKeySpline = thisScrollViewer.ScrollingSpline;

            DoubleAnimationUsingKeyFrames animateHScrollKeyFramed = new();
            DoubleAnimationUsingKeyFrames animateVScrollKeyFramed = new();

            SplineDoubleKeyFrame HScrollKey1 = new(thisScrollViewer.TargetHorizontalOffset, targetKeyTime, targetKeySpline);
            SplineDoubleKeyFrame VScrollKey1 = new(thisScrollViewer.TargetVerticalOffset, targetKeyTime, targetKeySpline);
            animateHScrollKeyFramed.KeyFrames.Add(HScrollKey1);
            animateVScrollKeyFramed.KeyFrames.Add(VScrollKey1);

            thisScrollViewer.BeginAnimation(HorizontalScrollOffsetProperty, animateHScrollKeyFramed);
            thisScrollViewer.BeginAnimation(VerticalScrollOffsetProperty, animateVScrollKeyFramed);
        }
    }
}
