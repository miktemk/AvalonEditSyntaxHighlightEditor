using AvalonEditSyntaxHighlightEditor.Code;
using ICSharpCode.AvalonEdit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;


namespace AvalonEditSyntaxHighlightEditor.Behaviors
{
    public static class AEExtensionMethods
    {
        public static void ApplyNamedWordHighlight(this TextEditor tEdit, WordHighlight highlight, string name, Brush brushFG, Brush brushBG)
        {
            var prevErrorColorizer = tEdit.TextArea.TextView.LineTransformers.FirstOrDefault(x => (x as RegionColorizer)?.Name == name);
            if (prevErrorColorizer != null)
                tEdit.TextArea.TextView.LineTransformers.Remove(prevErrorColorizer);
            if (highlight != null)
            {
                var highlightTransformer = new RegionColorizer(new[] { highlight }, brushFG, brushBG, name);
                tEdit.TextArea.TextView.LineTransformers.Add(highlightTransformer);
            }
        }
    }
}
