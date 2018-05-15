using Miktemk;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonEditSyntaxHighlightEditor.Code.Services
{
    public class MyAppStateService
    {
        //private readonly MyFileOpsService fileOpsService;
        //private readonly IDialogService dialogService;

        //public TtsBrowserConfigService(MyFileOpsService fileOpsService, IDialogService dialogService)
        //{
        //    this.fileOpsService = fileOpsService;
        //    this.dialogService = dialogService;
        //}

        //public TtsBrowserConfig GetTtsBrowserConfigOrDieTrying(INotifyPropertyChanged ownerViewModel)
        //{
        //    var configFilename = GetLastTtsBrowserConfigFilenameOrDieTrying(ownerViewModel);
        //    return (configFilename != null)
        //        ? JsonUtils.LoadFromFile<TtsBrowserConfig>(configFilename)
        //        : MakeSampleConfigFromResources();
        //}

        //public string GetTtsBrowserConfigScriptSilently()
        //{
        //    var configFilename = fileOpsService.GetLastTtsBrowserConfigFilenameIfValid();
        //    return (configFilename != null)
        //        ? File.ReadAllText(configFilename)
        //        : GetSampleConfigScriptFromResources();
        //}

        //public TtsBrowserConfig ParseTtsBrowserConfigScript(string text)
        //     => JsonUtils.JsonDeserialize<TtsBrowserConfig>(text);

        //public string SaveTtsBrowserConfigScriptAs(INotifyPropertyChanged ownerViewModel, string script)
        //{
        //    var configFilename = fileOpsService.OpenSaveFileDialogForTtsBrowserConfigGetFilename(ownerViewModel);
        //    if (configFilename != null)
        //    {
        //        File.WriteAllText(configFilename, script);
        //        fileOpsService.SaveLastTtsBrowserConfigFilename(configFilename);
        //    }
        //    return configFilename;
        //}

        //public string SaveTtsBrowserConfigScript(INotifyPropertyChanged ownerViewModel, string script)
        //{
        //    var configFilename = fileOpsService.GetLastTtsBrowserConfigFilenameIfValid();
        //    if (configFilename == null)
        //        return SaveTtsBrowserConfigScriptAs(ownerViewModel, script);
        //    File.WriteAllText(configFilename, script);
        //    return configFilename;
        //}

        //private string GetSampleConfigScriptFromResources()
        //    => MyIdeUtils.LoadStringContentsFromResource(Constants.Resources.SampleTtsBrowserConfigJson);

        //private TtsBrowserConfig MakeSampleConfigFromResources()
        //{
        //    var script = GetSampleConfigScriptFromResources();
        //    return JsonUtils.JsonDeserialize<TtsBrowserConfig>(script);
        //}

        //public string GetLastTtsBrowserConfigFilenameOrDieTrying(INotifyPropertyChanged ownerViewModel)
        //{
        //    var configFilename = fileOpsService.GetLastTtsBrowserConfigFilenameIfValid();
        //    if (configFilename == null)
        //    {
        //        var result = dialogService.ShowMessageBox(
        //            ownerViewModel,
        //            "Looks like this is your first time running TTS browser. Create a config file?",
        //            "No previous config file",
        //            System.Windows.MessageBoxButton.YesNo,
        //            System.Windows.MessageBoxImage.Question);
        //        if (result == System.Windows.MessageBoxResult.Yes)
        //        {
        //            configFilename = fileOpsService.OpenSaveFileDialogForTtsBrowserConfigGetFilename(ownerViewModel);
        //            if (configFilename != null)
        //            {
        //                if (!File.Exists(configFilename))
        //                    File.WriteAllText(configFilename, GetSampleConfigScriptFromResources());
        //                fileOpsService.SaveLastTtsBrowserConfigFilename(configFilename);
        //            }
        //        }
        //    }
        //    return configFilename;
        //}

        private string IdeStateFilename => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            Constants.MyFilenames.LocalDataDir_StateFile
            );

        public MyAppState LoadOrCreateNewAppState()
        {
            if (!File.Exists(IdeStateFilename))
                return SaveAppState(new MyAppState());
            return JsonUtils.LoadFromFile<MyAppState>(IdeStateFilename);
        }

        public void SaveAppState_FilenameXshd(string filename)
        {
            var appState = LoadOrCreateNewAppState();
            appState.LastFilenameXshd = filename;
            SaveAppState(appState);
        }

        public void SaveAppState_FilenameSample(string filename)
        {
            var appState = LoadOrCreateNewAppState();
            appState.LastFilenameSample = filename;
            SaveAppState(appState);
        }

        //------------------------------------ helpers -------------------------------------------

        private MyAppState SaveAppState(MyAppState ideState)
        {
            if (!File.Exists(IdeStateFilename))
            {
                var dirPath = Path.GetDirectoryName(IdeStateFilename);
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);
            }
            JsonUtils.WriteToFile(ideState, IdeStateFilename);
            return ideState;
        }
    }
}
