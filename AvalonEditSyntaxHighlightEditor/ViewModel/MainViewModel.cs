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
using Miktemk.Wpf.Core.Behaviors.VM;
using System.IO;
using AvalonEditSyntaxHighlightEditor.Code.Services;

namespace AvalonEditSyntaxHighlightEditor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly MyAppStateService appStateService;
        private string curFilenameXshd;

        public string AppTitle { get; set; } = "XshdEditor";

        // ui config
        public UIElementDragDropConfig DragDropConfigXshd { get; }
        public UIElementDragDropConfig DragDropConfigSample { get; }

        // avalon-edit
        public TextDocument CodeDocumentXshd { get; } = new TextDocument();
        public TextDocument CodeDocumentSample { get; } = new TextDocument();
        public IHighlightingDefinition SyntaxHighlightingSample { get; set; }
        public string CurErrorMessage { get; set; }
        public WordHighlight CurErrorWordHighlight { get; set; }

        // commands
        public ICommand CmdWindow_Loaded { get; }
        public ICommand CmdWindow_Closing { get; }
        public ICommand CmdUser_OnDragDropXshd { get; }
        public ICommand CmdUser_OnDragDropSample { get; }
        public ICommand CmdUser_SaveFile { get; }
        public ICommand CmdUser_TriggerBuild { get; }
        public ICommand CmdAvalon_CaretPositionChanged { get; }

        public MainViewModel(MyAppStateService appStateService)
        {
            this.appStateService = appStateService;

            // set up view
            DragDropConfigXshd = Constants.Config.DragDropConfigXshd;
            DragDropConfigSample = Constants.Config.DragDropConfigSample;

            CodeDocumentXshd.Text = @"";
            CodeDocumentSample.Text = @"";

            // assign commands
            CmdWindow_Loaded = new RelayCommand(_CmdWindow_Loaded);
            CmdWindow_Closing = new RelayCommand(_CmdWindow_Closing);
            CmdUser_OnDragDropXshd = new RelayCommand<string>(_CmdUser_OnDragDropXshd);
            CmdUser_OnDragDropSample = new RelayCommand<string>(_CmdUser_OnDragDropSample);
            CmdUser_SaveFile = new RelayCommand(_CmdUser_SaveFile);
            CmdUser_TriggerBuild = new RelayCommand(_CmdUser_TriggerBuild);
            CmdAvalon_CaretPositionChanged = new RelayCommand<Caret>(_CmdAvalon_CaretPositionChanged);

            var appState = appStateService.LoadOrCreateNewAppState();
            if (appState != null)
                LoadPrevAppState(appState);
        }

        #region ------------------ commands/events -----------------------

        private void LoadPrevAppState(MyAppState appState)
        {
            if (appState.LastFilenameXshd != null)
            {
                curFilenameXshd = appState.LastFilenameXshd;
                CodeDocumentXshd.Text = File.ReadAllText(appState.LastFilenameXshd);
            }
            if (appState.LastFilenameSample != null)
               CodeDocumentSample.Text = File.ReadAllText(appState.LastFilenameSample);
            _CmdUser_TriggerBuild();
        }

        private void _CmdWindow_Loaded() { }

        private void _CmdWindow_Closing() { }

        private void _CmdUser_OnDragDropXshd(string filename)
        {
            CodeDocumentXshd.Text = File.ReadAllText(filename);
            curFilenameXshd = filename;
            appStateService.SaveAppState_FilenameXshd(filename);
        }

        private void _CmdUser_OnDragDropSample(string filename)
        {
            CodeDocumentSample.Text = File.ReadAllText(filename);
            appStateService.SaveAppState_FilenameSample(filename);
        }

        private void _CmdUser_SaveFile()
        {
            if (curFilenameXshd != null)
                File.WriteAllText(curFilenameXshd, CodeDocumentXshd.Text);
        }
        private void _CmdUser_TriggerBuild()
        {
            try
            {
                SyntaxHighlightingSample = UtilsAvalonEdit.LoadSyntaxHighlightingFromString(CodeDocumentXshd.Text);
                CurErrorMessage = null;
            }
            catch (HighlightingDefinitionInvalidException ex)
            {
                var errorStruct = MyIdeUtils.GetErrorPositionFromAvalonException(ex, CodeDocumentXshd);
                CurErrorMessage = errorStruct.Message;
                CurErrorWordHighlight = errorStruct.Highlight;
            }
            catch (FormatException ex)
            {
                CurErrorMessage = ex.Message;
            }
        }

        private void _CmdAvalon_CaretPositionChanged(Caret caret)
        {
            var curCaretOffset = caret.Offset;
            var curCaretLine = CodeDocumentXshd.GetLineByNumber(caret.Line);
            var curLineText = CodeDocumentXshd.GetText(curCaretLine);
            CurErrorWordHighlight = null;
            //CurErrorMessage = null;
        }

        #endregion
    }
}