using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSharp
{
    //public enum NonTerminalStates
    //{
    //    Nothing,
    //    Head,
    //    MathAddSub,  //MathAddSub  -> MathMultDiv    + MathAddSub  | MathMultDiv           -  MathAddSub  | MathMultDiv
    //    MathMultDiv, //MathMultDiv -> MathExpLog     * MathMultDiv | MathExpLog            /  MathMultDiv | MathExpLog
    //    MathExpLog,  //MathExpLog  -> MathExpression ^ MathExpLog  | MathExpression[base] log MathExpLog  | MathExpression
    //    MathExpression, //MathExpression -> (MathAddSub) | Identifier[Token] | AddSub[Token] Identifier[Token] | Number[Token] | AddSub[Token] Number[Token]
    //}

    [DebuggerDisplay("{DebugDisplay}")]
    public class ParserNode
    {
        public const string RootName = "Head";

        //public NonTerminalStates State;
        //public string State;
        public Classifications Classification;
        public Token Token;
        public ParserNode[] Children;
        public bool IsTerminal;
        public bool IsSpecific;
        public bool ForAST
        {
            get
            {
                switch (Classification)
                {
                    case Classifications.WhiteSpace:
                        return false;

                    case Classifications.LeftParenthesis:
                        return false;

                    case Classifications.RightParenthesis:
                        return false;

                    case Classifications.BlockComment:
                        return false;

                    case Classifications.PreciseComment:
                        return false;

                    default:
                        return true;
                }
            }
        }

        public string DebugDisplay
        {
            get
            {
                string display;
                if(IsSpecific)
                {

                }
                if (Token != null)
                {
                    display = Token.Lexeme.ToString();
                }
                else
                {
                    display = State.ToString();
                }
                return display;
            }
        }

        //public ParserNode(NonTerminalStates state)
        //{
        //    State = state;
        //    IsTerminal = false;
        //}
        //public ParserNode(string state)
        //{
        //    State = state;
        //    IsTerminal = false;
        //    IsSpecific = true;
        //}

        public ParserNode(Classifications classification)
        {
            Classification = classification;
            IsTerminal = true;
            switch(classification)
            {
                case Classifications.Head:
                    IsTerminal = false;
                    break;

                case Classifications.Expression:
                    IsSpecific = true;
                    break;

                default:
                    IsSpecific = false;
                    break;
            }

        }

        public ParserNode(Classifications classification, bool isSpecific)
            :this(classification)
        {
            IsSpecific = isSpecific;
        }

    }

    public class Parser
    {
        public Token[] Tokens;
        public ParserNode Head;
        public ParserGenerator ParserGenerator;
        //public Dictionary<NonTerminalStates, List<Func<ParserNode[]>>> MakeChildrenFunctions;
        public Dictionary<string, List<Func<ParserNode[]>>> MakeChildrenFunctions => ParserGenerator.MakeChildrenFunctions;

        //MathAddSub  -> MathMultDiv    + MathAddSub  | MathMultDiv           -  MathAddSub  | MathMultDiv
        //MathMultDiv -> MathExpLog     * MathMultDiv | MathExpLog            /  MathMultDiv | MathExpLog
        //MathExpLog  -> MathExpression ^ MathExpLog  | MathExpression[base] log MathExpLog  | MathExpression
        //MathExpression -> (MathAddSub) | Identifier[Token] | AddSub[Token] Identifier[Token] | Number[Token] | AddSub[Token] Number[Token]

        public Parser(Token[] tokens)
        {
            Tokens = tokens;
            Head = new ParserNode(ParserNode.RootName);
            ParserGenerator = new ParserGenerator();

            ParserGenerator.AddRule("MathAddSub  -> MathMultDiv    + MathAddSub  | MathMultDiv           - MathAddSub   | MathMultDiv;");
            ParserGenerator.AddRule("MathMultDiv -> MathExpLog     * MathMultDiv | MathExpLog            /  MathMultDiv | MathExpLog;");
            ParserGenerator.AddRule("MathExpLog  -> MathExpression ^ MathExpLog  | MathExpression[base] log MathExpLog  | MathExpression;");
            ParserGenerator.AddRule("MathExpression -> (MathAddSub) | Identifier | AddSub Identifier | Number | AddSub Number;");
            //Head = new ParserNode(NonTerminalStates.Head);
            //MakeChildrenFunctions = new Dictionary<NonTerminalStates, List<Func<ParserNode[]>>>();

            //List<Func<ParserNode[]>> mathAddSub = new List<Func<ParserNode[]>>();
            //mathAddSub.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathMultDiv), new ParserNode(Classifications.AddSub), new ParserNode(NonTerminalStates.MathAddSub) });
            //mathAddSub.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathMultDiv) });
            //MakeChildrenFunctions.Add(NonTerminalStates.MathAddSub, mathAddSub);

            //List<Func<ParserNode[]>> mathMultDiv = new List<Func<ParserNode[]>>();
            //mathMultDiv.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathExpLog), new ParserNode(Classifications.MultDiv), new ParserNode(NonTerminalStates.MathMultDiv) });
            //mathMultDiv.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathExpLog) });
            //MakeChildrenFunctions.Add(NonTerminalStates.MathMultDiv, mathMultDiv);

            //List<Func<ParserNode[]>> mathExpLog = new List<Func<ParserNode[]>>();
            //mathExpLog.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathExpression), new ParserNode(Classifications.ExpLog), new ParserNode(NonTerminalStates.MathExpLog) });
            //mathExpLog.Add(() => new ParserNode[] { new ParserNode(NonTerminalStates.MathExpression) });
            //MakeChildrenFunctions.Add(NonTerminalStates.MathExpLog, mathExpLog);

            //List<Func<ParserNode[]>> mathExpression = new List<Func<ParserNode[]>>();
            //mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.LeftParenthesis), new ParserNode(NonTerminalStates.MathAddSub), new ParserNode(Classifications.RightParenthesis) });
            //mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.Identifier) });
            //mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.AddSub), new ParserNode(Classifications.Identifier) });
            //mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.Number) });
            //mathExpression.Add(() => new ParserNode[] { new ParserNode(Classifications.AddSub), new ParserNode(Classifications.Number) });
            //MakeChildrenFunctions.Add(NonTerminalStates.MathExpression, mathExpression);
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
                    if (!Token.IsIgnorable(Tokens[i].Classification))
                    {
                        currentLine.Add(Tokens[i]);
                    }
                }
            }

            return Head;
        }

        private ParserNode ParseLine(List<Token> targetLine)
        {
            Head.Children = new ParserNode[1] { new ParserNode("MathAddSub") };
            List<Token> result = ParseMathEquation(targetLine, Head.Children[0], true);
            if (result != null)
            {
                return Head;
            }
            else
            {
                throw new Exception("Failed Parse");
            }

            return null;
        }

        private List<Token> ParseMathEquation(List<Token> targetEquation, ParserNode current, bool isEnd)
        {
            if (targetEquation.Count == 0)
            {
                if (isEnd)
                {
                    return targetEquation;
                }
                throw new Exception("Improper Parse");
                return null;
            }

            if (current.IsTerminal)
            {
                if (current.Classification == targetEquation[0].Classification)
                {
                    current.Token = targetEquation[0];
                    targetEquation.RemoveAt(0);
                    if (!isEnd & targetEquation.Count == 0)
                    {
                        return null;
                    }
                    return targetEquation;
                }
                return null;
            }

            var childrenFunctions = MakeChildrenFunctions[current.State];
            foreach (var func in childrenFunctions)
            {
                current.Children = func();
                List<Token> copyEquation = new List<Token>(targetEquation);
                for (int i = 0; i < current.Children.Length; i++)
                {
                    bool isTheEnd = i == current.Children.Length - 1;
                    copyEquation = ParseMathEquation(copyEquation, current.Children[i], isTheEnd & isEnd);
                    if (copyEquation == null)
                    {
                        break;
                    }
                }
                if (copyEquation != null)
                {
                    if (isEnd & copyEquation.Count != 0)
                    {
                        throw new Exception("FAIL");
                    }
                    return copyEquation;
                }
            }
            return null;
        }
    }
}
