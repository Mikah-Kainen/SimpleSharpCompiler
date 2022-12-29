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
        public Token Token;
        public ParserNode[] Children;
        public bool IsSpecific;

        public bool ShouldBeInAST => Token.ShouldBeInAST(Token.Classification);
        public bool IsTerminal => Token.IsTerminal(Token.Classification);
        public bool IsIgnorable => Token.IsIgnorable(Token.Classification);

        public string DebugDisplay
        {
            get
            {
                string display;
                if (Token.Lexeme.ToString() != "")
                {
                    if (IsSpecific)
                    {
                        display = Token.Lexeme.ToString();
                    }
                    else
                    {
                        display = Token.Classification.ToString();
                    }
                }
                else
                {
                    display = Token.Classification.ToString();
                }
                return display;
            }
        }

        public ParserNode(Classifications classification)
            : this(new Token(new ReadOnlyMemory<char>(new char[0]), classification), false)
        {
        }

        public ParserNode(Token token, bool isSpecific)
        {
            Token = token;
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
            Head = new ParserNode(Classifications.Root);
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
            Head.Children = new ParserNode[1] { new ParserNode(new Token(new ReadOnlyMemory<char>("MathAddSub".ToCharArray()), Classifications.Expression), true) };
            List<Token> result = ParseEquation(targetLine, Head.Children[0], true);
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

        //Make sure this equation factors in the IsSpecific bool!
        private List<Token> ParseEquation(List<Token> targetEquation, ParserNode current, bool isEnd)
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
                if (!isEnd & targetEquation.Count == 0)
                {
                    return null;
                }
                bool doTokensMatch = (current.Token.Classification == targetEquation[0].Classification && (!current.IsSpecific || /*these checks in particular killed my code*/current.Token.Lexeme.ToString() == targetEquation[0].Lexeme.ToString()));
                //This broke my code
                int error
                if (doTokensMatch)
                {
                    current.Token = targetEquation[0];
                    targetEquation.RemoveAt(0);
                    return targetEquation;
                }
                else
                {
                    return null;
                }
            }

            var childrenFunctions = MakeChildrenFunctions[current.Token.Lexeme.ToString()];
            foreach (var func in childrenFunctions)
            {
                current.Children = func();
                List<Token> copyEquation = new List<Token>(targetEquation);
                for (int i = 0; i < current.Children.Length; i++)
                {
                    bool isTheEnd = i == current.Children.Length - 1;
                    copyEquation = ParseEquation(copyEquation, current.Children[i], isTheEnd & isEnd);
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
