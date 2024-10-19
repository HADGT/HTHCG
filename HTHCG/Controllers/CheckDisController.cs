using HTHCG.Models;
using HTHCG.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HTHCG.Controllers
{
    public class CheckDisController : Controller
    {
        private readonly HthcgContext HCGctx = new HthcgContext();
        // GET: CheckDisController
        protected void SetAlert(string message)
        {
            TempData["AlertMessage"] = message;
            TempData["AlertType"] = "alert-light";
        }
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
            string message = string.Empty;
            if (Name == null)
            {
                message = "Hãy nhập thông tin cần tìm kiếm!";
                SetAlert(message);
            }
            else
            {
                var txtName = from b in HCGctx.Diseases select b;
                if (!string.IsNullOrEmpty(Name))
                {
                    txtName = txtName.Where(x => x.NameDis.Contains(Name));
                }
                ViewBag.Dis = txtName.ToList();
            }
            return View("Index");
        }

        // GET: CheckDisController/Details/5
        public ActionResult Details(string id)
        {
            string message = string.Empty;
            // Lấy thông tin triệu chứng theo ID
            var disease = HCGctx.Diseases.Find(id);
            if (disease == null)
            {
                message = "Dữ liệu không tồn tại!";
                SetAlert(message);
                return View("Index");
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
                return PartialView("Details", ViewBag.Symptoms + ViewBag.idcha);
            }
        }

        public ActionResult Create(string id)
        {
            string message = string.Empty;
            var disease = HCGctx.Diseases.Find(id);

            if (disease == null)
            {
                message = "Dữ liệu không tồn tại!";
                SetAlert(message);
                return View("Index");
            }
            else
            {
                // Lấy danh sách ID triệu chứng liên quan đến bệnh
                var symptomDetails = HCGctx.SymptomsDiseases
                    .Where(sd => sd.IdDis == id)
                    .Select(sd => sd.IdSym)
                    .ToList();

                // Lấy danh sách các triệu chứng không liên quan đến bệnh
                var listnoSymptom = HCGctx.Symptoms
                    .Where(d => !symptomDetails.Contains(d.IdSym))
                    .ToList();

                ViewBag.Symptoms = listnoSymptom;
                ViewBag.IdCha = id; // Truyền ID bệnh cha
                ViewBag.Name = disease.NameDis;
                return View();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="diseaseName"></param>
        /// <param name="symptomIds"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(string diseaseName, string[] symptomIds)
        {
            string message = string.Empty;
            // In ra để kiểm tra giá trị của diseaseName
            Console.WriteLine("diseaseName: " + diseaseName);
            // Lấy bệnh dựa trên tên
            var disease = HCGctx.Diseases.FirstOrDefault(d => d.NameDis == diseaseName);
            if (disease == null)
            {
                message = "Dữ liệu không tồn tại!";
                SetAlert(message);
            }
            else
            {
                if (symptomIds != null && symptomIds.Length > 0)
                {
                    foreach (var id in symptomIds)
                    {
                        // Kiểm tra nếu triệu chứng đã được gán cho bệnh này chưa
                        var existingSymptom = HCGctx.SymptomsDiseases
                            .FirstOrDefault(ds => ds.IdDis == disease.IdDis && ds.IdSym == id);

                        if (existingSymptom == null)
                        {
                            // Nếu chưa, thêm triệu chứng vào bệnh
                            var diseaseSymptom = new SymptomsDisease
                            {
                                IdDis = disease.IdDis,
                                IdSym = id
                            };
                            HCGctx.SymptomsDiseases.Add(diseaseSymptom);
                        }
                        Console.WriteLine($"Triệu chứng đã chọn: {id}");
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
            SymptomsDisease symptomsDisease = HCGctx.SymptomsDiseases.FirstOrDefault(c => c.IdSym == id && c.IdDis == idcha);
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
