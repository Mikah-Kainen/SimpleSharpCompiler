using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSharp
{
    public enum Classifications
    {
        WhiteSpace = 0,
        BlockComment,
        PreciseComment,
        Keyword,
        AddSub,
        MultDiv,
        ExpLog,
        LeftParenthesis,
        RightParenthesis,
        CodeSeparator,
        Type,
        Number,
        Identifier,
        Invalid,
    };

    [DebuggerDisplay("{Lexeme} : {Classification}")]
    public class Token
    {
        public ReadOnlyMemory<char> Lexeme;
        public Classifications Classification;

        public Token(ReadOnlyMemory<char> lexeme, Classifications classification)
        {
            Lexeme = lexeme;
            Classification = classification;
        }
    }
}
