using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Miktemk.Wpf.Core.Behaviors.VM;

namespace AvalonEditSyntaxHighlightEditor.Code
{
    public static class Constants
    {
        public static class Resources
        {
            // TODO: find a t4 template to generate this from contents of "Resource" folder
            public const string SyntaxXshd = "AvalonEditSyntaxHighlightEditor.Resources.syntax.xshd";
        }

        public static class MyFilenames
        {
            //public static readonly string LocalDataDir_StateFile = $@"{Miktemk.Properties.Settings.Default.LocalDataDir}\tts-browser-state.json";
            public static readonly string LocalDataDir_StateFile = $@"miktemk\AvalonEditSyntaxHighlightEditor\app-state.json";
        }

        public static class Config
        {
            public static readonly UIElementDragDropConfig DragDropConfigXshd = new UIElementDragDropConfig
            {
                Mode = UIElementDragDropMode.FileBased,
                ValidFileExtensions = new [] { ".xshd" },
            };

            public static readonly UIElementDragDropConfig DragDropConfigSample = new UIElementDragDropConfig
            {
                Mode = UIElementDragDropMode.FileBased,
                ValidFileExtensions = null,
            };
        }
    }
}
