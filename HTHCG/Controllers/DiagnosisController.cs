using HTHCG.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HTHCG.Controllers
{
    public class DiagnosisController : Controller
    {
        private readonly HthcgContext HCGctx = new HthcgContext();

        public IActionResult Index()
        {
            List<Symptom> symptom = HCGctx.Symptoms.ToList();
            ViewBag.Sys = symptom;
            return View();
        }

        [HttpPost]
        public IActionResult Result(string[] selectedSymptoms)
        {
            Index();
            // Khởi tạo danh sách bệnh khả thi
            // Khởi tạo danh sách bệnh khả thi
            var possibleDiseases = HCGctx.Diseases
                                    .Include(d => d.SymptomsDiseases)          // Liên kết với SymptomsDiseases
                                    .ThenInclude(sd => sd.IdSymNavigation)       // Liên kết với bảng Symptoms thông qua IdSymNavigation
                                    .ToList();
            var matchedDiseases = new List<dynamic>(); // Danh sách chứa thông tin bệnh đã khớp
            foreach (var disease in possibleDiseases)
            {
                // Đếm số triệu chứng khớp
                var matchingCount = disease.SymptomsDiseases.Count(sd => selectedSymptoms.Contains(sd.IdSym));

                // Nếu số triệu chứng khớp > 0
                if (matchingCount > 0)
                {
                    // Tính toán điểm số hoặc tỷ lệ khớp
                    double matchScore = (double)matchingCount / disease.SymptomsDiseases.Count * 100;

                    // Tìm triệu chứng thiếu
                    var missingSymptoms = disease.SymptomsDiseases
                                        .Where(sd => !selectedSymptoms.Contains(sd.IdSym))
                                        .Select(sd => sd.IdSymNavigation.NameSym) // Lấy tên triệu chứng từ bảng Symptoms thông qua IdSymNavigation
                                        .ToList();

                    // Lấy triệu chứng khớp
                    var matchedSymptoms = disease.SymptomsDiseases
                                             .Where(sd => selectedSymptoms.Contains(sd.IdSym))
                                             .Select(sd => sd.IdSymNavigation.NameSym) // Lấy tên triệu chứng khớp
                                             .ToList();

                    // Thêm thông tin bệnh vào danh sách
                    matchedDiseases.Add(new
                    {
                        DiseaseName = disease.NameDis,
                        MatchingCount = matchingCount,
                        MatchScore = matchScore,
                        MissingSymptoms = missingSymptoms, // Thêm triệu chứng thiếu
                        MatchedSymptoms = matchedSymptoms // Thêm triệu chứng khớp
                    });
                }
            }

            // Sắp xếp danh sách bệnh khả thi theo tỷ lệ khớp
            matchedDiseases = matchedDiseases.OrderByDescending(d => d.MatchScore).ToList();

            // Truyền dữ liệu vào ViewBag
            ViewBag.MatchedDiseases = matchedDiseases; // Danh sách bệnh khả thi
            return View("Index");
        }
    }
}
