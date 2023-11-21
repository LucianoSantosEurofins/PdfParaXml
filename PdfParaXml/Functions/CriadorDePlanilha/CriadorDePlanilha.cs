using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PdfParaXml.Functions.Mendelics.MendelicsPDF_Reader;

namespace PdfParaXml.Functions.CriadorDePlanilha
{
    public class CriadorDePlanilha
    {
        public void CriarPlanilhaExcel(List<ModeloDePDFEExemplo> listaDeStrings, string nomeArquivo)
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Dados");


                // Preenche as strings nas células de uma coluna
                int linha = 1;
                var a = listaDeStrings.Count();
                foreach (var texto in listaDeStrings)
                {
                    worksheet.Cell(linha, 1).Value = texto.ExameNome;
                    worksheet.Cell(linha, 2).Value = texto.fileName;
                    linha++;
                }
                workbook.SaveAs(nomeArquivo + ".xlsx");
            }
        }
    }
}
