using System.Data;
using Microsoft.Data.SqlClient;

namespace GestaoTickets.Models
{
    public class testes: SuperHelper
    {
        public void CriarTicket(Ticket ticket)
        {
            using (SqlConnection conexao = new SqlConnection(base.ConnectionDB))
            {
                conexao.Open();
                SqlTransaction transaction = conexao.BeginTransaction();

                try
                {
                    // Inserir o ticket na tabela t_Ticket
                    SqlCommand comandoTicket = new SqlCommand
                    {
                        Connection = conexao,
                        Transaction = transaction,
                        CommandType = CommandType.Text,
                        CommandText = "INSERT INTO t_Ticket (id, dataCriacao, dataAlteracao, userCriador, userAlteracao, status, statusAtendimento, tipo) " +
                                      "VALUES (@id, @dataCriacao, @dataAlteracao, @userCriador, @userAlteracao, @status, @statusAtendimento, @tipo)"
                    };

                    comandoTicket.Parameters.AddWithValue("@id", ticket.Id);
                    comandoTicket.Parameters.AddWithValue("@dataCriacao", ticket.DataCriacao);
                    comandoTicket.Parameters.AddWithValue("@dataAlteracao", ticket.DataAlteracao);
                    comandoTicket.Parameters.AddWithValue("@userCriador", ticket.UserCriador);
                    comandoTicket.Parameters.AddWithValue("@userAlteracao", ticket.UserAlteracao);
                    comandoTicket.Parameters.AddWithValue("@status", ticket.Status);
                    comandoTicket.Parameters.AddWithValue("@statusAtendimento", ticket.StatusAtendimento);
                    comandoTicket.Parameters.AddWithValue("@tipo", ticket.Tipo);

                    comandoTicket.ExecuteNonQuery();

                    // Inserir os dados adicionais conforme o tipo de ticket
                    if (ticket is HardwareTicket hardwareTicket)
                    {
                        SqlCommand comandoHardware = new SqlCommand
                        {
                            Connection = conexao,
                            Transaction = transaction,
                            CommandType = CommandType.Text,
                            CommandText = "INSERT INTO t_HardwareTicket (id, equipamento, avaria, descReparacao, pecas) " +
                                          "VALUES (@id, @equipamento, @avaria, @descReparacao, @pecas)"
                        };

                        comandoHardware.Parameters.AddWithValue("@id", hardwareTicket.Id);
                        comandoHardware.Parameters.AddWithValue("@equipamento", hardwareTicket.Equipamento);
                        comandoHardware.Parameters.AddWithValue("@avaria", hardwareTicket.Avaria);
                        comandoHardware.Parameters.AddWithValue("@descReparacao", hardwareTicket.DescReparacao);
                        comandoHardware.Parameters.AddWithValue("@pecas", hardwareTicket.Pecas);

                        comandoHardware.ExecuteNonQuery();
                    }
                    else if (ticket is SoftwareTicket softwareTicket)
                    {
                        SqlCommand comandoSoftware = new SqlCommand
                        {
                            Connection = conexao,
                            Transaction = transaction,
                            CommandType = CommandType.Text,
                            CommandText = "INSERT INTO t_SoftwareTicket (id, software, necessidade, descIntervencao) " +
                                          "VALUES (@id, @software, @necessidade, @descIntervencao)"
                        };

                        comandoSoftware.Parameters.AddWithValue("@id", softwareTicket.Id);
                        comandoSoftware.Parameters.AddWithValue("@software", softwareTicket.Software);
                        comandoSoftware.Parameters.AddWithValue("@necessidade", softwareTicket.Necessidade);
                        comandoSoftware.Parameters.AddWithValue("@descIntervencao", softwareTicket.DescIntervencao);

                        comandoSoftware.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}
