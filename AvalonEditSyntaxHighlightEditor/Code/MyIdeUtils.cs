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
using Miktemk.Models;

namespace AvalonEditSyntaxHighlightEditor.Code
{
    public class MyIdeUtils
    {
        public static XmlSyntaxErrorVM GetErrorPositionFromAvalonException(HighlightingDefinitionInvalidException ex, TextDocument codeDocument)
        {
            if (String.IsNullOrEmpty(codeDocument.Text))
                return null;
            if (ex.InnerException != null && ex.InnerException is System.Xml.XmlException)
            {
                var exXml = ex.InnerException as System.Xml.XmlException;
                var errorLine = codeDocument.GetLineByNumber(exXml.LineNumber);
                return new XmlSyntaxErrorVM
                {
                    Message = exXml.Message,
                    Highlight = new WordHighlight(errorLine.Offset, errorLine.Length)
                };
            }
            if (ex.InnerException != null && ex.InnerException is System.Xml.Schema.XmlSchemaValidationException)
            {
                var exXml = ex.InnerException as System.Xml.Schema.XmlSchemaValidationException;
                var errorLine = codeDocument.GetLineByNumber(exXml.LineNumber);
                return new XmlSyntaxErrorVM
                {
                    Message = exXml.Message,
                    Highlight = new WordHighlight(errorLine.Offset, errorLine.Length)
                };
            }

            var regexMatch = TryTheseRegexUntilMatches(ex.Message, new[]
            {
                // https://regex101.com/r/ieg7bu/4
                @"Error at line (\d+):",
                // https://regex101.com/r/9ZbB2G/2
                @"Error at position \(line (\d+)"
            });

            if (regexMatch?.Success?? false)
            {
                var lineNum = regexMatch.Groups[1].Value.ParseIntOrDefault();
                var errorLine = codeDocument.GetLineByNumber(lineNum);
                return new XmlSyntaxErrorVM
                {
                    Message = ex.Message,
                    Highlight = new WordHighlight(errorLine.Offset, errorLine.Length),
                };
            }

            return new XmlSyntaxErrorVM
            {
                Message = ex.Message,
                Highlight = null,
            };
        }

        private static Match TryTheseRegexUntilMatches(string input, string[] regexes)
        {
            return regexes
                .Select(x => Regex.Match(input, x))
                .FirstOrDefault(x => x.Success);
        }
    }
}
