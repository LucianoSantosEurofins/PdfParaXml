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
                            metodo = getMetodo(line);
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

                itemDeExame.Nome = "RESSFET"; // Verificar como ficará as variáveis 

                valor = getFormatacaoValor();
                valor.Text = diagnostico;

                resultados.ControleDeLote = controleDeLote;
                conteudo.Valor = valor;
                resultado.Conteudo = conteudo;
                itemDeExame.Resultado = resultado;
                exame.ItemDeExame = getResultadosComVariaveisDefinidas(dadosExame[nomeExame][2]);
                superExame.Exame = exame;
                pedido.SuperExame = superExame;
                resultados.Pedidos.Add(pedido);
            }

            var listaDeExames = resultados.Pedidos.Select(p => new ModeloDePDFEExemplo { ExameNome = p.SuperExame.ExameNome, fileName = p.fileName }).GroupBy(Ex => Ex.ExameNome).Select(g => g.First()).ToList();
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

        private List<TemplateIPOG.ItemDeExame> getResultadosComVariaveisDefinidas(string exame, params string[] resultados)
        {
            var itens = new List<TemplateIPOG.ItemDeExame>();
            switch (exame)
            {
                case "DST I":
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "CLTA" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "NEGO" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "MYGE" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "MYHO" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "TRVA" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "URUR" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "URP" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "NOTA" });
                    break;
                case "HPVAB":
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "CAPTURA" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "RLUPC1" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "RLUPC2" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "CONCLUS" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "NOTA" });
                    break;
                case "HPVB":
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "CAPTURA" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "RLUPC1" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "CONCL" });
                    itens.Add(new TemplateIPOG.ItemDeExame() { Nome = "NOTA" });
                    break;
            }

            return itens;
        }

        private string[] getDSTResults(string PDFcontend)
        {
            return null;
        }

        private string[] getHPVABResults(string PDFcontend)
        {
            return null;
        }

        private string[] getHPVB(string PDFcontend)
        {
            return null;
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

        public class ModeloDePDFEExemplo
        {
            public string ExameNome;
            public string fileName;
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

    }
}
