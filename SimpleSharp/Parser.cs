using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSharp
{
    public enum NonTerminalStates
    {
        MathAddSub,  //MathAddSub  -> MathMultDiv    + MathAddSub     | MathMultDiv           -  MathAddSub      | MathMultDiv
        MathMultDiv, //MathMultDiv -> MathExpLog     * MathMultDiv    | MathExpLog            /  MathMultDiv     | MathExpLog
        MathExpLog,  //MathExpLog  -> MathExpression ^ MathExpression | MathExpression[base] log MathExpression  | MathExpression
        MathExpression, //MathExpression -> (Math1) | Identifier[Token] | Number[Token]
    }

    public class ParserNode
    {
        public NonTerminalStates State;
        public Classifications Classification;
        public bool IsTerminal;
        public ParserNode[] Children;

        public ParserNode(NonTerminalStates state)
        {
            State = state;
            IsTerminal = false;
        }

        public ParserNode(Classifications classification)
        {
            Classification = classification;
            IsTerminal = true;
        }
    }

    public class Parser
    {
        public Token[] Tokens;
        public ParserNode Head;
        public Dictionary<NonTerminalStates, List<Func<ParserNode[]>>> MakeChildrenFunctions;

        //MathAddSub  -> MathMultDiv    + MathAddSub     | MathMultDiv           -  MathAddSub      | MathMultDiv
        //MathMultDiv -> MathExpLog     * MathMultDiv    | MathExpLog            /  MathMultDiv     | MathExpLog
        //MathExpLog  -> MathExpression ^ MathExpression | MathExpression[base] log MathExpression  | MathExpression
        //MathExpression -> (Math1) | Identifier[Token] | Number[Token]

        //public Parser(Token[] tokens)
        //{
        //    Tokens = tokens;
        //    Head = Parse();
        //    MakeChildrenFunctions = new Dictionary<NonTerminalStates, List<Func<ParserNode[]>>>();
        //    List<Func<ParserNode[]>> mathAddSub = new List<Func<ParserNode[]>>();
        //    mathAddSub.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathMultDiv), new ParserNode(Classifications.Operator), new ParserNode(NonTerminalStates.MathAddSub) });
        //    mathAddSub.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathMultDiv), new ParserNode(Classifications.Operator), new ParserNode(NonTerminalStates.MathAddSub) });
        //    mathAddSub.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathMultDiv) });
        //    MakeChildrenFunctions.Add(NonTerminalStates.MathAddSub, );
        //}

        //private ParserNode Parse()
        //{
        //    List<Token> currentLine = new List<Token>();
        //    List<ParserNode> parsedLines = new List<ParserNode>();
        //    for(int i = 0; i < Tokens.Length; i ++)
        //    {
        //        if (Tokens[i].Classification == Classifications.CodeSeparator)
        //        {
        //            parsedLines.Add(ParseLine(currentLine));
        //            currentLine = new List<Token>();
        //        }
        //        else
        //        {
        //            currentLine.Add(Tokens[i]);
        //        }
        //    }
        //}

        //private ParserNode ParseLine(List<Token> targetLine)
        //{
        //    int currentIndex = 0; 

        //}
    }
}
