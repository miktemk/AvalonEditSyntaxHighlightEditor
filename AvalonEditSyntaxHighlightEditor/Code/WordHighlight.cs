using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvalonEditSyntaxHighlightEditor.Code
{
    public class WordHighlight
    {
        public WordHighlight(long startChar, int length)
        {
            StartIndex = startChar;
            Length = length;
        }

        public long StartIndex { get; set; }
        public int Length { get; set; }

        public override string ToString()
        {
            return $"[{StartIndex}:{Length}]";
        }
        public override bool Equals(object obj)
        {
            if (!(obj is WordHighlight))
                return false;
            var y = obj as WordHighlight;
            if (StartIndex != y.StartIndex)
                return false;
            if (Length != y.Length)
                return false;
            return true;
        }

        public WordHighlight MakeCopy()
        {
            return new WordHighlight(StartIndex, Length);
        }

        public bool Contains(long pos) => (StartIndex <= pos) && (pos <= StartIndex + Length);
    }
}
