using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIXModelToSQL
{
    public class DisplayOrCommentsGenerator
    {
        /// <summary>
        /// 根据注释生成Display(Name = "注释" )
        /// </summary>
        public static void GenerateDisplayNameByPropertyComment(DTE dte)
        {
            var clazz = Utility.GetCodeClass2(dte);
            if (clazz != null)
            {
                string functionContent = clazz.StartPoint.CreateEditPoint().GetText(clazz.EndPoint);
                var properties = Utility.GetCodeProperty2s(clazz);
                foreach (var p in properties)
                {
                    //如果已经包含了Display特性，则不添加
                    var attrs = Utility.GetCodeAttribute2s(p);
                    if (attrs.Any(t => t.Name == "Display"))
                    {
                        continue;
                    }

                    TextPoint pStart = p.StartPoint;

                    string comment = Utility.GetCommentFromXMLString(p.DocComment);
                    string displayText = "[Display(Name = \"" + comment + "\")]" + Environment.NewLine;

                    EditPoint editPoint = pStart.CreateEditPoint();
                    editPoint.MoveToLineAndOffset(pStart.Line, pStart.DisplayColumn);
                    editPoint.Insert(displayText);

                    //格式化代码
                    editPoint.SmartFormat(pStart);

                }
            }
        }

        /// <summary>
        /// 根据DisplayName生成注释
        /// </summary>
        /// <param name="dte"></param>
        public static void GenerateCommentByPropertyDisplayName(DTE dte)
        {
            var clazz = Utility.GetCodeClass2(dte);
            if (clazz != null)
            {
                string functionContent = clazz.StartPoint.CreateEditPoint().GetText(clazz.EndPoint);
                var properties = Utility.GetCodeProperty2s(clazz);
                foreach (var p in properties)
                {
                    //如果已经包含了注释，则不添加
                    if (!string.IsNullOrWhiteSpace(p.DocComment))
                    {
                        continue;
                    }

                    TextPoint pStart = p.StartPoint;

                    //获取第一个属性的位置
                    var attrs = Utility.GetCodeAttribute2s(p);
                    int minLine = attrs.Count > 0 ? attrs.Min(s => s.StartPoint.Line) : p.StartPoint.Line;
                    var displayAttr = attrs.FirstOrDefault(s => s.Name == "Display");
                    string msg = "";
                    if (displayAttr != null)
                    {
                        var nameArg = Utility.GetCodeAttrArgs(displayAttr).FirstOrDefault(s => s.Name == "Name");
                        msg = nameArg.Value.Trim('"');
                    }

                    string comment = @" /// <summary>
                                        /// " + msg + @"
                                        /// </summary>" + Environment.NewLine;

                    EditPoint editPoint = pStart.CreateEditPoint();
                    editPoint.MoveToLineAndOffset(minLine, pStart.DisplayColumn);
                    editPoint.Insert(comment);

                    //格式化代码
                    editPoint.SmartFormat(pStart);

                }
            }
        }

    }
}
