namespace GestaoTickets.Models
{
    public class SoftwareTicket:Ticket
    {
        private string _software;
        private string _necessidade;
        private string _descIntervencao;

        public string Software
        {
            get { return _software; }
            set { _software = value; }
        }
        public string Necessidade
        {
            get { return _necessidade; }
            set { _necessidade = value; }
        }
        public string DescIntervencao
        {
            get { return _descIntervencao; }
            set { _descIntervencao = value; }
        }

        public SoftwareTicket()
        {
            Tipo = "Software";
            Software = "";
            Necessidade = "";
            DescIntervencao = "";
        }
    }
}
