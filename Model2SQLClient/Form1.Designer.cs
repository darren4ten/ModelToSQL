namespace Model2SQLClient
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.txtDllPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblClassName = new System.Windows.Forms.Label();
            this.rtxtSQL = new System.Windows.Forms.RichTextBox();
            this.btnGenerateAndCopy = new System.Windows.Forms.Button();
            this.cbClassNames = new System.Windows.Forms.ComboBox();
            this.btnGetTypes = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtDllPath
            // 
            this.txtDllPath.Location = new System.Drawing.Point(65, 12);
            this.txtDllPath.Name = "txtDllPath";
            this.txtDllPath.Size = new System.Drawing.Size(551, 21);
            this.txtDllPath.TabIndex = 0;
            this.txtDllPath.Text = "D:\\\\Code\\\\BPM_20161102\\\\Web\\\\BPM.Web\\\\bin\\\\BPM.Models.dll";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "dll路径";
            // 
            // lblClassName
            // 
            this.lblClassName.AutoSize = true;
            this.lblClassName.Location = new System.Drawing.Point(12, 49);
            this.lblClassName.Name = "lblClassName";
            this.lblClassName.Size = new System.Drawing.Size(29, 12);
            this.lblClassName.TabIndex = 2;
            this.lblClassName.Text = "类名";
            // 
            // rtxtSQL
            // 
            this.rtxtSQL.Location = new System.Drawing.Point(12, 81);
            this.rtxtSQL.Name = "rtxtSQL";
            this.rtxtSQL.Size = new System.Drawing.Size(713, 269);
            this.rtxtSQL.TabIndex = 4;
            this.rtxtSQL.Text = "";
            // 
            // btnGenerateAndCopy
            // 
            this.btnGenerateAndCopy.Location = new System.Drawing.Point(635, 47);
            this.btnGenerateAndCopy.Name = "btnGenerateAndCopy";
            this.btnGenerateAndCopy.Size = new System.Drawing.Size(90, 28);
            this.btnGenerateAndCopy.TabIndex = 5;
            this.btnGenerateAndCopy.Text = "生成并复制到剪贴板";
            this.btnGenerateAndCopy.UseVisualStyleBackColor = true;
            this.btnGenerateAndCopy.Click += new System.EventHandler(this.btnGenerateAndCopy_Click);
            // 
            // cbClassNames
            // 
            this.cbClassNames.FormattingEnabled = true;
            this.cbClassNames.Location = new System.Drawing.Point(65, 49);
            this.cbClassNames.Name = "cbClassNames";
            this.cbClassNames.Size = new System.Drawing.Size(551, 20);
            this.cbClassNames.TabIndex = 6;
            this.cbClassNames.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbClassNames_KeyDown);
            // 
            // btnGetTypes
            // 
            this.btnGetTypes.Location = new System.Drawing.Point(635, 7);
            this.btnGetTypes.Name = "btnGetTypes";
            this.btnGetTypes.Size = new System.Drawing.Size(90, 34);
            this.btnGetTypes.TabIndex = 7;
            this.btnGetTypes.Text = "获取类型信息";
            this.btnGetTypes.UseVisualStyleBackColor = true;
            this.btnGetTypes.Click += new System.EventHandler(this.btnGetTypes_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(737, 362);
            this.Controls.Add(this.btnGetTypes);
            this.Controls.Add(this.cbClassNames);
            this.Controls.Add(this.btnGenerateAndCopy);
            this.Controls.Add(this.rtxtSQL);
            this.Controls.Add(this.lblClassName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDllPath);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ModelToSQL";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDllPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblClassName;
        private System.Windows.Forms.RichTextBox rtxtSQL;
        private System.Windows.Forms.Button btnGenerateAndCopy;
        private System.Windows.Forms.ComboBox cbClassNames;
        private System.Windows.Forms.Button btnGetTypes;
    }
}

