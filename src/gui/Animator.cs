using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MochaDBStudio.gui {
  /// <summary>
  /// Animator for animations.
  /// </summary>
  public sealed class Animator {
    #region Fields

    private static int[] dirmap = { 1,5,4,6,2,10,8,9 };
    private static int[] effmap = { 0,0x40000,0x10,0x80000 };

    #endregion

    #region API

    [DllImport("user32.dll")]
    private static extern bool AnimateWindow(IntPtr handle,int ms,int flags);

    #endregion

    /// <summary>
    /// Animate show-hide control.
    /// </summary>
    /// <param name="control">Control to target.</param>
    /// <param name="effect">Animation effect to use.</param>
    /// <param name="ms">Animation effect time in miliseconds.</param>
    /// <param name="angle">Angle.</param>
    public static void ShowHide(Control control,AnimationEffect effect,int ms,int angle) {
      int flags = effmap[(int)effect];
      if(control.Visible) {
        flags |= 0x10000;
        angle += 180;
      } else {
        if(control.TopLevelControl == control) {
          flags |= 0x20000;
        } else if(effect == AnimationEffect.Blend) {
          throw new ArgumentException();
        }
      }
      flags |= dirmap[(angle % 360) / 45];
      AnimateWindow(control.Handle,ms,flags);
    }

    /// <summary>
    /// Show form with fade effect.
    /// </summary>
    /// <param name="form">Form to target.</param>
    /// <param name="delay">Step delay of animation.</param>
    public static void FormFadeShow(Form form,int delay) {
      var task = new Task(() => {
        form.Refresh();
        for(double TOpacity = form.Opacity; TOpacity <= 1; TOpacity += 0.1) {
          form.Opacity = TOpacity;
          Thread.Sleep(delay);
        }
        form.Opacity = 1;
      });
      task.Start();
    }

    /// <summary>
    /// Fade form.
    /// </summary>
    /// <param name="form">Form.</param>
    /// <param name="delay">Step delay of animation.</param>
    public static void FormFadeHide(Form form,int delay) {
      for(double TOpacity = form.Opacity; TOpacity >= 0; TOpacity -= 0.1) {
        form.Opacity = TOpacity;
        Thread.Sleep(delay);
      }
    }
  }

  /// <summary>
  /// Animation effects for animator.
  /// </summary>
  public enum AnimationEffect {
    Roll = 0,
    Slide = 1,
    Center = 2,
    Blend = 3
  }

  /// <summary>
  /// Item list style for MenuItemListView.
  /// </summary>
  public enum ListViewListStyle {
    RelaxList = 0,
    List = 1
  }
}