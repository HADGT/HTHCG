using Azure.Messaging;
using HTHCG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using NuGet.Protocol.Plugins;
using System.Net;

namespace HTHCG.Controllers
{
    public class CheckSymController : Controller
    {
        protected void SetAlert(string message)
        {
            TempData["AlertMessage"] = message;
            TempData["AlertType"] = "alert-light";
        }
        private readonly HthcgContext HCGctx = new HthcgContext();
        // GET: CheckSymController
        public ActionResult Index()
        {
            List<Symptom> symptom = HCGctx.Symptoms.ToList();
            ViewBag.Sys = symptom;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IActionResult SysSearch(string Name)
        {
            string message = string.Empty;
            if (Name == null)
            {
                message = "Hãy nhập thông tin cần tìm kiếm!";
                SetAlert(message);
            }
            else
            {
                var txtName = from b in HCGctx.Symptoms select b;
                if (!string.IsNullOrEmpty(Name))
                {
                    txtName = txtName.Where(x => x.NameSym.Contains(Name));
                }
                ViewBag.Sys = txtName.ToList();
            }
            return View("Index");
        }

        // GET: CheckSymController/Details/5
        public ActionResult Details(string id)
        {
            string message = string.Empty;
            var symptom = HCGctx.Symptoms.Find(id);
            if (symptom == null)
            {
                message = "Hãy nhập thông tin cần tìm kiếm!";
                SetAlert(message);
                return View("Index");
            }
            else
            {
                // Lấy danh sách các bệnh liên quan đến triệu chứng này
                var diseaseDetails = HCGctx.SymptomsDiseases
                    .Where(sd => sd.IdSym == symptom.IdSym)
                    .Select(sd => sd.IdDis) // Lấy ID bệnh
                    .ToList();

                // Lấy thông tin chi tiết của các bệnh dựa trên ID
                var listDisease = HCGctx.Diseases
                    .Where(d => diseaseDetails.Contains(d.IdDis))
                    .ToList();
                ViewBag.Diseases = listDisease;
                ViewBag.idcha = id;
            }
            return PartialView("Details", ViewBag.Diseases + ViewBag.idcha);
        }

        // GET: CheckSymController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CheckSymController/Create
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

        [HttpPost]
        public ActionResult Delete(string id, string idcha)
        {
            string message = string.Empty;
            SymptomsDisease symptomsDisease = HCGctx.SymptomsDiseases.FirstOrDefault(c => c.IdDis == id && c.IdSym == idcha);
            if (symptomsDisease == null)
            {
                message = "Dữ liệu không tồn tại!";
                SetAlert(message);
            }
            HCGctx.SymptomsDiseases.Remove(symptomsDisease);
            HCGctx.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
