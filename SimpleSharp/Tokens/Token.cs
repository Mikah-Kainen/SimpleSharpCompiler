using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSharp.Tokens
{
    public enum Classifications
    {
        WhiteSpace = 0,
        BlockComment,
        PreciseComment,
        Keyword,
        Operator,
        LeftParenthesis,
        RightParenthesis,
        Semicolon,
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
