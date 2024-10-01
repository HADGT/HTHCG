using System;
using System.Collections.Generic;

namespace HTHCG.Models;

public partial class SymptomsDisease
{
    public long Id { get; set; }

    public string? IdDis { get; set; }

    public string? IdSym { get; set; }

    public virtual Disease? IdDisNavigation { get; set; }

    public virtual Symptom? IdSymNavigation { get; set; }
}
