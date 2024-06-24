using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;

namespace GestaoTickets.Models
{
    public class ContaHelper : SuperHelper
    {


        public Conta setGuest()
        {
            return new Conta
            {
                Uid = Guid.Empty.ToString(),
                Nome = "NoLOGIN",
                Email = "",
                NivelAcesso = 0
            };
        }

        public Conta authUser(string username, string password)
        {
            using (SqlConnection conexao = new SqlConnection(base.ConnectionDB))
            {
                conexao.Open();
                SqlCommand comando = new SqlCommand();
                comando.Connection = conexao;
                comando.CommandType = CommandType.Text;
                comando.CommandText = "SELECT id, username, email, nivelAcesso FROM t_Users WHERE username = @username AND password = @password";
                comando.Parameters.AddWithValue("@username", username);
                comando.Parameters.AddWithValue("@password", password);

                using (SqlDataReader reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Conta
                        {
                            Uid = reader["id"].ToString(),
                            Nome = reader["username"].ToString(),
                            Email = reader["email"].ToString(),
                            NivelAcesso = Convert.ToInt32(reader["nivelAcesso"])
                        };
                    }
                }
            }

            return setGuest();
        }

        public string serializeConta(Conta conta)
        {
            return JsonSerializer.Serialize<Conta>(conta);
        }

        public Conta deserializeConta(string contaSerializada)
        {
            Conta? conta;
            try
            {
                conta = JsonSerializer.Deserialize<Conta>(contaSerializada);
            }
            catch
            {
                conta = null;
            }
            return conta;


        }
    }
}
