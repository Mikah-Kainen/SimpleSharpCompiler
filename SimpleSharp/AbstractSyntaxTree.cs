using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSharp
{

    public class AbstractSyntaxTree
    {
        public ParserNode ConcreteSyntaxTree;

        public AbstractSyntaxTree(ParserNode concreteSyntaxTree)
        {
            ConcreteSyntaxTree = concreteSyntaxTree;
        }


        public ParserNode BuildAST()
        {
            ParserNode AbstractSyntaxTree = ParserNode.CreateRootNode();
            if (ConcreteSyntaxTree.Children == null)
            {
                return AbstractSyntaxTree;
            }

            AbstractSyntaxTree.Children = new ParserNode[ConcreteSyntaxTree.Children.Length];

            AbstractSyntaxTree = BuildAST(ConcreteSyntaxTree);
            return AbstractSyntaxTree;
        }

        private ParserNode BuildAST(ParserNode currentNode)
        {
            if (currentNode.Children == null || currentNode.Children.Length == 0)
            {
                if (currentNode.ShouldBeInAST)
                {
                    return currentNode;
                }
                return null;
            }

            List<ParserNode> possibleChildren = new List<ParserNode>();
            possibleChildren = currentNode.Children.ToList();

            List<List<ParserNode>> grandChildren = new List<List<ParserNode>>();
            if (ContainsTerminalASTNode(possibleChildren))
            {
                grandChildren.Add(possibleChildren);
            }
            else
            {
                for (int i = 0; i < possibleChildren.Count; i++)
                {
                    if (possibleChildren[i].Children != null)
                    {
                        if (ContainsTerminalASTNode(possibleChildren[i].Children.ToList()))
                        {
                            grandChildren.Add(possibleChildren[i].Children.ToList());
                        }
                        else
                        {
                            foreach(ParserNode child in possibleChildren[i].Children)
                            {
                                possibleChildren.Add(child);
                            }
                        }
                    }

                }
            }

            List<List<ParserNode>> newGrandChildren = new List<List<ParserNode>>();
            List<ParserNode> terminalChildren = new List<ParserNode>();
            foreach (List<ParserNode> childrenGroup in grandChildren)
            {
                bool wasTerminalFound = false;
                List<ParserNode> currentNewGrandChildren = new List<ParserNode>();
                foreach (ParserNode child in childrenGroup)
                {
                    if(child == null)
                    {

                    }    
                    if (child.IsTerminal & child.ShouldBeInAST)
                    {
                        if (!wasTerminalFound)
                        {
                            wasTerminalFound = true;
                            terminalChildren.Add(child);
                        }
                        else
                        {
                            currentNewGrandChildren.Add(child);
                            //throw new Exception("I could just add this node as a child of the terminal child but I was curious to see if this happened");
                        }
                    }
                    else
                    {
                        currentNewGrandChildren.Add(child);
                    }
                }
                newGrandChildren.Add(currentNewGrandChildren);
            }

            if (terminalChildren.Count != newGrandChildren.Count)
            {
                throw new Exception("Mismatched Children(look at count)");
            }
            List<ParserNode> currentNodeChildren = new List<ParserNode>();
            for (int i = 0; i < terminalChildren.Count; i++)
            {
                terminalChildren[i].Children = newGrandChildren[i].ToArray();
                currentNodeChildren.Add(BuildAST(terminalChildren[i]));
            }
            currentNode.Children = currentNodeChildren.ToArray();

            if (currentNode.IsTerminal & currentNode.ShouldBeInAST)
            {
                return currentNode;
            }
            else
            {
                
                //ParserNode returnNode = new ParserNode(NonTerminalStates.Head);
                ParserNode returnNode = new ParserNode(Classifications.Root);
                returnNode.Children = currentNodeChildren.ToArray();
                return returnNode;
            }
        }

        private bool ContainsTerminalASTNode(List<ParserNode> currentNodes)
        {
            foreach (ParserNode child in currentNodes)
            {
                if (child.IsTerminal & child.ShouldBeInAST)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
