﻿using iTextSharp.text.pdf;
using PdfParaXml.TemplateSollutio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text.pdf.parser;
using System.Text.RegularExpressions;
using PdfParaXml.Functions.PDFImgCapture;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace PdfParaXml.Functions.Sollutio
{
    public class SollutioPDF_Reader
    {
        public void SollutioPDFToXML(string pastaRaiz, string localizacaoXML)
        {
            string outputFilePath = Directory.GetCurrentDirectory();
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
                List<ItemDeExame> itemDeExame = new List<ItemDeExame>();
                Resultado resultado = new Resultado();
                Valor valor = new Valor();
                Conteudo conteudo = new Conteudo();

                string nome = "";
                string sexo = "";
                string nomeExame = "";
                string dataNascimento = "";
                string solicitante = "";
                string requisicao = "";
                string cliente = "";
                string codExterno = "";
                string dtAbertura = "";
                string material = "";
                string metodo = "";
                string interpretacao = "";
                string resultadoTxt = "";
                string bandeamento = "";
                string numDeCelulas = "";

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
                            interpretacao = nomeExame.Contains("CARIÓTIPO HEMATOLÓGICO") ? line.Replace("Interpretação:", "") : getInterPretacao(textConted, reader);

                        if (line.Contains("Resultado:"))
                            resultadoTxt =  getResultado(line);

                        if (line.Contains("Método:"))
                            material = nomeExame.Contains("CARIÓTIPO HEMATOLÓGICO") ? line.Replace("Método:", "") : $"{pdfLines[pdfLines.IndexOf(line) - 1]} {pdfLines[pdfLines.IndexOf(line) + 1]}";

                        if (line.Contains("Bandeamento:"))
                            bandeamento = nomeExame.Contains("CARIÓTIPO HEMATOLÓGICO") ? line.Replace("Bandeamento:", "") : pdfLines[pdfLines.IndexOf(line) + 1];

                        if(line.Contains("Total de Células"))
                            numDeCelulas = nomeExame.Contains("CARIÓTIPO HEMATOLÓGICO") ? line.Replace("Total de Células Analisadas:", "") : pdfLines[pdfLines.IndexOf(line) + 1];
                    }

                    //Capturar resultado de imagem
                    var img = SaveExamImageResult(arquivo, CreateImgResultDirectory(localizacaoXML), nome, codExterno);

                    resultados.Protocolo = 1;
                    resultados.ID = 200;

                    controleDeLote.Emissor = "Aplicacao pdf para xml centro de genomas";
                    controleDeLote.DataEmissao = DateTime.Now.ToString("dd/MM/yyyy");
                    controleDeLote.HoraEmissao = DateTime.Now.ToString("HH:mm:ss");
                    controleDeLote.CodLab = cliente;

                    pedido.fileName = arquivo;
                    pedido.CodPedApoio = requisicao;

                    Regex regex = new Regex(@"^\d+$");
                    ConsultarBancoDeDados consultarBancoDeDados = new ConsultarBancoDeDados();
                    var dadosExame = getExamesDict();
                    if (string.IsNullOrEmpty(codExterno.Trim()) || !regex.IsMatch(codExterno.Trim()))
                        codExterno = consultarBancoDeDados.GetNumAtendimento(nome, dadosExame[nomeExame][1]).numAtendimento;

                    pedido.CodPedLab = $"002{codExterno}"; //Colocar 002 no começo ;
                    pedido.Nome = nome;
                    pedido.nomeDoExame = dadosExame[nomeExame][1];

                    superExame.MaterialNome = material;
                    superExame.ExameNome = dadosExame[nomeExame][1];
                    superExame.CodExmApoio = $"{dadosExame[nomeExame][1]}|{dadosExame[nomeExame][2]}|1";// Concatenar nome do exame | abreviação | 1;
                    superExame.CodigoFormato = 1;
                    
                    exame.Metodo = metodo;
                    var image = img == null ?  "" : Convert.ToBase64String(img);
                    var resultadosLista = new List<string> { numDeCelulas, bandeamento, interpretacao, material, image }; 
                    valor = getFormatacaoValor();
                    valor.Text = resultadoTxt;

                    resultados.ControleDeLote = controleDeLote;
                    conteudo.Valor = valor;
                    resultado.Conteudo = conteudo;
                    exame.ItemDeExame = getResultadosComVariaveisDefinidas(dadosExame[nomeExame][2], resultadosLista, resultado, nome);
                    superExame.Exame = exame;
                    pedido.SuperExame = superExame;
                    resultados.Pedidos.Add(pedido);
                }
            }

            //var listaDeExames = resultados.Pedidos.Select(p => new ModeloDePDFEExemplo { ExameNome = p.SuperExame.ExameNome, fileName = p.fileName }).GroupBy(Ex => Ex.ExameNome).Select(g => g.First()).ToList();
            XmlSerializer xmlSerializer = new XmlSerializer(resultados.GetType());
            xmlSerializer.Serialize(Console.Out, resultados);           
            var fileName = $"ResultadosSollutio{DateTime.Now.ToString("dd_MM_yyyyy")}.XML"; //System.IO.Path.GetFileName("Lote teste").Replace(".pdf", ".XML");
            //CriadorDePlanilha.CriadorDePlanilha criadorDePlanilha = new CriadorDePlanilha.CriadorDePlanilha();
            //criadorDePlanilha.CriarPlanilhaExcel(listaDeExames, "MendelicsExel");
            var destinoDosPDFs = CreatePdfsDir("PDFs Usados\\PDFs Sollutio");

            var pacientesSemIDAtendimento = resultados.Pedidos.Where(p => p.CodPedLab == "002").ToList();

            if (pacientesSemIDAtendimento.Count > 0)
            {
                Form2 form2 = new Form2(localizacaoXML, fileName,null, resultados);
                form2.Show();
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(localizacaoXML, fileName)))
                {
                    xmlSerializer.Serialize(writer, resultados);
                }
                MessageBox.Show("Concluído Sollutio");
            }
            MoverArquivos(pastaRaiz, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, destinoDosPDFs), arquivos);
        }
        private List<TemplateSollutio.ItemDeExame> getResultadosComVariaveisDefinidas(string exame, List<string> resultados, Resultado resultado ,string nomePaciente)
        {
            var itens = new List<TemplateSollutio.ItemDeExame>();
            var formatacao = getFormatacaoValor();
            switch (exame)
            {
                case "CARIHEMA":
                    itens.Add(new ItemDeExame { Nome = "RES",    Resultado = resultado });
                    itens.Add(new ItemDeExame { Nome = "MET",    Resultado = new Resultado() { Conteudo = new Conteudo() { Valor = new Valor() { Text = resultados[3], CasasDecimais = formatacao.CasasDecimais, TamanhoMaximo = formatacao.TamanhoMaximo, Tipo = formatacao.Tipo } } } });
                    itens.Add(new ItemDeExame { Nome = "RESOLU", Resultado = new Resultado() { Conteudo = new Conteudo() { Valor = new Valor() { Text = resultados[1], CasasDecimais = formatacao.CasasDecimais, TamanhoMaximo = formatacao.TamanhoMaximo, Tipo = formatacao.Tipo } } } });
                    itens.Add(new ItemDeExame { Nome = "NUMERO", Resultado = new Resultado() { Conteudo = new Conteudo() { Valor = new Valor() { Text = resultados[0], CasasDecimais = formatacao.CasasDecimais, TamanhoMaximo = formatacao.TamanhoMaximo, Tipo = formatacao.Tipo } } } });
                    itens.Add(new ItemDeExame { Nome = "OBS",    Resultado = new Resultado() { Conteudo = new Conteudo() { Valor = new Valor() { Text = resultados[2], CasasDecimais = formatacao.CasasDecimais, TamanhoMaximo = formatacao.TamanhoMaximo, Tipo = formatacao.Tipo } } } });
                    itens.Add(new ItemDeExame { Nome = "IMAGEM", Resultado = new Resultado() { Conteudo = new Conteudo() { Valor = new Valor() { Text = resultados[4], CasasDecimais = formatacao.CasasDecimais, TamanhoMaximo = formatacao.TamanhoMaximo, Tipo = formatacao.Tipo } } } });
                    break;

                case "CG_CONSTCATG":
                    itens.Add(new ItemDeExame { Nome = "RESULT", Resultado = resultado });
                    itens.Add(new ItemDeExame { Nome = "RES",    Resultado = new Resultado() { Conteudo = new Conteudo() { Valor = new Valor() { Text = resultados[1], CasasDecimais = formatacao.CasasDecimais, TamanhoMaximo = formatacao.TamanhoMaximo, Tipo = formatacao.Tipo } } } });
                    itens.Add(new ItemDeExame { Nome = "NUMERO", Resultado = new Resultado() { Conteudo = new Conteudo() { Valor = new Valor() { Text = resultados[0], CasasDecimais = formatacao.CasasDecimais, TamanhoMaximo = formatacao.TamanhoMaximo, Tipo = formatacao.Tipo } } } });
                    itens.Add(new ItemDeExame { Nome = "INTERP", Resultado = new Resultado() { Conteudo = new Conteudo() { Valor = new Valor() { Text = resultados[2], CasasDecimais = formatacao.CasasDecimais, TamanhoMaximo = formatacao.TamanhoMaximo, Tipo = formatacao.Tipo } } } });
                    itens.Add(new ItemDeExame { Nome = "IMAGEM", Resultado = new Resultado() { Conteudo = new Conteudo() { Valor = new Valor() { Text = resultados[4], CasasDecimais = formatacao.CasasDecimais, TamanhoMaximo = formatacao.TamanhoMaximo, Tipo = formatacao.Tipo } } } });
                    break;
            }
            return itens;
        }
         private Dictionary<string, List<string>> getExamesDict()
        {
            var examsDict = new Dictionary<string, List<string>>();
            examsDict.Add("CARIÓTIPO CONSTITUCIONAL", new List<string>() { "BANDGSP", "CARIOTIPO DE SANGUE PERIFÉRICO COM BANDEAMENTO G", "CG_CONSTCATG" });              
            examsDict.Add("CARIÓTIPO HEMATOLÓGICO", new List<string>()   { "CARIHEMA", "CARIOTIPO DE DOENÇAS HEMATOLÓGICAS", "CARIHEMA" });             
            examsDict.Add("3", new List<string>() { "BANDGMO", "CARIÓTIPO DE MEDULA ÓSSEA COM BANDEAMENTO G", "BANDGMO" });              
            examsDict.Add("4", new List<string>() { "DUFFY", "GENOTIPAGEM SISTEMA FY(DUFFY)", "DUFFY" });             
            examsDict.Add("5", new List<string>() { "BANDG100", "CARIÓTIPO DE SANGUE PERIFÉRICO(100 CÉLULAS)", "CG_100C" });
            examsDict.Add("6", new List<string>() { "BANDG50", "CARIÓTIPO DE SANGUE PERIFÉRICO(50 CÉLULAS)", "CG_MOSAI" });
            return examsDict;
        }

        private string CreatePdfsDir(string pastaRaiz)
        {
            var path = pastaRaiz;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        static void MoverArquivos(string origem, string destino, string[] arquivos)
        {          
            foreach (var arquivo in arquivos)
            {
                string caminhoOrigem = System.IO.Path.Combine(origem, arquivo);
                string caminhoDestino = System.IO.Path.Combine(destino, System.IO.Path.GetFileName(arquivo));

                // Use o método Move da classe File para mover o arquivo
                try
                {
                    File.Move(caminhoOrigem, caminhoDestino);
                }
                catch
                {
                    MessageBox.Show("Erro ao mover arquivos");
                }
            }
        }

        private string CreateImgResultDirectory(string pastaRaiz)
        {
            var path = System.IO.Path.Combine(pastaRaiz, "ResultadosDeImagemSollutio");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        private byte [] SaveExamImageResult(string arquivo, string destino, string paciente, string numeroAtendimento)
        {
            PDF_ImgCapture pDFImgCapture = new PDF_ImgCapture();
            var fileName = System.IO.Path.GetFileNameWithoutExtension(arquivo);
            return pDFImgCapture.CaptureRegionFromPdf(arquivo, 1, fileName, 4, destino, paciente, numeroAtendimento);
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

        private string getResultado(string pdfContend)
        {
            string metodoPatern = @"(?<=Resultado:).*$";
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
                return result.Replace("Interpretação:", "");
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