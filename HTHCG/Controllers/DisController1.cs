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
            var txtName = from b in HCGctx.Diseases select b;
            if (!string.IsNullOrEmpty(Name))
            {
                txtName = txtName.Where(x => x.NameDis.Contains(Name));
            }
            ViewBag.Dis = txtName.ToList();
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
            if (!string.IsNullOrEmpty(txtInput))
            {
                ViewBag.Error = null;
                data.IdDis = "";
                data.NameDis = txtInput;
                HCGctx.Diseases.Add(data);
                HCGctx.SaveChanges();
                return RedirectToAction(nameof(ListDis));
            }
            else
            {
                ViewBag.Error = "< div class=\"alert alert-success\" role=\"alert\">" + "Dữ liệu thêm bị trống, hãy điền dữ liệu!" + "</div>";
                return View();
            }
        }

        [HttpPost]
        public IActionResult DisRemove(string id)
        {
            Disease disease = HCGctx.Diseases.Find(id);
            if (disease == null) return NotFound();
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
            Disease disease = HCGctx.Diseases.FirstOrDefault(c => c.IdDis == id);
            if (disease == null) return NotFound();
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
