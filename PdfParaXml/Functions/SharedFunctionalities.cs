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
    }
}
