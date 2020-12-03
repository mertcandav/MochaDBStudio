using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using MochaDBStudio.Properties;

namespace MochaDBStudio.gui {
  /// <summary>
  /// Slide menu for MochaDB Studio.
  /// </summary>
  public partial class slidemenu:Panel {
    #region Constructors

    /// <summary>
    /// Constructor.
    /// </summary>
    public slidemenu() {
      Init();
    }

    #endregion

    #region Drawing

    protected override void OnPaintBackground(PaintEventArgs e) {
      base.OnPaintBackground(e);

      if(CurrentItem != null) {
        e.Graphics.FillRectangle(Brushes.DodgerBlue,0,CurrentItem.Location.Y,2,30);
      }

      using(var brush = new SolidBrush(Color.FromArgb(24,24,24)))
        e.Graphics.FillRectangle(brush,0,0,Width,30);


      using(var font = new Font("Arial",11)) {
        e.Graphics.DrawString("Connection List",font,Brushes.White,40,5);
        if(Controls.Count == 1) {
          using(var centerFormat = new StringFormat() {
            LineAlignment = StringAlignment.Center,
            Alignment = StringAlignment.Center
          }) {
            e.Graphics.DrawString("Not exists any connected database :(",
                font,Brushes.Gray,ClientRectangle,centerFormat);
          }
        }
      }
    }

    #endregion

    #region Events

    /// <summary>
    /// This happens any current item changed.
    /// </summary>
    public event EventHandler<EventArgs> CurrentItemChanged;
    protected virtual void OnCurrentItemChanged(object sender,EventArgs e) {
      if(CurrentItem != null)
        ((Control)((sbutton)sender).Tag).BringToFront();

      // Invoke.
      CurrentItemChanged?.Invoke(sender,e);
    }

    #endregion

    #region Animations

    /// <summary>
    /// Open menu now with slide animation.
    /// </summary>
    public void Open() {
      IsOpen = true;
      Animator.ShowHide(this,AnimationEffect.Roll,100,360);
      Visible = true;
      Enabled = true;
    }

    /// <summary>
    /// Close menu now with slide animation.
    /// </summary>
    public void Close() {
      IsOpen = false;
      Animator.ShowHide(this,AnimationEffect.Roll,100,360);
      Visible = false;
      Enabled = false;
    }

    /// <summary>
    /// Toggle state.
    /// </summary>
    public void Toggle() {
      if(IsOpen)
        Close();
      else
        Open();
    }

    /// <summary>
    /// Open menu now with slide animation and asynchrone task.
    /// </summary>
    public void OpenAsync() {
      Thread openThread = new Thread(Open);
      openThread.Start();
    }

    /// <summary>
    /// Close menu now with slide animation and asynchrone task.
    /// </summary>
    public void CloseAsync() {
      Thread closeThread = new Thread(Close);
      closeThread.Start();
    }

    /// <summary>
    /// Toggle state with asynchrone task.
    /// </summary>
    public void ToggleAsync() {
      if(IsOpen)
        CloseAsync();
      else
        OpenAsync();
    }

    #endregion

    #region iconButton

    private void iconButton_Click(object sender,EventArgs e) {
      CloseAsync();
    }

    #endregion

    #region Items

    /// <summary>
    /// Add item.
    /// </summary>
    /// <param name="item">Item to add.</param>
    public void AddItem(sbutton item) {
      item.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Left;
      item.Size = new Size(Width-2,30);
      item.Location = new Point(2,Controls.Count * 30);
      item.BackColor = BackColor;
      item.ForeColor = ForeColor;
      item.MouseEnterColor = Color.Gray;
      item.MouseDownColor = Color.DodgerBlue;
      item.Image = Resources.Database;
      item.Tag2 = "Database";
      item.Click +=item_Click;
      Grid.Controls.Add((cncpanel)item.Tag);
      Controls.Add(item);
      CurrentItem = item;
    }

    #endregion

    #region Item events

    private void item_Click(object sender,EventArgs e) {
      CurrentItem = sender as sbutton;
      Close();
    }

    private void scriptitem_Click(object sender,EventArgs e) {
      CurrentItem = sender as sbutton;
      Close();
    }

    #endregion

    #region Methods

    /// <summary>
    /// Close from current item.
    /// </summary>
    public void close() {
      if(CurrentItem.Tag2 == "Database") {
        var ccpanel = (CurrentItem.Tag as cncpanel);
        ccpanel.Database.Disconnect();
        ccpanel.Dispose();
      }
      var dex = Controls.IndexOf(CurrentItem);
      Controls.RemoveAt(dex);
      CurrentItem.Dispose();
      CurrentItem =
          Controls.Count == 1 ? null :
              dex > Controls.Count-1 ?
              Controls[dex-1] as sbutton : Controls[dex] as sbutton;
    }

    #endregion

    #region Control override

    protected override void OnControlRemoved(ControlEventArgs e) {
      base.OnControlRemoved(e);
      for(int dex = 1; dex < Controls.Count; dex++)
        Controls[dex].Location = new Point(2,dex * 30);
    }

    #endregion

    #region Properties

    private sbutton currentItem = null;
    /// <summary>
    /// Current item.
    /// </summary>
    public sbutton CurrentItem {
      get {
        return currentItem;
      }
      set {
        if(value == currentItem)
          return;

        currentItem = value;
        OnCurrentItemChanged(currentItem,null);
        Invalidate();
      }
    }

    /// <summary>
    /// Open state.
    /// </summary>
    public bool IsOpen { get; private set; }

    /// <summary>
    /// Target grid.
    /// </summary>
    public Panel Grid { get; set; }

    #endregion
  }

  public partial class slidemenu {
    #region Components

    private sbutton
        iconButton;

    #endregion

    /// <summary>
    /// Initialize component.
    /// </summary>
    public void Init() {
      #region Base

      SetStyle(
          ControlStyles.AllPaintingInWmPaint |
          ControlStyles.OptimizedDoubleBuffer |
          ControlStyles.ResizeRedraw |
          ControlStyles.SupportsTransparentBackColor,
          true);
      ForeColor = Color.White;
      BackColor = Color.FromArgb(50,50,50);
      Font = new Font("Arial",10);

      #endregion

      #region iconButton

      iconButton = new sbutton();
      iconButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
      iconButton.Location = new Point(0,0);
      iconButton.Size = new Size(30,30);
      iconButton.BackColor = Color.FromArgb(24,24,24);
      iconButton.MouseEnterColor = Color.Gray;
      iconButton.MouseDownColor = Color.DodgerBlue;
      iconButton.Image = Resources.MochaDB_Logo.ToBitmap();
      iconButton.Click +=iconButton_Click;
      Controls.Add(iconButton);

      #endregion
    }
  }
}
