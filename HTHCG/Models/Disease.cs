using System;
using System.Collections.Generic;

namespace HTHCG.Models;

public partial class Disease
{
    public string IdDis { get; set; } = null!;

    public string? NameDis { get; set; }

    public virtual ICollection<SymptomsDisease> SymptomsDiseases { get; set; } = new List<SymptomsDisease>();
}
