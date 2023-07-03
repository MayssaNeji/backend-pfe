using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class PlanAgence
{
    public int Id { get; set; }

    public string? Agence { get; set; }

    public string? Destination { get; set; }

    public int? Effectif { get; set; }

    public int? BesoinEnBus { get; set; }

    public string? Mois { get; set; }

    public string? Annee { get; set; }

    public int? Navette { get; set; }

    public string? Circuit { get; set; }

    public string? RefSemaine { get; set; }
}
