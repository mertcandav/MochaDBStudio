using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Windows.Forms;
using MochaDB;
using MochaDBStudio.Properties;

namespace MochaDBStudio.GUI.Controls {
    /// <summary>
    /// Terminal sayfası.
    /// </summary>
    public sealed partial class Terminal:Page {
        #region Fields

        private const int WM_CHAR = 0x102;

        public VScrollBar VerScroll;
        public HScrollBar HorScroll;

        private readonly Font BaseFont = new Font("Consolas",13,FontStyle.Bold,GraphicsUnit.Pixel);
        private readonly Font InputFont = new Font("Consolas",13,FontStyle.Regular,GraphicsUnit.Pixel);


        private List<TerminalInput> inputs;

        private string input;
        private int caretIndex;
        private bool useUserInput;

        #endregion

        #region Cosntructors

        /// <summary>
        /// Create new Terminal.
        /// </summary>
        public Terminal() {
            Text = "Terminal";
            Image = Resources.Terminal;
            BackColor = Color.FromArgb(17,17,17);
            Dock = DockStyle.Fill;
            Base="[MochaDB_Studio]";

            inputs = new List<TerminalInput>();
            input = string.Empty;
            caretIndex = 0;
            useUserInput=true;
            
            VerScroll = new VScrollBar();
            VerScroll.Dock = DockStyle.Right;
            VerScroll.Width = 13;
            VerScroll.Visible = false;
            VerScroll.Maximum = 0;
            VerScroll.ValueChanged += VerScroll_ValueChanged;

            Controls.Add(VerScroll);

            HorScroll = new HScrollBar();
            HorScroll.Dock = DockStyle.Bottom;
            HorScroll.Height = 13;
            HorScroll.Visible = false;
            HorScroll.Maximum = 0;
            HorScroll.ValueChanged += HorScroll_ValueChanged;
            
            Controls.Add(HorScroll);
        }

        #endregion

        #region Events

        /// <summary>
        /// This happens before input process.
        /// </summary>
        public event EventHandler<TerminalInputProcessEventArgs> InputProcessing;
        private void OnInputProcessing(object sender,TerminalInputProcessEventArgs e) {
            //Invoke.
            InputProcessing?.Invoke(sender,e);
        }

        #endregion

        #region Drawing

