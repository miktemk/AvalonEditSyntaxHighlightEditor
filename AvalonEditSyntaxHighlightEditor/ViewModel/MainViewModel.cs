﻿using AvalonEditSyntaxHighlightEditor.Code;
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
using System.Windows;
using System.Windows.Threading;

namespace AvalonEditSyntaxHighlightEditor.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private readonly MyAppStateService appStateService;

        private string CurFilenameXshd { get; set; }
        private bool IsDocumentsChanged { get; set; } = false;
        public string WindowTitle => $"AvalonEdit XSHD - {CurFilenameXshd}{(IsDocumentsChanged ? " *" : "")}";

        // ui config
        public UIElementDragDropConfig DragDropConfigXshd { get; } = Constants.Config.DragDropConfigXshd;
        public UIElementDragDropConfig DragDropConfigSample { get; } = Constants.Config.DragDropConfigSample;

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
        public ICommand CmdUser_RevealInExplorer { get; }
        public ICommand CmdAvalon_CaretPositionChanged { get; }

        public MainViewModel(MyAppStateService appStateService)
        {
            this.appStateService = appStateService;

            // global error handling
            Application.Current.DispatcherUnhandledException += Application_DispatcherUnhandledException;

            // set up view
            CodeDocumentXshd.Text = @"";
            CodeDocumentSample.Text = @"";
            CodeDocumentXshd.UndoStack.ClearAll();
            CodeDocumentSample.UndoStack.ClearAll();

            CodeDocumentXshd.Changed += CodeDocument_Changed;

            // assign commands
            CmdWindow_Loaded = new RelayCommand(_CmdWindow_Loaded);
            CmdWindow_Closing = new RelayCommand(_CmdWindow_Closing);
            CmdUser_OnDragDropXshd = new RelayCommand<string>(_CmdUser_OnDragDropXshd);
            CmdUser_OnDragDropSample = new RelayCommand<string>(_CmdUser_OnDragDropSample);
            CmdUser_SaveFile = new RelayCommand(_CmdUser_SaveFile);
            CmdUser_TriggerBuild = new RelayCommand(_CmdUser_TriggerBuild);
            CmdUser_RevealInExplorer = new RelayCommand(_CmdUser_RevealInExplorer);
            CmdAvalon_CaretPositionChanged = new RelayCommand<Caret>(_CmdAvalon_CaretPositionChanged);
        }

        #region ------------------ commands/events -----------------------

        private void LoadAppState(MyAppState prevAppState)
        {
            var argFilename = (string)Application.Current.Resources[Constants.Resources.Arg1Key];
            if (TryToLoadXshdFile(argFilename))
                appStateService.SaveAppState_FilenameXshd(argFilename);
            else if (TryToLoadXshdFile(prevAppState.LastFilenameXshd))
            { }
            
            if (prevAppState.LastFilenameSample != null && File.Exists(prevAppState.LastFilenameSample))
            {
                CodeDocumentSample.Text = File.ReadAllText(prevAppState.LastFilenameSample);
                CodeDocumentSample.UndoStack.ClearAll();
            }
            _CmdUser_TriggerBuild();
        }

        private bool TryToLoadXshdFile(string filename)
        {
            if (filename == null || !File.Exists(filename))
                return false;
            CurFilenameXshd = filename;
            CodeDocumentXshd.Text = File.ReadAllText(filename);
            CodeDocumentXshd.UndoStack.ClearAll();
            IsDocumentsChanged = false;
            return true;
        }

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleCatastrophicException(e.Exception);
            e.Handled = true;
        }

        private void _CmdWindow_Loaded()
        {
            var appState = appStateService.LoadOrCreateNewAppState();
            if (appState != null)
                LoadAppState(appState);
        }

        private void _CmdWindow_Closing() { }

        private void _CmdUser_OnDragDropXshd(string filename)
        {
            CodeDocumentXshd.Text = File.ReadAllText(filename);
            CurFilenameXshd = filename;
            IsDocumentsChanged = false;
            appStateService.SaveAppState_FilenameXshd(filename);
        }

        private void _CmdUser_OnDragDropSample(string filename)
        {
            CodeDocumentSample.Text = File.ReadAllText(filename);
            appStateService.SaveAppState_FilenameSample(filename);
        }

        private void _CmdUser_SaveFile()
        {
            if (CurFilenameXshd != null)
            {
                File.WriteAllText(CurFilenameXshd, CodeDocumentXshd.Text);
                IsDocumentsChanged = false;
            }
        }
        private void _CmdUser_TriggerBuild()
        {
            if (String.IsNullOrEmpty(CodeDocumentXshd.Text))
                return;
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
            catch (Exception ex)
            {
                HandleCatastrophicException(ex);
            }
        }
        private void _CmdUser_RevealInExplorer()
        {
            if (CurFilenameXshd != null)
                UtilsOp.OpenWinExplorerAndSelectThisFile(CurFilenameXshd);
        }

        private void HandleCatastrophicException(Exception ex)
        {
            var errorStruct = MyIdeUtils.GetErrorPositionFromGenericException(ex, CodeDocumentXshd);
            CurErrorMessage = errorStruct.Message;
            if (errorStruct.Highlight != null)
                CurErrorWordHighlight = errorStruct.Highlight;
            SyntaxHighlightingSample = null;
        }

        private void _CmdAvalon_CaretPositionChanged(Caret caret)
        {
            var curCaretOffset = caret.Offset;
            var curCaretLine = CodeDocumentXshd.GetLineByNumber(caret.Line);
            var curLineText = CodeDocumentXshd.GetText(curCaretLine);
            CurErrorWordHighlight = null;
            //CurErrorMessage = null;
        }

        private void CodeDocument_Changed(object sender, DocumentChangeEventArgs e)
        {
            IsDocumentsChanged = true;
        }

        #endregion
    }
}