namespace GestaoTickets.Models
{
    public class HardwareTicket : Ticket
    {
        private string _equipamento;
        private string _avaria;
        private string _descReparacao;
        private string _pecas;

        public string Equipamento 
        {
            get { return _equipamento; }
            set { _equipamento = value;}
        }
        public string Avaria
        {
            get { return _avaria; }
            set { _avaria = value; }
        }
        public string DescReparacao
        {
            get { return _descReparacao; }
            set { _descReparacao = value; }
        }
        public string Pecas
        {
            get { return _pecas; }
            set { _pecas = value; }
        }

        public HardwareTicket()
        {
            Tipo = "Hardware";
            Equipamento = "";
            Avaria = "";
            DescReparacao = "";
            Pecas = "";
        }
    }
}
