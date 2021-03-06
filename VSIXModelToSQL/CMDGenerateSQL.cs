﻿//------------------------------------------------------------------------------
// <copyright file="CMDGenerateSQL.cs" company="Microsoft">
//     Copyright (c) Microsoft.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using System.Runtime.InteropServices;
using EnvDTE;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnvDTE80;
using System.Windows.Forms;

namespace VSIXModelToSQL
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class CMDGenerateSQL
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("632d9672-850f-4e45-9820-bf635b7dde08");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package package;

        /// <summary>
        /// Initializes a new instance of the <see cref="CMDGenerateSQL"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private CMDGenerateSQL(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }

            this.package = package;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                //var menuItem = new MenuCommand(this.OpenModelToSQLClient, menuCommandID);
                var menuItem = new OleMenuCommand(this.OpenModelToSQLClient, menuCommandID);
                menuItem.BeforeQueryStatus += Utility.CheckMenuStatus;
                commandService.AddCommand(menuItem);
            }

        }


        #region 自定义代码
        private void OpenModelToSQLClient(object sender, EventArgs e)
        {
            DTE dte = Microsoft.VisualStudio.Shell.ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE;
            if (dte == null)
                return;
            //DisplayOrCommentsGenerator.GenerateDisplayNameByPropertyComment(dte);
            DisplayOrCommentsGenerator.GenerateCommentByPropertyDisplayName(dte);

            //获取配置信息
            ModelToSQLHelper.GetSettings(this.package, out ModelToSQLHelper.IgnoreAttributeList, out ModelToSQLHelper.IgnoreFieldList);
            string sql = ModelToSQLHelper.GenerateSQL(dte);

            // 拷贝到剪贴板
            Clipboard.SetText(sql);
        }

        #endregion

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static CMDGenerateSQL Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new CMDGenerateSQL(package);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void MenuItemCallback(object sender, EventArgs e)
        {
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "CMDGenerateSQL";

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.ServiceProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
