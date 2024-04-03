using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PdfParaXml.Functions
{
    public class SharedFunctionalities
    {
        public void MoverArquivos(string origem, string destino, string[] arquivos)
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

        public string CreatePdfsDir(string pastaRaiz)
        {
            var path = pastaRaiz;

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        static string RemoveSpecialChars(string input)
        {
            // Remove os caracteres :;,. utilizando Replace
            return input.Replace(":", "").Replace(";", "").Replace(",", "").Replace(".", "");
        }

        private string getResultadoConclusao(string pdfContend, string reader, List<string> resultados, string nomePaciente)
        {
            //pdfContend = CapturarRestante(pdfContend, "RESULTADO");
            string startWord = "CONCLUSÃO";
            string endWord = ".";
            int startIndex = pdfContend.IndexOf(startWord);
            int endIndex = pdfContend.IndexOf(endWord);

            if (startIndex != -1 && endIndex != -1)
            {
                int startIndexToUse = startIndex + startWord.Length;
                string result = pdfContend.Substring(startIndexToUse, endIndex - startIndexToUse).Trim();
                var restanteConclusaoParte1 = resultados[resultados.IndexOf(resultados.First(r => r.Contains(RemoveSpecialChars(result)))) - 1];
                var parte2 = result.Replace("Interpretação:", "");
                //restanteConclusaoParte1.Contains("NEGATIVO") || restanteConclusaoParte1.Contains("POSITIVO") ? parte2.Replace(":", "") :
                var resultadoRetorno = $"{restanteConclusaoParte1}{parte2}".Replace("CONCLUSÃO", "").Replace("Interpretação:", "").Replace(":", "");
                return resultadoRetorno;
            }
            else
            {
                return "";
            }
        }
    }
}
