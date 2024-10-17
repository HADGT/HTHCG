using HTHCG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace HTHCG.Controllers
{
    public class CheckSymController : Controller
    {
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
            var txtName = from b in HCGctx.Symptoms select b;
            if (!string.IsNullOrEmpty(Name))
            {
                txtName = txtName.Where(x => x.NameSym.Contains(Name));
            }
            ViewBag.Sys = txtName.ToList();
            return View("Index");
        }

        // GET: CheckSymController/Details/5
        public ActionResult Details(string id)
        {
            var symptom = HCGctx.Symptoms.Find(id);
            if (symptom == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy triệu chứng
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
            SymptomsDisease symptomsDisease = HCGctx.SymptomsDiseases.FirstOrDefault(c => c.IdDis == id && c.IdSym == idcha);
            if (symptomsDisease == null) return NotFound();
            HCGctx.SymptomsDiseases.Remove(symptomsDisease);
            HCGctx.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
