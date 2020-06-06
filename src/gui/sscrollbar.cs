using System;
using System.Drawing;
using System.Windows.Forms;
using MochaDBStudio.engine;

namespace MochaDBStudio.gui {
    public class sscrollbar:Control {
        #region Fields

        private bool
            isFirstScrollEventVertical = true,
            isFirstScrollEventHorizontal = true,
            inUpdate,
            topBarClicked,
            bottomBarClicked,
            thumbClicked;

        private Rectangle
            clickedBarRectangle,
            thumbRectangle;

        private int
            thumbWidth = 6,
            thumbHeight,
            thumbBottomLimitBottom,
            thumbBottomLimitTop,
            thumbTopLimit,
            thumbPosition,
            trackPosition,
            minimum = 0,
            maximum = 100,
            smallChange = 1,
            largeChange = 10,
            curValue = 0;

        private ScrollOrientation
            orientation = ScrollOrientation.Vertical;

        private System.Windows.Forms.ScrollOrientation
            scrollOrientation = System.Windows.Forms.ScrollOrientation.VerticalScroll;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public sscrollbar() {
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.Selectable |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint,true);

            Width = 10;
            Height = 200;

            SetupScrollBar();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="orientation">Orientation.</param>
        public sscrollbar(ScrollOrientation orientation)
            : this() {
            Orientation = orientation;
        }

        #endregion

        #region Events

        /// <summary>
        /// This happens scrolling.
        /// </summary>
        public event ScrollEventHandler Scroll;
        private void OnScroll(ScrollEventType type,int oldValue,int newValue,System.Windows.Forms.ScrollOrientation orientation) {
            if(Scroll == null) return;

            if(orientation == System.Windows.Forms.ScrollOrientation.HorizontalScroll) {
                if(type != ScrollEventType.EndScroll && isFirstScrollEventHorizontal) {
                    type = ScrollEventType.First;
                } else if(!isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll) {
                    isFirstScrollEventHorizontal = true;
                }
            } else {
                if(type != ScrollEventType.EndScroll && isFirstScrollEventVertical) {
                    type = ScrollEventType.First;
                } else if(!isFirstScrollEventHorizontal && type == ScrollEventType.EndScroll) {
                    isFirstScrollEventVertical = true;
                }
            }

            Scroll(this,new ScrollEventArgs(type,oldValue,newValue,orientation));
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            // no painting here
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.Clear(BackColor);
            DrawScrollBar(e.Graphics,BackColor,ThumbColor,Color.SlateGray);
        }

        protected virtual void DrawScrollBar(Graphics g,Color backColor,Color thumbColor,Color barColor) {
            using(var b = new SolidBrush(backColor)) {
                var thumbRect = new Rectangle(thumbRectangle.X - 1,thumbRectangle.Y - 1,thumbRectangle.Width + 2,thumbRectangle.Height + 2);
                g.FillRectangle(b,thumbRect);
            }

            using(var b = new SolidBrush(thumbColor)) {
                g.FillRectangle(b,thumbRectangle);
            }
        }

        #endregion

        #region General overrides

        protected override void SetBoundsCore(int x,int y,int width,int height,BoundsSpecified specified) {
            base.SetBoundsCore(x,y,width,height,specified);

            if(DesignMode) {
                SetupScrollBar();
            }
        }

        protected override void OnSizeChanged(EventArgs e) {
            base.OnSizeChanged(e);
            SetupScrollBar();
        }

