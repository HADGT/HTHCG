using HTHCG.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HTHCG.Controllers
{
    public class DisController1 : Controller
    {
        private readonly HthcgContext HCGctx = new HthcgContext();
        protected void SetAlert(string message)
        {
            TempData["AlertMessage"] = message;
            TempData["AlertType"] = "alert-light";
        }
        public IActionResult ListDis()
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
            return View("ListDis");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txtInput"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DisCreate(Disease data, string txtInput)
        {
            string message = string.Empty;
            if (!string.IsNullOrEmpty(txtInput))
            {
                ViewBag.Error = null;
                data.IdDis = "";
                data.NameDis = txtInput;
                HCGctx.Diseases.Add(data);
                HCGctx.SaveChanges();
            }
            else
            {
                message = "Hãy nhập thông tin cần tạo!";
                SetAlert(message);
            }
            return RedirectToAction(nameof(ListDis));
        }

        [HttpPost]
        public IActionResult DisRemove(string id)
        {
            string message = string.Empty;
            Disease disease = HCGctx.Diseases.Find(id);
            if (disease == null)
            {
                message = "Dữ liệu không tồn tại!";
                SetAlert(message);
            }
            var disNameRecords = HCGctx.SymptomsDiseases.Where(c => c.IdDis == id).ToList();
            if (disNameRecords.Any())
            {
                HCGctx.SymptomsDiseases.RemoveRange(disNameRecords);
            }
            HCGctx.Diseases.Remove(disease);
            HCGctx.SaveChanges();
            //Load l?i d? li?u
            return RedirectToAction(nameof(ListDis));
        }

        [HttpGet]
        public IActionResult DisEdit(string id)
        {
            string message = string.Empty;
            Disease disease = HCGctx.Diseases.FirstOrDefault(c => c.IdDis == id);
            if (disease == null)
            {
                message = "Dữ liệu không tồn tại!";
                SetAlert(message);
            }
            return Json(disease);
        }

        [HttpPost]
        public JsonResult DisEdit(string id, string name)
        {
            var disease = HCGctx.Diseases.FirstOrDefault(c => c.IdDis == id);
            // Cập nhật thông tin
            disease.NameDis = name;
            // Cập nhật dữ liệu vào database
            HCGctx.Diseases.Update(disease);
            HCGctx.SaveChanges();
            return Json(new { success = true, message = "Dữ liệu đã được lưu" });
        }
    }
}
