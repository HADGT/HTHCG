using HTHCG.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace HTHCG.Controllers
{
    public class HomeController : Controller
    {
        private readonly HthcgContext HCGctx = new HthcgContext();

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListSys()
        {
            List<Symptom> symptom = HCGctx.Symptoms.ToList();
            ViewBag.Sys = symptom;
            return View();
        }

        public IActionResult ListDis()
        {
            List<Disease> diseases = HCGctx.Diseases.ToList();
            ViewBag.Dis = diseases;
            return View();
        }

        public IActionResult Check()
        {
            return View();
        }
    }
}
