using System.Collections.Generic;
using System;

namespace MochaDBStudio.Editor
{
    public class CommandManager
    {
        #region Tanımlar

        private int autoUndoCommands = 0;
        protected int disabledCommands = 0;
        private readonly int maxHistoryLength = 200;
        private LimitedStack<UndoableCommand> history;
        private Stack<UndoableCommand> redoStack = new Stack<UndoableCommand>();
        public TextSource TextSource{ get; private set; }
        public bool UndoRedoStackIsEnabled { get; set; }

        public event EventHandler RedoCompleted = delegate { };

        #endregion

        #region Ana Yükleme

        public CommandManager(TextSource ts)
        {
            history = new LimitedStack<UndoableCommand>(maxHistoryLength);
            TextSource = ts;
            UndoRedoStackIsEnabled = true;
        }

        #endregion

        #region Metods

        public virtual void ExecuteCommand(Command cmd)
        {
            if (disabledCommands > 0)
                return;

            //multirange ?
            if (cmd.ts.CurrentTB.Selection.ColumnSelectionMode)
            if (cmd is UndoableCommand)
                //make wrapper
                cmd = new MultiRangeCommand((UndoableCommand)cmd);


            if (cmd is UndoableCommand)
            {
                //if range is ColumnRange, then create wrapper
                (cmd as UndoableCommand).autoUndo = autoUndoCommands > 0;
                history.Push(cmd as UndoableCommand);
            }

            try
            {
                cmd.Execute();
            }
            catch (ArgumentOutOfRangeException)
            {
                //OnTextChanging cancels enter of the text
                if (cmd is UndoableCommand)
                    history.Pop();
            }
            //
            if (!UndoRedoStackIsEnabled)
                ClearHistory();
            //
            redoStack.Clear();
            //
            TextSource.CurrentTB.OnUndoRedoStateChanged();
        }

        public void Undo()
        {
            if (history.Count > 0)
            {
                var cmd = history.Pop();
                //
                BeginDisableCommands();//prevent text changing into handlers
                try
                {
                    cmd.Undo();
                }
                finally
                {
                    EndDisableCommands();
                }
                //
                redoStack.Push(cmd);
            }

            //undo next autoUndo command
            if (history.Count > 0)
            {
                if (history.Peek().autoUndo)
                    Undo();
            }

            TextSource.CurrentTB.OnUndoRedoStateChanged();
        }

        private void EndDisableCommands()
        {
            disabledCommands--;
        }

        private void BeginDisableCommands()
        {
            disabledCommands++;
        }

        public void EndAutoUndoCommands()
        {
            autoUndoCommands--;
            if (autoUndoCommands == 0)
                if (history.Count > 0)
                    history.Peek().autoUndo = false;
        }

        public void BeginAutoUndoCommands()
        {
            autoUndoCommands++;
        }

        internal void ClearHistory()
        {
            history.Clear();
            redoStack.Clear();
            TextSource.CurrentTB.OnUndoRedoStateChanged();
        }

        internal void Redo()
        {
            if (redoStack.Count == 0)
                return;
            UndoableCommand cmd;
            BeginDisableCommands();//prevent text changing into handlers
            try
            {
                cmd = redoStack.Pop();
                if (TextSource.CurrentTB.Selection.ColumnSelectionMode)
                    TextSource.CurrentTB.Selection.ColumnSelectionMode = false;
                TextSource.CurrentTB.Selection.Start = cmd.sel.Start;
                TextSource.CurrentTB.Selection.End = cmd.sel.End;
                cmd.Execute();
                history.Push(cmd);
            }
            finally
            {
                EndDisableCommands();
            }

            //call event
            RedoCompleted(this, EventArgs.Empty);

            //redo command after autoUndoable command
            if (cmd.autoUndo)
                Redo();

            TextSource.CurrentTB.OnUndoRedoStateChanged();
        }

        #endregion

        #region Get/Set

        public bool UndoEnabled 
        { 
            get
            {
                return history.Count > 0;
            }
        }

        public bool RedoEnabled
        {
            get
            {
                return redoStack.Count > 0;
            }
        }

        #endregion
    }

    public abstract class Command
    {
        #region Tanımlar

        public TextSource ts;

        #endregion

        #region Metods

        public abstract void Execute();

        #endregion
    }

    internal class RangeInfo
    {
        #region Ana Yükleme

        public RangeInfo(Range r)
        {
            Start = r.Start;
            End = r.End;
        }

        #endregion

        #region Get/Set

        public Place Start { get; set; }
        public Place End { get; set; }

        internal int FromX
        {
            get
            {
                if (End.iLine < Start.iLine) return End.iChar;
                if (End.iLine > Start.iLine) return Start.iChar;
                return Math.Min(End.iChar, Start.iChar);
            }
        }

        #endregion
    }

    public abstract class UndoableCommand : Command
    {
        #region Tanımlar

        internal RangeInfo sel;
        internal RangeInfo lastSel;
        internal bool autoUndo;

        #endregion

        #region Ana Yükleme

        public UndoableCommand(TextSource ts)
        {
            this.ts = ts;
            sel = new RangeInfo(ts.CurrentTB.Selection);
        }

        #endregion

        #region Metods

        public virtual void Undo()
        {
            OnTextChanged(true);
        }

        public override void Execute()
        {
            lastSel = new RangeInfo(ts.CurrentTB.Selection);
            OnTextChanged(false);
        }

        protected virtual void OnTextChanged(bool invert)
        {
            bool b = sel.Start.iLine < lastSel.Start.iLine;
            if (invert)
            {
                if (b)
                    ts.OnTextChanged(sel.Start.iLine, sel.Start.iLine);
                else
                    ts.OnTextChanged(sel.Start.iLine, lastSel.Start.iLine);
            }
            else
            {
                if (b)
                    ts.OnTextChanged(sel.Start.iLine, lastSel.Start.iLine);
                else
                    ts.OnTextChanged(lastSel.Start.iLine, lastSel.Start.iLine);
            }
        }

        public abstract UndoableCommand Clone();

        #endregion
    }
}