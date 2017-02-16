using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VSIXModelToSQL
{
    public class Utility
    {
        #region 获取代码信息
        /// <summary>
        /// 根据DTE获取CodeClass2
        /// </summary>
        /// <param name="dte"></param>
        /// <returns></returns>
        public static CodeClass2 GetCodeClass2(DTE dte)
        {
            CodeClass2 codeClzz = null;
            if (dte == null)
                return codeClzz;
            var item = dte.ActiveDocument.ProjectItem;
            var codeModel = item.FileCodeModel;
            var eles = codeModel.CodeElements;

            foreach (CodeElement element in eles)
            {
                string codeKind = element.Kind.ToString();

                //查找命名空间
                if (element.Kind == vsCMElement.vsCMElementNamespace)
                {
                    foreach (CodeElement c in element.Children)
                    {
                        //定位到类
                        if (c.Kind == vsCMElement.vsCMElementClass)
                        {
                            codeClzz = c as CodeClass2;
                        }
                    }
                }
            }

            return codeClzz;
        }

        /// <summary>
        /// 根据DTE获取CodeClass2
        /// </summary>
        /// <param name="dte"></param>
        /// <returns></returns>
        public static CodeClass2 GetCodeClass2(CodeElements eles)
        {
            CodeClass2 codeClzz = null;
            foreach (CodeElement element in eles)
            {
                string codeKind = element.Kind.ToString();

                //查找命名空间
                if (element.Kind == vsCMElement.vsCMElementNamespace)
                {
                    foreach (CodeElement c in element.Children)
                    {
                        //定位到类
                        if (c.Kind == vsCMElement.vsCMElementClass)
                        {
                            codeClzz = c as CodeClass2;
                        }
                    }
                }
            }

            return codeClzz;
        }

        /// <summary>
        /// 获取类的所有Property信息
        /// </summary>
        /// <param name="codeClass"></param>
        /// <param name="isRecursive">是否递归获取父类的信息</param>
        /// <returns></returns>
        public static List<CodeProperty2> GetCodeProperty2s(CodeClass2 codeClass, bool isRecursive = false)
        {
            List<CodeProperty2> list = new List<CodeProperty2>();
            //获取基类的属性信息
            if (isRecursive && codeClass.Bases.Count > 0)
            {
                var baseElements = codeClass.Bases as CodeElements;
                if (baseElements != null)
                {
                    CodeClass2 clazz = GetCodeClass2(baseElements);
                    list.AddRange(GetCodeProperty2s(clazz));
                }
            }

            //获取当前类的属性
            foreach (CodeElement prop in codeClass.Members)
            {
                if (prop.Kind == vsCMElement.vsCMElementProperty)
                {
                    CodeProperty2 p = prop as CodeProperty2;
                    list.Add(p);
                }
                else if (prop.Kind == vsCMElement.vsCMElementVariable)
                {
                    CodeVariable2 v = prop as CodeVariable2;
                }
                else
                {
                    Console.WriteLine("" + prop.Kind);
                }
            }
            return list;
        }

        /// <summary>
        /// 获取属性的所有自定义特性
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static List<CodeAttribute2> GetCodeAttribute2s(CodeProperty2 property)
        {
            List<CodeAttribute2> list = new List<CodeAttribute2>();
            //定位自定义属性信息
            foreach (CodeAttribute2 attr in property.Attributes)
            {
                list.Add(attr);
            }
            return list;
        }

        /// <summary>
        /// 获取特性参数信息
        /// </summary>
        /// <param name="attr"></param>
        /// <returns></returns>
        public static List<CodeAttributeArgument> GetCodeAttrArgs(CodeAttribute2 attr)
        {
            List<CodeAttributeArgument> list = new List<CodeAttributeArgument>();
            foreach (CodeAttributeArgument attrArg in attr.Arguments)
            {
                list.Add(attrArg);
            }
            return list;
        }

        /// <summary>
        ///  获取注释Summary的文字内容
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string GetCommentFromXMLString(string xml)
        {
            string msg = "";
            Regex reg = new Regex("summary>\r\n(.*)\r\n<", RegexOptions.Multiline);

            var matches = reg.Matches(xml);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    msg = match.Groups[1].Value;
                    break;
                }
            }
            return msg;
        }
        #endregion

        /// <summary>
        /// 只有.cs的文件邮件可以弹出快捷菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CheckMenuStatus(object sender, EventArgs e)
        {
            OleMenuCommand menuCommand = sender as OleMenuCommand;
            if (menuCommand != null)
            {
                IntPtr hierarchyPtr, selectionContainerPtr;
                uint projectItemId;
                IVsMultiItemSelect mis;
                IVsMonitorSelection monitorSelection = (IVsMonitorSelection)Package.GetGlobalService(typeof(SVsShellMonitorSelection));
                monitorSelection.GetCurrentSelection(out hierarchyPtr, out projectItemId, out mis, out selectionContainerPtr);

                IVsHierarchy hierarchy = Marshal.GetTypedObjectForIUnknown(hierarchyPtr, typeof(IVsHierarchy)) as IVsHierarchy;
                if (hierarchy != null)
                {
                    object value;
                    hierarchy.GetProperty(projectItemId, (int)__VSHPROPID.VSHPROPID_Name, out value);

                    if (value != null && value.ToString().EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
                    {
                        menuCommand.Visible = true;
                    }
                    else
                    {
                        menuCommand.Visible = false;
                    }
                }
            }
        }
    }
}
