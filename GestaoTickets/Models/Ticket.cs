namespace GestaoTickets.Models
{
    public class Ticket
    {
        private Guid _id;

        private DateTime _dataCriacao;

        private DateTime? _dataAlteracao;

        private string _userCriador;

        private string _userAlteracao;

        //Estado dos tickets: porAtender, emAtendimento, atendido.
        private string _status;
        
        //Estado do atendimento: aberto, resolvido, naoResolvido.
        private string _statusAtendimento;

        private string _tipo;


        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public DateTime DataCriacao
        {
            get { return _dataCriacao;}
            set { _dataCriacao = value;}
        }
        public DateTime? DataAlteracao
        {
            get { return _dataAlteracao;}
            set { _dataAlteracao = value;}
        }
        public string UserCriador
        {
            get { return _userCriador;}
            set { _userCriador = value;}
        }
        public string UserAlteracao
        {
            get { return _userAlteracao;}
            set { _userAlteracao = value;}
        }
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }
        public string StatusAtendimento
        {
            get { return _statusAtendimento;}
            set { _statusAtendimento = value;}
        }
        public string Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }

        }

        public Ticket()
        {
            Id = new Guid();
            DataCriacao = DateTime.Now;
            DataAlteracao = null;
            UserCriador = "";
            UserAlteracao = "";
            Status = "porAtender";
            StatusAtendimento = "Aberto";
        }
    }
}
