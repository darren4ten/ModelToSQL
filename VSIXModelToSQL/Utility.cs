using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VSIXModelToSQL
{
    public class Utility
    {
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
