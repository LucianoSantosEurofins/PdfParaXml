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
using PdfParaXml.Functions.CriadorDePlanilha;


namespace PdfParaXml.Functions.Mendelics
{
    public class MendelicsPDF_Reader
    {

        public void MendelixPDFsTOXML(string pastaRaiz, string localizacaoXML)
        {
            string outputFilePath = Directory.GetCurrentDirectory(); ;
            string[] arquivos = Directory.GetFiles(pastaRaiz, "*.pdf");
            Resultados resultados = new Resultados();
            resultados.Pedidos = new List<Pedido>();
            foreach (var arquivo in arquivos)
            {

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
                string metodo = "";

                using (PdfReader reader = new PdfReader(arquivo))
                {
                    //using (StreamWriter writer = new StreamWriter(outputFilePath))
                    //{
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
                                liberacaoResult = getDiagnostico(line, exameYT);

                            if (line.Contains("Exame") && !line.Contains(":"))
                                exameYT = getExameYT(line);

                            if (line.Contains("Resultado"))
                                diagnostico = getDiagnostico(textConted, exameYT);

                            if (line.Contains("Método"))
                                metodo = getMetodo(textConted, reader);
                    }
                }

                ConsultarBancoDeDados consultarBancoDeDados = new ConsultarBancoDeDados();

                resultados.Protocolo = 1;
                resultados.ID = 200;

                controleDeLote.Emissor = "Aplicacao pdf para xml";
                controleDeLote.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy");
                controleDeLote.HoraEmissao = DateTime.Now.ToString("HH:mm:ss");
                controleDeLote.CodLab = "Centro de genomas";

                pedido.fileName = arquivo;
                pedido.CodPedApoio = exameYT; //Provavelmente precisara ser ajustado futuramente
                pedido.CodPedLab = consultarBancoDeDados.GetNumAtendimento(nome, nomeExame).nome;
                pedido.Nome = nome;

                superExame.MaterialNome = material;
                superExame.ExameNome = nomeExame;
                superExame.CodExmApoio = "Teste exameApoio";
                superExame.CodigoFormato = 1;

                exame.Metodo = metodo;

                itemDeExame.Nome = "RESSFET"; // Verificar como ficará as variáveis 

                valor = getFormatacaoValor();
                valor.Text = diagnostico;

                resultados.ControleDeLote = controleDeLote;
                conteudo.Valor = valor;
                resultado.Conteudo = conteudo;
                itemDeExame.Resultado = resultado;
                exame.ItemDeExame = itemDeExame;
                superExame.Exame = exame;
                pedido.SuperExame = superExame;
                resultados.Pedidos.Add(pedido);
            }

            var listaDeExames = resultados.Pedidos.Select(p => new ModeloDePDFEExemplo { ExameNome = p.SuperExame.ExameNome, fileName = p.fileName }).GroupBy(Ex => Ex.ExameNome).Select(g => g.First()).ToList();
            XmlSerializer xmlSerializer = new XmlSerializer(resultados.GetType());
            xmlSerializer.Serialize(Console.Out, resultados);
            var fileName = "ResultadosMendelics.XML"; //System.IO.Path.GetFileName("Lote teste").Replace(".pdf", ".XML");
            //CriadorDePlanilha.CriadorDePlanilha criadorDePlanilha = new CriadorDePlanilha.CriadorDePlanilha();
            //criadorDePlanilha.CriarPlanilhaExcel(listaDeExames, "MendelicsExel");
            var destinoDosPDFs = CreatePdfsDir("PDFs Usados\\PDFs Mendelics");
            MoverArquivos(pastaRaiz, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, destinoDosPDFs), arquivos);
            using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(localizacaoXML, fileName)))
            {
                xmlSerializer.Serialize(writer, resultados);
            }
        }

        static void MoverArquivos(string origem, string destino, string[] arquivos)
        {

            foreach (var arquivo in arquivos)
            {
                string caminhoOrigem = System.IO.Path.Combine(origem, arquivo);
                string caminhoDestino = System.IO.Path.Combine(destino, System.IO.Path.GetFileName(arquivo));

                // Use o método Move da classe File para mover o arquivo
                File.Move(caminhoOrigem, caminhoDestino);
            }
        }

        public class ModeloDePDFEExemplo
        {
            public string ExameNome;
            public string fileName;
        }

        private string CreatePdfsDir(string pastaRaiz)
        {
            var path = pastaRaiz;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        private Valor getFormatacaoValor()
        {
            Valor valor = new Valor();
            valor.CasasDecimais = 0;
            valor.TamanhoMaximo = 245;
            valor.Tipo = "alfanumerico";
            valor.IdValor = 1;
            valor.CasasDecimais = 0;
            valor.TamanhoMaximo = 245;
            valor.Tipo = "alfanumerico";
            valor.IdValor = 1;
            return valor;
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

        private string getDiagnostico(string pdfContend, string exameYT)
        {
            string startWord = "Resultado";
            string endWord = "Comentários";
            string endWord2 = "Genes analisados:";

            int startIndex = pdfContend.IndexOf(startWord);
            int endIndex = pdfContend.IndexOf(endWord);
            int endIndex2 = pdfContend.IndexOf(endWord2);
            if (endIndex > endIndex2 && endIndex2 > 0)
                endIndex = endIndex2;

            if (startIndex != -1 && endIndex != -1)
            {
                int startIndexToUse = startIndex + startWord.Length;
                string result = pdfContend.Substring(startIndexToUse, endIndex - startIndexToUse).Trim();
                var fraseDeVerificacaoDeQuebraDLinha = "A seção de resultados continua na próxima página.";
                var textoASerRemovido = "";
                if (result.Contains(fraseDeVerificacaoDeQuebraDLinha))
                {
                    string startWord2 = fraseDeVerificacaoDeQuebraDLinha;
                    string endWordTwo = "mendelics.com";
                    var t = result.Count();
                    int startIndex2 = result.IndexOf(startWord2);
                    int endIndexTwo = result.IndexOf(endWordTwo);
                    int startIndexToUse2 = startIndex2 + startWord2.Length;
                    textoASerRemovido = pdfContend.Substring(startIndexToUse2, endIndexTwo - startIndexToUse2).Trim();
                    result = result.Remove(startIndex2, endIndexTwo - startIndex2);//.Replace(char.Parse( "Exame" + exameYT), '.');
                }

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

        private string getPdfTextLastPage(PdfReader pdfReader)
        {
            string text = "";

            for (int i = 1; i <= pdfReader.NumberOfPages; i++)
            {
                text = PdfTextExtractor.GetTextFromPage(pdfReader, i);
            }

            return text;
        }

        private string getMetodo(string pdfContend, PdfReader reader)
        {
            string startWord = "Método";
            string endWord = "Responsável:";
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
    }
}
