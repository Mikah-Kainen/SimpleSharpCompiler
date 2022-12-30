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
        Comparison,
        Equals,
        AddSub,
        MultDiv,
        ExpLog,
        LeftParenthesis,
        RightParenthesis,
        LeftCurlyBrackets,
        RightCurlyBrackets,
        QuotationMark,
        CodeSeparator,
        Declare,
        Type,
        Number,
        Identifier,
        Invalid,
        Root,
        Expression,
    };

    [DebuggerDisplay("{Lexeme} : {Classification}")]
    public class Token
    {
        public ReadOnlyMemory<char> Lexeme;
        public Classifications Classification;

        public static int ClassificationStartIndex = (int)Classifications.WhiteSpace;
        public static int ClassificationEndIndex = (int)Classifications.Invalid;


        public static bool ShouldBeInAST(Classifications classification)
        {
            switch (classification)
            {
                case Classifications.WhiteSpace:
                    return false;

                case Classifications.LeftParenthesis:
                    return false;

                case Classifications.RightParenthesis:
                    return false;

                case Classifications.BlockComment:
                    return false;

                case Classifications.PreciseComment:
                    return false;

                case Classifications.LeftCurlyBrackets:
                    return false;

                case Classifications.RightCurlyBrackets:
                    return false;

                case Classifications.CodeSeparator:
                    return false;

                case Classifications.Root:
                    return false;

                case Classifications.Expression:
                    return false;

                case Classifications.QuotationMark:
                    return false;

                default:
                    return true;
            }
        }

        public static bool IsTerminal(Classifications classification)
        {
            switch (classification)
            {
                case Classifications.Root:
                    return false;

                case Classifications.Expression:
                    return false;

                default:
                    return true;
            }
        }

        public static bool IsIgnorable(Classifications classification)
        {
            switch(classification)
            {
                case Classifications.WhiteSpace:
                    return true;

                case Classifications.BlockComment:
                    return true;

                case Classifications.PreciseComment:
                    return true;

                default:
                    return false;
            }
        }


        public Token(ReadOnlyMemory<char> lexeme, Classifications classification)
        {
            Lexeme = lexeme;
            Classification = classification;
        }
    }
}
