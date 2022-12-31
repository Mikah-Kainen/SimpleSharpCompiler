using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;
using System.Security.Cryptography;

namespace SimpleSharp
{
    public class CodeGenerator
    {

        public CodeGenerator()
        {
            //paste the path of the file I am writing into Developer Command Prompt for VS 2022 and use these funtions to:
            //ildasm: view the program
            //peverify: verify the code
        }

        public void GenerateProgram(string programName)
        {
            string fileName = $"{programName}.exe";

            string namespaceName = "ProgramNamespace";
            string className = "Program";
            string moduleName = $"{namespaceName}.{className}";

            string entryPointMethodName = "Seinfeld";
            AssemblyName assemblyName = new AssemblyName("ProgramAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName,
                                                                    AssemblyBuilderAccess.Save);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName, fileName);

            TypeBuilder programTypeBuilder = moduleBuilder.DefineType("Program",
                                            TypeAttributes.Public | TypeAttributes.Class);

            MethodBuilder mainMethodBuilder = programTypeBuilder.DefineMethod(entryPointMethodName,
                                                     MethodAttributes.Public | MethodAttributes.Static,
                                                     returnType: typeof(void),
                                                     parameterTypes: new[] { typeof(string[]) });

            ILGenerator mainILGen = mainMethodBuilder.GetILGenerator();
            mainILGen.Emit(OpCodes.Ldstr, "Wassup");
            mainILGen.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }));
            mainILGen.Emit(OpCodes.Ret);

            var programBuilder = programTypeBuilder.CreateType();
            assemblyBuilder.SetEntryPoint(mainMethodBuilder, PEFileKinds.ConsoleApplication);
            assemblyBuilder.Save(fileName);
        }
    }
}