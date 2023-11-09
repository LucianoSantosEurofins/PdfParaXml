using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PdfParaXml.Functions.AlvaroLab;
using System.Xml.Serialization;
using PdfParaXml.TemplateXML;
namespace PdfParaXml.Functions
{
    public class TesteLabAlvaro
    {
        public void Teste(string caminho)
        {

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Resultados));

            using (FileStream fileStream = new FileStream(@"C:\Users\d9lb\Desktop\TestesPdf\ResultadoApoio_3848_20231107170326.XML", FileMode.Open))
            {
                Resultados resultados = (Resultados)xmlSerializer.Deserialize(fileStream);
            }

            string pdfFilePath = @"C:\\Users\d9lb\Desktop\TestesPdf\1372938_OTACILIO ROCHA VASCONCELOS.PDF";
            string outputFilePath = @"C:\\Users\\d9lb\Desktop\\TestesPdf\\output.txt";
            AlvaroLabObject alvaroLabObject = new AlvaroLabObject();

            using (PdfReader reader = new PdfReader(pdfFilePath))
            {
                using (StreamWriter writer = new StreamWriter(outputFilePath))
                {
                    var textConted = getPdfText(reader);
                    var pdfLines = textConted.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                    string nomePaciente = "";
                    string dataNascimentoPaciente = "";
                    string ordemServicoPaciente = "";
                    string dataEmissao = "";
                    string solicitante = "";
                    string resultado = "";
                    string exame = "";
                    string valoresdeReferencia = "";

                    int numLine = 0;
                    foreach (var line in pdfLines)
                    {
                        if (line.Contains("Paciente"))
                        {
                             nomePaciente =           getPaciente(pdfLines ,numLine);
                             dataNascimentoPaciente = getDataNacimentoPaciente(pdfLines, numLine);
                             ordemServicoPaciente =   getOrdemServicoPaciente(pdfLines, numLine);
                             dataEmissao          =   getDataDeEmissao(pdfLines, numLine);
                        }

                        if (line.Contains("Solicitante"))
                        {
                             solicitante = getSolicitante(pdfLines, numLine);
                        }
                            
                        List<string> result = new List<string>();
                        if (line.Contains("Exame"))
                        {
                            result = getExameEResultadoEValorReferencia(pdfLines, numLine);
                            valoresdeReferencia = result[2];
                            resultado = result[1];
                            exame = result[0];
                        }

                        string metodo;
                        if (line.Contains("Método"))
                            metodo = getMetodo(line).Replace("Método:", "");

                        string material;
                        if (line.Contains("Material:"))
                            material = getMaterial(line).Replace("Material:", "");


                        numLine += 1;
                    }

                    alvaroLabObject.nomePaciente = nomePaciente;
                    alvaroLabObject.dataNascimentoPaciente = dataNascimentoPaciente;
                    alvaroLabObject.ordemServicoPaciente = ordemServicoPaciente;
                    alvaroLabObject.dataEmissao = dataEmissao;
                    alvaroLabObject.solicitante = solicitante;
                    alvaroLabObject.resultado = resultado;
                    alvaroLabObject.exame = exame;
                    alvaroLabObject.valoresdeReferencia = valoresdeReferencia;


                }
            }
        }

        private string getMetodo(string pdfContend)
        {
            string metodoPatern = @"Método:\s*(.*)";
            Match match = Regex.Match(pdfContend, metodoPatern);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }

        private string getMaterial(string pdfContend)
        {
            string metodoPatern = @"Material:(.+)";
            Match match = Regex.Match(pdfContend, metodoPatern);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }

        private string getExame(string[] lines, int linhaReferencia)
        {
            string examePatern = @".+";
            Match match = Regex.Match(lines[linhaReferencia + 1], examePatern);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }

        private List<String> getExameEResultadoEValorReferencia(string[] lines, int linhaReferencia)
        {
            string texto = lines[linhaReferencia + 4] ;
            string padrao = @"\b(HLA B27|Não Detectado|Detectado)\b";

            List<string> resultado = new List<string>();
            MatchCollection matches = Regex.Matches(texto, padrao);

            foreach (Match match in matches)
            {
                resultado.Add(match.Value);
            }

            return resultado;
        }

        private string getPaciente(string[] lines, int linhaReferencia)
        {
            string pacientePattern = @"\b[A-Z][a-zA-Z]*\b(?:\s[A-Z][a-zA-Z]*){0,3}";
            Match match = Regex.Match(lines[linhaReferencia +1], pacientePattern);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }

        private string getSolicitante(string[] lines, int linhaReferencia)
        {
            string solicitantePatern = @"\b[a-zA-ZÁÉÍÓÚÃÕÂÊÎÔÛÀÈÌÒÙÇáéíóúãõâêîôûàèìòùç][a-zA-ZÁÉÍÓÚÃÕÂÊÎÔÛÀÈÌÒÙÇáéíóúãõâêîôûàèìòùç\s]+\b";
            Match match = Regex.Match(lines[linhaReferencia + 1], solicitantePatern);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }

        private string getDataDeEmissao(string[] lines, int linhaReferencia)
        {
            string dataEmissaoPattern = @"\d{1,2}/\d{1,2}/\d{4}\s\d{1,2}:\d{1,2}:\d{1,2}";
            Match match = Regex.Match(lines[linhaReferencia + 1], dataEmissaoPattern);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }
        private string getOrdemServicoPaciente(string[] lines, int linhaReferencia)
        {
            string ordemPattern = @"\d{9}";
            Match match = Regex.Match(lines[linhaReferencia + 1], ordemPattern);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }


        private string getDataNacimentoPaciente(string[] lines, int linhaReferencia)
        {
            string dataPattern = @"\b(\d{1,2}\/\d{1,2}\/\d{4})\b";
            Match match = Regex.Match(lines[linhaReferencia + 1], dataPattern);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }
        private string getPdfText(PdfReader pdfReader)
        {
            string text = "";

            for (int i = 1; i <= pdfReader.NumberOfPages; i++)
            {
                 text = PdfTextExtractor.GetTextFromPage(pdfReader, i);
            }

            return text;
        }
    }
}

