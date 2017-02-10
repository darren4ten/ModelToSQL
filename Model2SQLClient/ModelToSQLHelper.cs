using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Model2SQLClient
{
    public class ModelToSQLHelper
    {
        private const int DefaultStringLength = 200;
        /// <summary>
        /// 需要忽略的字段名称
        /// </summary>
        public static List<string> IgnoreFieldList = new List<string>() { "ObjectState" };

        /// <summary>
        /// 根据C#类型获取对应SQL类型和长度
        /// </summary>
        /// <param name="CSharpType"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        private static string GetSQLType(Type CSharpType, int maxLength)
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

        public static string GenerateSQLFromModel(Type type)
        {
            string sql = "";
            //表名不包含“.”
            string typeName = type.ToString().ToUpper();
            typeName = typeName.Substring(typeName.LastIndexOf(".") + 1);
            sql += "CREATE TABLE [dbo].[" + typeName + "] (" + Environment.NewLine;
            var props = type.GetProperties();
            string descriptions = "";
            string keyConstrait = "";
            foreach (var prop in props)
            {

                //去掉需要忽略的列
                if (IgnoreFieldList.Any(t => t == prop.Name))
                {
                    continue;
                }
                //属性是否有标记最大或者最小长度、字符串长度
                var maxLengthAttr = prop.GetCustomAttribute(typeof(MaxLengthAttribute));
                int maxLengthAttrValue = DefaultStringLength;
                if (maxLengthAttr != null)
                {
                    var lenAtt = maxLengthAttr.GetType().GetProperty("Length");
                    maxLengthAttrValue = Convert.ToInt32(lenAtt.GetValue(maxLengthAttr));
                }

                var minLengthAttr = prop.GetCustomAttribute(typeof(MinLengthAttribute));
                int minLengthAttrValue = DefaultStringLength;
                if (minLengthAttr != null)
                {
                    var lenAtt = minLengthAttr.GetType().GetProperty("Length");
                    minLengthAttrValue = Convert.ToInt32(lenAtt.GetValue(minLengthAttr));
                }

                var strLengthAttr = prop.GetCustomAttribute(typeof(StringLengthAttribute));
                int strLengthAttrValue = DefaultStringLength;
                if (strLengthAttr != null)
                {
                    var maxLenAtt = strLengthAttr.GetType().GetProperty("MaximumLength");
                    strLengthAttrValue = Convert.ToInt32(maxLenAtt.GetValue(strLengthAttr));
                }
                //根据长度约束条件获取最大长度
                int maxLength = Math.Max(maxLengthAttrValue, Math.Max(minLengthAttrValue, strLengthAttrValue));

                //属性名称
                sql += "    [" + prop.Name + "] " + GetSQLType(prop.PropertyType, maxLength) + "," + Environment.NewLine;
                //判断是否主键
                var keyAttr = prop.GetCustomAttribute(typeof(KeyAttribute));
                string desc = "";
                if (keyAttr != null)
                {
                    desc += "主键 ";
                    keyConstrait = @" CONSTRAINT [PK_" + prop.Name + @"] PRIMARY KEY CLUSTERED
                   (
	                    [" + prop.Name + @"] ASC
                     )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]";
                }
                //查看显示名称
                var displyAttr = prop.GetCustomAttribute(typeof(DisplayAttribute));
                if (displyAttr != null)
                {
                    desc += displyAttr.GetType().GetProperty("Name").GetValue(displyAttr);
                }
                descriptions += (Environment.NewLine + @"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'" + desc + "' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'" + typeName + "', @level2type=N'COLUMN',@level2name=N'" + prop.Name + "'") + Environment.NewLine + "GO";
            }
            sql += keyConstrait;
            sql += ");" + Environment.NewLine + "GO " + descriptions;

            return sql;
        }
        /// <summary>
        /// 根据C#
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static string GenerateSQLFromModel<T>() where T : new()
        {
            return GenerateSQLFromModel(typeof(T)); ;
        }

        public static Type[] GetModelTypes(string assemblyPath)
        {
            Assembly ass = Assembly.LoadFrom(assemblyPath);
            var types = ass.GetTypes();
            return types;
        }
    }
}
