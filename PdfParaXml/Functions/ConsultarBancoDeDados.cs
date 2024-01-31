using PdfParaXml.objBancoDeDados;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PdfParaXml.Functions
{
    public class ConsultarBancoDeDados
    {
        public ResultadoConsultaExame GetNumAtendimento(string nomePaciente,string examePleres , string connectionString = "Server=CATGSRV-LD02;Database=Pleres-CentroDeGenomas;User Id=luciano.oliveira;Password=Eurofins$#@!2023;")
        {
            ResultadoConsultaExame resultadoConsultaExame = new ResultadoConsultaExame();
            try
            {
                // Cria uma nova conexão com o SQL Server
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Abre a conexão
                    connection.Open();

                    //consulta SQL
                    string query = "SELECT DISTINCT " +
                                   "    exame.id, " +
                                   " 	pessoa_fisica.str_nome, " +
                                   " 	atendimento.id, " +
                                   " 	atendimento.num_atendimento  " +
                                   " FROM exame_atendimento as ea " +
                                   "     inner Join exame on ea.id_exame = exame.id " +
                                   "     inner join atendimento_exame_resultado on(ea.id = atendimento_exame_resultado.id_exame_atendimento) " +
                                   "     inner join atend_exame_valor_resultado on(atend_exame_valor_resultado.id_resultado = atendimento_exame_resultado.id) " +
                                   "     inner join atendimento on(ea.id_atendimento = atendimento.id) " +
                                   "     inner join prontuario on(atendimento.id_prontuario = prontuario.id) " +
                                   "     inner join pessoa_fisica on(prontuario.id_pessoa = pessoa_fisica.id) " +
                                   $" WHERE exame.str_nome like '%{examePleres}%' " +
                                   $"     AND pessoa_fisica.str_nome like '%{nomePaciente}%'";

                    // Cria um objeto SqlCommand com a consulta e a conexão associada
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        // Executa a consulta
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            // Processa os resultados da consulta
                            while (reader.Read())
                            {
                                resultadoConsultaExame.idExame =        reader["id"].ToString();
                                resultadoConsultaExame.nome =           reader["str_nome"].ToString();
                                resultadoConsultaExame.idAtendimento =  reader["IdAtendimento"].ToString();
                                resultadoConsultaExame.numAtendimento = reader["num_atendimento"].ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
                MessageBox.Show(ex.Message);
            }
            return resultadoConsultaExame;
        }
    }
}
