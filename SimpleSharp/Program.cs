using System.Text.RegularExpressions;

namespace SimpleSharp
{
    internal class Program
    {
        static void Main(string[] args) 
        {
            string moreCode = "/**/ for logg+er log3 log(3)/ *+^  + /*foreach(foreach)foreach; */ for 123 - -32 int string invalid((( char each \n //I like to make Money\n char + 5";
            Lexer lexer = new Lexer(moreCode);

            var result2 = lexer.Tokenize();
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