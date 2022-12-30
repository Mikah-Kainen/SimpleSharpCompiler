using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleSharp
{

    public class Lexer
    {
        public static string[] RegexStrings =
        {
            /*[(int)Classifications.WhiteSpace] =         */@"(\s+)",
            /*[(int)Classifications.BlockComment] =       */@"(\/\/.*?)\n",
            /*[(int)Classifications.PreciseComment] =     */@"(\/\*.*?\*\/)",//maybe include /s at the end
            /*[(int)Classifications.Keyword] =            */@"(for|each|foreach|if|end)\b",
            /*[(int)Classifications.Comparison] =         */@"(>|<|==)",
            /*[(int)Classifications.Equals] =             */@"(=)",
            /*[(int)Classifications.AddSub] =             */@"([-+])",
            /*[(int)Classifications.MultDiv] =            */@"([*\/])",
            /*[(int)Classifications.ExpLog] =             */@"((\^|\blog\b))",
            /*[(int)Classifications.LeftParenthesis] =    */@"([(])",
            /*[(int)Classifications.RightParenthesis] =   */@"([)])",
            /*[(int)Classifications.LeftCurlyBrackets] =  */@"({)",
            /*[(int)Classifications.RightCurlyBrackets] = */@"(})",
            /*[(int)Classifications.QuotationMarks] =     */"(\")",
            /*[(int)Classifications.CodeSeparator] =      */@"(;)",
            /*[(int)Classifications.Declare] =            */@"\b(declare)",
            /*[(int)Classifications.Type] =               */@"(int|string|char|bool)\b",
            /*[(int)Classifications.Number] =             */@"(-??\d+)",
            /*[(int)Classifications.Identifier] =         */@"(\w+)\b",
            /*[(int)Classifications.Invalid] =            */@"(.+?)\b",
        };

        //Cool Websites:
        //https://www.ibm.com/docs/en/developer-for-zos/14.1?topic=file-regular-expressions
        //https://regex101.com/

        public ReadOnlyMemory<char> Memory;

        public Lexer(string code)
        {
            Memory = new ReadOnlyMemory<char>(code.ToCharArray());

        }

        public Token[] Tokenize()
        {
            List<Token> tokens = new List<Token>();
            string memory = Memory.ToString();
            int currentIndex = 0;

            while (currentIndex < Memory.Length)
            {
                for (Classifications classification = (Classifications)Token.ClassificationStartIndex; (int)classification <= Token.ClassificationEndIndex; classification++)
                {
                    Regex currentRegex = new Regex(RegexStrings[(int)classification]);
                    Match result = currentRegex.Match(memory, currentIndex);

                    if (result != Match.Empty && result.Index == currentIndex)
                    {
                        tokens.Add(new Token(Memory.Slice(currentIndex, result.Groups[1].Value.Length), classification));
                        currentIndex += result.Groups[1].Value.Length;
                        break;
                    }
                }
            }

            return tokens.ToArray();
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char GetNextCharacter(int index)
        {
            return Memory.Slice(index, 1).ToString()[0];
        }
    }
}
