using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class Shift
{
    public string ReferenceShift { get; set; } = null!;

    public string? Lundi { get; set; }

    public string? Mardi { get; set; }

    public string? Mercredi { get; set; }

    public string? Jeudi { get; set; }

    public string? Vendredi { get; set; }

    public string? Samedi { get; set; }

    public string? Dimanche { get; set; }

    public virtual ICollection<Employe> Employes { get; } = new List<Employe>();
}
