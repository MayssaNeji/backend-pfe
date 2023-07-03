using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class Cotisation
{
    public int Id { get; set; }

    public string? Organization { get; set; }

    public string? Annee { get; set; }

    public string? Mois { get; set; }

    public string? Ps { get; set; }

    public string? Employe { get; set; }

    public string? Segment { get; set; }

    public string? Circuit { get; set; }

    public string? Cotisation1 { get; set; }
}
