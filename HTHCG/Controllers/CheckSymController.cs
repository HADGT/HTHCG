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

        public ActionResult Create(string id)
        {
            string message = string.Empty;
            var symptom = HCGctx.Symptoms.Find(id);

            if (symptom == null)
            {
                message = "Dữ liệu không tồn tại!";
                SetAlert(message);
                return View("Index");
            }
            else
            {
                var diseaseDetails = HCGctx.SymptomsDiseases
                    .Where(sd => sd.IdSym == id)
                    .Select(sd => sd.IdDis)
                    .ToList();

                var listnoDisease = HCGctx.Diseases
                    .Where(d => !diseaseDetails.Contains(d.IdDis))
                    .ToList();

                ViewBag.Diseases = listnoDisease;
                ViewBag.IdCha = id; // Truyền ID bệnh cha
                ViewBag.Name = symptom.NameSym;
                return View();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="symptomName"></param>
        /// <param name="diseaseIdd"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(string symptomName, string[] diseaseIdd)
        {
            string message = string.Empty;
            // In ra để kiểm tra giá trị của diseaseName
            Console.WriteLine("diseaseName: " + symptomName);
            // Lấy bệnh dựa trên tên
            var symptom = HCGctx.Symptoms.FirstOrDefault(d => d.NameSym == symptomName);
            if (symptom == null)
            {
                message = "Dữ liệu không tồn tại!";
                SetAlert(message);
            }
            else
            {
                if (diseaseIdd != null && diseaseIdd.Length > 0)
                {
                    foreach (var id in diseaseIdd)
                    {
                        var existingDiseases = HCGctx.SymptomsDiseases
                            .FirstOrDefault(ds => ds.IdSym == symptom.IdSym && ds.IdDis == id);

                        if (existingDiseases == null)
                        {
                            // Nếu chưa, thêm triệu chứng vào bệnh
                            var diseaseSymptom = new SymptomsDisease
                            {
                                IdDis = id,
                                IdSym = symptom.IdSym
                            };
                            HCGctx.SymptomsDiseases.Add(diseaseSymptom);
                        }
                        Console.WriteLine($"Bệnh đã chọn: {id}");
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    HCGctx.SaveChanges();
                }
            }
            return RedirectToAction(nameof(Index));
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
