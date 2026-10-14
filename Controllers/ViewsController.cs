using Microsoft.AspNetCore.Mvc;

namespace EndpointsPage.Controllers
{
    [Route("GamePlays")]
    public class ViewsController : Controller
    {
        private readonly IWebHostEnvironment _env;

        public ViewsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("Login")]
        public IActionResult GetLoginPage()
        {
            // Busca en la raíz física donde se configuró "Frontend"
            var filePath = Path.Combine(_env.WebRootPath, "View", "Login.html");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound($"No se encontró el HTML en la ruta: {filePath}");
            }

            return PhysicalFile(filePath, "text/html");
        }

        [HttpGet("SiginUp")]
        public IActionResult GetDashboardPage()
        {
            var filePath = Path.Combine(_env.WebRootPath, "View", "SiginUp.html");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("La página de Dashboard no existe.");
            }

            return PhysicalFile(filePath, "text/html");
        }
    }
}
