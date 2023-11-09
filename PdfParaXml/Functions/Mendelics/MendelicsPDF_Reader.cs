using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PdfParaXml.TemplateXML;
using System.Xml.Serialization;


namespace PdfParaXml.Functions.Mendelics
{
    public class MendelicsPDF_Reader
    {

        public void MendelixPDFsTOXML()
        {
            string outputFilePath = @"C:\\Users\\d9lb\Desktop\\TestesPdf\\output.txt";
            string pastaRaiz = @"C:\Users\d9lb\Desktop\TestesPdf\Mendelics\";


            string[] arquivos = Directory.GetFiles(pastaRaiz, "*.pdf");

            foreach(var arquivo in arquivos)
            {
                Resultados resultados = new Resultados();
                ControleDeLote controleDeLote = new ControleDeLote();
                Pedido pedido = new Pedido();
                List<Pedido> pedidos = new List<Pedido>();
                SuperExame superExame = new SuperExame();
                Exame exame = new Exame();
                ItemDeExame itemDeExame = new ItemDeExame();
                Resultado resultado = new Resultado();
                Valor valor = new Valor();
                Conteudo conteudo = new Conteudo();

                string nome = "";
                string sexo = "";
                string nomeExame = "";
                string dataNascimento = "";
                string solicitante = "";
                string sumarioClinico = "";
                string material = "";
                string entradaLab = "";
                string liberacaoResult = "";
                string diagnostico = "";
                string exameYT = "";

                using (PdfReader reader = new PdfReader(arquivo))
                {
                    using (StreamWriter writer = new StreamWriter(outputFilePath))
                    {
                        var textConted = getPdfText(reader);
                        var pdfLines = textConted.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                        foreach (var line in pdfLines)
                        {
                            if (line.Contains("Nome:"))
                                nome = getNome(line);

                            if (line.Contains("Sexo:"))
                                sexo = getSexo(line);

                            if (line.Contains("Exame:"))
                                nomeExame = getExame(line);

                            if (line.Contains("Data de nascimento:"))
                                dataNascimento = getDataDeNascimento(line);

                            if (line.Contains("Solicitante:"))
                                solicitante = getSolicitante(line);

                            if (line.Contains("Sumário clínico:"))
                                sumarioClinico = getSumarioClinico(line);

                            if (line.Contains("Material:"))
                                material = getMaterial(line);

                            if (line.Contains("Entrada no laboratório:"))
                                entradaLab = getEntradanoLaboratório(line);

                            if (line.Contains("Liberação do resultado:"))
                                liberacaoResult = getLiberaçãodoResultado(line);

                            if (line.Contains("Diagnóstico:"))
                                liberacaoResult = getDiagnostico(line);

                            if (line.Contains("Exame"))
                                exameYT = getExameYT(line);

                            if (line.Contains("Resultado"))
                                diagnostico = getDiagnostico(textConted);
                        }
                    }
                }

                resultados.Protocolo = 1;
                resultados.ID = 200;

                controleDeLote.Emissor = "Aplicacao pdf para xml";
                controleDeLote.DataEmissao = DateTime.Now.ToString();
                controleDeLote.HoraEmissao = DateTime.Now.ToString();
                controleDeLote.CodLab = "Centro de genomas";

                pedido.CodPedApoio = 1;
                pedido.CodPedLab = "Teste"; //Provavelmente precisara ser ajustado futuramente
                pedido.Nome = nome;

                superExame.MaterialNome = "Teste Material";
                superExame.ExameNome = nomeExame;
                superExame.CodExmApoio = "Teste exameApoio";
                superExame.CodigoFormato = 1;

                exame.Metodo = "teste metodo";

                itemDeExame.Nome = "RESSFET";

                valor.CasasDecimais = 0;
                valor.TamanhoMaximo = 245;
                valor.Tipo = "alfanumerico";
                valor.IdValor = 1;
                valor.Text = diagnostico;
                valor.CasasDecimais = 0;
                valor.TamanhoMaximo = 245;
                valor.Tipo = "alfanumerico";
                valor.IdValor = 1;

                resultados.ControleDeLote = controleDeLote;
                conteudo.Valor = valor;
                resultado.Conteudo = conteudo;
                itemDeExame.Resultado = resultado;
                exame.ItemDeExame = itemDeExame;
                superExame.Exame = exame;
                pedido.SuperExame = superExame;
                resultados.Pedidos = new List<Pedido>() { pedido };


                XmlSerializer xmlSerializer = new XmlSerializer(resultados.GetType());
                xmlSerializer.Serialize(Console.Out, resultados);
                var fileName = System.IO.Path.GetFileName(arquivo).Replace(".pdf", ".XML");
                using (StreamWriter writer = new StreamWriter(fileName))
                {
                    xmlSerializer.Serialize(writer, resultados);
                }
            }
        }

        private string getExameYT(string pdfContend)
        {
            string metodoPatern = @"(?<=Exame).*$";
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

        private string getResultadoText(string pdfContend)
        {
            string pattern = @"Resultado:(.*?)(?=Comentários|$)";
            Regex regex = new Regex(pattern, RegexOptions.Singleline);
            Match match = regex.Match(pdfContend);

            if (match.Success)
            {
                string result = match.Groups[1].Value.Trim();
                return result;
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

        private string getDiagnostico(string pdfContend)
        {
            string startWord = "Resultado";
            string endWord = "Comentários";
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

        private string getExame(string pdfContend)
        {
            string metodoPatern = @"(?<=Exame:).*$";
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

        private string getDataDeNascimento(string pdfContend)
        {
            string metodoPatern = @"(?<=Data de nascimento:).*$";
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

        private string getSolicitante(string pdfContend)
        {
            string metodoPatern = @"(?<=Solicitante:).*$";
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

        private string getSumarioClinico(string pdfContend)
        {
            string metodoPatern = @"(?<=Sumário clínico:).*$";
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
            string metodoPatern = @"(?<=Material:).*$";
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

        private string getEntradanoLaboratório(string pdfContend)
        {
            string metodoPatern = @"(?<=Entrada no laboratório:).*$";
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

        private string getLiberaçãodoResultado(string pdfContend)
        {
            string metodoPatern = @"(?<=Liberação do resultado:).*$";
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

        private string getNome(string pdfContend)
        {
            string metodoPatern = @"(?<=Nome:).*$";
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
    }

}
