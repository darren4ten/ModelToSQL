using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VSIXModelToSQL
{
    public class OptionSettingPage : DialogPage
    {

        private string ignoreAttributeNames;
        private string ignoreFieldNames;

        /// <summary>
        /// 需要忽略的自定义特性名称
        /// </summary>
        [Description("需要忽略的自定义特性名称，用“;”分割多个特性名称")]
        [DisplayName("需要忽略的自定义特性名称")]
        public string IgnoreAttributeNames
        {
            get { return ignoreAttributeNames; }
            set { ignoreAttributeNames = value; }
        }

        /// <summary>
        /// 需要忽略的属性名称
        /// </summary>
        [Description("需要忽略的属性名称，用“;”分割多个属性名称")]
        [DisplayName("需要忽略的属性名称")]
        public string IgnoreFieldNames
        {
            get { return ignoreFieldNames; }
            set { ignoreFieldNames = value; }
        }
    }
}
