﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSharp
{
    public class Lexer
    {
        public ReadOnlyMemory<char> Memory;
        private Dictionary<string, Func<ReadOnlyMemory<char>, Token>> tokenDictionary;

        private Token GenerateOperatorToken(ReadOnlyMemory<char> lexeme) => new Token(lexeme, Classifications.Operator);
        private Token GenerateKeywordToken(ReadOnlyMemory<char> lexeme) => new Token(lexeme, Classifications.Keyword);
        private Token GenerateTypeToken(ReadOnlyMemory<char> lexeme) => new Token(lexeme, Classifications.Type);

        public Lexer(string code)
        {
            Memory = new ReadOnlyMemory<char>(code.ToCharArray());
            tokenDictionary = new Dictionary<string, Func<ReadOnlyMemory<char>, Token>>();

            #region Operators
            tokenDictionary.Add("+", GenerateOperatorToken);
            tokenDictionary.Add("-", GenerateOperatorToken);
            tokenDictionary.Add("*", GenerateOperatorToken);
            tokenDictionary.Add("/", GenerateOperatorToken);
            #endregion
            #region Keywords
            tokenDictionary.Add("for", GenerateKeywordToken);
            tokenDictionary.Add("each", GenerateKeywordToken);
            tokenDictionary.Add("foreach", GenerateKeywordToken);
            #endregion
            #region Types
            tokenDictionary.Add("int", GenerateTypeToken);
            tokenDictionary.Add("char", GenerateTypeToken);
            tokenDictionary.Add("string", GenerateTypeToken);
            #endregion

            //tokenDictionary.Add("", new Token(new ReadOnlyMemory<char>(new char[] { '' }), Classifications));
            //tokenDictionary.Add("", new Token(new ReadOnlyMemory<char>(new char[] { '' }), Classifications));
            //tokenDictionary.Add("", new Token(new ReadOnlyMemory<char>(new char[] { '' }), Classifications));

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
            int previousIndex = 0;
            for (int currentIndex = 0; currentIndex < Memory.Length; currentIndex++)
            {
                char newwestChar = Memory.Slice(currentIndex, 1).ToString()[0];
                if (previousIndex != currentIndex && (newwestChar == ' ' || newwestChar == '\n' || currentIndex + 1 == Memory.Length || !IsPossibleToken(Memory.Slice(previousIndex, currentIndex - previousIndex).ToString())))
                {
                    Token currentToken = GetToken(previousIndex, currentIndex - previousIndex);
                    if (currentToken != null)
                    {
                        tokens.Add(currentToken);
                        previousIndex = currentIndex;
                    }
                    else
                    {
                        throw new Exception("Space Too Early"); //add in the concept of separators so I don't have to predict if something might be tokenizable
                    }
                }

            }

            return tokens.ToArray();
        }

        private Token GetToken(int currentIndex, int length)
        {
            ReadOnlyMemory<char> memory = Memory.Slice(currentIndex, length);
            string input = memory.ToString();
            if (input[0] >= 48 && input[0] <= 57)
            {
                return new Token(memory, Classifications.Number);
            }
            if (tokenDictionary.ContainsKey(input))
            {
                return tokenDictionary[input](memory);
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private char GetNextCharacter(int index)
        {
            return Memory.Slice(index, 1).ToString()[0];
        }

        //private int GetNextValidCharacterIndex(int currentIndex)
        //{
        //    for (currentIndex++; currentIndex < Memory.Length && GetNextCharacter(currentIndex) == '\n'; currentIndex++)
        //    {
        //    }

        //    if (currentIndex == Memory.Length)
        //    {
        //        return -1;
        //    }
        //    return currentIndex;
        //}

        private bool IsPossibleToken(string input)
        {
            if (input[0] >= 48 & input[0] <= 57 & input[input.Length - 1] >= 48 & input[input.Length - 1] <= 57)
            {
                return true;
            }
            if (tokenDictionary.ContainsKey(input))
            {
                return true;
            }
            foreach (var kvp in tokenDictionary)
            {
                string currentString = kvp.Key;
                if (currentString.Length > input.Length)
                {
                    for (int i = 0; i < input.Length; i++)
                    {
                        if (currentString[i] != input[i])
                        {
                            break;
                        }
                        if (i == input.Length - 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
