using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using TrackerWeb.Models;

namespace TrackerWeb.Controllers
{
    public class AvisoController : Controller
    {
        public IConfiguration Configuration { get; set; }

        public AvisoViewModel model { get; set; }

        public AvisoController(IConfiguration _configuration)
        {
            Configuration = _configuration;
            model = new AvisoViewModel(Configuration);
        }

        // GET: AvisoController
        public ActionResult Index()
        {
            var json = JsonConvert.SerializeObject(model);
            return View("Index",model);
        }

        // GET: AvisoController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AvisoController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AvisoController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AvisoController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AvisoController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AvisoController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AvisoController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
