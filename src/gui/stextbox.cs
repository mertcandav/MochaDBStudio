using System;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
  /// <summary>
  /// TextInput for text inputs.
  /// </summary>
  public sealed class stextbox:Panel {
    #region Fields

    private TextBox input;
    private Color borderColor;

    #endregion

    #region Constructors

    /// <summary>
    /// Create new TextInput.
    /// </summary>
    public stextbox() {
      input = new TextBox();
      BackColor = Color.White;
      ForeColor = Color.Black;
      Cursor = Cursors.IBeam;
      input.BorderStyle = BorderStyle.None;
      input.Font = Font;
      input.SizeChanged+=Input_SizeChanged;
      input.TextChanged+=Input_TextChanged;
      input.LostFocus+=Input_LostFocus;
      AdapdateSize();
      input.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
      Controls.Add(input);
      borderColor = Color.DodgerBlue;
      input.Hide();
    }

    #endregion

    #region Events

    /// <summary>
    /// This happends after text changed.
    /// </summary>
    public new event EventHandler<EventArgs> TextChanged;

    #endregion

    #region Drawing

    protected override void OnPaint(PaintEventArgs e) {
      e.Graphics.TextRenderingHint=TextRenderingHint.ClearTypeGridFit;

      if(BorderColor != Color.Transparent)
        using(Pen BorderPen = new Pen(BorderColor,2))
          e.Graphics.DrawRectangle(BorderPen,1,1,ClientSize.Width-2,ClientSize.Height-2);

      e.Graphics.DrawString(Placeholder,Font,Brushes.Gray,new Rectangle(input.Location,input.Size));
    }

    #endregion

    #region input

    private void Input_TextChanged(object sender,EventArgs e) {
      TextChanged?.Invoke(sender,e);
    }

    private void Input_SizeChanged(object sender,EventArgs e) {
      AdapdateSize();
    }

    private void Input_LostFocus(object sender,EventArgs e) {
      input.Visible=!string.IsNullOrEmpty(input.Text);
    }

    #endregion

    #region Overrides

    protected override void OnClick(EventArgs e) {
      base.OnClick(e);
      input.Show();
      input.Focus();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Adaptade size by input.
    /// </summary>
    public void AdapdateSize() {
      Height = input.Height + 10;
      Width = input.Width + 10;
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
    public string Placeholder { get; set; }

    /// <summary>
    /// Multiline.
    /// </summary>
    public bool Multiline { get => input.Multiline; set => input.Multiline = value; }

    /// <summary>
    /// INput box size.
    /// </summary>
    public Size InputSize { get => input.Size; set => input.Size = value; }

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
    public bool Readonly {
      get => input.ReadOnly;
      set => input.ReadOnly =value;
    }

    public override string Text {
      get => input.Text;
      set {
        input.Text=value;
        input.Visible=!string.IsNullOrEmpty(value);
      }
    }
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