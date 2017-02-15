using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VSIXModelToSQL
{
    public class ModelToSQLHelper
    {
        /// <summary>
        /// 需要忽略的字段名称
        /// </summary>
        public static List<string> IgnoreFieldList = new List<string>() { };

        public static List<string> IgnoreAttributeList = new List<string>() { "NotMapped" };

        /// <summary>
        /// 默认字符串长度
        /// </summary>
        public static int DefaultStringLength = 200;

        public static void GetSettings(Package package, out List<string> ignoreAttributeNamesList, out List<string> ignoreFieldNamesList)
        {
            IVsPackage pack = package as IVsPackage;
            ignoreAttributeNamesList = new List<string>();
            ignoreFieldNamesList = new List<string>();
            if (pack != null)
            {
                object obj;
                pack.GetAutomationObject("ModelToSQL设置.General", out obj);

                OptionSettingPage options = obj as OptionSettingPage;
                if (options != null)
                {
                    string ignoreAttributeNames = options.IgnoreAttributeNames;
                    string ignoreFieldNames = options.IgnoreFieldNames;
                    if (!string.IsNullOrEmpty(ignoreAttributeNames))
                    {
                        ignoreAttributeNamesList = ignoreAttributeNames.Trim(';').Split(';').ToList();
                    }
                    if (!string.IsNullOrEmpty(ignoreFieldNames))
                    {
                        ignoreFieldNamesList = ignoreFieldNames.Trim(';').Split(';').ToList();
                    }
                }
            }
        }

        /// <summary>
        /// 根据C#类型获取对应SQL类型和长度
        /// </summary>
        /// <param name="CSharpType"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string GetSQLType(Type CSharpType, int maxLength)
        {
            string sqlType = "";
            if (typeof(string) == CSharpType || typeof(char) == CSharpType)
            {
                sqlType = "NVARCHAR(" + maxLength + ")";
            }
            else if (typeof(int) == CSharpType)
            {
                sqlType = "INT";
            }
            else if (typeof(bool) == CSharpType)
            {
                sqlType = "BIT";
            }
            else if (typeof(decimal) == CSharpType || typeof(decimal?) == CSharpType || typeof(float) == CSharpType || typeof(float?) == CSharpType || typeof(double) == CSharpType || typeof(double?) == CSharpType)
            {
                sqlType = "DECIMAL(18,2)";
            }
            else if (typeof(byte[]) == CSharpType)//默认byte[]类型识别为时间戳
            {
                sqlType = "TIMESTAMP";
            }
            else if (typeof(DateTime) == CSharpType || typeof(DateTime?) == CSharpType)
            {
                sqlType = "DATETIME";
            }
            else if (typeof(Guid) == CSharpType)
            {
                sqlType = "UNIQUEIDENTIFIER";
            }

            return sqlType;
        }

        /// <summary>
        /// 根据C#类型获取对应SQL类型和长度
        /// </summary>
        /// <param name="rawCSharpType"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        public static string GetSQLType(string rawCSharpType, int maxLength)
        {
            string sqlType = "";
            if (rawCSharpType.Contains("System.String") || rawCSharpType.Contains("System.Char"))
            {
                sqlType = "NVARCHAR(" + maxLength + ")";
            }
            else if (rawCSharpType.Contains("System.Int32"))
            {
                sqlType = "INT";
            }
            else if (rawCSharpType.Contains("System.Boolean"))
            {
                sqlType = "BIT";
            }
            else if (rawCSharpType.Contains("System.Decimal") || rawCSharpType.Contains("System.Single") || rawCSharpType.Contains("System.Double"))
            {
                sqlType = "DECIMAL(18,2)";
            }
            else if (rawCSharpType.Contains("System.Byte[]"))//默认byte[]类型识别为时间戳
            {
                sqlType = "TIMESTAMP";
            }
            else if (rawCSharpType.Contains("System.DateTime"))
            {
                sqlType = "DATETIME";
            }
            else if (rawCSharpType.Contains("System.Guid"))
            {
                sqlType = "UNIQUEIDENTIFIER";
            }

            return sqlType;
        }
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

        public static string GenerateSQL(DTE dte)
        {
            dte = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
            if (dte == null)
                return "";

            string sql = "";
            string descriptions = "";
            string keyConstrait = "";
            var clazz = GetCodeClass2(dte);
            if (clazz != null)
            {
                sql += "CREATE TABLE [dbo].[" + clazz.Name + "] (" + Environment.NewLine;
                //string functionContent = clazz.StartPoint.CreateEditPoint().GetText(clazz.EndPoint);
                //递归获取所有基类和子类的字段
                GetPropertiesInfo(clazz, clazz.Name, ref sql, ref keyConstrait, ref descriptions);

                sql += keyConstrait;
                sql += ");" + Environment.NewLine + "GO " + descriptions;
            }

            return sql;
        }

        private static void GetPropertiesInfo(CodeClass2 codeClass, string modelName, ref string sql, ref string keyConstrait, ref string descriptions)
        {
            //获取基类信息
            if (codeClass.Bases.Count > 0)
            {
                var baseElements = codeClass.Bases as CodeElements;
                if (baseElements != null)
                {
                    foreach (CodeElement2 baseEle in baseElements)
                    {
                        //查找命名空间
                        if (baseEle.Kind == vsCMElement.vsCMElementClass)
                        {
                            CodeClass2 clazz = baseEle as CodeClass2;
                            GetPropertiesInfo(clazz, modelName, ref sql, ref keyConstrait, ref descriptions);
                        }
                    }
                }
            }

            //定位到属性
            foreach (CodeElement prop in codeClass.Members)
            {
                if (prop.Kind == vsCMElement.vsCMElementProperty)
                {
                    //去掉需要忽略的列
                    if (IgnoreFieldList.Any(t => t == prop.Name))
                    {
                        continue;
                    }

                    CodeProperty2 p = prop as CodeProperty2;
                    string rawTypeName = p.Type.AsFullName;
                    string rawDocComment = p.DocComment;

                    //描述信息
                    bool isPrimaryKey = false;
                    //是否需要跳过
                    bool isSkipProperty = false;
                    // 字段描述
                    string desc = "";
                    int maxLength = DefaultStringLength;

                    int maxAttrLen = DefaultStringLength;
                    int minAttrLen = DefaultStringLength;

                    int maxStrLen = DefaultStringLength;
                    int minStrLen = DefaultStringLength;
                    //定位自定义属性信息
                    foreach (CodeAttribute2 attr in p.Attributes)
                    {
                        //属性是否有标记最大或者最小长度、字符串长度
                        if (attr.Name == "MaxLength")
                        {
                            //500, ErrorMessage = "最大长度是500啊"
                            foreach (CodeAttributeArgument attrArg in attr.Arguments)
                            {
                                if (attrArg.Name == "" || attrArg.Name == "MaximumLength")
                                {
                                    //第一个没有名称的参数为最大长度
                                    maxAttrLen = Convert.ToInt32(attrArg.Value);
                                }
                                else if (attrArg.Name == "ErrorMessage")
                                {
                                    //错误描述
                                }
                            }
                        }
                        else if (attr.Name == "MinLength")
                        {
                            foreach (CodeAttributeArgument attrArg in attr.Arguments)
                            {
                                if (attrArg.Name == "" || attrArg.Name == "MinimumLength")
                                {
                                    //第一个没有名称的参数为最小长度
                                    minAttrLen = Convert.ToInt32(attrArg.Value);
                                }
                                else if (attrArg.Name == "ErrorMessage")
                                {
                                    //错误描述
                                }
                            }
                        }
                        else if (attr.Name == "StringLength")
                        {
                            foreach (CodeAttributeArgument attrArg in attr.Arguments)
                            {
                                if (attrArg.Name == "" || attrArg.Name == "MaximumLength")
                                {
                                    //第一个没有名称的参数为最大长度
                                    maxStrLen = Convert.ToInt32(attrArg.Value);
                                }
                                else if (attrArg.Name == "MinimumLength")
                                {
                                    //第一个没有名称的参数为最大长度
                                    minStrLen = Convert.ToInt32(attrArg.Value);
                                }
                                else if (attrArg.Name == "ErrorMessage")
                                {
                                    //错误描述
                                }
                            }
                        }
                        else if (attr.Name == "Display")
                        {
                            foreach (CodeAttributeArgument attrArg in attr.Arguments)
                            {
                                if (attrArg.Name == "Name")
                                {
                                    desc = attrArg.Value.Trim('"');
                                    break;
                                }
                            }
                        }
                        else if (IgnoreAttributeList.Any(x => x == attr.Name))
                        {
                            //如果是需要忽略的字段，则跳过
                            isSkipProperty = true;
                            break;
                        }
                        else if (attr.Name == "Key")
                        {
                            //如果是Key属性，则认为其是主键
                            isPrimaryKey = true;
                            keyConstrait = @" CONSTRAINT [PK_" + prop.Name + @"] PRIMARY KEY CLUSTERED" +
                            "(" +
                            "    [" + prop.Name + @"] ASC" +
                            " )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";
                        }
                    }

                    //如果需要跳过，则跳过
                    if (isSkipProperty)
                    {
                        continue;
                    }

                    //根据长度约束条件获取最大长度
                    maxLength = Math.Max(maxStrLen, Math.Max(minAttrLen, maxLength));
                    //描述信息以Display为准
                    desc = string.IsNullOrEmpty(desc) ? GetCommentFromXMLString(rawDocComment) : desc;
                    desc = isPrimaryKey ? "主键 " + desc : desc;
                    //属性名称
                    sql += "    [" + prop.Name + "] " + GetSQLType(rawTypeName, maxLength) + "," + Environment.NewLine;
                    //如果描述信息不为空，则添加描述信息
                    if (!string.IsNullOrEmpty(desc))
                    {
                        descriptions += (Environment.NewLine + @"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'" + desc + "' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'" + modelName + "', @level2type=N'COLUMN',@level2name=N'" + prop.Name + "'") + Environment.NewLine + "GO";
                    }
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

        }


    }
}
