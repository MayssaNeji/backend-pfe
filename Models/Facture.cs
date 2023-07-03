using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class Facture
{
    public int Id { get; set; }

    public string? Annee { get; set; }

    public string? Mois { get; set; }

    public string? Circuit { get; set; }

    public int? NbrNav { get; set; }

    public double? CoutKm { get; set; }

    public double? NbrKm { get; set; }

    public double? Totale { get; set; }

    public string? Agence { get; set; }
}
