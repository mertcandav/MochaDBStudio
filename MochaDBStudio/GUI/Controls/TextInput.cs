using System;
using System.Drawing;
using System.Windows.Forms;

namespace MochaDBStudio.GUI.Controls {
    /// <summary>
    /// TextInput for text inputs.
    /// </summary>
    public sealed class TextInput:Panel {
        #region Fields

        private TextBox input;
        private Color borderColor;

        #endregion

        #region Constructors

        /// <summary>
        /// Create new TextInput.
        /// </summary>
        public TextInput() {
            input = new TextBox();
            BackColor = Color.White;
            ForeColor = Color.Black;
            input.BorderStyle = BorderStyle.None;
            input.Font = Font;
            input.SizeChanged+=İnput_SizeChanged;
            input.TextChanged+=İnput_TextChanged;
            AdapdateSize();
            input.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
            Controls.Add(input);
            borderColor = Color.DodgerBlue;
        }

        #endregion

        #region Events

        public new event EventHandler<EventArgs> TextChanged;

        #endregion

        #region Drawing

        protected override void OnPaint(PaintEventArgs e) {
            if(BorderColor != Color.Transparent && input.CanSelect)
                using(Pen BorderPen = new Pen(BorderColor,2))
                    e.Graphics.DrawRectangle(BorderPen,1,1,ClientSize.Width-2,ClientSize.Height-2);
        }

        #endregion

        #region input

        private void İnput_TextChanged(object sender,EventArgs e) {
            TextChanged?.Invoke(sender,e);
        }

        private void İnput_SizeChanged(object sender,EventArgs e) {
            AdapdateSize();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adaptade size by input.
        /// </summary>
        public void AdapdateSize() {
            Height = input.Height + 10;
            input.Location = new Point(5,5);
        }

        /// <summary>
        /// Clear text.
        /// </summary>
        public void Clear() =>
            input.Clear();

        #endregion

        #region Properties

        /// <summary>
        /// Password char.
        /// </summary>
        public char PasswordChar {
            get => input.PasswordChar;
            set => input.PasswordChar = value;
        }

        /// <summary>
        /// Placeholder.
        /// </summary>
        public string Placeholder {
            get => input.PlaceholderText;
            set => input.PlaceholderText = value;
        }

        /// <summary>
        /// Color or border. Set transparent for disable.
        /// </summary>
        public Color BorderColor {
            get =>
                borderColor;
            set {
                if(value == borderColor)
                    return;

                borderColor = value;
                Invalidate();
            }
        }

        /// <summary>
        /// Read only.
        /// </summary>
        public bool ReadOnly {
            get => input.ReadOnly;
            set => input.ReadOnly =value;
        }

        public override string Text { 
            get => input.Text;
            set => input.Text=value; }
        public override Color BackColor {
            get => base.BackColor;
            set {
                base.BackColor = value;
                input.BackColor = value;
            }
        }
        public override Color ForeColor {
            get => base.ForeColor;
            set {
                base.ForeColor = value;
                input.ForeColor = value;
            }
        }
        public override Font Font {
            get => base.Font;
            set {
                base.Font = value;
                input.Font = value;
            }
        }

        #endregion
    }
}