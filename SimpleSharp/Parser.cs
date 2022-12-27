using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSharp
{
    public enum NonTerminalStates
    {
        Nothing,
        Head,
        MathAddSub,  //MathAddSub  -> MathMultDiv    + MathAddSub  | MathMultDiv           -  MathAddSub  | MathMultDiv
        MathMultDiv, //MathMultDiv -> MathExpLog     * MathMultDiv | MathExpLog            /  MathMultDiv | MathExpLog
        MathExpLog,  //MathExpLog  -> MathExpression ^ MathExpLog  | MathExpression[base] log MathExpLog  | MathExpression
        MathExpression, //MathExpression -> (MathAddSub) | Identifier[Token] | Number[Token] | AddSub[Token] Number[Token]
    }

    [DebuggerDisplay("{DebugDisplay}")]
    public class ParserNode
    {
        public NonTerminalStates State;
        public Classifications Classification;
        public Token Token;
        public bool IsTerminal;
        public ParserNode[] Children;

        public string DebugDisplay
        {
            get
            {
                string returnString = "";
                for (int i = 0; i < Children.Length; i++)
                {
                    string toAdd;
                    if (Children[i].State != NonTerminalStates.Nothing)
                    {
                        toAdd = "State: " + Children[i].State.ToString() + "   ";
                    }
                    else/* if (Children[i].Token != null)*/
                    {
                        toAdd = "Lexeme: " + Children[i].Token.Lexeme.ToString() + "   ";
                    }
                    //else
                    //{
                    //    toAdd = "Classification: " + Children[i].Classification.ToString() + "   ";
                    //}
                    returnString += $"Node{i} " + toAdd;
                }
                return returnString;
            }
        }

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
            Head = new ParserNode(NonTerminalStates.Head);
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
            //mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.LeftParenthesis), new ParserNode(NonTerminalStates.MathAddSub), new ParserNode(Classifications.RightParenthesis) });
            mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.Identifier) });
            mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.Number) });
            //mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.AddSub), new ParserNode(Classifications.Number) });
            MakeChildrenFunctions.Add(NonTerminalStates.MathExpression, mathExpression);
        }

        public ParserNode Parse()
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
                    if (Tokens[i].Classification != Classifications.WhiteSpace)
                    {
                        currentLine.Add(Tokens[i]);
                    }
                }
            }

            return Head;
        }

        private ParserNode ParseLine(List<Token> targetLine)
        {
            Head.Children = new ParserNode[1] { new ParserNode(NonTerminalStates.MathAddSub) };
            List<Token> result = ParseMathEquation(targetLine, Head.Children[0], true);
            if (result != null)
            {
                return Head;
            }

            return null;
        }

        private List<Token> ParseMathEquation(List<Token> targetEquation, ParserNode current, bool isEnd)
        {
            if (targetEquation.Count == 0)
            {
                if(isEnd)
                {
                    return targetEquation;
                }
                return null;
            }

            if(current.IsTerminal)
            {
                if (current.Classification == targetEquation[0].Classification)
                {
                    current.Token = targetEquation[0];
                    targetEquation.RemoveAt(0);
                    return targetEquation;
                }
                return null;
            }

            var childrenFunctions = MakeChildrenFunctions[current.State];
            foreach(var func in childrenFunctions)
            {
                current.Children = func();
                List<Token> copyEquation = new List<Token>(targetEquation);
                for(int i = 0; i < current.Children.Length; i ++)
                {
                    bool isTheEnd = i == current.Children.Length - 1;
                    copyEquation = ParseMathEquation(copyEquation, current.Children[i], isTheEnd & isEnd);
                    if (copyEquation == null)
                    {
                        break;
                    }
                }
                if(copyEquation != null)
                {
                    return copyEquation;
                }
            }
            return null;
        }
    }
}
