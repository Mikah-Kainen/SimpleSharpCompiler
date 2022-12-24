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
        MathAddSub,  //MathAddSub  -> MathMultDiv    + MathAddSub  | MathMultDiv           -  MathAddSub  | MathMultDiv
        MathMultDiv, //MathMultDiv -> MathExpLog     * MathMultDiv | MathExpLog            /  MathMultDiv | MathExpLog
        MathExpLog,  //MathExpLog  -> MathExpression ^ MathExpLog  | MathExpression[base] log MathExpLog  | MathExpression
        MathExpression, //MathExpression -> (MathAddSub) | Identifier[Token] | Number[Token] | AddSub[Token] Number[Token]
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

        //MathAddSub  -> MathMultDiv    + MathAddSub  | MathMultDiv           -  MathAddSub  | MathMultDiv
        //MathMultDiv -> MathExpLog     * MathMultDiv | MathExpLog            /  MathMultDiv | MathExpLog
        //MathExpLog  -> MathExpression ^ MathExpLog  | MathExpression[base] log MathExpLog  | MathExpression
        //MathExpression -> (MathAddSub) | Identifier[Token] | Number[Token] | AddSub[Token] Number[Token]

        public Parser(Token[] tokens)
        {
            Tokens = tokens;
            Head = Parse();
            MakeChildrenFunctions = new Dictionary<NonTerminalStates, List<Func<ParserNode[]>>>();

            List<Func<ParserNode[]>> mathAddSub = new List<Func<ParserNode[]>>();
            mathAddSub.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathMultDiv), new ParserNode(Classifications.AddSub), new ParserNode(NonTerminalStates.MathAddSub) });
            mathAddSub.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathMultDiv) });
            MakeChildrenFunctions.Add(NonTerminalStates.MathAddSub, mathAddSub);

            List<Func<ParserNode[]>> mathMultDiv = new List<Func<ParserNode[]>>();
            mathMultDiv.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathExpLog), new ParserNode(Classifications.MultDiv), new ParserNode(NonTerminalStates.MathMultDiv) });
            mathMultDiv.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathExpLog) });
            MakeChildrenFunctions.Add(NonTerminalStates.MathMultDiv, mathMultDiv);

            List<Func<ParserNode[]>> mathExpLog = new List<Func<ParserNode[]>>();
            mathExpLog.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathExpression), new ParserNode(Classifications.ExpLog), new ParserNode(NonTerminalStates.MathExpLog) });
            mathExpLog.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathExpression) });
            MakeChildrenFunctions.Add(NonTerminalStates.MathExpLog, mathExpLog);

            List<Func<ParserNode[]>> mathExpression = new List<Func<ParserNode[]>>();
            mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.LeftParenthesis), new ParserNode(NonTerminalStates.MathAddSub), new ParserNode(Classifications.RightParenthesis) });
            mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.Identifier) });
            mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.Number) });
            mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.AddSub), new ParserNode(Classifications.Number) });
            MakeChildrenFunctions.Add(NonTerminalStates.MathExpression, mathExpression);
        }

        private ParserNode Parse()
        {
            List<Token> currentLine = new List<Token>();
            List<ParserNode> parsedLines = new List<ParserNode>();
            for (int i = 0; i < Tokens.Length; i++)
            {
                if (Tokens[i].Classification == Classifications.CodeSeparator)
                {
                    parsedLines.Add(ParseLine(currentLine));
                    currentLine = new List<Token>();
                }
                else
                {
                    currentLine.Add(Tokens[i]);
                }
            }
        }

        private ParserNode ParseLine(List<Token> targetLine)
        {
            int currentIndex = 0;

        }
    }
}
