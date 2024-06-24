using GestaoTickets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace GestaoTickets.Controllers
{
    public class TicketController : Controller
    {

        private Conta? _conta;
        public override void OnActionExecuting(ActionExecutingContext aec)
        {
            base.OnActionExecuting(aec);

            ContaHelper cc = new ContaHelper();
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(Program.SessionContainerName)))
            {
                HttpContext.Session.SetString(Program.SessionContainerName, cc.serializeConta(cc.setGuest()));
            }
            _conta = cc.deserializeConta("" + HttpContext.Session.GetString(Program.SessionContainerName));
            if (_conta != null) ViewBag.ContaAtiva = _conta;
            else ViewBag.ContaAtiva = cc.setGuest();
        }

        [HttpGet]
        public IActionResult Listar(string tipo)
        {
            if (_conta.NivelAcesso > 0)
            {
                TicketsHelper th = new TicketsHelper();
                List<Ticket> list = th.list(tipo);
                ViewBag.TipoEscolhido = tipo;
                return View(list);
            }
            return RedirectToAction("Login", "Conta");
        }

        /*
        public IActionResult Criar() 
        {
            testes testes = new testes();
            SoftwareTicket ticket = new SoftwareTicket();
            ticket.Software = "Windows";
            testes.CriarTicket(ticket);
            return RedirectToAction("Listar", "Ticket");
        }
        */

        [HttpGet]
        public IActionResult Criar()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Criar(Ticket ticket)
        {

            if (_conta.NivelAcesso > 0)
            {
                if (ticket == null) return BadRequest();

                // Identificar o tipo de ticket e criar a instância apropriada
                if (Request.Form["tipoticket"] == "Hardware")
                {
                    HardwareTicket hardwareTicket = new HardwareTicket
                    {
                        Id = ticket.Id,
                        DataCriacao = ticket.DataCriacao,
                        DataAlteracao = ticket.DataAlteracao,
                        UserCriador = _conta.Nome,
                        UserAlteracao = ticket.UserAlteracao,
                        Status = ticket.Status,
                        StatusAtendimento = ticket.StatusAtendimento,
                        Tipo = Request.Form["tipoticket"],
                        // Campos específicos do HardwareTicket
                        Equipamento = Request.Form["equipamento"],
                        Avaria = Request.Form["avaria"],
                    };

                    SaveTicket(hardwareTicket);
                }
                else if (Request.Form["tipoticket"] == "Software")
                {
                    SoftwareTicket softwareTicket = new SoftwareTicket
                    {
                        Id = ticket.Id,
                        DataCriacao = ticket.DataCriacao,
                        DataAlteracao = ticket.DataAlteracao,
                        UserCriador = _conta.Nome,
                        UserAlteracao = ticket.UserAlteracao,
                        Status = ticket.Status,
                        StatusAtendimento = ticket.StatusAtendimento,
                        Tipo = Request.Form["tipoticket"],
                        // Campos específicos do SoftwareTicket
                        Software = Request.Form["software"],
                        Necessidade = Request.Form["necessidade"],

                    };

                    SaveTicket(softwareTicket);
                }
                else
                {
                    return BadRequest("Tipo de ticket inválido.");
                }
            }
            
            return RedirectToAction("Listar", "Ticket", new { tipo = "Todos" });

            
        }
  

        private void SaveTicket(Ticket ticket)
        {
            TicketsHelper th = new TicketsHelper();
            th.Save(ticket);
        }

        /*
        [HttpGet]
        public IActionResult Detalhes(string id)
        {
            if (_conta.NivelAcesso > 0)
            {
                TicketsHelper dh = new TicketsHelper();
                Ticket? ticket = dh.get(id);

                if (ticket == null) return RedirectToAction("Listar", "Documento");
                TempData["Ticket"] = ticket;
                return View();

            }
            return RedirectToAction("Login", "Conta");

        }*/


        [HttpGet]
        public IActionResult Detalhes(string id)
        {
            if (_conta.NivelAcesso > 0)
            {
                TicketsHelper th = new TicketsHelper();
                Ticket ticket = th.get(id); // Obtém o ticket pelo ID

                if (ticket == null)
                {
                    return RedirectToAction("Listar", "Ticket", new { tipo = "Todos" });
                }

                return View(ticket); 

                
            }

            return RedirectToAction("Login", "Conta");
        }


        [HttpPost]
        public IActionResult Detalhes()
        {
            if (_conta.NivelAcesso > 0)
            {

                return RedirectToAction("Listar", "Ticket", new { tipo = "Todos" });


            }

            return RedirectToAction("Login", "Conta");
        }

    }
}
