using PdfParaXml.TemplateXML;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace PdfParaXml
{
    public partial class Form2 : Form
    {
        private class resultadosAlterados
        {
            public string nomePaciente { get; set; }
            public string IdExame { get; set; }
        }

        TemplateIPOG.Resultados TemplateIPOG = new TemplateIPOG.Resultados();
        TemplateSollutio.Resultados TemplateSollutio = new TemplateSollutio.Resultados();
        TemplateXML.Resultados TemplateXML = new TemplateXML.Resultados();
        string fileLocation;
        string fileName1;
        public Form2(string locationXML, string fileName ,TemplateIPOG.Resultados resultadosIpog = null, TemplateSollutio.Resultados resultadosSollutio = null, TemplateXML.Resultados resultadosMendelics = null)
        {
            InitializeComponent();
            if (resultadosIpog != null)
            {
                var dtg = new BindingList<TemplateIPOG.Pedido>(resultadosIpog.Pedidos.Where(p => p.CodPedLab == "002").ToList());
                this.dataGridView1.DataSource = dtg;
            }

            if (resultadosSollutio != null)
            {
                var dtg = new BindingList<TemplateSollutio.Pedido>(resultadosSollutio.Pedidos.Where(p => p.CodPedLab == "002").ToList());
                this.dataGridView1.DataSource = dtg;
            }

            if (resultadosMendelics != null)
            {
                var dtg = new BindingList<TemplateXML.Pedido>(resultadosMendelics.Pedidos.Where(p => p.CodPedLab == "002").ToList());
                this.dataGridView1.DataSource = dtg;
            }

            
            TemplateIPOG     = resultadosIpog;
            TemplateSollutio = resultadosSollutio;
            TemplateXML = resultadosMendelics;
            fileLocation = locationXML;
            this.fileName1 = fileName;
            
            this.dataGridView1.Columns["fileName"].Visible = false;
            this.dataGridView1.Columns["SuperExame"].Visible = false;
            this.dataGridView1.Columns["CodPedApoio"].Visible = false;

            this.dataGridView1.Columns["Nome"].Width = 300;
            this.dataGridView1.Columns["Nome"].ReadOnly = true;
            this.dataGridView1.Columns["nomeDoExame"].Width = 300;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            var listaResultados = new List<resultadosAlterados>();
            for (int index = 0; index < dataGridView1.Rows.Count; index++)
            {
                var result = new resultadosAlterados();
                result.nomePaciente = (string)dataGridView1.Rows[index].Cells[3].Value;
                result.IdExame      = (string)dataGridView1.Rows[index].Cells[2].Value;
                listaResultados.Add(result);
            }
            
            if (TemplateIPOG != null)
            {
                foreach (var r in listaResultados)
                {
                    if (TemplateIPOG.Pedidos.Where(p => p.Nome == r.nomePaciente).Any())
                        TemplateIPOG.Pedidos.Where(p => p.Nome == r.nomePaciente).FirstOrDefault().CodPedLab = r.IdExame;
                }
                XmlSerializer xmlSerializer = new XmlSerializer(TemplateIPOG.GetType());
                using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(fileLocation, this.fileName1)))
                {
                    xmlSerializer.Serialize(writer, TemplateIPOG);
                }
                MessageBox.Show("Concluído Ipog");
            }

            if (TemplateSollutio != null)
            {
                foreach (var r in listaResultados)
                {
                    if (TemplateSollutio.Pedidos.Where(p => p.Nome == r.nomePaciente).Any())
                        TemplateSollutio.Pedidos.Where(p => p.Nome == r.nomePaciente).FirstOrDefault().CodPedLab = r.IdExame;
                }
                XmlSerializer xmlSerializer = new XmlSerializer(TemplateSollutio.GetType());
                using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(fileLocation, this.fileName1)))
                {
                    xmlSerializer.Serialize(writer, TemplateSollutio);
                }
                MessageBox.Show("Concluído Sollutio");
            }

            if (TemplateXML != null)
            {
                foreach (var r in listaResultados)
                {
                    if (TemplateXML.Pedidos.Where(p => p.Nome == r.nomePaciente).Any())
                        TemplateXML.Pedidos.Where(p => p.Nome == r.nomePaciente).FirstOrDefault().CodPedLab = r.IdExame;
                }
                XmlSerializer xmlSerializer = new XmlSerializer(TemplateXML.GetType());
                using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(fileLocation, this.fileName1)))
                {
                    xmlSerializer.Serialize(writer, TemplateXML);
                }
                MessageBox.Show("Concluído Mendelics ");
            }

            this.Close();
        }
    }
}
