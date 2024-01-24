using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PdfParaXml.TemplateXML
{
    public class TemplateIPOG 
    {
        [XmlRoot("Resultados")]
        public class Resultados
        {
            [XmlElement("Protocolo")]
            public int Protocolo { get; set; }

            [XmlElement("ID")]
            public long ID { get; set; }

            [XmlElement("ControleDeLote")]
            public ControleDeLote ControleDeLote { get; set; }

            [XmlElement("Pedido")]
            public List<Pedido> Pedidos { get; set; }
        }

        public class ControleDeLote
        {
            [XmlElement("Emissor")]
            public string Emissor { get; set; }

            [XmlElement("DataEmissao")]
            public string DataEmissao { get; set; }

            [XmlElement("HoraEmissao")]
            public string HoraEmissao { get; set; }

            [XmlElement("CodLab")]
            public string CodLab { get; set; }
        }

        public class Pedido
        {
            [XmlIgnore]
            public string fileName { get; set; }
            [XmlElement("CodPedApoio")]
            public string CodPedApoio { get; set; }

            [XmlElement("CodPedLab")]
            public string CodPedLab { get; set; }

            [XmlElement("Nome")]
            public string Nome { get; set; }

            [XmlIgnore]
            public string nomeDoExame { get; set; }

            [XmlElement("SuperExame")]
            public SuperExame SuperExame { get; set; }
        }

        public class SuperExame
        {
            [XmlElement("MaterialNome")]
            public string MaterialNome { get; set; }

            [XmlElement("ExameNome")]
            public string ExameNome { get; set; }

            [XmlElement("CodExmApoio")]
            public string CodExmApoio { get; set; }

            [XmlElement("CodExmLab")]
            public string CodExmLab { get; set; }

            [XmlElement("CodigoFormato")]
            public int CodigoFormato { get; set; }

            [XmlElement("Exame")]
            public Exame Exame { get; set; }
        }

        public class Exame
        {
            [XmlElement("Metodo")]
            public string Metodo { get; set; }

            [XmlElement("ItemDeExame")]
            public List<ItemDeExame> ItemDeExame { get; set; }
        }

        public class ItemDeExame
        {
            [XmlElement("Nome")]
            public string Nome { get; set; }

            [XmlElement("Resultado")]
            public Resultado Resultado { get; set; }
        }

        public class Resultado
        {
            [XmlElement("Conteudo")]
            public Conteudo Conteudo { get; set; }
        }

        public class Conteudo
        {
            [XmlElement("Valor")]
            public Valor Valor { get; set; }
        }

        public class Valor
        {
            [XmlAttribute("CasasDecimais")]
            public int CasasDecimais { get; set; }

            [XmlAttribute("TamanhoMaximo")]
            public int TamanhoMaximo { get; set; }

            [XmlAttribute("Tipo")]
            public string Tipo { get; set; }

            [XmlAttribute("idValor")]
            public int IdValor { get; set; }

            [XmlText]
            public string Text { get; set; }
        }
    }
}
