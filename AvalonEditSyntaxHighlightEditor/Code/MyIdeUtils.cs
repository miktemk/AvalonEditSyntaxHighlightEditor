using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AvalonEditSyntaxHighlightEditor.Code
{
    public class MyIdeUtils
    {
        public static IHighlightingDefinition LoadSyntaxHighlightingFromResource(string resourceName)
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (XmlTextReader xshd_reader = new XmlTextReader(stream))
            {
                return HighlightingLoader.Load(xshd_reader, HighlightingManager.Instance);
            }
        }

        public static IHighlightingDefinition LoadSyntaxHighlightingFromString(string xshdScript)
        {
            using (var reader = new StringReader(xshdScript))
            using (XmlTextReader xshd_reader = new XmlTextReader(reader))
            {
                return HighlightingLoader.Load(xshd_reader, HighlightingManager.Instance);
            }
        }
    }
}
