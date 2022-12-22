//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using SimpleSharp.Tokens;

//namespace SimpleSharp
//{
//    internal class Trie
//    {
//        public class TrieNode
//        {
//            public bool IsWord;
//            public char Character;
//            public Classifications Classification;
//            public Dictionary<char, TrieNode> Children;

//            public TrieNode(char character, Classifications classification)
//            {
//                IsWord = false;
//                Character = character;
//                Children = new Dictionary<char, TrieNode>();
//                Classification = classification;
//            }
//        }

//        private TrieNode head;
//        public int Count;

//        public Trie()
//        {
//            head = new TrieNode('$', Classifications.None);
//        }


//        public bool Add(string newWord, Classifications classification)
//        {
//            TrieNode currentNode = head;
//            bool alreadyContained = true;
//            for (int currentIndex = 0; currentIndex < newWord.Count(); currentIndex++)
//            {
//                if (!alreadyContained || !currentNode.Children.ContainsKey(newWord[currentIndex]))
//                {
//                    currentNode.Children.Add(newWord[currentIndex], new TrieNode(newWord[currentIndex], classification));
//                    alreadyContained = false;
//                }
//                currentNode = currentNode.Children[newWord[currentIndex]];
//            }
//            if (!alreadyContained || currentNode.IsWord == false)
//            {
//                currentNode.IsWord = true;
//                Count++;
//                return true;
//            }
//            return false;
//        }

//        public bool Contains(string targetWord)
//        {
//            TrieNode currentNode = head;
//            for (int currentIndex = 0; currentIndex < targetWord.Count(); currentIndex++)
//            {
//                if (!currentNode.Children.ContainsKey(targetWord[currentIndex]))
//                {
//                    return false;
//                }
//                currentNode = currentNode.Children[targetWord[currentIndex]];
//            }
//            if (currentNode.IsWord)
//            {
//                return true;
//            }
//            return false;
//        }

//        public bool Remove(string targetWord)
//        {
//            if(Remove(head, targetWord, 0))
//            {
//                Count--;
//                return true;
//            }
//            return false;
//        }

//        private bool Remove(TrieNode parent, string targetWord, int currentIndex)
//        {
//            if (currentIndex >= targetWord.Count())
//            {
//                return true;
//            }
//            if (!parent.Children.ContainsKey(targetWord[currentIndex]))
//            {
//                return false;
//            }

//            if(Remove(parent.Children[targetWord[currentIndex]], targetWord, currentIndex + 1))
//            {
//                if (parent.Children[targetWord[currentIndex]].Children.Count() == 0)
//                {
//                    parent.Children.Remove(targetWord[currentIndex]);
//                }
//                return true;
//            }
//            return false;
//        }
//    }
//}
