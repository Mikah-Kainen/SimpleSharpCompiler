using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleSharp
{

    public class Function
    {

    }

    public class ProgramType
    {
        public string TypeName;
        public int Header;
        public Dictionary<string, Function> Functions;
        
        public ProgramType(string typeName)
        {
            TypeName = typeName;
        }
    }

    public class SemanticAnalyzer
    {
        public Stack<Dictionary<string, ProgramType>> ScopeStack;
        public ParserNode AST;
        public List<string> Types;
        public ParserGenerator AnalysisGenerator;
        public Dictionary<string, List<Func<ParserNode[]>>> GetChildrenFunctions => AnalysisGenerator.GetChildrenFunctions;

        public SemanticAnalyzer(ParserNode AST)
        {
            ScopeStack = new Stack<Dictionary<string, ProgramType>>();
            this.AST = AST;
            Types = Lexer.TypeKeywordList.Split('|').ToList();
            AnalysisGenerator = new ParserGenerator();
            string intName = "int";
            AnalysisGenerator.AddRule($"{intName} -> {intName} {Classifications.AddSub.ToString()} {intName} | {intName} {Classifications.MultDiv.ToString()} {intName} | {intName} {Classifications.ExpLog.ToString()} {intName} | {Classifications.Number};");
            string boolName = "bool";
            AnalysisGenerator.AddRule($"{boolName} -> {intName} {Classifications.Comparison.ToString()} {intName} | true | false;");
        }
    }
}
