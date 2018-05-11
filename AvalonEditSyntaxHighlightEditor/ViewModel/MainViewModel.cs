using AvalonEditSyntaxHighlightEditor.Code;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows.Input;
using System;
using System.Diagnostics;

namespace AvalonEditSyntaxHighlightEditor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public string WelcomeTitle { get; set; } = "MVVM test app";

        // avalon-edit
        public TextDocument CodeDocument { get; } = new TextDocument();
        public TextDocument CodeDocumentSample { get; } = new TextDocument();
        public IHighlightingDefinition SyntaxHighlightingSample { get; set; }
        public WordHighlight CurErrorWordHighlight { get; set; }

        // commands
        public ICommand WindowLoadedCommand { get; }
        public ICommand WindowClosingCommand { get; }
        public ICommand TriggerBuildCommand { get; }
        public ICommand CaretPositionChangedCommand { get; }

        public MainViewModel()
        {
            // set up view
            CodeDocument.Text = @"";
            CodeDocumentSample.Text = @"";

            // assign commands
            WindowLoadedCommand = new RelayCommand(WindowLoaded);
            WindowClosingCommand = new RelayCommand(WindowClosing);
            TriggerBuildCommand = new RelayCommand(TriggerBuild);
            CaretPositionChangedCommand = new RelayCommand<Caret>(CaretPositionChanged);
        }

        #region ------------------ commands -----------------------

        private void WindowLoaded() { }

        private void WindowClosing() { }

        private void TriggerBuild()
        {
            try
            {
                SyntaxHighlightingSample = MyIdeUtils.LoadSyntaxHighlightingFromString(CodeDocument.Text);
            }
            catch (ICSharpCode.AvalonEdit.Highlighting.HighlightingDefinitionInvalidException ex)
            {
                if (ex.InnerException != null && ex.InnerException is System.Xml.XmlException)
                {
                    var exXml = ex.InnerException as System.Xml.XmlException;
                    var errorLine = CodeDocument.GetLineByNumber(exXml.LineNumber);
                    CurErrorWordHighlight = new WordHighlight(errorLine.Offset, errorLine.Length);
                }
                //ex.InnerException
            }
        }

        private void CaretPositionChanged(Caret caret)
        {
            var curCaretOffset = caret.Offset;
            var curCaretLine = CodeDocument.GetLineByNumber(caret.Line);
            var curLineText = CodeDocument.GetText(curCaretLine);
            CurErrorWordHighlight = null;
        }

        #endregion
    }
}