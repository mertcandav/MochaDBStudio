using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace MochaDBStudio.Editor {
    public class SyntaxHighlighter {
        protected CodeEditor currentTb;

        //General Styles.
        protected readonly Style
            keywordStyle = new TextStyle(new SolidBrush(Color.FromArgb(255,110,110)),Brushes.Transparent,FontStyle.Bold),
            commentStyle = new TextStyle(new SolidBrush(Color.FromArgb(87,166,74)),Brushes.Transparent,FontStyle.Regular),
            numberStyle = new TextStyle(Brushes.LightGreen,Brushes.Transparent,FontStyle.Regular),
            tagStyle = new TextStyle(Brushes.LightSeaGreen,Brushes.Transparent,FontStyle.Bold),
            stringStyle = new TextStyle(new SolidBrush(Color.FromArgb(235,185,0)),Brushes.Transparent,FontStyle.Regular),
            parameterStyle = new TextStyle(Brushes.LightSkyBlue,Brushes.Transparent,FontStyle.Regular),
            funcStyle = new TextStyle(new SolidBrush(Color.FromArgb(255,220,155)),Brushes.Transparent,FontStyle.Regular);

        public SyntaxHighlighter(CodeEditor currentTb) {
            this.currentTb = currentTb;
        }

        public virtual void Highlight() {
            if(currentTb.Language == Language.None)
                return;
            else if(currentTb.Language == Language.MochaScript)
                MochaScript();
            else
                Mhql();
        }

        /// <summary>
        /// Highlight MochaScript code.
        /// </summary>
        public virtual void MochaScript() {
            currentTb.VisibleRange.ClearStyle(parameterStyle,stringStyle,numberStyle,funcStyle,keywordStyle,commentStyle);
            currentTb.VisibleRange.SetStyle(numberStyle,"^[0-9]*$");
            currentTb.VisibleRange.SetStyle(commentStyle,@"//.*");
            currentTb.VisibleRange.SetStyle(stringStyle,@""".*""");
            currentTb.VisibleRange.SetStyle(keywordStyle,@":");
            currentTb.VisibleRange.SetStyle(parameterStyle,@"(:.*)|(:.*:)");
            currentTb.VisibleRange.SetStyle(funcStyle,@".*?:",
RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            currentTb.VisibleRange.SetStyle(keywordStyle,@"(if|else|True|False|Provider|elif|echo|compilerevent|Begin|Final|delete|func|
String|Char|Long|Integer|Short|ULong|UInteger|UShort|Decimal|Double|Float|Boolean|Byte|SByte|DateTime)",RegexOptions.Multiline);
            currentTb.VisibleRange.SetStyle(funcStyle,
                @"(\(\))");
            currentTb.VisibleRange.SetStyle(funcStyle,
                @"(^.*).*?$|func( ).*?\(\)");
        }

        /// <summary>
        /// Highlight Mhql code.
        /// </summary>
        public virtual void Mhql() {
            currentTb.VisibleRange.ClearStyle(tagStyle,stringStyle,numberStyle,funcStyle,keywordStyle,commentStyle);
            currentTb.VisibleRange.SetStyle(tagStyle,@"\@\w.*?(( )|\n|$)");
            currentTb.VisibleRange.SetStyle(numberStyle,"^[0-9]*$");
            currentTb.VisibleRange.SetStyle(keywordStyle,
@"\b(USE|RETURN|ORDERBY|ASC|DESC|MUST|AND|GROUPBY|FROM|AS|SELECT|REMOVE)\b",
RegexOptions.IgnoreCase|RegexOptions.CultureInvariant);
            currentTb.VisibleRange.SetStyle(funcStyle,@"\(.*\)",RegexOptions.Multiline);
            currentTb.VisibleRange.SetStyle(commentStyle,@"/\*.*\*/",RegexOptions.Multiline);
            currentTb.VisibleRange.SetStyle(stringStyle,@""".*""");
        }
    }

    /// <summary>
    /// Language
    /// </summary>
    public enum Language {
        None = 0,
        Mhql = 1,
        MochaScript = 2
    }
}
