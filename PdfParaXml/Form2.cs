using PdfParaXml.TemplateXML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PdfParaXml
{
    public partial class Form2 : Form
    {
        public Form2(TemplateIPOG.Resultados resultadosIpog = null)
        {
            var dtg = new BindingList<TemplateIPOG.Pedido>(resultadosIpog.Pedidos);
            InitializeComponent();
            this.dataGridView1.DataSource = dtg;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var resultados = new List<TemplateIPOG.Resultados>();
            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                
            }
            this.Close();
        }
    }
}
