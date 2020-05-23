using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MochaDB;
using MochaDB.FileSystem;
using MochaDB.Logging;
using MochaDB.Mhql;
using MochaDB.MochaScript;
using MochaDB.Querying;
using MochaDB.Streams;
using MochaDBStudio.dialogs;
using MochaDBStudio.engine;
using MochaDBStudio.gui.codesense;
using MochaDBStudio.gui.editor;
using MochaDBStudio.Properties;

namespace MochaDBStudio.gui {
    /// <summary>
    /// Script Panel.
    /// </summary>
    public sealed partial class scriptpanel:Panel {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="path">Path of script file.</param>
        public scriptpanel(string path) {
            Path = path;

            Init();

            scriptEditor.Text = debugger.MochaScript;
            scriptEditor.TextChanged+=ScriptEditor_TextChanged;
        }

        #endregion

        #region Overrides

        protected override void Dispose(bool disposing) {
            debugger.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region debugger

        private void Debugger_Echo(object sender,MochaScriptEchoEventArgs e) {
            term.AddInput(new TerminalInput(string.Empty,
                e.Message == null ? "{nil}" : e.Message.ToString(),null,term.Font),false);
        }

        #endregion

        #region term

        private void Term_InputProcessing(object sender,TerminalInputProcessEventArgs e) {
            if(e.Input == "save") {
                e.Cancel=true;
                Save();
            } else if(e.Input=="debug") {
                e.Cancel=true;
                DebugAsync();
            }
        }

        #endregion

        #region scriptEditor

        private void ScriptEditor_KeyDown(object sender,KeyEventArgs e) {
            if(e.KeyCode==Keys.F5) {
                DebugAsync();
            } else if(e.Control && e.KeyCode ==Keys.S) {
                Save();
            }
        }

        private void ScriptEditor_TextChanged(object sender,TextChangedEventArgs e) {
            ((Studio)Parent.Parent).scriptButton.Text = $"*Script";
        }

        #endregion

        #region Methods

        /// <summary>
        /// Debug MochaScript codes asynchronous.
        /// </summary>
        public void DebugAsync() {
            Save();
            term.Select();
            term.UseUserInput=false;
            scriptEditor.Enabled = false;
            term.AddInput(new TerminalInput(string.Empty,
                "Debugging MochaScript code and running...",null,term.Font),false);
            new Task(() => {
                term.Clear();
                try {
                    debugger.DebugRun();
                    term.AddInput(new TerminalInput(string.Empty,
                        "This MochaScript code debugged and runed sucessfully.\n",Color.LimeGreen,null,term.Font),false);
                } catch(Exception excep) {
                    term.AddInput(new TerminalInput(string.Empty,
                        excep.ToString() + "\n",Color.Red,null,term.Font),false);
                }
                term.UseUserInput=true;
                scriptEditor.Enabled = true;
                scriptEditor.Select();
            }).Start();
        }

        /// <summary>
        /// Save MochaScript from editor.
        /// </summary>
        public void Save() {
            debugger.Dispose();
            fs.WriteTextFile(Path,scriptEditor.Text);
            debugger = new MochaScriptDebugger(Path);
            debugger.Echo += Debugger_Echo;
            ((Studio)Parent.Parent).scriptButton.Text = "Script";
        }

        #endregion

        #region Path

        /// <summary>
        /// Path of script file.
        /// </summary>
        public string Path { get; private set; }

        #endregion
    }

    // Designer.
    public sealed partial class scriptpanel {
        #region Components

        private MochaScriptDebugger
            debugger;

        private terminal
            term;

        private editor.editor
            scriptEditor;

        private codesense.codesense
            scriptCodeSense;

        private ImageList
            scriptIL;

        private SplitContainer
            scriptContainer;

        #endregion

