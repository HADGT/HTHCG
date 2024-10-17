using HTHCG.Models;
using HTHCG.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HTHCG.Controllers
{
    public class CheckDisController : Controller
    {
        private readonly HthcgContext HCGctx = new HthcgContext();
        // GET: CheckDisController
        public ActionResult Index()
        {
            List<Disease> diseases = HCGctx.Diseases.ToList();
            ViewBag.Dis = diseases;
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public IActionResult DisSearch(string Name)
        {
            var txtName = from b in HCGctx.Diseases select b;
            if (!string.IsNullOrEmpty(Name))
            {
                txtName = txtName.Where(x => x.NameDis.Contains(Name));
            }
            ViewBag.Dis = txtName.ToList();
            return View("Index");
        }

        // GET: CheckDisController/Details/5
        public ActionResult Details(string id)
        {
            // Lấy thông tin triệu chứng theo ID
            var disease = HCGctx.Diseases.Find(id);
            if (disease == null)
            {
                return NotFound(); // Trả về 404 nếu không tìm thấy triệu chứng
            }
            else
            {
                // Lấy danh sách các triệu chứng liên quan đến bệnh này
                var symptomDetails = HCGctx.SymptomsDiseases
                    .Where(sd => sd.IdDis == disease.IdDis)
                    .Select(sd => sd.IdSym) // Lấy ID bệnh
                    .ToList();

                // Lấy thông tin chi tiết của các triệu chứng dựa trên ID
                var listSymptom = HCGctx.Symptoms
                    .Where(d => symptomDetails.Contains(d.IdSym))
                    .ToList();
                ViewBag.Symptoms = listSymptom;
                ViewBag.idcha = id;
            }
            return PartialView("Details", ViewBag.Symptoms + ViewBag.idcha);
        }

        // GET: CheckDisController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CheckDisController/Create
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
            SymptomsDisease symptomsDisease = HCGctx.SymptomsDiseases.FirstOrDefault(c => c.IdSym == id && c.IdDis == idcha);
            if (symptomsDisease == null) return NotFound();
            HCGctx.SymptomsDiseases.Remove(symptomsDisease);
            HCGctx.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
