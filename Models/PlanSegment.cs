using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class PlanSegment
{
    public int? Matricule { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? Samedi { get; set; }

    public string? Dimanche { get; set; }

    public string? Lundi { get; set; }

    public string? Mardi { get; set; }

    public string? Mercredi { get; set; }

    public string? Jeudi { get; set; }

    public string? Vendredi { get; set; }

    public string? Segment { get; set; }

    public string RefSemaine { get; set; } = null!;

    public string? Shift { get; set; }

    public int? Annee { get; set; }

    public int? Mois { get; set; }

    public int Id { get; set; }
    [JsonIgnore]
    public virtual Employe? MatriculeNavigation { get; set; }
    [JsonIgnore]
    public virtual Segment? SegmentNavigation { get; set; }
}
