using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Reflection;
using System.Globalization;
using Microsoft.CSharp;
using System.CodeDom;
using System.CodeDom.Compiler;

namespace Shove._System.DynamicCode
{
    /// <summary>
    /// 动态执行 CShap 代码
    /// </summary>
    public class CShap
    {
        /// <summary>
        /// 编译代码
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="ErrorList"></param>
        /// <param name="dll_ReferencedAssemblies"></param>
        /// <returns></returns>
        public static CompilerResults Compile(string Code, ref IList<string> ErrorList, params string[] dll_ReferencedAssemblies)
        {
            CompilerParameters cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");
            cp.ReferencedAssemblies.Add("System.Drawing.dll");
            cp.ReferencedAssemblies.Add("System.Xml.dll");
            cp.ReferencedAssemblies.Add("System.Web.dll");
            cp.ReferencedAssemblies.Add("System.Web.Services.dll");
            cp.ReferencedAssemblies.Add("System.Windows.Forms.dll");

            if (dll_ReferencedAssemblies != null && dll_ReferencedAssemblies.Length > 0)
            {
                foreach(string dll in dll_ReferencedAssemblies)
                {
                    if (!cp.ReferencedAssemblies.Contains(dll))
                    {
                        cp.ReferencedAssemblies.Add(dll);
                    }
                }
            }

            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;

            //CSharpCodeProvider objCSharpCodePrivoder = new CSharpCodeProvider();
            //ICodeCompiler objICodeCompiler = objCSharpCodePrivoder.CreateCompiler();

            //CodeDomProvider cdp = CodeDomProvider.CreateProvider("CSharp");
            CSharpCodeProvider cdp = new CSharpCodeProvider();
            CompilerResults cr = cdp.CompileAssemblyFromSource(cp, Code);

            //CompilerResults cr = cdp.CompileAssemblyFromSource(objCompilerParameters, Code);

            if (cr.Errors.HasErrors)
            {
                if (ErrorList == null)
                {
                    ErrorList = new List<string>();
                }

                foreach (CompilerError err in cr.Errors)
                {
                    ErrorList.Add(err.ErrorText);
                }

                return null;
            }

            return cr;
        }

        /// <summary>
        /// 执行代码块
        /// </summary>
        /// <param name="CodeBlock"></param>
        /// <param name="ErrorList"></param>
        /// <param name="using_NameSpaces"></param>
        /// <param name="dll_ReferencedAssemblies"></param>
        /// <returns></returns>
        public static object ExecuteCodeBlock(string CodeBlock, ref IList<string> ErrorList, string using_NameSpaces, params string[] dll_ReferencedAssemblies)
        {
            CompilerResults cr = Compile(GenerateCode(CodeBlock, using_NameSpaces), ref ErrorList, dll_ReferencedAssemblies);

            if (cr == null)
            {
                return null;
            }

            // 通过反射，调用HelloWorld的实例
            Assembly objAssembly = cr.CompiledAssembly;
            object objHelloWorld = objAssembly.CreateInstance("DynamicCodeGenerate.HelloWorld");
            MethodInfo objMI = objHelloWorld.GetType().GetMethod("MyMethod");

            return objMI.Invoke(objHelloWorld, null);
        }

        private static string GenerateCode(string CodeBlock, string using_NameSpaces)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using System; using System.Data; using System.Data.SqlClient; using System.Collections; using System.Collections.Generic; using System.Text.RegularExpressions; using System.Drawing; using System.Web; using System.Web.UI; using System.Web.UI.WebControls; using System.Web.UI.HtmlControls; using System.Windows.Forms;");
            if (!String.IsNullOrEmpty(using_NameSpaces))
            {
                sb.Append(using_NameSpaces);
            }
            sb.AppendLine();
            sb.Append("namespace DynamicCodeGenerate");
            sb.AppendLine();
            sb.Append("{");
            sb.AppendLine();
            sb.Append("      public class HelloWorld");
            sb.AppendLine();
            sb.Append("      {");
            sb.AppendLine();
            sb.Append("          public object MyMethod()");
            sb.AppendLine();
            sb.Append("          {");
            sb.AppendLine();
            sb.Append(CodeBlock);
            sb.AppendLine();
            sb.Append("          }");
            sb.AppendLine();
            sb.Append("      }");
            sb.AppendLine();
            sb.Append("}");

            string code = sb.ToString();

            return code;
        }

        /// <summary>
        /// 执行类
        /// </summary>
        /// <param name="ClassCode"></param>
        /// <param name="ClassFullName"></param>
        /// <param name="EntryMethodName"></param>
        /// <param name="ErrorList"></param>
        /// <param name="dll_ReferencedAssemblies"></param>
        /// <returns></returns>
        public static object ExecuteClass(string ClassCode, string ClassFullName, string EntryMethodName, IList<string> ErrorList, params string[] dll_ReferencedAssemblies)
        {
            CompilerResults cr = Compile(ClassCode, ref ErrorList, dll_ReferencedAssemblies);

            if (cr == null)
            {
                return null;
            }

            // 通过反射，调用HelloWorld的实例
            Assembly objAssembly = cr.CompiledAssembly;
            object objHelloWorld = objAssembly.CreateInstance(ClassFullName);
            MethodInfo objMI = objHelloWorld.GetType().GetMethod(EntryMethodName);

            return objMI.Invoke(objHelloWorld, null);
        }
    }
}
