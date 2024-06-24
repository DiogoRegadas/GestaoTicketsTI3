using System.Data;
using System.Net.Sockets;
using Microsoft.Data.SqlClient;

namespace GestaoTickets.Models
{
    public class TicketsHelper: SuperHelper
    {
        public List<Ticket> list(string tipo)
        {
            DataTable tickets = new DataTable();
            List<Ticket> outList = new List<Ticket>();
            SqlDataAdapter telefone = new SqlDataAdapter();
            SqlCommand comando = new SqlCommand();
            SqlConnection conexao = new SqlConnection(base.ConnectionDB);


            if (tipo == "Todos")
            {
                comando.CommandText = "SELECT * FROM t_Ticket ORDER BY [dataCriacao] ASC";
            }
            else
            {
                comando.CommandText = "SELECT * FROM t_Ticket WHERE tipo = @tipo ORDER BY [dataCriacao] ASC";
                comando.Parameters.AddWithValue("@tipo", tipo);
            }

            comando.Connection = conexao;
            telefone.SelectCommand = comando;
            telefone.Fill(tickets);

            conexao.Dispose();

            foreach (DataRow linhaticket in tickets.Rows)
            {
                string tipoTicket = linhaticket["tipo"].ToString();
                Ticket ticket;

                if (tipoTicket == "Hardware")
                {
                    var hardwareTicket = new HardwareTicket();
                    PreencherHardwareTicket(hardwareTicket, linhaticket["id"].ToString());
                    ticket = hardwareTicket;
                }
                else if (tipoTicket == "Software")
                {
                    var softwareTicket = new SoftwareTicket();
                    PreencherSoftwareTicket(softwareTicket, linhaticket["id"].ToString());
                    ticket = softwareTicket;
                }
                else
                {
                    ticket = new Ticket();
                }

                
                // Preencher propriedades comuns
                ticket.Id = Guid.Parse(linhaticket["id"].ToString());
                ticket.DataCriacao = Convert.ToDateTime(linhaticket["dataCriacao"]);
                ticket.DataAlteracao = Convert.ToDateTime(linhaticket["dataAlteracao"]);
                ticket.UserCriador = linhaticket["userCriador"].ToString();
                ticket.UserAlteracao = linhaticket["userAlteracao"].ToString();
                ticket.Status = linhaticket["status"].ToString();
                ticket.StatusAtendimento = linhaticket["statusAtendimento"].ToString();
                ticket.Tipo = linhaticket["tipo"].ToString();

                outList.Add(ticket);
            }

            return outList;
        }

        private void PreencherHardwareTicket(HardwareTicket hardwareTicket, string id)
        {
            SqlConnection conexao = new SqlConnection(base.ConnectionDB);
            SqlCommand comando = new SqlCommand();
            DataTable tickets = new DataTable();

            comando.CommandType = CommandType.Text;
            comando.CommandText = "SELECT * FROM t_HardwareTicket WHERE id = @id";
            comando.Parameters.AddWithValue("@id", id.ToString());

            comando.Connection = conexao;
            SqlDataAdapter telefone = new SqlDataAdapter(comando);
            telefone.Fill(tickets);

            if (tickets.Rows.Count > 0)
            {
                DataRow linhaticket = tickets.Rows[0];
                hardwareTicket.Equipamento = linhaticket["equipamento"].ToString();
                hardwareTicket.Avaria = linhaticket["avaria"].ToString();
                hardwareTicket.DescReparacao = linhaticket["descReparacao"].ToString();
                hardwareTicket.Pecas = linhaticket["pecas"].ToString();
            }

            conexao.Close();
            conexao.Dispose();
        }

