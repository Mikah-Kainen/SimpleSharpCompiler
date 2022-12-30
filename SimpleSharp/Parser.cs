using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Dynamic;
using System.Formats.Asn1;
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
                return display + $" IsSpecific:{IsSpecific}";
            }
        }

        public static ParserNode CreateRootNode() => new ParserNode(new Token(new ReadOnlyMemory<char>(Parser.StartingState.ToCharArray()), Classifications.Root), true);

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
        public ParserGenerator ParserGenerator;
        public const string StartingState = "Root";
        //public Dictionary<NonTerminalStates, List<Func<ParserNode[]>>> MakeChildrenFunctions;
        public Dictionary<string, List<Func<ParserNode[]>>> GetChildrenFunctions => ParserGenerator.GetChildrenFunctions;

        //MathAddSub  -> MathMultDiv    + MathAddSub  | MathMultDiv           -  MathAddSub  | MathMultDiv
        //MathMultDiv -> MathExpLog     * MathMultDiv | MathExpLog            /  MathMultDiv | MathExpLog
        //MathExpLog  -> MathExpression ^ MathExpLog  | MathExpression[base] log MathExpLog  | MathExpression
        //MathExpression -> (MathAddSub) | Identifier[Token] | AddSub[Token] Identifier[Token] | Number[Token] | AddSub[Token] Number[Token]

        public Parser(Token[] tokens)
        {
            Tokens = tokens;
            ParserGenerator = new ParserGenerator();

            string numberOrIdentifier = "NumberOrIdentifier";
            ParserGenerator.AddRule($"{numberOrIdentifier} -> {Classifications.Identifier.ToString()} | {Classifications.AddSub.ToString()} {Classifications.Identifier.ToString()} | {Classifications.Number.ToString()} | {Classifications.AddSub.ToString()} {Classifications.Number.ToString()};");

            string allocateInt = "AllocateInt";
            string addSubStage = "MathAddSub";
            string multDivStage = "MathMultDiv";
            string expLogStage = "MathExpLog";
            string finalValuesStage = "MathExpression";

            ParserGenerator.AddRule($"{allocateInt} -> int {Classifications.Identifier} = {addSubStage};");
            ParserGenerator.AddRule($"{addSubStage} -> {multDivStage} {Classifications.AddSub.ToString()}  {addSubStage} | {multDivStage};");
            ParserGenerator.AddRule($"{multDivStage} -> {expLogStage} {Classifications.MultDiv.ToString()} {multDivStage} | {expLogStage};");
            ParserGenerator.AddRule($"{expLogStage} -> {finalValuesStage} {Classifications.ExpLog.ToString()}  {expLogStage} | {finalValuesStage};");
            ParserGenerator.AddRule($"{finalValuesStage} -> ({addSubStage}) | {numberOrIdentifier};");

            string comparisonExpression = "ComparisonExpression";
            ParserGenerator.AddRule($"{comparisonExpression} -> {numberOrIdentifier} {Classifications.Comparison} {numberOrIdentifier};");

            string conditional = "Conditional";
            ParserGenerator.AddRule($"{conditional} -> {comparisonExpression} | {Classifications.Identifier};");

            string ifStatement = "IfStatement";
            ParserGenerator.AddRule($"{ifStatement} -> if ({conditional}) {Classifications.LeftCurlyBrackets} {StartingState} {Classifications.RightCurlyBrackets};");

            string allocateBool = "AllocateBool";
            ParserGenerator.AddRule($"{allocateBool} -> bool {Classifications.Identifier} = {conditional};");

            string allocateString = "AllocateString";
            ParserGenerator.AddRule($"{allocateString} -> string {Classifications.Identifier} = {Classifications.QuotationMark} {Classifications.Identifier} {Classifications.QuotationMark};");

            string middleState = "MiddleState";
            ParserGenerator.AddRule($"{middleState} -> {allocateInt} | {allocateBool} | {allocateString} | {ifStatement};");
            ParserGenerator.AddRule($"{StartingState} -> {middleState} {Classifications.CodeSeparator.ToString()} {StartingState} | {middleState} {Classifications.CodeSeparator.ToString()};");
            #region old
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
            #endregion
        }

        public ParserNode Parse()
        {
            List<Token> usefulTokens = new List<Token>();
            for(int i = 0; i < Tokens.Length; i ++)
            {
                if (!Token.IsIgnorable(Tokens[i].Classification))
                {
                    usefulTokens.Add(Tokens[i]);
                }
            }
            ParserNode returnValue = ParserNode.CreateRootNode();
            var shouldNotBeNull = Parse(usefulTokens, returnValue);
            if(shouldNotBeNull == null)
            {
                throw new Exception("This should not be null");
            }
            return returnValue;
        }

        private List<Token> Parse(List<Token> targetEquation, ParserNode current)
        {
            if (targetEquation.Count == 0)
            {
                return targetEquation;
            }

            if (current.IsTerminal)
            {
                bool doTokensMatch = (current.Token.Classification == targetEquation[0].Classification && (!current.IsSpecific || current.Token.Lexeme.ToString() == targetEquation[0].Lexeme.ToString()));
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

            var childrenFunctions = GetChildrenFunctions[current.Token.Lexeme.ToString()];
            for (int functionIndex = 0; functionIndex < childrenFunctions.Count; functionIndex++)
            {
                current.Children = childrenFunctions[functionIndex]();
                List<Token> copyEquation = new List<Token>(targetEquation);
                for (int i = 0; i < current.Children.Length; i++)
                {
                    copyEquation = Parse(copyEquation, current.Children[i]);
                    if (copyEquation == null)
                    {
                        break;
                    }
                }
                if (copyEquation != null)
                {
                    return copyEquation;
                }
            }
            return null;
        }
    }
}
