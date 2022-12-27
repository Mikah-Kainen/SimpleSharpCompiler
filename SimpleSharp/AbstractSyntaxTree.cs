using System;
using System.Collections.Generic;
using System.Linq;
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
            return BuildAST(ConcreteSyntaxTree);
        }

        private ParserNode BuildAST(ParserNode parentNode)
        {
            if(parentNode.Children.Length == 0)
            {

            }

            foreach (ParserNode child in parentNode.Children)
            {
                BuildAST(child);
            }
        }

    }
}