        protected override void OnPaintBackground(PaintEventArgs e) {
            //Background.
            using(SolidBrush BackBrush = new SolidBrush(BackColor)) {
                e.Graphics.FillRectangle(BackBrush,ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e) {
            e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

            //Inputs.
            for(int index = 0; index < InputCount; index++) {
                DrawInput(e.Graphics,index);
            }

            if(UseUserInput) {
                //Draw InputLine.
                DrawBase(e.Graphics,InputLineRect.Y - VerScroll.Value);
                e.Graphics.DrawString(Input,InputFont,Brushes.Gainsboro,
                    CurrentTitleWidth - HorScroll.Value,InputLineRect.Y - VerScroll.Value);

                //Caret.
                DrawCaret(e.Graphics);
            }
        }

        /// <summary>
        /// Draw caret.
        /// </summary>
        /// <param name="graph">Graphics object to use.</param>
        private void DrawCaret(Graphics graph) {
            Rectangle rect = CaretRect;
            graph.FillRectangle(Brushes.White,rect.X - HorScroll.Value,rect.Y - VerScroll.Value,rect.Width,rect.Height);
        }

        /// <summary>
        /// Draw base.
        /// </summary>
        /// <param name="graph">Graphics object to use.</param>
        /// <param name="y">Y coordinate to use.</param>
        private void DrawBase(Graphics graph,int y) {
            using(SolidBrush ForeBrush = new SolidBrush(CurrentTitle.ForeColor)) {
                graph.DrawString(CurrentTitle.Base,BaseFont,ForeBrush,0 - HorScroll.Value,y);
            }
        }

        /// <summary>
        /// Draw base.
        /// </summary>
        /// <param name="graph">Graphics object to use.</param>
        /// <param name="input">Input object to draw.</param>
        /// <param name="y">Y coordinate to use.</param>
        private void DrawBase(Graphics graph,TerminalInput input,int y) {
            using(SolidBrush ForeBrush = new SolidBrush(input.BaseForeColor)) {
                graph.DrawString(input.Base,BaseFont,ForeBrush,0 - HorScroll.Value,y);
            }
        }

        /// <summary>
        /// Draw input.
        /// </summary>
        /// <param name="graph">Graphics object to use.</param>
        /// <param name="index">Index of input.</param>
        private void DrawInput(Graphics graph,int index) {
            TerminalInput input = this[index];

            Rectangle rect = GetInputRect(index);
            rect.Location = new Point(rect.X - HorScroll.Value,rect.Y - VerScroll.Value);

            //Base.
            DrawBase(graph,input,rect.Y);

            //Input.
            using(SolidBrush ForeBrush = new SolidBrush(input.ForeColor)) {
                graph.DrawString(input,InputFont,ForeBrush,rect);
            }
        }

        #endregion

        #region Scrolls Events

        private void HorScroll_ValueChanged(object sender,EventArgs e) {
            Invalidate(ValidateRect);
        }

        private void VerScroll_ValueChanged(object sender,EventArgs e) {
            Invalidate(ValidateRect);
        }

        #endregion

        #region Mouse

        protected override void OnMouseClick(MouseEventArgs e) {
            Focus();

            if(e.Button == MouseButtons.Right) {
                Input = Clipboard.GetText();
                CaretIndex = Input.Length;
            }

            base.OnMouseClick(e);
        }

        #endregion

        #region Keyboard

        protected override void OnPreviewKeyDown(PreviewKeyDownEventArgs e) {
            if(UseUserInput && !ProcessKey(e.KeyCode)) {
                if(e.KeyCode == Keys.Enter) {
                    ProcessCommand();
                    Input = "";
                    CaretIndex = 0;
                } else if(e.KeyCode == Keys.Back) {
                    if(!string.IsNullOrEmpty(Input)) {
                        CaretIndex--;
                        Input = Input.Remove(CaretIndex,1);
                    }
                } else if(e.KeyCode == Keys.Delete) {
                    if(!string.IsNullOrEmpty(Input)) {
                        if(CaretIndex < Input.Length)
                            Input = Input.Remove(CaretIndex,1);
                    }
                } else if(e.KeyCode == Keys.Right) {
                    CaretIndex++;
                } else if(e.KeyCode == Keys.Left) {
                    CaretIndex--;
                } else if(e.KeyCode == Keys.Up) {
                    int Value = HistoryIndex;
                    if(Value - 1 >= 0) {
                        HistoryIndex--;
                        CaretIndex = 0;
                        Input = History[HistoryIndex];
                        CaretIndex = Input.Length;
                    }
                } else if(e.KeyCode == Keys.Down) {
                    if(HistoryIndex + 1 <= History.Count - 1) {
                        HistoryIndex++;
                        CaretIndex = 0;
                        Input = History[HistoryIndex];
                        CaretIndex = Input.Length;
                    }
                } else if(e.KeyCode == Keys.Home) {
                    CaretIndex = 0;
                } else if(e.KeyCode == Keys.End) {
                    CaretIndex = Input.Length;
                }
            }

            base.OnPreviewKeyDown(e);
        }

        protected override bool IsInputKey(Keys keyData) {
            return true;
        }

        protected override bool ProcessKeyMessage(ref Message m) {
            if(UseUserInput && m.Msg == WM_CHAR) {
                int CharValue = m.WParam.ToInt32();

                if(!IsBannedChar(CharValue)) {
                    Input = Input.Insert(CaretIndex,((char)CharValue).ToString());
                    CaretIndex++;
                }
            }

            return base.ProcessKeyMessage(ref m);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear all inputs.
        /// </summary>
        public void Clear() {
            inputs.Clear();
            Invalidate();
            AdapteScrolls();
        }

        /// <summary>
        /// Set base string.
        /// </summary>
        /// <param name="value">Value to set base string.</param>
        public void SetBase(string value) {
            Base="[" + value + "]";
        }

        /// <summary>
        /// Add input.
        /// </summary>
        /// <param name="input">Input to add.</param>
        /// <param name="historyInput">this input is history input. Add input in history if true.</param>
        public void AddInput(string input,bool historyInput) {
            inputs.Add(new TerminalInput(CurrentTitle.Base,input,BaseFont,InputFont));
            if(historyInput && !string.IsNullOrEmpty(input)) {
                History.RemoveAt(History.Count - 1);
                History.Add(input);
                History.Add("");
                HistoryIndex = History.Count - 1;
            }
            Invalidate(ClientRectangle);
            AdapteScrolls();
            ScrollCaret();
        }

        /// <summary>
        /// Add input.
        /// </summary>
        /// <param name="input">Input object to add.</param>
        /// <param name="historyInput">this input is history input. Add input in history if true.</param>
        public void AddInput(TerminalInput input,bool historyInput) {
            inputs.Add(input);
            if(historyInput && !string.IsNullOrEmpty(input)) {
                History.RemoveAt(History.Count - 1);
                History.Add(input);
                History.Add("");
                HistoryIndex = History.Count - 1;
            }
            Invalidate(ClientRectangle);
            AdapteScrolls();
            ScrollCaret();
        }

        /// <summary>
        /// Add terminal based input.
        /// </summary>
        /// <param name="content">Echo content.</param>
        public void TerminalEcho(string content) {
            AddInput(new TerminalInput(TerminalTitle.Base,content,TerminalTitle.BaseForeColor,Color.Khaki,BaseFont,InputFont),false);
            ScrollCaret();
        }

        /// <summary>
        /// Add terminal based error input.
        /// </summary>
        /// <param name="error">Error message.</param>
        public void TerminalErrorEcho(string error) {
            AddInput(new TerminalInput(TerminalTitle.Base,error,TerminalTitle.BaseForeColor,Color.Red,BaseFont,InputFont),false);
            ScrollCaret();
        }

        /// <summary>
        /// Return input rectangle.
        /// </summary>
        /// <param name="index">Index of rectangle.</param>
        public Rectangle GetInputRect(int index) {
            TerminalInput input = this[index];
            int Y = 0;
            for(int inputIndex = 0; inputIndex < index; inputIndex++)
                Y += this[inputIndex].Height;
            Rectangle Rect = new Rectangle(input.BaseWidth,Y,ValidateRect.Width,input.Height);
            return Rect;
        }

        /// <summary>
        /// return size of string.
        /// </summary>
        /// <param name="value">String.</param>
        /// <param name="font">Font for measuring.</param>
        public Size MeasureString(string value,Font font) {
            if(IsDisposed)
                return Size.Empty;

            using(Graphics graph = CreateGraphics()) {
                Size MSize = graph.MeasureString(value + ".",font).ToSize();
                return new Size(MSize.Width - graph.MeasureString(".",font).ToSize().Width + 2,
                    font.Height * value.Split('\n').Length);
            }
        }

        /// <summary>
        /// Set scrollbars.
        /// </summary>
        public void AdapteScrolls() {
            int vertical = 0, horizontal = 0;

            for(int index = 0; index < InputCount; index++) {
                Rectangle rect = GetInputRect(index);
                Size ssize = new Size(rect.Width,rect.Y + rect.Height);
                if(ssize.Width > horizontal)
                    horizontal = ssize.Width;
                if(ssize.Height > vertical)
                    vertical = ssize.Height;
            }

            Rectangle inputLineRect = InputLineRect;
            Size size = new Size(inputLineRect.Width,inputLineRect.Y + inputLineRect.Height);
            if(size.Width > horizontal)
                horizontal = size.Width;
            if(size.Height > vertical)
                vertical = size.Height;

            vertical += 5000;
            VerScroll.Maximum = vertical;
            HorScroll.Maximum = horizontal;

            if(VerScroll.Maximum > Height - 150)
                VerScroll.Visible = true;
            else
                VerScroll.Visible = false;

            if(HorScroll.Maximum > Width - 150)
                HorScroll.Visible = true;
            else
                HorScroll.Visible = false;
        }

        /// <summary>
        /// Scroll to caret.
        /// </summary>
        public void ScrollCaret() {
            if(!HorScroll.Visible && !VerScroll.Visible) {
                HorScroll.Value = 0;
                VerScroll.Value = 0;
            }

            Rectangle rect = CaretRect;
            int
                vertical = rect.Y + rect.Height,
                horizontal = rect.X + rect.Width;

            if(vertical <= Height) {
                VerScroll.Value = 0;
            } else {
                if(VerScroll.Visible) {
                    int verticalvalue = VerScroll.Maximum - 150 - 5000;

                    if(verticalvalue > 0) {
                        VerScroll.Value = verticalvalue;
                    }
                }
            }

            if(horizontal <= HorScroll.Maximum) {
                HorScroll.Value = 0;
            } else {
                if(HorScroll.Visible) {
                    int horizontalvalue = HorScroll.Maximum - 150;

                    if(horizontalvalue > 0) {
                        HorScroll.Value = horizontalvalue;
                    }
                }
            }
        }

        /// <summary>
        /// Get ban state of char.
        /// </summary>
        /// <param name="charValue">ASCII value of char.</param>
        public bool IsBannedChar(int charValue) {
            /*if(((char)charValue) == '')
                return true;*/
            if(charValue == 1)
                return true;
            else if(charValue == 19)
                return true;
            else if(charValue == 8)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Proces key value.
        /// </summary>
        /// <param name="msg">Message.</param>
        public bool ProcessKey(ref Message msg) {
            return ProcessKey((Keys)msg.WParam.ToInt32());
        }

        /// <summary>
        /// Process key value.
        /// </summary>
        /// <param name="keyData">Key.</param>
        public bool ProcessKey(Keys keyData) {
            if(keyData == Keys.Enter) {
                return false;
            } else if(keyData == Keys.ShiftKey) {
                return false;
            } else if(keyData == Keys.Shift) {
                return false;
            } else if(keyData == Keys.Alt) {
                return false;
            } else if(keyData == Keys.Back) {
                return false;
            } else if(keyData == Keys.Delete) {
                return false;
            } else if(keyData == Keys.Control) {
                return false;
            } else if(keyData == Keys.ControlKey) {
                return false;
            } else if(keyData == Keys.Escape) {
                return false;
            } else if(keyData == Keys.End) {
                return false;
            } else if(keyData == Keys.Home) {
                return false;
            } else if(keyData == Keys.Up) {
                return false;
            } else if(keyData == Keys.Left) {
                return false;
            } else if(keyData == Keys.Right) {
                return false;
            } else if(keyData == Keys.Down) {
                return false;
            } else {
                return true;
            }
        }

        /// <summary>
        /// Return char.
        /// </summary>
        /// <param name="msg">Message.</param>
        public char GetChar(ref Message msg) {
            return (char)msg.WParam.ToInt32();
        }

        /// <summary>
        /// Return char.
        /// </summary>
        /// <param name="keyData">Key.</param>
        public char GetChar(Keys keyData) {
            return (char)keyData;
        }

        #region Commands

        public void cnc(string arg) {
            if(arg == "cnc") {
                TerminalEcho("cnc: Namespace of connection commands.\n\nParameters;\n" +
                    "\n-c Database path: Connect.\n-c Database path,Password: Connect.\n-d: Disconnect.");
            } else if(arg.Contains("cnc -c ")) {
                string rInput = arg.Substring(7).Trim();
                string[] cmmd = rInput.Split(',');
                string password = "";
                if(cmmd.Length == 2)
                    password = cmmd[1];
                try {
                    DB = new MochaDatabase(cmmd[0],password);
                } catch(Exception Excep) {
                    TerminalErrorEcho(Excep.Message);
                }
            } else if(arg == "cnc -d") {
                DB = null;
            } else {
                AddInput(new TerminalInput(CurrentTitle.Base,"Command is not found!",Color.Red,BaseFont,InputFont),false);
            }
        }

        public void mochaq(string arg) {
            if(DB == null) {
                AddInput(new TerminalInput(TerminalTitle.Base,"Command is not found!",TerminalTitle.BaseForeColor,
                    Color.Red,BaseFont,InputFont),false);
                return;
            }

            try {
                DB.Query.Run(arg);
            } catch {
                try {
                    string rData = DB.Query.GetRun(arg).ToString();
                    TerminalEcho(rData);
                } catch(Exception excep) {
                    TerminalErrorEcho(excep.Message);
                }
            }
        }

        public void release(string arg) {
            if(arg == "release") {
                TerminalEcho("release: Namespace of release commands.\n\nParameters;\n" +
                    "-version: Show release version.\n-distro: Show release distribution.");
            } else if(arg == "release -version") {
                TerminalEcho(Resources.Version);
            } else if(arg == "release -distro") {
                TerminalEcho(Resources.Distribution);
            } else {
                AddInput(new TerminalInput(CurrentTitle.Base,"Command is not found!",Color.Red,BaseFont,InputFont),false);
            }
        }

        #endregion

        /// <summary>
        /// Process command.
        /// </summary>
        public void ProcessCommand() {
            string tInput = Input.TrimStart().TrimEnd();

            var result =
                from value in BannedCommandNamespaces
                where tInput.StartsWith(value)
                select value;

            if(result.Count() > 0) {
                TerminalErrorEcho("Can't use this command namespace in this terminal!");
                return;
            }

            TerminalInputProcessEventArgs args = new TerminalInputProcessEventArgs(tInput);
            OnInputProcessing(this,args);
            if(args.Cancel)
                return;

            AddInput(new TerminalInput(CurrentTitle.Base,Input,BaseFont,InputFont),true);

            if(!string.IsNullOrEmpty(tInput)) {
                if(tInput == "clear") {
                    Clear();
                    return;
                } else if(tInput == "close") {
                    PageView parent = Parent as PageView;
                    parent.Remove(this);
                } else if(tInput.StartsWith("release")) {
                    release(tInput);
                } else if(tInput.StartsWith("cnc")) {
                    cnc(tInput);
                } else if(DB != null) {
                    mochaq(tInput);
                } else {
                    AddInput(new TerminalInput(CurrentTitle.Base,"Command is not found!",Color.Red,BaseFont,InputFont),false);
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Current base.
        /// </summary>
        public string Base { get; private set; }

        /// <summary>
        /// Use user input write and enter.
        /// </summary>
        public bool UseUserInput {
            get =>
                useUserInput;
            set {
                if(value==useUserInput)
                    return;

                if(!value) {
                    Input=string.Empty;
                }

                useUserInput=value;
                Invalidate();
            }
        }

        /// <summary>
        /// Entered command history.
        /// </summary>
        public List<string> History { get; private set; }
            = new List<string>(new string[] { "" });

        /// <summary>
        /// Banned command namespaces.
        /// </summary>
        public string[] BannedCommandNamespaces { get; set; }

        /// <summary>
        /// Index of selected index.
        /// </summary>
        public int HistoryIndex { get; private set; }

        /// <summary>
        /// Targetted database.
        /// </summary>
        public MochaDatabase DB { get; internal set; }

        /// <summary>
        /// Count of input.
        /// </summary>
        public int InputCount =>
            inputs.Count;

        /// <summary>
        /// Girdiler.
        /// </summary>
        public IList<TerminalInput> Inputs =>
            inputs;

        /// <summary>
        /// Return input by index.
        /// </summary>
        /// <param name="index">Index of input.</param>
        public TerminalInput this[int index] =>
            inputs[index];

        /// <summary>
        /// Current title.
        /// </summary>
        public TerminalInput CurrentTitle =>
            DB switch
            {
                null => new TerminalInput(Base,"",Color.GreenYellow,BaseFont,InputFont),
                _ => new TerminalInput("[" + DB.Name + "]","",Color.GreenYellow,BaseFont,InputFont)
            };

        /// <summary>
        /// Base title of Terminal.
        /// </summary>
        public TerminalInput TerminalTitle =>
            new TerminalInput("[Terminal~]",string.Empty,Color.FromArgb(255,110,110),Color.Khaki,BaseFont,InputFont);

        /// <summary>
        /// Rectangle of invalidate.
        /// </summary>
        public Rectangle ValidateRect =>
            new Rectangle(0,0,Width + HorScroll.Maximum,Height + VerScroll.Maximum);

        /// <summary>
        /// Width of current base.
        /// </summary>
        public int CurrentTitleWidth =>
            MeasureString(CurrentTitle.Base,BaseFont).Width;

        /// <summary>
        /// Width of input line.
        /// </summary>
        public int InputLineWidth =>
            CurrentTitleWidth + MeasureString(Input,InputFont).Width;

        /// <summary>
        /// Rectangle of input line.
        /// </summary>
        public Rectangle InputLineRect {
            get {
                if(InputCount > 0) {
                    Rectangle lastLineRect = GetInputRect(InputCount - 1);
                    return new Rectangle(0,lastLineRect.Y + lastLineRect.Height,InputLineWidth,InputFont.Height);
                } else {
                    return new Rectangle(0,0,InputLineWidth,InputFont.Height);
                }
            }
        }

        /// <summary>
        /// Full rectangle of input line.
        /// </summary>
        public Rectangle FullInputLineRect {
            get {
                Rectangle rect = CaretRect;
                if(InputCount > 0) {
                    Rectangle lastLineRect = GetInputRect(InputCount - 1);
                    return new Rectangle(0,lastLineRect.Y + lastLineRect.Height,InputLineWidth + rect.X + rect.Width,InputFont.Height);
                } else {
                    return new Rectangle(0,0,InputLineWidth + rect.X + rect.Width,InputFont.Height);
                }
            }
        }

        /// <summary>
        /// Rectangle of caret.
        /// </summary>
        public Rectangle CaretRect {
            get {
                int sWidth = 0;
                if(!string.IsNullOrEmpty(Input)) {
                    sWidth = MeasureString(Input[0..CaretIndex],InputFont).Width;
                }
                Rectangle rect = new Rectangle(CurrentTitleWidth + sWidth,
                InputLineRect.Y,(int)InputFont.Size / 2 + 1,InputFont.Height);
                return rect;
            }
        }

        /// <summary>
        /// Caret position.
        /// </summary>
        public int CaretIndex {
            get => caretIndex;
            set {
                if(value == caretIndex)
                    return;

                if(value < 0)
                    value = 0;
                else if(value > Input.Length)
                    value = Input.Length;

                caretIndex = value;
                Invalidate(FullInputLineRect);
            }
        }

        /// <summary>
        /// Current command.
        /// </summary>
        public string Input {
            get => input;
            set {
                if(value == Input)
                    return;

                input = value;
                if(caretIndex>input.Length-1)
                    caretIndex=value.Length;

                Invalidate();
                AdapteScrolls();
                ScrollCaret();
            }
        }

        #endregion
    }

    /// <summary>
    /// Input for Terminal.
    /// </summary>
    public struct TerminalInput {
        #region Constructors

        /// <summary>
        /// Create new TerminalInput.
        /// </summary>
        /// <param name="basevalue">Base input.</param>
        /// <param name="input">Input.</param>
        /// <param name="baseFont">Font for base input.</param>
        /// <param name="inputFont">Font for input.</param>
        public TerminalInput(string basevalue,string input,Font baseFont,Font inputFont) {
            Base = basevalue;
            Input = input;
            BaseForeColor = Color.GreenYellow;
            ForeColor = Color.Gainsboro;
            Size BaseSize = TextRenderer.MeasureText(basevalue,baseFont);
            BaseWidth = BaseSize.Width;
            Size InputSize = TextRenderer.MeasureText(input,inputFont);
            int InputSplitLength = input.Split('\n').Length;
            if(InputSplitLength > 1)
                InputSplitLength++;
            Width = InputSize.Width;
            Height = inputFont.Height * InputSplitLength;
        }

        /// <summary>
        /// Create new TerminalInput.
        /// </summary>
        /// <param name="basevalue">Base input.</param>
        /// <param name="input">Input.</param>
        /// <param name="foreColor">Fore color for input</param>
        /// <param name="baseFont">Font for base input.</param>
        /// <param name="inputFont">Font for input.</param>
        public TerminalInput(string basevalue,string input,Color foreColor,Font baseFont,Font inputFont) :
            this(basevalue,input,baseFont,inputFont) {
            ForeColor = foreColor;
        }

        /// <summary>
        /// Create new TerminalInput.
        /// </summary>
        /// <param name="basevalue">Base input.</param>
        /// <param name="input">Input.</param>
        /// <param name="baseForeColor">Fore color for base input.</param>
        /// <param name="foreColor">Fore color for input</param>
        /// <param name="baseFont">Font for base input.</param>
        /// <param name="inputFont">Font for input.</param>
        public TerminalInput(string basevalue,string input,Color baseForeColor,Color foreColor,Font baseFont,Font inputFont) :
            this(basevalue,input,foreColor,baseFont,inputFont) {
            BaseForeColor = baseForeColor;
            ForeColor = foreColor;
        }

        #endregion

        #region Implicits & Explicits

        public static implicit operator string(TerminalInput Input) =>
            Input.Input;

        #endregion

        #region Properties

        /// <summary>
        /// Input.
        /// </summary>
        public string Input { get; private set; }

        /// <summary>
        /// ForeColor of input.
        /// </summary>
        public Color ForeColor { get; set; }

        /// <summary>
        /// Base ForeColor.
        /// </summary>
        public Color BaseForeColor { get; set; }

        /// <summary>
        /// Base.
        /// </summary>
        public string Base { get; private set; }

        /// <summary>
        /// Input width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Input height.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Base width.
        /// </summary>
        public int BaseWidth { get; private set; }

        #endregion
    }
}