        /// <summary>
        /// Initialize component.
        /// </summary>
        public void Init() {
            #region Base

            Dock = DockStyle.Fill;
            BackColor = Color.FromArgb(60,60,60);
            ForeColor = Color.White;

            #endregion

            #region debugger

            debugger = new MochaScriptDebugger(Path);
            debugger.Echo += Debugger_Echo;

            #endregion

            #region scriptContainer

            scriptContainer = new SplitContainer();
            scriptContainer.Dock = DockStyle.Fill;
            scriptContainer.BackColor = BackColor;
            scriptContainer.Orientation = Orientation.Horizontal;
            scriptContainer.BorderStyle = BorderStyle.FixedSingle;
            Controls.Add(scriptContainer);

            #endregion

            #region scriptEditor

            scriptEditor = new editor.editor();
            scriptEditor.Dock = DockStyle.Fill;
            scriptEditor.Language = Language.MochaScript;
            scriptEditor.BackColor = BackColor;
            scriptEditor.ForeColor = ForeColor;
            scriptEditor.LineNumberColor = Color.Khaki;
            scriptEditor.CurrentLineColor = Color.Black;
            scriptEditor.ServiceLinesColor = Color.Transparent;
            scriptEditor.CaretColor = Color.White;
            scriptEditor.WordWrap = false;
            scriptEditor.KeyDown+=ScriptEditor_KeyDown;
            scriptContainer.Panel1.Controls.Add(scriptEditor);

            #endregion

            #region term

            term = new terminal();
            term.BackColor = Color.FromArgb(40,40,40);
            term.BannedCommandNamespaces = new[] { "cnc" };
            term.InputProcessing+=Term_InputProcessing;
            scriptContainer.Panel2.Controls.Add(term);

            #endregion

            #region scriptIL

            scriptIL = new ImageList();
            scriptIL.ColorDepth=ColorDepth.Depth32Bit;
            scriptIL.Images.Add("Keyword",Resources.Key);
            scriptIL.Images.Add("Snippet",Resources.Brackets);

            #endregion

            #region scriptCodeSense

            scriptCodeSense = new codesense.codesense();
            scriptCodeSense.ImageList = scriptIL;
            scriptCodeSense.AllowsTabKey = true;
            scriptCodeSense.Colors.BackColor = Color.FromArgb(50,50,50);
            scriptCodeSense.Colors.SelectedBackColor = Color.Black;
            scriptCodeSense.Colors.SelectedBackColor2 = Color.Black;
            scriptCodeSense.Colors.HighlightingColor = Color.DodgerBlue;
            scriptCodeSense.Colors.ForeColor = Color.White;
            scriptCodeSense.Colors.SelectedForeColor = Color.Khaki;
            scriptCodeSense.AppearInterval = 100;
            scriptCodeSense.Font = new Font("Consolas",12,FontStyle.Regular,GraphicsUnit.Pixel);
            scriptCodeSense.AddItem(
                new Item("String",0,"String","String - Keyword",
                "String."));
            scriptCodeSense.AddItem(
                new Item("Char",0,"Char","Char - Keyword",
                "Char."));
            scriptCodeSense.AddItem(
                new Item("Long",0,"Long","Long - Keyword",
                "Long."));
            scriptCodeSense.AddItem(
                new Item("Integer",0,"Integer","Integer - Keyword",
                "Integer."));
            scriptCodeSense.AddItem(
                new Item("Short",0,"Short","Short - Keyword",
                "Short."));
            scriptCodeSense.AddItem(
                new Item("ULong",0,"ULong","ULong - Keyword",
                "ULong."));
            scriptCodeSense.AddItem(
                new Item("UInteger",0,"UInteger","UInteger - Keyword",
                "UInteger."));
            scriptCodeSense.AddItem(
                new Item("UShort",0,"UShort","UShort - Keyword",
                "UShort."));
            scriptCodeSense.AddItem(
                new Item("Decimal",0,"Decimal","Decimal - Keyword",
                "Decimal."));
            scriptCodeSense.AddItem(
                new Item("Double",0,"Double","Double - Keyword",
                "Double."));
            scriptCodeSense.AddItem(
                new Item("Float",0,"Float","Float - Keyword",
                "Float."));
            scriptCodeSense.AddItem(
                new Item("Boolean",0,"Boolean","Boolean - Keyword",
                "Boolean."));
            scriptCodeSense.AddItem(
                new Item("Byte",0,"Byte","Byte - Keyword",
                "Byte."));
            scriptCodeSense.AddItem(
                new Item("SByte",0,"SByte","SByte - Keyword",
                "SByte."));
            scriptCodeSense.AddItem(
                new Item("DateTime",0,"DateTime","DateTime - Keyword",
                "DateTime."));
            scriptCodeSense.AddItem(
                new Item("if",0,"if","if - Keyword",
                "if."));
            scriptCodeSense.AddItem(
                new Item("elif",0,"elif","elif - Keyword",
                "elif."));
            scriptCodeSense.AddItem(
                new Item("else",0,"else","else - Keyword",
                "else."));
            scriptCodeSense.AddItem(
                new Item("True",0,"True","True - Keyword",
                "True."));
            scriptCodeSense.AddItem(
                new Item("False",0,"False","False - Keyword",
                "False."));
            scriptCodeSense.AddItem(
                new Item("Provider",0,"Provider","Provider - Keyword",
                "Provider."));
            scriptCodeSense.AddItem(
                new Item("echo",0,"echo","echo - Keyword",
                "echo."));
            scriptCodeSense.AddItem(
                new Item("compilerevent",0,"compilerevent","compilerevent - Keyword",
                "compilerevent."));
            scriptCodeSense.AddItem(
                new Item("Begin",0,"Begin","Begin - Keyword",
                "Begin."));
            scriptCodeSense.AddItem(
                new Item("delete",0,"delete","delete - Keyword",
                "delete."));
            scriptCodeSense.AddItem(
                new Item("func",0,"func","func - Keyword",
                "func."));
            scriptCodeSense.AddItem(new Item("func Main()\n{\n    \n}",1,"MAIN","MAIN - Snippet",
                "Main body snippet."));
            scriptCodeSense.AddItem(new Item("func MyFunc()\n{\n    \n}",1,"FUNC","FUNC - Snippet",
                "Function body snippet."));

            scriptCodeSense.SortItems();
            scriptCodeSense.SetCodeSense(scriptEditor,scriptCodeSense);

            #endregion
        }
    }
}
