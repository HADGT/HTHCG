using HTHCG.Models;
using Microsoft.AspNetCore.Mvc;

namespace HTHCG.Controllers
{
    public class SysController1 : Controller
    {
        private readonly HthcgContext HCGctx = new HthcgContext();
        protected void SetAlert(string message)
        {
            TempData["AlertMessage"] = message;
            TempData["AlertType"] = "alert-light";
        }
        public IActionResult ListSys()
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
            return View("ListSys");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txtInput"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult SymCreate(Symptom data, string txtInput)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(txtInput))
            {
                data.IdSym = "";
                data.NameSym = txtInput;
                HCGctx.Symptoms.Add(data);
                HCGctx.SaveChanges();
            }
            else
            {
                message = "Hãy nhập thông tin cần tạo!";
                SetAlert(message);
            }
            return RedirectToAction(nameof(ListSys));
        }

        [HttpPost]
        public IActionResult SymRemove(string id)
        {
            string message = string.Empty;
            Symptom symptom = HCGctx.Symptoms.Find(id);
            if (symptom == null)
            {
                message = "Dữ liệu không tồn tại!";
                SetAlert(message);
            }
            var symNameRecords = HCGctx.SymptomsDiseases.Where(c => c.IdSym == id).ToList();
            if (symNameRecords.Any())
            {
                HCGctx.SymptomsDiseases.RemoveRange(symNameRecords);
            }
            HCGctx.Symptoms.Remove(symptom);
            HCGctx.SaveChanges();
            return RedirectToAction(nameof(ListSys));
        }

        [HttpGet]
        public IActionResult SymEdit(string id)
        {
            string message = string.Empty;
            Symptom symptom = HCGctx.Symptoms.FirstOrDefault(c => c.IdSym == id);
            if (symptom == null)
            {
                message = "Dữ liệu không tồn tại!";
                SetAlert(message);
            }
            return Json(symptom);
        }

        [HttpPost]
        public JsonResult SymEdit(string id, string name)
        {
            var symptom = HCGctx.Symptoms.FirstOrDefault(c => c.IdSym == id);
            // Cập nhật thông tin
            symptom.NameSym = name;
            // Cập nhật dữ liệu vào database
            HCGctx.Symptoms.Update(symptom);
            HCGctx.SaveChanges();
            return Json(new { success = true, message = "Dữ liệu đã được lưu" });
        }
    }
}
