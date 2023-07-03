using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class PlanHebdo
{
    public Guid Id { get; set; }

    public int? Matricule { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? Segment { get; set; }

    public string? Ps { get; set; }

    public string? Organization { get; set; }

    public string? Circuit { get; set; }

    public string? Station { get; set; }

    public string? Lundi { get; set; }

    public string? Mardi { get; set; }

    public string? Mercredi { get; set; }

    public string? Jeudi { get; set; }

    public string? Samedi { get; set; }

    public string? RefSemaine { get; set; }

    public string? Shift { get; set; }

    public string? Mois { get; set; }

    public string? Annee { get; set; }

    public string? Vendredi { get; set; }

    public string? Dimanche { get; set; }
}
