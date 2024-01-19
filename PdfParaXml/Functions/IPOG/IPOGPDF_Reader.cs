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
using static PdfParaXml.TemplateXML.TemplateIPOG;

namespace PdfParaXml.Functions.IPOG
{
    public class IPOGPDF_Reader
    {
        public void IPOGPdfToXML(string pastaRaiz, string localizacaoXML)
        {
            string outputFilePath = Directory.GetCurrentDirectory(); ;
            string[] arquivos = Directory.GetFiles(pastaRaiz, "*.pdf");
            TemplateIPOG.Resultados resultados = new TemplateIPOG.Resultados();
            resultados.Pedidos = new List<TemplateIPOG.Pedido>();
            foreach (var arquivo in arquivos)
            {

                TemplateIPOG.ControleDeLote controleDeLote = new TemplateIPOG.ControleDeLote();
                TemplateIPOG.Pedido pedido = new TemplateIPOG.Pedido();
                List<TemplateIPOG.Pedido> pedidos = new List<TemplateIPOG.Pedido>();
                TemplateIPOG.SuperExame superExame = new TemplateIPOG.SuperExame();
                TemplateIPOG.Exame exame = new TemplateIPOG.Exame();
                TemplateIPOG.ItemDeExame itemDeExame = new TemplateIPOG.ItemDeExame();
                TemplateIPOG.Resultado resultado = new TemplateIPOG.Resultado();
                TemplateIPOG.Valor valor = new TemplateIPOG.Valor();
                TemplateIPOG.Conteudo conteudo = new TemplateIPOG.Conteudo();

                TemplateIPOG.Exame exame1 = new TemplateIPOG.Exame();
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
                string idade = "";
                string codExterno = "";
                string resultadoSemTratamento = "";
                List<ObjResultado> objResultados = new List<ObjResultado>();

                using (PdfReader reader = new PdfReader(arquivo))
                {

                    var textConted = getPdfText(reader);

                    var pdfLines = textConted.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                    foreach (var line in pdfLines)
                    {
                        if (line.Contains("Nome"))
                        {
                            nome = pdfLines[10];
                            dataNascimento = pdfLines[8];
                            idade = pdfLines[9];
                            codExterno = pdfLines[14]; 
                            nomeExame = pdfLines[16];
                        }

                        if (line.Contains("MATERIAL"))
                            material = getMaterial(line).Trim();

                        if (line.Contains("MÉTODO"))
                        {
                            metodo = getMetodo(line);
                            resultadoSemTratamento = getResultadoSemTratamento(textConted, reader, metodo).Trim();

                            if (nomeExame.Contains("PAINEL DE IST I (CT/NG/MHOM/MGEN/UUREA/UPAR/TVAG)"))
                            {
                                var dadosExames = getExamesDict();
                                var nomeDoExame = dadosExames[nomeExame][2];
                                var resultadoComTratamento = RemoverQuebrasDeLinha(resultadoSemTratamento, nome);
                                exame1.ItemDeExame = getResultadosComVariaveisDefinidas(nomeDoExame, resultadoComTratamento, nome);
                            }

                            if (nomeExame.Contains("CAPTURA HÍBRIDA PARA HPV ALTO E BAIXO RISCO"))
                            {
                                var dadosExames = getExamesDict();
                                var nomeDoExame = dadosExames[nomeExame][2];
                                var resultadoComTratamento = RemoverQuebrasDeLinha(resultadoSemTratamento, nome);
                                exame1.ItemDeExame = getResultadosComVariaveisDefinidas(nomeDoExame, resultadoComTratamento, nome);
                            }

                            if (nomeExame.Contains("CAPTURA HÍBRIDA PARA HPV ALTO RISCO"))
                            {
                                var dadosExames = getExamesDict();
                                var nomeDoExame = dadosExames[nomeExame][2];
                                var resultadoComTratamento = RemoverQuebrasDeLinha(resultadoSemTratamento, nome);
                                exame1.ItemDeExame = getResultadosComVariaveisDefinidas(nomeDoExame, resultadoComTratamento, nome);
                            }

                        }
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
                pedido.CodPedLab = codExterno;
                pedido.Nome = nome;

                superExame.MaterialNome = material;
                var dadosExame = getExamesDict();
                superExame.ExameNome = dadosExame[nomeExame][1];
                superExame.CodExmApoio = $"{dadosExame[nomeExame][1]}|{dadosExame[nomeExame][2]}|1";
                superExame.CodigoFormato = 1;

                exame.Metodo = metodo;

                valor = getFormatacaoValor();
                valor.Text = diagnostico;

                resultados.ControleDeLote = controleDeLote;
                conteudo.Valor = valor;
                resultado.Conteudo = conteudo;
                itemDeExame.Resultado = resultado;
                exame.ItemDeExame = exame1.ItemDeExame;
                superExame.Exame = exame;
                pedido.SuperExame = superExame;
                resultados.Pedidos.Add(pedido);
            }

            //var listaDeExames = resultados.Pedidos.Select(p => new ModeloDePDFEExemplo { ExameNome = p.SuperExame.ExameNome, fileName = p.fileName }).GroupBy(Ex => Ex.ExameNome).Select(g => g.First()).ToList();
            XmlSerializer xmlSerializer = new XmlSerializer(resultados.GetType());
            xmlSerializer.Serialize(Console.Out, resultados);
            var fileName = "ResultadosIPOG.XML"; //System.IO.Path.GetFileName("Lote teste").Replace(".pdf", ".XML");
            //CriadorDePlanilha.CriadorDePlanilha criadorDePlanilha = new CriadorDePlanilha.CriadorDePlanilha();
            //criadorDePlanilha.CriarPlanilhaExcel(listaDeExames, "MendelicsExel");
            var destinoDosPDFs = CreatePdfsDir("PDFs Usados\\PDFs IPOG");
            //MoverArquivos(pastaRaiz, System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, destinoDosPDFs), arquivos);
            using (StreamWriter writer = new StreamWriter(System.IO.Path.Combine(localizacaoXML, fileName)))
            {
                xmlSerializer.Serialize(writer, resultados);
            }
        }

        private string RemoverQuebrasDeLinha(string texto, string nomePaciente)
        {
            string padrao = @":\s*RESULTADOS\s*:";
            texto = texto.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Trim();

            string teste = "";
            MatchCollection correspondencias = Regex.Matches(texto, padrao);

            foreach (Match correspondencia in correspondencias)
            {
                 teste = correspondencia.Value;
            }

            return string.IsNullOrEmpty(teste) ? texto : texto.Replace(teste, "");
        }

        private List<ObjResultado> getResultadosHPVAltoeBaixoRisco(string txtContend, string NomePaciente)
        {
            var resultados = txtContend.Split(':').Where(s => !string.IsNullOrEmpty(s)).ToList();
            var listaOrdenada = new List<ObjResultado>();
            var objResultadoBaixo = new ObjResultado();

            var controleResultadoAltoRisco = false;
            var controleResultadoBaixoRisco = false;
            var controleResultadoRLUBaixoRisco = false;
            foreach (var resultado in resultados)
            {
                if (resultado.Contains("ALTO RISCO") && controleResultadoAltoRisco == false)
                {
                    var objResultado = new ObjResultado();
                    objResultado.nome = "ALTO RISCO";
                    objResultado.resultado = resultados[resultados.IndexOf(resultado) + 1].Contains("POSITIVO") ? "POSITIVO" : "NEGATIVO";
                    string padrao = @"(\d+(?:\.\d{1,2})?)";

                    // Criando um objeto Regex
                    Regex regex = new Regex(padrao);

                    // Encontrando correspondências na string
                    MatchCollection correspondencias = regex.Matches(resultados[resultados.IndexOf(resultado) + 1]);

                    objResultado.RLUPCAlto = $"{correspondencias[0].ToString()},{correspondencias[1].ToString()}";
                    listaOrdenada.Add(objResultado);
                    controleResultadoAltoRisco = true;
                }

                
                if (resultado.Contains("BAIXO RISCO") && controleResultadoBaixoRisco == false)
                {
                    objResultadoBaixo.nome = "BAIXO RISCO";
                    objResultadoBaixo.resultado = resultado.Contains("POSITIVO") ? "POSITIVO" : "NEGATIVO";
                    controleResultadoBaixoRisco = true;
                }

                if (resultado.Contains("RLU/PC") && controleResultadoRLUBaixoRisco == false && controleResultadoBaixoRisco == true)
                {
                    var resultBruto = resultados[resultados.IndexOf(resultado) + 1];
                    string padrao = @"(\d+(?:\.\d{1,2})?)";

                    // Criando um objeto Regex
                    Regex regex = new Regex(padrao);

                    // Encontrando correspondências na string
                    MatchCollection correspondencias = regex.Matches(resultBruto);
                    objResultadoBaixo.RLUPCBaixo = $"{correspondencias[0]},{correspondencias[1]}";
                    listaOrdenada.Add(objResultadoBaixo);
                }

                if (resultado.Contains("RESULTADO"))
                {
                    string resultadoInter = getResultadoInterPretacao(txtContend, txtContend).Replace(":", ""); //txtContend[txtContend.IndexOf("RESULTADO") : 2];
                    string ConclusaoInter = getResultadoConclusao(txtContend, txtContend).Replace(":", ""); //txtContend[txtContend.IndexOf("RESULTADO") : 2];
                    listaOrdenada.Add(new ObjResultado() { nome = "CAPTURA", resultado = resultadoInter, variavel = ConclusaoInter });
                }
            }
            return listaOrdenada;
        }


        private string getResultadoInterPretacao(string pdfContend, string reader)
        {
            string startWord = "RESULTADO";
            string endWord = "CONCLUSÃO";
            pdfContend = reader;
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

        private string getResultadoConclusao(string pdfContend, string reader)
        {
            string startWord = "CONCLUSÃO";
            string endWord = ".";
            pdfContend = reader;
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

        private List<ObjResultado> getResultadosHPVAltoRisco(string txtContend, string NomePaciente)
        {
            var resultados = txtContend.Split(':').Where(s => !string.IsNullOrEmpty(s)).ToList();
            var listaOrdenada = new List<ObjResultado>();
            var objResultado = new ObjResultado();
           // foreach (var resultado in resultados)
           // {
           //     if (resultado.Contains("ALTO RISCO"))
           //     {
           //         objResultado.nome = "ALTO RISCO:";
           //         objResultado.resultado = resultado[resultados.IndexOf(resultado) + 1].ToString().Contains("POSITIVO") ? "POSITIVO" : "NEGATIVO";
           //     }
           //         
           // }

            return listaOrdenada;
        }

        private List<ObjResultado> GetResultadosIST(string txtContend, string NomePaciente)
        {
            // Verifica se a posição fornecida é válida

            var resultados = txtContend.Split(':').Where(s => !string.IsNullOrEmpty(s)).ToList();
            var listaOrdenada = new List<ObjResultado>();
            var resultadoConsiderado = new List<string>() {"NEGATIVO", "POSITIVO"};
            foreach (var resultado in resultados)
            {
                string value = "NEGATIVO";
                string value2 = "POSITIVO";
                string Ignore = "RESULT*";
                MatchCollection matchCollection = Regex.Matches(resultado, value);
                MatchCollection matchCollectionPOSITIVO = Regex.Matches(resultado, value2);
                var proximaIteracao = Regex.Match(resultado, Ignore).Success;

                if (proximaIteracao)
                    continue;

                //Esse codigo trata resultados negativos
                if (resultado.Contains("NEGATIVO"))
                {
                    var result = new ObjResultado();
                    result.nome = resultados[resultados.IndexOf(resultado) - 1].Replace("POSITIVO", "").Replace("NEGATIVO", "").Trim();
                    result.resultado = "NEGATIVO";
                    if (matchCollection.Count >= 2 || resultado.Contains("POSITIVO"))
                    {
                        var posicaoResultadoNegativo = resultado.IndexOf("NEGATIVO");
                        var posicaoResultadoPositivo = resultado.IndexOf("POSITIVO");

                        if (posicaoResultadoNegativo < posicaoResultadoPositivo || !resultado.Contains("POSITIVO"))
                        {
                            result.resultado = "NEGATIVO";
                        }
                        else if (posicaoResultadoNegativo > posicaoResultadoPositivo)
                        {
                            result.resultado = "POSITIVO";
                        }
                    }
                    listaOrdenada.Add(result);
                }
                
                // Caso a linha fique com dois resultados, esse codigo ajusta qual resultado é de qual parametro analisado
                if ((resultado.Contains("POSITIVO") && resultado.Contains("NEGATIVO")) || (matchCollection.Count >= 2) || (matchCollectionPOSITIVO.Count >= 2))
                {
                    var result = new ObjResultado();
                    result.nome = resultado.Replace("POSITIVO", "").Replace("NEGATIVO", "").Trim().Trim();

                    var posicaoResultadoNegativo = resultado.IndexOf("NEGATIVO");
                    var posicaoResultadoPositivo = resultado.IndexOf("POSITIVO");

                    if (posicaoResultadoNegativo > posicaoResultadoPositivo)
                    {
                        result.resultado = "NEGATIVO";
                    }
                    else if (posicaoResultadoNegativo < posicaoResultadoPositivo)
                    {
                        result.resultado = "POSITIVO";
                    }

                    listaOrdenada.Add(result);
                }

                //Esse codigo trata resultados positivos
                if (resultado.Contains("POSITIVO"))
                {
                    var result = new ObjResultado();
                    result.nome = resultados[resultados.IndexOf(resultado) - 1].Replace("POSITIVO", "").Replace("NEGATIVO", "").Trim();
                    result.resultado = "POSITIVO";
                    if (matchCollection.Count >= 2 || resultado.Contains("NEGATIVO"))
                    {
                        var posicaoResultadoNegativo = resultado.IndexOf("NEGATIVO");
                        var posicaoResultadoPositivo = resultado.IndexOf("POSITIVO");

                        if (posicaoResultadoNegativo < posicaoResultadoPositivo)
                        {
                            result.resultado = "NEGATIVO";
                        }
                        else if (posicaoResultadoNegativo > posicaoResultadoPositivo)
                        {
                            result.resultado = "POSITIVO";
                        }
                    }
                    listaOrdenada.Add(result);
                }
            }
            return listaOrdenada;
        }

        private List<TemplateIPOG.ItemDeExame> getResultadosComVariaveisDefinidas(string exame,string resultadoTxt, string nomePaciente)
        {
            var itens = new List<TemplateIPOG.ItemDeExame>();

            switch (exame)
            {
                case "DST I":
                    var resultadosIST = GetResultadosIST(resultadoTxt, nomePaciente);
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "CLTA" });
                    var cltaResult = resultadosIST.First(r => r.nome == "CHLAMYDIA TRACHOMATIS");
                    itens[0].Resultado = new TemplateIPOG.Resultado() { Conteudo = new TemplateIPOG.Conteudo() { Valor = getFormatacaoValor() } };
                    itens[0].Resultado.Conteudo.Valor.Text = cltaResult.resultado;

                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "NEGO" });
                    var negoResult = resultadosIST.First(r => r.nome == "NEISSERIA GONORRHOEAE");
                    itens[1].Resultado = new TemplateIPOG.Resultado() { Conteudo = new TemplateIPOG.Conteudo() { Valor = getFormatacaoValor() } };
                    itens[1].Resultado.Conteudo.Valor.Text = negoResult.resultado;

                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "MYGE" });
                    var mygeResult = resultadosIST.First(r => r.nome == "MYCOPLASMA GENITALIUM");
                    itens[2].Resultado = new TemplateIPOG.Resultado() { Conteudo = new TemplateIPOG.Conteudo() { Valor = getFormatacaoValor() } };
                    itens[2].Resultado.Conteudo.Valor.Text = mygeResult.resultado;

                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "MYHO" });
                    var myhoResult = resultadosIST.First(r => r.nome == "MYCOPLASMA HOMINIS");
                    itens[3].Resultado = new TemplateIPOG.Resultado() { Conteudo = new TemplateIPOG.Conteudo() { Valor = getFormatacaoValor() } };
                    itens[3].Resultado.Conteudo.Valor.Text = myhoResult.resultado;

                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "TRVA" });
                    var trvaResult = resultadosIST.First(r => r.nome == "TRICHOMONAS VAGINALIS");
                    itens[4].Resultado = new TemplateIPOG.Resultado() { Conteudo = new TemplateIPOG.Conteudo() { Valor = getFormatacaoValor() } };
                    itens[4].Resultado.Conteudo.Valor.Text = trvaResult.resultado;

                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "URUR" });
                    var ururResult = resultadosIST.First(r => r.nome == "UREAPLASMA UREALYTICUM");
                    itens[5].Resultado = new TemplateIPOG.Resultado() { Conteudo = new TemplateIPOG.Conteudo() { Valor = getFormatacaoValor() } };
                    itens[5].Resultado.Conteudo.Valor.Text = ururResult.resultado;

                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "URP"  });
                    var urpResult = resultadosIST.First(r => r.nome == "UREAPLASMA PARVUM");
                    itens[6].Resultado = new TemplateIPOG.Resultado() { Conteudo = new TemplateIPOG.Conteudo() { Valor = getFormatacaoValor() } };
                    itens[6].Resultado.Conteudo.Valor.Text = urpResult.resultado;

                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "NOTA" });
                    break;
                case "HPVAB":
                    var resultadosHPVAltoeBaixoRisco = getResultadosHPVAltoeBaixoRisco(resultadoTxt, nomePaciente);
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "CAPTURA" });
                    var CAPTURA = resultadosHPVAltoeBaixoRisco.First(r => r.nome == "CAPTURA");
                    itens[0].Resultado = new TemplateIPOG.Resultado() { Conteudo = new TemplateIPOG.Conteudo() { Valor = getFormatacaoValor() } };
                    itens[0].Resultado.Conteudo.Valor.Text = CAPTURA.resultado;

                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "RLUPC1"  });
                    var RLUPC1 = resultadosHPVAltoeBaixoRisco.First(r => r.nome == "BAIXO RISCO");
                    itens[1].Resultado = new TemplateIPOG.Resultado() { Conteudo = new TemplateIPOG.Conteudo() { Valor = getFormatacaoValor() } };
                    itens[1].Resultado.Conteudo.Valor.Text = RLUPC1.RLUPCBaixo;

                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "RLUPC2"  });
                    var RLUPC2 = resultadosHPVAltoeBaixoRisco.First(r => r.nome == "ALTO RISCO");
                    itens[2].Resultado = new TemplateIPOG.Resultado() { Conteudo = new TemplateIPOG.Conteudo() { Valor = getFormatacaoValor() } };
                    itens[2].Resultado.Conteudo.Valor.Text = RLUPC2.RLUPCAlto;

                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "CONCLUS" });
                    var CONCLUS = resultadosHPVAltoeBaixoRisco.First(r => r.nome == "CAPTURA");
                    itens[3].Resultado = new TemplateIPOG.Resultado() { Conteudo = new TemplateIPOG.Conteudo() { Valor = getFormatacaoValor() } };
                    itens[3].Resultado.Conteudo.Valor.Text = CONCLUS.variavel;

                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "NOTA"    });
                    break;
                case "HPVB":
                    var resultadosHPVAltoRisco = getResultadosHPVAltoRisco(resultadoTxt, nomePaciente);
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "CAPTURA"});
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "RLUPC1" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "CONCL"  });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "NOTA"   });
                    break;
            }
            return itens;
        }

        private string getResultadoSemTratamento(string pdfContend, PdfReader reader, string remover)
        {
            string startWord = "MÉTODO";
            string endWord = "VALOR DE REFERÊNCIA";
            pdfContend = getPdfTextLastPage(reader);
            int startIndex = pdfContend.IndexOf(startWord);
            int endIndex = pdfContend.IndexOf(endWord) == -1 ? pdfContend.IndexOf("VALORES DE REFERÊNCIA") : pdfContend.IndexOf(endWord);

            if (startIndex != -1 && endIndex != -1)
            {
                int startIndexToUse = startIndex + startWord.Length;
                string result = pdfContend.Substring(startIndexToUse, endIndex - startIndexToUse).Trim();
                return result.Replace(remover, "");
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

        private Dictionary<string, List<string>> getExamesDict()
        {
            var examsDict = new Dictionary<string, List<string>>();
            examsDict.Add("PAINEL DE IST I (CT/NG/MHOM/MGEN/UUREA/UPAR/TVAG)", new List<string>() { "DST", "PAINEL PARA INFECÇÕES SEXUALMENTE TRANSMISSÍVEIS (ISTS)", "DST I", "CLTA", "NEGO", "MYGE", "MYHO", "TRVA", "URUR", "URP", "NOTA" });
            examsDict.Add("CAPTURA HÍBRIDA PARA HPV ALTO E BAIXO RISCO", new List<string>() { "HPVCAPAB", "HPV CAPTURA HÍBRIDA (ALTO E BAIXO RISCO)", "HPVAB", "CAPTURA", "RLUPC1", "RLUPC2", "CONCLUS", "NOTA" });
            examsDict.Add("CAPTURA HÍBRIDA PARA HPV ALTO RISCO", new List<string>() { "HPVCAPA", "HPV CAPTURA HÍBRIDA (ALTO RISCO)", "HPVB", "CAPTURA", "RLUPC1", "CONCL", "NOTA"});
            examsDict.Add("A definir", new List<string>() { "CLAGONCH", "CTNG - CAPTURA HÍBRIDA", "CLAGONCH" });
            return examsDict;
        }

        private string getMaterial(string pdfContend)
        {
            string metodoPatern = @"(?<=:).*$";
            Match match = Regex.Match(pdfContend.Trim(), metodoPatern);

            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "";
            }
        }

        private string getMetodo(string pdfContend)
        {
            string metodoPatern = @"(?<=MÉTODO).*$";
            Match match = Regex.Match(pdfContend, metodoPatern);

            if (match.Success)
            {
                return match.Value.Replace(":", "").Trim();
            }
            else
            {
                return "";
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

        private string CreatePdfsDir(string pastaRaiz)
        {
            var path = pastaRaiz;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
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

        private TemplateIPOG.Valor getFormatacaoValor()
        {
            TemplateIPOG.Valor valor = new TemplateIPOG.Valor();
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

        private class ObjResultado
        {
            public string variavel   { get; set; }
            public string resultado  { get; set; }
            public string nome       { get; set; }
            public string RLUPCAlto  { get; set; }
            public string RLUPCBaixo { get; set; }
        }

        public class ModeloDePDFEExemplo
        {
            public string ExameNome;
            public string fileName;
        }

    }
}
