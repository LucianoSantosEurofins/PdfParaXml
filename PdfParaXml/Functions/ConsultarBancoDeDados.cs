using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfParaXml.Functions
{
    public class ConsultarBancoDeDados
    {
        public void GetNumAtendimento(string nomePaciente, string connectionString = "Data Source=192.168.2.13;Initial Catalog=Pleres-CentroDeGenomas;User ID=luciano.oliveira;Password=Eurofins$#@!2023;")
        { 
            try
            {
                // Cria uma nova conexão com o SQL Server
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    // Abre a conexão
                    connection.Open();

                    // Exemplo de uma consulta SQL
                    string query = "    SELECT DISTINCT " +
                                   "     exame.id, " +
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
                                   " WHERE exame.str_nome like '%' " +
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
                                // Exemplo: obtendo valores da coluna "Nome"
                                string nome = reader["Nome"].ToString();
                                Console.WriteLine("Nome: " + nome);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro: " + ex.Message);
            }
        }
    }
}
