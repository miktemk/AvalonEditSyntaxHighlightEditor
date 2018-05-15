using AvalonEditSyntaxHighlightEditor.Code;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Windows.Input;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Miktemk;
using Miktemk.Wpf.AvalonEdit.Code;
using Miktemk.Models;

namespace AvalonEditSyntaxHighlightEditor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public string WelcomeTitle { get; set; } = "MVVM test app";

        // avalon-edit
        public TextDocument CodeDocument { get; } = new TextDocument();
        public TextDocument CodeDocumentSample { get; } = new TextDocument();
        public IHighlightingDefinition SyntaxHighlightingSample { get; set; }
        public string CurErrorMessage { get; set; }
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
                SyntaxHighlightingSample = UtilsAvalonEdit.LoadSyntaxHighlightingFromString(CodeDocument.Text);
                CurErrorMessage = null;
            }
            catch (HighlightingDefinitionInvalidException ex)
            {
                var errorStruct = MyIdeUtils.GetErrorPositionFromAvalonException(ex, CodeDocument);
                CurErrorMessage = errorStruct.Message;
                CurErrorWordHighlight = errorStruct.Highlight;
            }
        }

        private void CaretPositionChanged(Caret caret)
        {
            var curCaretOffset = caret.Offset;
            var curCaretLine = CodeDocument.GetLineByNumber(caret.Line);
            var curLineText = CodeDocument.GetText(curCaretLine);
            CurErrorWordHighlight = null;
            //CurErrorMessage = null;
        }

        #endregion
    }
}