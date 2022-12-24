using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SimpleSharp.Tokens;

namespace SimpleSharp
{
    public class Lexer
    {
        //Cool Websites:
        //https://www.ibm.com/docs/en/developer-for-zos/14.1?topic=file-regular-expressions
        //https://regex101.com/

        public ReadOnlyMemory<char> Memory;
        public string[] RegexStrings;

        //WhiteSpace,
        //Keyword,
        //Operator,
        //LeftParenthesis,
        //RightParenthesis,
        //Type,
        //Number,
        //Comment,
        //Identifier,
        //Invalid,
        //EndOfCode,

        //E -> E op E
        //   | (E)
        //   | id

        public Lexer(string code)
        {
            Memory = new ReadOnlyMemory<char>(code.ToCharArray());
            RegexStrings = new string[(int)Classifications.Invalid + 1];
            RegexStrings[(int)Classifications.WhiteSpace] = @"(\s)+";
            RegexStrings[(int)Classifications.BlockComment] = @"(\/\/.*?)\n";
            RegexStrings[(int)Classifications.PreciseComment] = @"(\/\*.*?\*\/)\s"; //This regex does not work. I need a way to capture all charcters including the breaking characters
            RegexStrings[(int)Classifications.Keyword] = @"(for|each|foreach|if)\b";
            RegexStrings[(int)Classifications.Operator] = @"([-+*\/])";
            RegexStrings[(int)Classifications.LeftParenthesis] =  @"([(])"; 
            RegexStrings[(int)Classifications.RightParenthesis] = @"([)])";
            RegexStrings[(int)Classifications.Semicolon] = @"(;)";
            RegexStrings[(int)Classifications.Type] = @"(int|string|char)\b";
            RegexStrings[(int)Classifications.Number] = @"(-??\d+)";
            RegexStrings[(int)Classifications.Identifier] = @"(\w+)\b";
            RegexStrings[(int)Classifications.Invalid] = @"(.+?)\b";
        }
        #region TestForIsPossibleToken
        //tokenDictionary.Add("Bob", new Token(new ReadOnlyMemory<char>(new char[] { '/' }), Classifications.Operator));
        //tokenDictionary.Add("Robert", new Token(new ReadOnlyMemory<char>(new char[] { '/' }), Classifications.Operator));
        //tokenDictionary.Add("Rob", new Token(new ReadOnlyMemory<char>(new char[] { '/' }), Classifications.Operator));
        //tokenDictionary.Add("robber", new Token(new ReadOnlyMemory<char>(new char[] { '/' }), Classifications.Operator));
        //tokenDictionary.Add("rob", new Token(new ReadOnlyMemory<char>(new char[] { '/' }), Classifications.Operator));

        //bool result1 = IsPossibleKey("ro");
        //bool result2 = IsPossibleKey("Rob");
        //bool result3 = IsPossibleKey("Robe");

        //bool result4 = IsPossibleKey("bob");
        //bool result5 = IsPossibleKey("++");
        //bool result6 = IsPossibleKey("seven");
        #endregion

        public Token[] Tokenize()
        {
            List<Token> tokens = new List<Token>();
            string memory = Memory.ToString();
            int currentIndex = 0;

            while (currentIndex < Memory.Length)
            {
                for (Classifications classification = 0; (int)classification <= (int)Classifications.Invalid; classification++)
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

        //public Token[] Tokenize()
        //{
        //    List<Token> tokens = new List<Token>();
        //    int previousIndex = 0;
        //    for (int currentIndex = 0; currentIndex < Memory.Length; currentIndex++)
        //    {
        //        char newwestChar = Memory.Slice(currentIndex, 1).ToString()[0];
        //        if (previousIndex != currentIndex && (newwestChar == ' ' || newwestChar == '\n' || currentIndex + 1 == Memory.Length || !IsPossibleToken(Memory.Slice(previousIndex, currentIndex - previousIndex).ToString())))
        //        {
        //            Token currentToken = GetToken(previousIndex, currentIndex - previousIndex);
        //            if (currentToken != null)
        //            {
        //                tokens.Add(currentToken);
        //                previousIndex = currentIndex;
        //            }
        //            else
        //            {
        //                throw new Exception("Space Too Early"); //add in the concept of separators so I don't have to predict if something might be tokenizable
        //            }
        //        }

        //    }

        //    return tokens.ToArray();
        //}

        //private Token GetToken(int currentIndex, int length)
        //{
        //    ReadOnlyMemory<char> memory = Memory.Slice(currentIndex, length);
        //    string input = memory.ToString();
        //    if (input[0] >= 48 && input[0] <= 57)
        //    {
        //        return new Token(memory, Classifications.Number);
        //    }
        //    if (tokenDictionary.ContainsKey(input))
        //    {
        //        return tokenDictionary[input](memory);
        //    }

        //    return null;
        //}


        //private bool IsPossibleToken(string input)
        //{
        //    if (input[0] >= 48 & input[0] <= 57 & input[input.Length - 1] >= 48 & input[input.Length - 1] <= 57)
        //    {
        //        return true;
        //    }
        //    if (tokenDictionary.ContainsKey(input))
        //    {
        //        return true;
        //    }
        //    foreach (var kvp in tokenDictionary)
        //    {
        //        string currentString = kvp.Key;
        //        if (currentString.Length > input.Length)
        //        {
        //            for (int i = 0; i < input.Length; i++)
        //            {
        //                if (currentString[i] != input[i])
        //                {
        //                    break;
        //                }
        //                if (i == input.Length - 1)
        //                {
        //                    return true;
        //                }
        //            }
        //        }
        //    }
        //    return false;
        //}
    }
}
