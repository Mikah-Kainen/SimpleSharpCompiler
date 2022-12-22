using System.Text.RegularExpressions;

namespace SimpleSharp
{
    internal class Program
    {
        static void Main(string[] args) 
        {

            string code = "10+20-5\n2/*";
            string moreCode = "for+foreach(for\neach)foreach a";
            Lexer lexer = new Lexer(moreCode);

            var result2 = lexer.Tokenize();

            ReadOnlyMemory<char> temp = new ReadOnlyMemory<char>(code.ToCharArray());
            var result1 = temp.Slice(0, 1);
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