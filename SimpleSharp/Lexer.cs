using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSharp
{
    public class Lexer
    {
        public ReadOnlyMemory<char> Memory;
        private Dictionary<ReadOnlyMemory<char>, Token> tokenDictionary;

        public Lexer(string code)
        {
            Memory = new ReadOnlyMemory<char>(code.ToCharArray());
            tokenDictionary = new Dictionary<ReadOnlyMemory<char>, Token>();
            tokenDictionary.Add(new ReadOnlyMemory<char>(new char[]{'+'}), new Token(new ReadOnlyMemory<char>(new char[] {'+'}), Classifications.Operator));
        }

        public Token[] Tokenize()
        {
            List<Token> tokens = new List<Token>();
            string current = "";
            for(int currentIndex = 0; currentIndex < Memory.Length; currentIndex ++)
            {
                char newwestChar = Memory.Slice(currentIndex, 1).ToString()[0];
                if(newwestChar == '+')
                {

                }
                if (newwestChar != '\n' && newwestChar != 32)
                {
                    current += newwestChar;
                }
                if (current != "" && newwestChar == 32)
                {
                    Token currentToken = GetToken(current);
                    if (currentToken != null)
                    {
                        tokens.Add(currentToken);
                        currentIndex = currentIndex + 1;
                        current = "";
                    }
                    else
                    {
                        throw new Exception("Space Too Early");
                    }
                }
            }

            return tokens.ToArray();
        }

        private Token GetToken(string input)
        {
            ReadOnlyMemory<char> memory = new ReadOnlyMemory<char>(input.ToCharArray());
            if (input[0] >= 48 && input[0] <= 57)
            {
                return new Token(memory, Classifications.Number);
            }
            if(tokenDictionary.ContainsKey(memory))
            {
                return tokenDictionary[memory];
            }

            return null;
        }
    }
}
