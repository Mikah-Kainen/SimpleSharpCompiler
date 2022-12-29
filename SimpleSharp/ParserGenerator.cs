using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleSharp
{
    public class ParserGenerator
    {
        public Dictionary<string, List<Func<ParserNode[]>>> MakeChildrenFunctions;

        public ParserGenerator()
        {
            MakeChildrenFunctions = new Dictionary<string, List<Func<ParserNode[]>>>();
        }

        public bool AddRule(string rule)
        {
            List<List<Token>> ruleBreakDown = TokenizeRule(rule);
            List<Func<ParserNode[]>> currentFuncs = new List<Func<ParserNode[]>>();

            for(int i = 1; i < ruleBreakDown.Count; i ++)
            {
                int ii = i;
                currentFuncs.Add(() => CreateChildren(ruleBreakDown[ii]));
            }
            MakeChildrenFunctions.Add(ruleBreakDown[0][0].Lexeme.ToString(), currentFuncs);
            return true;
        }

        public ParserNode[] CreateChildren(List<Token> childrenTokens)
        {
            List<ParserNode> children = new List<ParserNode>();
            for(int i = 0; i < childrenTokens.Count; i ++)
            {
                bool realToken = true;
                if (childrenTokens[i].Classification == Classifications.Identifier)
                {
                    realToken = false;
                    for(Classifications classification = (Classifications)Token.ClassificationStartIndex; (int)classification < Token.ClassificationEndIndex; classification ++)
                    {
                        if(classification.ToString() == childrenTokens[i].Lexeme.ToString())
                        {
                            childrenTokens[i].Classification = classification;
                            realToken = true;
                        }
                    }
                }
                if(realToken)
                {
                    children.Add(new ParserNode(childrenTokens[i].Classification));
                }
                else
                {
                    children.Add(new ParserNode(childrenTokens[i].Lexeme.ToString()));
                }
            }
            return children.ToArray();
        }

        private List<List<Token>> TokenizeRule(string rule) //List0 = name of expression, other lists are possible values for the expression
        {
            string ruleNameRegex = @"\s*(.+?)\s*->(.+)";
            string ruleValueRegex = @"\s*-*>*\|*(.+?)[\|;]";
            Regex nameRegex = new Regex(ruleNameRegex);
            Regex valueRegex = new Regex(ruleValueRegex);

            Match nameResult = nameRegex.Match(rule, 0);
            if (nameResult.Groups.Count < 3)
            {
                throw new Exception("Invalid Rule");
            }
            string ruleName = nameResult.Groups[1].Value;
            string ruleValues = nameResult.Groups[2].Value;

            List<string> ruleParts = new List<string>();
            ruleParts.Add(ruleName);

            MatchCollection valueResults = valueRegex.Matches(ruleValues);
            foreach (Match match in valueResults)
            {
                ruleParts.Add(match.Groups[1].Value);
            }

            List<List<Token>> tokens = new List<List<Token>>();
            for (int i = 0; i < ruleParts.Count; i++)
            {
                tokens.Add(new List<Token>());
                int currentIndex = 0;
                while (currentIndex < ruleParts[i].Length)
                {
                    for (Classifications classification = 0; (int)classification <= (int)Classifications.Invalid; classification++)
                    {
                        //MathAddSub  -> MathMultDiv    + MathAddSub  | MathMultDiv           -  MathAddSub  | MathMultDiv
                        //MathMultDiv -> MathExpLog     * MathMultDiv | MathExpLog            /  MathMultDiv | MathExpLog
                        //MathExpLog  -> MathExpression ^ MathExpLog  | MathExpression[base] log MathExpLog  | MathExpression
                        //MathExpression -> (MathAddSub) | Identifier[Token] | AddSub[Token] Identifier[Token] | Number[Token] | AddSub[Token] Number[Token]
                        Regex currentRegex = new Regex(Lexer.RegexStrings[(int)classification]);
                        Match result = currentRegex.Match(ruleParts[i], currentIndex);

                        if (result != Match.Empty && result.Index == currentIndex)
                        {
                            if (!Token.IsIgnorable(classification))
                            {
                                tokens[i].Add(new Token(new ReadOnlyMemory<char>(result.Groups[1].Value.ToCharArray()), classification));
                                //need to check the Identifier classification when I use this in case it represents an expression
                            }
                            currentIndex += result.Groups[1].Value.Length;
                            break;
                        }
                    }
                }
            }

            return tokens;
        }
    }
}
