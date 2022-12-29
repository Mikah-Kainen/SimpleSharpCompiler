﻿using System;
using System.Text.RegularExpressions;

namespace SimpleSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string moreCode = "/**/ declare intdeclare declareind for logg+er log3 log(3)/ *+^  + /*foreach(foreach)foreach; */ for 123 - -32 int string invalid((( char each \n //I like to make Money;;\n char + 5";

            string coolCode = "1 / - 1 log 1 - 1 * + 1 /*hi*/;";

            string knownExample = "1 / 1 + 1 ^ 1 * 1 - 1;";

            string errorCode = "1 - 1;";

            Lexer lexer = new Lexer(knownExample);
            var tokenList = lexer.Tokenize();

            Parser parser = new Parser(tokenList);
            var parsedStuff = parser.Parse();

            DisplayTree(parsedStuff, "C");
            Console.WriteLine();

            AbstractSyntaxTree AST = new AbstractSyntaxTree(parsedStuff);
            var abstractSyntaxTree = AST.BuildAST();

            DisplayTree(abstractSyntaxTree, "A");

        }

        static void DisplayTree(ParserNode tree, string spaces)
        {
            string display;
            if (tree.Token != null)
            {
                display = tree.Token.Lexeme.ToString();
            }
            else
            {
                display = tree.Token.Lexeme.ToString();
            }

            Console.WriteLine(spaces + display);
            if (tree.Children == null)
            {
                return;
            }
            foreach (ParserNode child in tree.Children)
            {
                DisplayTree(child, spaces + "| ");
            }
        }

        //CongaLine

        // Operator: ~('-')~
        // AnotherOperator: (╯°□°)╯︵ ┻━┻

        #region TrieTestCase
        //Trie trie = new Trie();
        //trie.Add("hello");
        //trie.Add("hi");
        //trie.Add("help");
        //trie.Add("hell");
        //trie.Add("fi");
        //trie.Add("fi");

        //bool remove1 = trie.Remove("fi");
        //bool remove2 = trie.Remove("help");
        //bool remove3 = trie.Remove("hellos");

        //bool result1 = trie.Contains("help");
        //bool result2 = trie.Contains("hell");
        //bool result3 = trie.Contains("fi");
        //bool result4 = trie.Contains("hel");
        //bool result5 = trie.Contains("$");
        //bool result6 = trie.Contains("bob");
        #endregion
    }
}