using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class Employe
{
    public int Matricule { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? ContreMaitre { get; set; }

    public string? NomDuGroupe { get; set; }

    public string? Ps { get; set; }

    public int? Telephone { get; set; }

    public string? CentreDeCout { get; set; }

    public string? Station { get; set; }

    public string? Segment { get; set; }

    public string? Shift { get; set; }

    public virtual ICollection<PlanSegment> PlanSegments { get; } = new List<PlanSegment>();
    [JsonIgnore]
    public virtual Segment? SegmentNavigation { get; set; }
    [JsonIgnore]
    public virtual Shift? ShiftNavigation { get; set; }
    [JsonIgnore]
    public virtual Station? StationNavigation { get; set; }
}
