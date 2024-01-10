using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PdfParaXml.Functions;
using PdfParaXml.Functions.Mendelics;
using PdfParaXml.Functions.Sollutio;

namespace PdfParaXml
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TesteLabAlvaro testeLabAlvaro = new TesteLabAlvaro();
            MendelicsPDF_Reader mendelicsPDF_Reader = new MendelicsPDF_Reader();
            SollutioPDF_Reader sollutioPDF_Reader = new SollutioPDF_Reader();
            sollutioPDF_Reader.SollutioPDFToXML();
            mendelicsPDF_Reader.MendelixPDFsTOXML();
            //testeLabAlvaro.Teste("");
        }
    }
}