        private void PreencherSoftwareTicket(SoftwareTicket softwareTicket, string id)
        {
            SqlConnection conexao = new SqlConnection(base.ConnectionDB);
            SqlCommand comando = new SqlCommand();
            DataTable tickets = new DataTable();

            comando.CommandType = CommandType.Text;
            comando.CommandText = "SELECT * FROM t_SoftwareTicket WHERE id = @id";
            comando.Parameters.AddWithValue("@id", id);

            comando.Connection = conexao;
            SqlDataAdapter telefone = new SqlDataAdapter(comando);
            telefone.Fill(tickets);

            if (tickets.Rows.Count > 0)
            {
                DataRow linhaticket = tickets.Rows[0];
                softwareTicket.Software = linhaticket["software"].ToString();
                softwareTicket.Necessidade = linhaticket["necessidade"].ToString();
                softwareTicket.DescIntervencao = linhaticket["descIntervencao"].ToString();
            }

            conexao.Close();
            conexao.Dispose();
        }

        public void Save(Ticket ticket)
        {
            using (SqlConnection conexao = new SqlConnection(base.ConnectionDB))
            {
                conexao.Open();
                SqlTransaction transaction = conexao.BeginTransaction();

                try
                {
                    // Verificar se o ticket já existe na tabela t_Ticket
                    bool ticketExists = false;
                    SqlCommand comandoVerificacao = new SqlCommand
                    {
                        Connection = conexao,
                        Transaction = transaction,
                        CommandType = CommandType.Text,
                        CommandText = "SELECT COUNT(*) FROM t_Ticket WHERE id = @id"
                    };
                    comandoVerificacao.Parameters.AddWithValue("@id", ticket.Id);

                    int count = (int)comandoVerificacao.ExecuteScalar();
                    if (count > 0)
                    {
                        ticketExists = true;
                    }

                    if (!ticketExists)
                    {
              
                        // Inserir o ticket na tabela t_Ticket
                        SqlCommand comandoInsertTicket = new SqlCommand
                        {
                            Connection = conexao,
                            Transaction = transaction,
                            CommandType = CommandType.Text,
                            CommandText = "INSERT INTO t_Ticket (id, dataCriacao, dataAlteracao, userCriador, userAlteracao, status, statusAtendimento, tipo) " +
                                          "VALUES (@id, @dataCriacao, @dataAlteracao, @userCriador, @userAlteracao, @status, @statusAtendimento, @tipo)"
                        };

                        comandoInsertTicket.Parameters.AddWithValue("@id", ticket.Id);
                        comandoInsertTicket.Parameters.AddWithValue("@dataCriacao", ticket.DataCriacao);
                        comandoInsertTicket.Parameters.AddWithValue("@dataAlteracao", ticket.DataCriacao);
                        comandoInsertTicket.Parameters.AddWithValue("@userCriador", ticket.UserCriador);
                        comandoInsertTicket.Parameters.AddWithValue("@userAlteracao", ticket.UserAlteracao);
                        comandoInsertTicket.Parameters.AddWithValue("@status", ticket.Status);
                        comandoInsertTicket.Parameters.AddWithValue("@statusAtendimento", ticket.StatusAtendimento);
                        comandoInsertTicket.Parameters.AddWithValue("@tipo", ticket.Tipo);

                        comandoInsertTicket.ExecuteNonQuery();

                        // Inserir os dados adicionais conforme o tipo de ticket
                        if (ticket is HardwareTicket hardwareTicket)
                        {
                            SqlCommand comandoInsertHardware = new SqlCommand
                            {
                                Connection = conexao,
                                Transaction = transaction,
                                CommandType = CommandType.Text,
                                CommandText = "INSERT INTO t_HardwareTicket (id, equipamento, avaria, descReparacao, pecas) " +
                                              "VALUES (@id, @equipamento, @avaria, @descReparacao, @pecas)"
                            };

                            comandoInsertHardware.Parameters.AddWithValue("@id", hardwareTicket.Id);
                            comandoInsertHardware.Parameters.AddWithValue("@equipamento", hardwareTicket.Equipamento);
                            comandoInsertHardware.Parameters.AddWithValue("@avaria", hardwareTicket.Avaria);
                            comandoInsertHardware.Parameters.AddWithValue("@descReparacao", hardwareTicket.DescReparacao);
                            comandoInsertHardware.Parameters.AddWithValue("@pecas", hardwareTicket.Pecas);

                            comandoInsertHardware.ExecuteNonQuery();
                        }
                        else if (ticket is SoftwareTicket softwareTicket)
                        {
                            SqlCommand comandoInsertSoftware = new SqlCommand
                            {
                                Connection = conexao,
                                Transaction = transaction,
                                CommandType = CommandType.Text,
                                CommandText = "INSERT INTO t_SoftwareTicket (id, software, necessidade, descIntervencao) " +
                                              "VALUES (@id, @software, @necessidade, @descIntervencao)"
                            };

                            comandoInsertSoftware.Parameters.AddWithValue("@id", softwareTicket.Id);
                            comandoInsertSoftware.Parameters.AddWithValue("@software", softwareTicket.Software);
                            comandoInsertSoftware.Parameters.AddWithValue("@necessidade", softwareTicket.Necessidade);
                            comandoInsertSoftware.Parameters.AddWithValue("@descIntervencao", softwareTicket.DescIntervencao);

                            comandoInsertSoftware.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // Se o ticket já existir na base de dados, atualizar os dados
                        SqlCommand comandoUpdateTicket = new SqlCommand
                        {
                            Connection = conexao,
                            Transaction = transaction,
                            CommandType = CommandType.Text,
                            CommandText = "UPDATE t_Ticket SET dataAlteracao = @dataAlteracao, userAlteracao = @userAlteracao, " +
                                          "status = @status, statusAtendimento = @statusAtendimento" +
                                          "WHERE id = @id"
                        };

                        comandoUpdateTicket.Parameters.AddWithValue("@id", ticket.Id);
                        comandoUpdateTicket.Parameters.AddWithValue("@dataAlteracao", ticket.DataAlteracao);
                        comandoUpdateTicket.Parameters.AddWithValue("@userAlteracao", ticket.UserAlteracao);
                        comandoUpdateTicket.Parameters.AddWithValue("@status", ticket.Status);
                        comandoUpdateTicket.Parameters.AddWithValue("@statusAtendimento", ticket.StatusAtendimento);

                        comandoUpdateTicket.ExecuteNonQuery();

                        // Atualizar os dados adicionais conforme o tipo de ticket
                        if (ticket is HardwareTicket hardwareTicket)
                        {
                            SqlCommand comandoUpdateHardware = new SqlCommand
                            {
                                Connection = conexao,
                                Transaction = transaction,
                                CommandType = CommandType.Text,
                                CommandText = "UPDATE t_HardwareTicket SET equipamento = @equipamento, avaria = @avaria, " +
                                              "descReparacao = @descReparacao, pecas = @pecas WHERE id = @id"
                            };

                            comandoUpdateHardware.Parameters.AddWithValue("@id", hardwareTicket.Id);
                            comandoUpdateHardware.Parameters.AddWithValue("@equipamento", hardwareTicket.Equipamento);
                            comandoUpdateHardware.Parameters.AddWithValue("@avaria", hardwareTicket.Avaria);
                            comandoUpdateHardware.Parameters.AddWithValue("@descReparacao", hardwareTicket.DescReparacao);
                            comandoUpdateHardware.Parameters.AddWithValue("@pecas", hardwareTicket.Pecas);

                            comandoUpdateHardware.ExecuteNonQuery();
                        }
                        else if (ticket is SoftwareTicket softwareTicket)
                        {
                            SqlCommand comandoUpdateSoftware = new SqlCommand
                            {
                                Connection = conexao,
                                Transaction = transaction,
                                CommandType = CommandType.Text,
                                CommandText = "UPDATE t_SoftwareTicket SET software = @software, necessidade = @necessidade, " +
                                              "descIntervencao = @descIntervencao WHERE id = @id"
                            };

                            comandoUpdateSoftware.Parameters.AddWithValue("@id", softwareTicket.Id);
                            comandoUpdateSoftware.Parameters.AddWithValue("@software", softwareTicket.Software);
                            comandoUpdateSoftware.Parameters.AddWithValue("@necessidade", softwareTicket.Necessidade);
                            comandoUpdateSoftware.Parameters.AddWithValue("@descIntervencao", softwareTicket.DescIntervencao);

                            comandoUpdateSoftware.ExecuteNonQuery();
                        }
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


        public Ticket GetById(string id)
        {
            Ticket ticket = null;

            using (SqlConnection conexao = new SqlConnection(base.ConnectionDB))
            {
                SqlCommand comando = new SqlCommand();
                comando.Connection = conexao;
                comando.CommandText = "SELECT * FROM t_Ticket WHERE id = @id";
                comando.Parameters.AddWithValue("@id", id);

                conexao.Open();

                SqlDataReader reader = comando.ExecuteReader();

                if (reader.Read())
                {
                    string tipoTicket = reader["tipo"].ToString();

                    if (tipoTicket == "Hardware")
                    {
                        ticket = new HardwareTicket();
                        PreencherHardwareTicket((HardwareTicket)ticket, id);
                    }
                    else if (tipoTicket == "Software")
                    {
                        ticket = new SoftwareTicket();
                        PreencherSoftwareTicket((SoftwareTicket)ticket, id);
                    }
                    else
                    {
                        ticket = new Ticket();
                    }

                    // Preencher propriedades comuns
                    ticket.Id = Guid.Parse(reader["dataCriacao"].ToString());
                    ticket.DataCriacao = Convert.ToDateTime(reader["dataCriacao"]);
                    ticket.DataAlteracao = Convert.ToDateTime(reader["dataAlteracao"]);
                    ticket.UserCriador = reader["userCriador"].ToString();
                    ticket.UserAlteracao = reader["userAlteracao"].ToString();
                    ticket.Status = reader["status"].ToString();
                    ticket.StatusAtendimento = reader["statusAtendimento"].ToString();
                    ticket.Tipo = tipoTicket;
                }

                conexao.Close();
            }

            return ticket;
        }

        public Ticket? get(string uidDoc)
        {
            DataTable tickets = new DataTable();
            Ticket? outDoc = new Ticket();
            SqlDataAdapter telefone = new SqlDataAdapter();
            SqlCommand comando = new SqlCommand();
            SqlConnection conexao = new SqlConnection(base.ConnectionDB);

            comando.CommandType = CommandType.Text;
            comando.CommandText = "SELECT * FROM t_Ticket WHERE id=@uid";
            comando.Parameters.AddWithValue("@uid", uidDoc);

            comando.Connection = conexao;
            telefone.SelectCommand = comando;
            telefone.Fill(tickets);

            conexao.Close();
            conexao.Dispose();

            if (tickets.Rows.Count == 1)
            {
                DataRow linha = tickets.Rows[0];
                outDoc.Id = Guid.Parse(outDoc.Id.ToString());
                outDoc.DataCriacao = Convert.ToDateTime(linha["dataCriacao"]);
                outDoc.DataAlteracao = Convert.ToDateTime(linha["dataAlteracao"]);
                outDoc.UserCriador = linha["userCriador"].ToString();
                outDoc.UserAlteracao = linha["userAlteracao"].ToString();
                outDoc.Status = linha["status"].ToString();
                outDoc.StatusAtendimento = linha["statusAtendimento"].ToString();
                outDoc.Tipo = linha["tipo"].ToString();
                return outDoc;
            }
            else
            {
                outDoc = null;
                
            }

            if (outDoc.Tipo == "Hardware")
            {
                var hardwareTicket = new HardwareTicket();
                PreencherHardwareTicket(hardwareTicket, outDoc.Id.ToString());
                outDoc = hardwareTicket;
            }
            else if (outDoc.Tipo == "Software")
            {
                var softwareTicket = new SoftwareTicket();
                PreencherSoftwareTicket(softwareTicket, outDoc.Id.ToString());
                outDoc = softwareTicket;
            }
            else
            {
                outDoc = null;
            }

            return outDoc;
        }

    }
}
