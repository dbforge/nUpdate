// Author: Dominic Beger (Trade/ProgTrade) 2016

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FastColoredTextBoxNS;
using nUpdate.Administration.UI.Popups;
using nUpdate.Internal.Core.Operations;

namespace nUpdate.Administration.Core.Operations.Panels
{
    public partial class ScriptExecuteOperationPanel : UserControl, IOperationPanel
    {
        private readonly TextStyle _blueStyle = new TextStyle(Brushes.Blue, null, FontStyle.Regular);
        private readonly TextStyle _boldStyle = new TextStyle(null, null, FontStyle.Bold | FontStyle.Underline);
        private readonly TextStyle _brownStyle = new TextStyle(Brushes.Brown, null, FontStyle.Italic);
        private readonly TextStyle _grayStyle = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        private readonly TextStyle _greenStyle = new TextStyle(Brushes.Green, null, FontStyle.Italic);
        private readonly TextStyle _magentaStyle = new TextStyle(Brushes.Magenta, null, FontStyle.Regular);
        //private TextStyle _maroonStyle = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        private readonly MarkerStyle _sameWordsStyle = new MarkerStyle(new SolidBrush(Color.FromArgb(40, Color.Gray)));
        private List<CompilerError> _activeCompilerErrors = new List<CompilerError>();

        public ScriptExecuteOperationPanel()
        {
            InitializeComponent();
        }

        public string Code
        {
            get { return codeTextBox.Text; }
            set { codeTextBox.Text = value; }
        }

        public Operation Operation => new Operation(OperationArea.Scripts, OperationMethod.Execute, Code);

        public bool IsValid
        {
            get
            {
                string newText =
                    string.Join("\n",
                        codeTextBox.Lines.Except(
                            codeTextBox.Lines.Where(item => item.StartsWith("using") || item.StartsWith("//")))).Trim();
                return newText.StartsWith("public class Program") && newText.Contains("static void Main") && !_activeCompilerErrors.Any();
            }
        }

        private void codeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            codeTextBox.LeftBracket = '(';
            codeTextBox.RightBracket = ')';
            codeTextBox.LeftBracket2 = '\x0';
            codeTextBox.RightBracket2 = '\x0';

            e.ChangedRange.ClearStyle(_blueStyle, _boldStyle, _grayStyle, _magentaStyle, _greenStyle, _brownStyle);
            e.ChangedRange.SetStyle(_brownStyle, @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'");
            e.ChangedRange.SetStyle(_greenStyle, @"//.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(_greenStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
            e.ChangedRange.SetStyle(_greenStyle, @"(/\*.*?\*/)|(.*\*/)",
                RegexOptions.Singleline | RegexOptions.RightToLeft);
            e.ChangedRange.SetStyle(_magentaStyle, @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b");
            e.ChangedRange.SetStyle(_grayStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(_boldStyle, @"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b");
            e.ChangedRange.SetStyle(_blueStyle,
                @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield)\b|#region\b|#endregion\b");

            e.ChangedRange.ClearFoldingMarkers();
            e.ChangedRange.SetFoldingMarkers("{", "}");
            e.ChangedRange.SetFoldingMarkers(@"#region\b", @"#endregion\b");
            e.ChangedRange.SetFoldingMarkers(@"/\*", @"\*/");

            if (!codeTextBox.Text.Trim().StartsWith("<?xml"))
                return;
            codeTextBox.Language = Language.XML;

            codeTextBox.ClearStylesBuffer();
            codeTextBox.Range.ClearStyle(StyleIndex.All);
            codeTextBox.AddStyle(_sameWordsStyle);

            codeTextBox.OnSyntaxHighlight(new TextChangedEventArgs(codeTextBox.Range));
        }

        private void codeTextBox_Leave(object sender, EventArgs e)
        {
            errorsTabPage.Text = "Errors: 0";
            errorListView.Items.Clear();

            _activeCompilerErrors = new CodeDomHelper().CompileCode(Code).ToList();
            if (_activeCompilerErrors.Count == 0)
                return;

            Popup.ShowPopup(this, SystemIcons.Error, "Compilation failed.",
                "The compilation of the source code failed. Please check the error list for more information.",
                PopupButtons.Ok);
            errorsTabPage.Text = $"Errors: ({_activeCompilerErrors.Count})";
            errorListView.Items.Clear();

            foreach (var activeError in _activeCompilerErrors)
            {
                var errorItem = new ListViewItem(activeError.ErrorNumber);
                errorItem.SubItems.Add(activeError.ErrorText);
                errorItem.SubItems.Add(activeError.Line.ToString(CultureInfo.InvariantCulture));
                errorItem.SubItems.Add(activeError.Column.ToString(CultureInfo.InvariantCulture));
                errorListView.Items.Add(errorItem);
            }
        }
    }
}