        protected override bool ProcessDialogKey(Keys keyData) {
            var keyUp = Keys.Up;
            var keyDown = Keys.Down;

            if(Orientation == ScrollOrientation.Horizontal) {
                keyUp = Keys.Left;
                keyDown = Keys.Right;
            }

            if(keyData == keyUp) {
                Value -= smallChange;

                return true;
            }

            if(keyData == keyDown) {
                Value += smallChange;

                return true;
            }

            if(keyData == Keys.PageUp) {
                Value = GetValue(false,true);

                return true;
            }

            if(keyData == Keys.PageDown) {
                if(curValue + largeChange > maximum) {
                    Value = maximum;
                } else {
                    Value += largeChange;
                }

                return true;
            }

            if(keyData == Keys.Home) {
                Value = minimum;

                return true;
            }

            if(keyData == Keys.End) {
                Value = maximum;

                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void OnEnabledChanged(EventArgs e) {
            base.OnEnabledChanged(e);
            Invalidate();
        }

        #endregion

        #region Focus overrides

        protected override void OnGotFocus(EventArgs e) {
            Invalidate();

            base.OnGotFocus(e);
        }

        protected override void OnLostFocus(EventArgs e) {
            Invalidate();

            base.OnLostFocus(e);
        }

        protected override void OnEnter(EventArgs e) {
            Invalidate();

            base.OnEnter(e);
        }

        protected override void OnLeave(EventArgs e) {
            Invalidate();

            base.OnLeave(e);
        }

        #endregion

        #region Mouse overrides

        protected override void OnMouseWheel(MouseEventArgs e) {
            base.OnMouseWheel(e);

            int v = e.Delta / 120 * (maximum - minimum) / 10;

            if(Orientation == ScrollOrientation.Vertical) {
                Value -= v;
            } else {
                Value += v;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            if(e.Button == MouseButtons.Left) {
                Invalidate();
            }

            base.OnMouseDown(e);

            Focus();

            if(e.Button == MouseButtons.Left) {

                var mouseLocation = e.Location;

                if(thumbRectangle.Contains(mouseLocation)) {
                    thumbClicked = true;
                    thumbPosition = orientation == ScrollOrientation.Vertical ? mouseLocation.Y - thumbRectangle.Y : mouseLocation.X - thumbRectangle.X;

                    Invalidate(thumbRectangle);
                } else {
                    trackPosition = orientation == ScrollOrientation.Vertical ? mouseLocation.Y : mouseLocation.X;

                    if(trackPosition < (orientation == ScrollOrientation.Vertical ? thumbRectangle.Y : thumbRectangle.X)) {
                        topBarClicked = true;
                    } else {
                        bottomBarClicked = true;
                    }

                    ProgressThumb(true);
                }
            } else if(e.Button == MouseButtons.Right) {
                trackPosition = orientation == ScrollOrientation.Vertical ? e.Y : e.X;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e) {
            base.OnMouseUp(e);

            if(e.Button == MouseButtons.Left) {
                if(thumbClicked) {
                    thumbClicked = false;
                    OnScroll(ScrollEventType.EndScroll,-1,curValue,scrollOrientation);
                } else if(topBarClicked) {
                    topBarClicked = false;
                } else if(bottomBarClicked) {
                    bottomBarClicked = false;
                }

                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e) {
            Invalidate();

            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e) {
            Invalidate();

            base.OnMouseLeave(e);

            ResetScrollStatus();
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            base.OnMouseMove(e);

            if(e.Button == MouseButtons.Left) {
                if(thumbClicked) {
                    int oldScrollValue = curValue;

                    int pos = orientation == ScrollOrientation.Vertical ? e.Location.Y : e.Location.X;
                    int thumbSize = orientation == ScrollOrientation.Vertical ? (pos / Height) / thumbHeight : (pos / Width) / thumbWidth;

                    if(pos <= (thumbTopLimit + thumbPosition)) {
                        ChangeThumbPosition(thumbTopLimit);
                        curValue = minimum;
                        Invalidate();
                    } else if(pos >= (thumbBottomLimitTop + thumbPosition)) {
                        ChangeThumbPosition(thumbBottomLimitTop);
                        curValue = maximum;
                        Invalidate();
                    } else {
                        ChangeThumbPosition(pos - thumbPosition);

                        int pixelRange, thumbPos;

                        if(Orientation == ScrollOrientation.Vertical) {
                            pixelRange = Height - thumbSize;
                            thumbPos = thumbRectangle.Y;
                        } else {
                            pixelRange = Width - thumbSize;
                            thumbPos = thumbRectangle.X;
                        }

                        float perc = 0f;

                        if(pixelRange != 0) {
                            perc = (thumbPos)/(float)pixelRange;
                        }

                        curValue = Convert.ToInt32((perc * (maximum - minimum)) + minimum);
                    }

                    if(oldScrollValue != curValue) {
                        OnScroll(ScrollEventType.ThumbTrack,oldScrollValue,curValue,scrollOrientation);
                        Refresh();
                    }
                }
            } else if(!ClientRectangle.Contains(e.Location)) {
                ResetScrollStatus();
            } else if(e.Button == MouseButtons.None) {
                if(thumbRectangle.Contains(e.Location)) {
                    Invalidate(thumbRectangle);
                } else if(ClientRectangle.Contains(e.Location)) {
                    Invalidate();
                }
            }
        }

        #endregion

        #region Keyboard overrides

        protected override void OnKeyDown(KeyEventArgs e) {
            Invalidate();

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e) {
            Invalidate();

            base.OnKeyUp(e);
        }

        #endregion

        #region Private methods

        private void SetupScrollBar() {
            if(inUpdate) return;

            if(Orientation == ScrollOrientation.Vertical) {
                thumbWidth = Width > 0 ? Width : 10;
                thumbHeight = GetThumbSize();

                clickedBarRectangle = ClientRectangle;
                clickedBarRectangle.Inflate(-1,-1);

                thumbRectangle = new Rectangle(ClientRectangle.X,ClientRectangle.Y,thumbWidth,thumbHeight);

                thumbPosition = thumbRectangle.Height/2;
                thumbBottomLimitBottom = ClientRectangle.Bottom;
                thumbBottomLimitTop = thumbBottomLimitBottom - thumbRectangle.Height;
                thumbTopLimit = ClientRectangle.Y;
            } else {
                thumbHeight = Height > 0 ? Height : 10;
                thumbWidth = GetThumbSize();

                clickedBarRectangle = ClientRectangle;
                clickedBarRectangle.Inflate(-1,-1);

                thumbRectangle = new Rectangle(ClientRectangle.X,ClientRectangle.Y,thumbWidth,thumbHeight);

                thumbPosition = thumbRectangle.Width/2;
                thumbBottomLimitBottom = ClientRectangle.Right;
                thumbBottomLimitTop = thumbBottomLimitBottom - thumbRectangle.Width;
                thumbTopLimit = ClientRectangle.X;
            }

            ChangeThumbPosition(GetThumbPosition());

            Refresh();
        }

        private void ResetScrollStatus() {
            bottomBarClicked = topBarClicked = false;
            Refresh();
        }

        private void ProgressTimerTick(object sender,EventArgs e) {
            ProgressThumb(true);
        }

        private int GetValue(bool smallIncrement,bool up) {
            int newValue;

            if(up) {
                newValue = curValue - (smallIncrement ? smallChange : largeChange);

                if(newValue < minimum) {
                    newValue = minimum;
                }
            } else {
                newValue = curValue + (smallIncrement ? smallChange : largeChange);

                if(newValue > maximum) {
                    newValue = maximum;
                }
            }

            return newValue;
        }

        private int GetThumbPosition() {
            int pixelRange;

            if(thumbHeight == 0 || thumbWidth == 0) {
                return 0;
            }

            int thumbSize = orientation == ScrollOrientation.Vertical ? (thumbPosition / Height) / thumbHeight : (thumbPosition / Width) / thumbWidth;

            if(Orientation == ScrollOrientation.Vertical) {
                pixelRange = Height - thumbSize;
            } else {
                pixelRange = Width - thumbSize;
            }

            int realRange = maximum - minimum;
            float perc = 0f;

            if(realRange != 0) {
                perc = (curValue - (float)minimum) / realRange;
            }

            return Math.Max(thumbTopLimit,Math.Min(thumbBottomLimitTop,Convert.ToInt32((perc*pixelRange))));
        }

        private int GetThumbSize() {
            int trackSize =
                orientation == ScrollOrientation.Vertical ?
                    Height : Width;

            if(maximum == 0 || largeChange == 0) {
                return trackSize;
            }

            float newThumbSize = (largeChange * (float)trackSize) / maximum;

            return Convert.ToInt32(Math.Min(trackSize,Math.Max(newThumbSize,10f)));
        }

        private void ChangeThumbPosition(int position) {
            if(Orientation == ScrollOrientation.Vertical) {
                thumbRectangle.Y = position;
            } else {
                thumbRectangle.X = position;
            }
        }

        private void ProgressThumb(bool enableTimer) {
            int scrollOldValue = curValue;
            var type = ScrollEventType.First;
            int thumbSize, thumbPos;

            if(Orientation == ScrollOrientation.Vertical) {
                thumbPos = thumbRectangle.Y;
                thumbSize = thumbRectangle.Height;
            } else {
                thumbPos = thumbRectangle.X;
                thumbSize = thumbRectangle.Width;
            }

            if((bottomBarClicked && (thumbPos + thumbSize) < trackPosition)) {
                type = ScrollEventType.LargeIncrement;

                curValue = GetValue(false,false);

                if(curValue == maximum) {
                    ChangeThumbPosition(thumbBottomLimitTop);

                    type = ScrollEventType.Last;
                } else {
                    ChangeThumbPosition(Math.Min(thumbBottomLimitTop,GetThumbPosition()));
                }
            } else if((topBarClicked && thumbPos > trackPosition)) {
                type = ScrollEventType.LargeDecrement;

                curValue = GetValue(false,true);

                if(curValue == minimum) {
                    ChangeThumbPosition(thumbTopLimit);

                    type = ScrollEventType.First;
                } else {
                    ChangeThumbPosition(Math.Max(thumbTopLimit,GetThumbPosition()));
                }
            }

            if(scrollOldValue != curValue) {
                OnScroll(type,scrollOldValue,curValue,scrollOrientation);

                Invalidate();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Contains thumb on point.
        /// </summary>
        /// <param name="point">Point to check.</param>
        public bool HitTest(Point point) {
            return thumbRectangle.Contains(point);
        }

        /// <summary>
        /// Update.
        /// </summary>
        public void BeginUpdate() {
            api.SendMessage(Handle,api.WM_SETREDRAW,false,0);
            inUpdate = true;
        }

        /// <summary>
        /// End update.
        /// </summary>
        public void EndUpdate() {
            api.SendMessage(Handle,api.WM_SETREDRAW,true,0);
            inUpdate = false;
            SetupScrollBar();
            Refresh();
        }

        #endregion

        #region Properties

        private Color thumbColor = Color.DodgerBlue;
        public virtual Color ThumbColor {
            get => thumbColor;
            set {
                if(value == thumbColor)
                    return;

                thumbColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Orientation of scrollbar.
        /// </summary>
        public ScrollOrientation Orientation {
            get => orientation;
            set {
                if(value == orientation) {
                    return;
                }

                orientation = value;

                if(value == ScrollOrientation.Vertical) {
                    scrollOrientation = System.Windows.Forms.ScrollOrientation.VerticalScroll;
                } else {
                    scrollOrientation = System.Windows.Forms.ScrollOrientation.HorizontalScroll;
                }

                Size = new Size(Height,Width);
                SetupScrollBar();
            }
        }

        /// <summary>
        /// Minimum value.
        /// </summary>
        public int Minimum {
            get { return minimum; }
            set {
                if(minimum == value || value < 0 || value >= maximum) {
                    return;
                }

                minimum = value;
                if(curValue < value) {
                    curValue = value;
                }

                if(largeChange > (maximum - minimum)) {
                    largeChange = maximum - minimum;
                }

                SetupScrollBar();

                if(curValue < value) {
                    Value = value;
                } else {
                    ChangeThumbPosition(GetThumbPosition());
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Maximum value.
        /// </summary>
        public int Maximum {
            get => maximum;
            set {
                if(value == maximum || value < 1 || value <= minimum) {
                    return;
                }

                maximum = value;
                if(largeChange > (maximum - minimum)) {
                    largeChange = maximum - minimum;
                }

                SetupScrollBar();

                if(curValue > value) {
                    Value = maximum;
                } else {
                    ChangeThumbPosition(GetThumbPosition());
                    Refresh();
                }
            }
        }

        /// <summary>
        /// Range of small value change.
        /// </summary>
        public int SmallChange {
            get => smallChange;
            set {
                if(value == smallChange || value < 1 || value >= largeChange) {
                    return;
                }

                smallChange = value;
                SetupScrollBar();
            }
        }

        /// <summary>
        /// Range of large value change.
        /// </summary>
        public int LargeChange {
            get => largeChange;
            set {
                if(value == largeChange || value < smallChange || value < 2) {
                    return;
                }

                if(value > (maximum - minimum)) {
                    largeChange = maximum - minimum;
                } else {
                    largeChange = value;
                }

                SetupScrollBar();
            }
        }

        /// <summary>
        /// Value.
        /// </summary>
        public int Value {
            get => curValue;

            set {
                if(curValue == value || value < minimum || value > maximum) {
                    return;
                }

                curValue = value;

                ChangeThumbPosition(GetThumbPosition());

                OnScroll(ScrollEventType.ThumbPosition,-1,value,scrollOrientation);

                Refresh();
            }
        }

        #endregion
    }

    /// <summary>
    /// Scroll orientations.
    /// </summary>
    public enum ScrollOrientation {
        Horizontal = 0,
        Vertical = 1
    }
}
