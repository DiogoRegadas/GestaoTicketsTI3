using GestaoTickets.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GestaoTickets.Controllers
{
    public class ContaController : Controller
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Conta");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();

        }

        [HttpPost]
        public IActionResult Login(ContaLogin contaLogin)
        {
            

            if (!string.IsNullOrEmpty(contaLogin.Username) && !string.IsNullOrEmpty(contaLogin.Password))
            {
                ContaHelper ch = new ContaHelper();
                Conta cOut = ch.authUser(contaLogin.Username, contaLogin.Password);

                if (cOut.NivelAcesso == 0)
                {
                    ViewBag.ErrorMessage = "Username or password is invalid.";
                    HttpContext.Session.SetString(Program.SessionContainerName, ch.serializeConta(cOut));
                    return View();
                }

                HttpContext.Session.SetString(Program.SessionContainerName, ch.serializeConta(cOut));
                
                return RedirectToAction("Listar", "Ticket", new { tipo = "Todos" });
            }

            
            ViewBag.ErrorMessage = "Please provide both username and password.";
            return View();
        }










    }


}

