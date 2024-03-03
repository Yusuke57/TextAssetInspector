using System.Collections.Generic;
using System.Text;

namespace Yusuke57.CommonPackage.Editor.TextAssetCustom
{
    public class CsvParser
    {
        private bool _isInQuote = false;

        private const char Comma = ',';
        private const char DoubleQuote = '"';
        private const char LineFeed = '\n';
        private const char CarriageReturn = '\r';
        
        public List<List<string>> Fields { get; }
        public int RowCount => Fields.Count;

        public CsvParser(string text)
        {
            Fields = new List<List<string>>();
            Parse(text);
        }
        
        public string GetRawText(int beginLine, int count)
        {
            var stringBuilder = new StringBuilder();
            for (var i = beginLine; i < beginLine + count; i++)
            {
                stringBuilder.AppendLine(string.Join(",", Fields[i]));
            }
            return stringBuilder.ToString();
        }

        private void Parse(string text)
        {
            var columns = new List<string>();
            var fieldStringBuilder = new StringBuilder();

            // 1文字ずつ処理
            var textLength = text.Length;
            for (var i = 0; i < textLength; i++)
            {
                var character = text[i];
                var nextCharacter = i + 1 < text.Length ? text[i + 1] : '\0';

                if (_isInQuote)
                {
                    switch (character)
                    {
                        case DoubleQuote when nextCharacter != DoubleQuote:
                            fieldStringBuilder.Append(DoubleQuote);
                            _isInQuote = false;
                            break;
                        case DoubleQuote when nextCharacter == DoubleQuote:
                            fieldStringBuilder.Append(DoubleQuote);
                            break;
                        default:
                            fieldStringBuilder.Append(character);
                            break;
                    }
                    continue;
                }

                switch (character)
                {
                    case DoubleQuote:
                        fieldStringBuilder.Append(DoubleQuote);
                        _isInQuote = true;
                        continue;
                    case Comma:
                        columns.Add(fieldStringBuilder.ToString());
                        fieldStringBuilder.Clear();
                        continue;
                    case LineFeed:
                    case CarriageReturn:
                        columns.Add(fieldStringBuilder.ToString());
                        Fields.Add(columns);
                        columns = new List<string>();
                        fieldStringBuilder.Clear();
                        if (character == CarriageReturn && nextCharacter == LineFeed)
                        {
                            ++i;
                        }
                        continue;
                    default:
                        fieldStringBuilder.Append(character);
                        continue;
                }
            }
        }
    }
}