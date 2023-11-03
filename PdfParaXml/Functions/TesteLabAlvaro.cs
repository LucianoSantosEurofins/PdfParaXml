using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;

namespace PdfParaXml.Functions
{
    public class TesteLabAlvaro
    {
        public void Teste(string caminho)
        {
            string pdfFilePath = @"C:\\Users\d9lb\Desktop\TestesPdf\1372862_GILBERTO ROSA FILHO.PDF";
            string outputFilePath = @"C:\\Users\\d9lb\Desktop\\TestesPdf\\output.txt";

            using (PdfReader reader = new PdfReader(pdfFilePath))
            {
                using (StreamWriter writer = new StreamWriter(outputFilePath))
                {
                    var textConted = getPdfText(reader);
                    var pdfLines = textConted.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                    bool paciente;
                    foreach (var line in pdfLines)
                    {

                        var nomePaciente = getPaciente(line);
                        var dataNascimentoPaciente = getDataNacimentoPaciente(line);
                        var ordemServicoPaciente = getOrdemServicoPaciente(line);
                        var dataEmissao = getDataDeEmissao(line);
                    }

                }
            }
        }
        private string getPaciente(string pdfContend)
        {
            //string pacientePattern = @"Paciente\s*(.+?)\s(\d{2}/\d{2}/\d{4})\s(\d+)\s(\d{2}/\d{2}/\d{4}\s\d{2}:\d{2}:\d{2})";
            string solicitantePattern = @"Solicitante\s*(.+?)\s(\d+)\s(\d+)\s(\d{2}/\d{2}/\d{4}\s\d{2}:\d{2}:\d{2})";
            string instituicaoPattern = @"Instituição\sLocal\s*(.+)";
            string dataColetaPattern = @"Data\ de\ coleta:\s(\d{2}/\d{2}/\d{4}\s\d{2}:\d{2}:\d{2})";
            string metodoPattern = @"Método:\s(.+)";
            string materialPattern = @"Material:\s(.+)";
            string responsavelPattern = @"Responsável\sTécnico:\s(.+)";
            string liberadoPorPattern = @"Liberado\s\por:\s(.+)";
            string locaisExecucaoPattern = @"Locais\ de\ execução\ dos\ exames:\s(.+)";
            string sobResponsabilidadePattern = @"Sob\ a\ responsabilidade\ do\ Dr\.\s(.+)";

            string pacientePattern = @"\b[A-Z][a-zA-Z]*\b(?:\s[A-Z][a-zA-Z]*){0,3}";
            Match match = Regex.Match(pdfContend, pacientePattern);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }

        private string getDataDeEmissao(string pdfContend)
        {
            string dataEmissaoPattern = @"\d{1,2}/\d{1,2}/\d{4}\s\d{1,2}:\d{1,2}:\d{1,2}";
            Match match = Regex.Match(pdfContend, dataEmissaoPattern);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }
        private string getOrdemServicoPaciente(string pdfContend)
        {
            string ordemPattern = @"\d{9}";
            Match match = Regex.Match(pdfContend, ordemPattern);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }

        private string getDataNacimentoPaciente(string pdfContend)
        {
            string dataPattern = @"\b(\d{1,2}\/\d{1,2}\/\d{4})\b";
            Match match = Regex.Match(pdfContend, dataPattern);
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

