using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class Circuit
{
    public string RefSapLeoni { get; set; } = null!;

    public int? NbKm { get; set; }

    public int? ContributionEmploye { get; set; }

    public int? CoutKm { get; set; }

    public string? PointArrivee { get; set; }

    public string? Agence { get; set; }

    public int Id { get; set; }

    public string? RefChemin { get; set; }
    [JsonIgnore]
    public virtual Agence? AgenceNavigation { get; set; }
    [JsonIgnore]
    public virtual Station? RefSapLeoniNavigation { get; set; } = null!;
}
