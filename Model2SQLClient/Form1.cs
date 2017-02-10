using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Model2SQLClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGenerateAndCopy_Click(object sender, EventArgs e)
        {
            Type type = (Type)cbClassNames.SelectedItem;
            if (type != null)
            {
                string sql = ModelToSQLHelper.GenerateSQLFromModel(type);
                rtxtSQL.Text = sql;
                Clipboard.SetText(sql);
            }else
            {
                MessageBox.Show("请选择一个要生成的类");
            }

        }

        private void btnGetTypes_Click(object sender, EventArgs e)
        {

            var types = ModelToSQLHelper.GetModelTypes(txtDllPath.Text);
            cbClassNames.DataSource = types;
            //搜索
            cbClassNames.AutoCompleteCustomSource.AddRange(types.Select(t => t.ToString()).ToArray());
            cbClassNames.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbClassNames.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        }

        private void cbClassNames_KeyDown(object sender, KeyEventArgs e)
        {

        }
    }
}
