using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSharp
{
    public enum Classifications
    {
        None,
        Keyword,
        Type,
        Identifier,
        Operator,
        Number,
    };

    public class Token
    {
        //Forget about spans
        public ReadOnlyMemory<char> Lexeme;
        public Classifications Classification;


        public Token(ReadOnlyMemory<char> lexeme, Classifications classification)
        {
            Lexeme = lexeme;
            Classification = classification;
        }
    }
}
