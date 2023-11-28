using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PdfParaXml.TemplateXML
{
    public class TemplatePardini
    {
        [XmlRoot("Registro")]
        public class Registro
        {
            [XmlElement("Protocolo")]
            public int Protocolo { get; set; }

            [XmlElement("ID")]
            public string ID { get; set; }

            [XmlElement("Lote")]
            public Lote Lote { get; set; }
        }

        public class Lote
        {
            [XmlElement("CodLab")]
            public string CodLab { get; set; }

            [XmlElement("CodLoteLab")]
            public int CodLoteLab { get; set; }

            [XmlElement("DataLote")]
            public DateTime DataLote { get; set; }

            [XmlElement("HoraLote")]
            public string HoraLote { get; set; }

            [XmlElement("Pedido")]
            public List<Pedido> Pedidos { get; set; }
        }

        public class Pedido
        {
            [XmlElement("CodPedLab")]
            public string CodPedLab { get; set; }

            [XmlElement("Paciente")]
            public Paciente Paciente { get; set; }

            [XmlElement("Exame")]
            public Exame Exame { get; set; }
        }

        public class Paciente
        {
            [XmlElement("CodPacLab")]
            public string CodPacLab { get; set; }

            [XmlElement("Nome")]
            public string Nome { get; set; }

            [XmlElement("Sexo")]
            public string Sexo { get; set; }

            [XmlElement("DataNasc")]
            public DateTime DataNasc { get; set; }
        }

        public class Exame
        {
            [XmlElement("CodExmApoio")]
            public string CodExmApoio { get; set; }

            [XmlElement("Conservante")]
            public string Conservante { get; set; }

            [XmlElement("DataColeta")]
            public DateTime DataColeta { get; set; }

            [XmlElement("HoraColeta")]
            public string HoraColeta { get; set; }
        }
    }
}
