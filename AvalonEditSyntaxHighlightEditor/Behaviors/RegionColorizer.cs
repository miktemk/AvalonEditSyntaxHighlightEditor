using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using AvalonEditSyntaxHighlightEditor.Code;

namespace AvalonEditSyntaxHighlightEditor.Behaviors
{
    public class RegionColorizer : DocumentColorizingTransformer
    {
        private IEnumerable<WordHighlight> regionsToColor;
        private Brush brushFG;
        private Brush brushBG;
        private string _name;

        public string Name => _name;

        public RegionColorizer(IEnumerable<WordHighlight> regionsToColor, Brush brushFG, Brush brushBG, string name = null)
        {
            this.regionsToColor = regionsToColor;
            this.brushFG = brushFG;
            this.brushBG = brushBG;
            this._name = name;
        }

        protected override void ColorizeLine(DocumentLine line)
        {
            var offset = line.Offset;
            var endOffset = line.EndOffset;
            // NOTE: need to check different kinds of intersections, otherwise only the line that
            //       contains x.StartIndex will be highlighted for those regions that encompass multiple lines
            var regionsThatApply = regionsToColor.Where(x =>
                (offset <= x.StartIndex && x.StartIndex <= endOffset) ||
                (offset <= x.StartIndex + x.Length && x.StartIndex + x.Length <= endOffset) ||
                (x.StartIndex <= offset && offset <= x.StartIndex + x.Length)
            );
            foreach (var region in regionsThatApply)
            {
                var start = Math.Max(region.StartIndex, offset);
                var end = Math.Min(region.StartIndex + region.Length, endOffset);
                ChangeLinePart((int)start, (int)end, ApplyChanges);
            }
        }

        void ApplyChanges(VisualLineElement element)
        {
            // This is where you do anything with the line
            if (brushFG != null)
                element.TextRunProperties.SetForegroundBrush(brushFG);
            if (brushBG != null)
                element.TextRunProperties.SetBackgroundBrush(brushBG);
        }
    }
}