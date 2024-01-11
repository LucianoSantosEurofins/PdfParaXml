using System;
using System.Windows.Forms;
using PdfParaXml.Functions;
using PdfParaXml.Functions.Mendelics;
using PdfParaXml.Functions.Sollutio;
using System.Configuration;

namespace PdfParaXml
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            textBox_LocationMendelics.Text = ConfigurationManager.AppSettings["pastaPadraoMendelics"];
            textBox_LocationSollutio.Text = ConfigurationManager.AppSettings["pastaPadraoSolutio"];
            textBox_XMLLocation.Text = ConfigurationManager.AppSettings["pastaPadraoXML"];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MendelicsPDF_Reader mendelicsPDF_Reader = new MendelicsPDF_Reader();
            SollutioPDF_Reader sollutioPDF_Reader = new SollutioPDF_Reader();
            var sollutioLocation = textBox_LocationSollutio.Text;
            var mendelicsLocation = textBox_LocationMendelics.Text;
            var xmlLocation = textBox_XMLLocation.Text;

            if (!string.IsNullOrEmpty(sollutioLocation) && !string.IsNullOrEmpty(xmlLocation))
                sollutioPDF_Reader.SollutioPDFToXML(sollutioLocation, xmlLocation);

            if (!string.IsNullOrEmpty(mendelicsLocation) && !string.IsNullOrEmpty(xmlLocation))
                mendelicsPDF_Reader.MendelixPDFsTOXML(mendelicsLocation, xmlLocation);

            if (checkBox1.Checked)
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["pastaPadraoMendelics"].Value = textBox_LocationMendelics.Text;
                config.AppSettings.Settings["pastaPadraoSolutio"].Value = textBox_LocationSollutio.Text;
                config.AppSettings.Settings["pastaPadraoXML"].Value = textBox_XMLLocation.Text;

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
        }

        private void btn_SelectFolderMendelics_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                // Define o título do diálogo
                folderBrowserDialog.Description = "Selecione uma pasta com os PDFs Mendelics";

                // Exibe o diálogo e verifica se o usuário clicou em OK
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // Obtém o caminho da pasta selecionada
                    string caminhoPasta = folderBrowserDialog.SelectedPath;

                    // Faça algo com o caminho da pasta, se necessário
                    textBox_LocationMendelics.Text = caminhoPasta;
                }
            }
        }

        private void btn_SelectFolderSollutio_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                // Define o título do diálogo
                folderBrowserDialog.Description = "Selecione uma pasta com os PDFs Sollutio";

                // Exibe o diálogo e verifica se o usuário clicou em OK
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // Obtém o caminho da pasta selecionada
                    string caminhoPasta = folderBrowserDialog.SelectedPath;

                    // Faça algo com o caminho da pasta, se necessário
                    textBox_LocationSollutio.Text = caminhoPasta;
                }
            }
        }

        private void btn_SelectXMLFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                // Define o título do diálogo
                folderBrowserDialog.Description = "Selecione uma pasta onde ficará os arquivos XML";

                // Exibe o diálogo e verifica se o usuário clicou em OK
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // Obtém o caminho da pasta selecionada
                    string caminhoPasta = folderBrowserDialog.SelectedPath;

                    // Faça algo com o caminho da pasta, se necessário
                    textBox_XMLLocation.Text = caminhoPasta;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string msg = "// < Transcrever PDF para XML.> " +
                         "    Copyright (C) < 2024 >  < Luciano Santos> " +
                         "    This program is free software: you can redistribute it and/or modify " +
                         "    it under the terms of the GNU Affero General Public License as " + 
                         "    published by the Free Software Foundation, either version 3 of the " +
                         "    License, or (at your option) any later version." + 
                         "    This program is distributed in the hope that it will be useful, " +
                         "    but WITHOUT ANY WARRANTY; without even the implied warranty of " +
                         "    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the " +
                         "    GNU Affero General Public License for more details. " +
                         "    You should have received a copy of the GNU Affero General Public License " +
                         "    along with this program.  If not, see <https://www.gnu.org/licenses/>.";
            MessageBox.Show(msg);
        }
    }
}
