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
using ICSharpCode.AvalonEdit.Document;
using System.Text.RegularExpressions;
using Miktemk;

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

        public static WordHighlight GetErrorPositionFromAvalonException(HighlightingDefinitionInvalidException ex, TextDocument codeDocument)
        {
            if (ex.InnerException != null && ex.InnerException is System.Xml.XmlException)
            {
                var exXml = ex.InnerException as System.Xml.XmlException;
                var errorLine = codeDocument.GetLineByNumber(exXml.LineNumber);
                return new WordHighlight(errorLine.Offset, errorLine.Length);
            }
            if (ex.InnerException != null && ex.InnerException is System.Xml.Schema.XmlSchemaValidationException)
            {
                var exXml = ex.InnerException as System.Xml.Schema.XmlSchemaValidationException;
                var errorLine = codeDocument.GetLineByNumber(exXml.LineNumber);
                return new WordHighlight(errorLine.Offset, errorLine.Length);
            }
            else
            {
                // https://regex101.com/r/ieg7bu/1
                var regexMatch = Regex.Match(ex.Message, @"Error at line (\d+):");
                if (regexMatch.Success)
                {
                    var lineNum = regexMatch.Groups[1].Value.ParseIntOrDefault();
                    var errorLine = codeDocument.GetLineByNumber(lineNum);
                    return new WordHighlight(errorLine.Offset, errorLine.Length);
                }
            }
            return null;
        }

    }
}
