using iTextSharp.text.pdf;
using PdfParaXml.TemplateXML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfParaXml.TemplateXML;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using PdfParaXml.Functions.PDFImgCapture;
using System.Drawing;

namespace PdfParaXml.Functions.Sollutio
{
    public class SollutioPDF_Reader
    {
        public void SollutioPDFToXML()
        {
            string outputFilePath = Directory.GetCurrentDirectory(); ;
            //string pastaRaiz = @"C:\Users\d9lb\Desktop\TestesPdf\Mendelics\";
            string pastaRaiz = @"C:\Users\d9lb\OneDrive - Eurofins\Área de Trabalho\TestesPdf\Sollutio";


            string[] arquivos = Directory.GetFiles(pastaRaiz, "*.pdf");
            Resultados resultados = new Resultados();
            resultados.Pedidos = new List<Pedido>();

            foreach (var arquivo in arquivos)
            {
                //XML Base científica
                ControleDeLote controleDeLote = new ControleDeLote();
                Pedido pedido = new Pedido();
                List<Pedido> pedidos = new List<Pedido>();
                SuperExame superExame = new SuperExame();
                Exame exame = new Exame();
                ItemDeExame itemDeExame = new ItemDeExame();
                Resultado resultado = new Resultado();
                Valor valor = new Valor();
                Conteudo conteudo = new Conteudo();

                //XML Pardini

                //Capturar resultado de imagem
                SaveExamImageResult(arquivo);

                string nome = "";
                string sexo = "";
                string nomeExame = "";
                string dataNascimento = "";
                string solicitante = "";
                string requisicao = "";
                string cliente = "";
                string codExterno = "";
                string dtAbertura = "";

                string metodo = "";
                string interpretacao = "";

                using (PdfReader reader = new PdfReader(arquivo))
                {
                    var textConted = getPdfText(reader);
                    var pdfLines = textConted.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();

                    foreach(var line in pdfLines)
                    {
                        if (line.Contains("Nome:"))
                            nome = pdfLines[pdfLines.IndexOf(line) - 1];

                        if (line.Contains("Requisição:"))
                            requisicao = pdfLines[pdfLines.IndexOf(line) - 1];

                        if (line.Contains("DN:"))
                            dataNascimento = pdfLines[pdfLines.IndexOf(line) - 1];

                        if (line.Contains("Cliente:"))
                            cliente = pdfLines[pdfLines.IndexOf(line) - 1];

                        if (line.Contains("Código externo:"))
                            codExterno = pdfLines[pdfLines.IndexOf(line) - 1];

                        if (line.Contains("Medico Solicitante:"))
                            solicitante = pdfLines[pdfLines.IndexOf(line) - 1];

                        if (line.Contains("Sexo:"))
                            sexo = getSexo(line);

                        if (line.Contains("Data da Abertura:"))
                        {
                            dtAbertura = getDataAbertura(line);
                            nomeExame = pdfLines[pdfLines.IndexOf(line) + 1];
                        }

                        if (line.Contains("Interpretação:"))
                            interpretacao = getInterPretacao(textConted, reader);

                    }
                }
            }
        }

        private void SaveExamImageResult(string arquivo)
        {
            PDF_ImgCapture pDFImgCapture = new PDF_ImgCapture();
            var fileName = System.IO.Path.GetFileNameWithoutExtension(arquivo);
            pDFImgCapture.CaptureRegionFromPdf(arquivo, 1, fileName, 4);
        }

        private string getInterPretacao(string pdfContend, PdfReader reader)
        {
            string startWord = "46,XY sexo masculino";
            string endWord = "Notas:";
            pdfContend = getPdfTextLastPage(reader);
            int startIndex = pdfContend.IndexOf(startWord);
            int endIndex = pdfContend.IndexOf(endWord);

            if (startIndex != -1 && endIndex != -1)
            {
                int startIndexToUse = startIndex + startWord.Length;
                string result = pdfContend.Substring(startIndexToUse, endIndex - startIndexToUse).Trim();
                return result;
            }
            else
            {
                return "";
            }
        }

        private string getPdfTextLastPage(PdfReader pdfReader)
        {
            string text = "";

            for (int i = 1; i <= pdfReader.NumberOfPages; i++)
            {
                text = PdfTextExtractor.GetTextFromPage(pdfReader, i);
            }

            return text;
        }

        private string getSexo(string pdfContend)
        {
            string metodoPatern = @"(?<=Sexo:).*$";
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

        private string getDataAbertura(string pdfContend)
        {
            string metodoPatern = @"(?<=Data da Abertura:).*$";
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

        private string getPdfText(PdfReader pdfReader)
        {
            string text = "";

            for (int i = 1; i <= pdfReader.NumberOfPages; i++)
            {
                text = text + "\n" + PdfTextExtractor.GetTextFromPage(pdfReader, i);
            }

            return text;
        }
    }
}
