using System;
using System.Collections.Generic;

namespace HTHCG.Models;

public partial class Symptom
{
    public string IdSym { get; set; } = null!;

    public string NameSym { get; set; }

    public virtual ICollection<SymptomsDisease> SymptomsDiseases { get; set; } = new List<SymptomsDisease>();
}
