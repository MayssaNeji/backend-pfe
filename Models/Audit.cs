using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace LTMS.Models;

public partial class Audit
{
    public int Id { get; set; }

    public string? Bus { get; set; }

    public DateTime? DateAudit { get; set; }

    public string? NomAuditeur { get; set; }

    public string? PersonneAuditee { get; set; }

    public bool Feux { get; set; }

    public bool Maintenance { get; set; }

    public bool Chaises { get; set; }

    public bool Pneux { get; set; }

    public bool Vitres { get; set; }

    public bool Assurance { get; set; }

    public bool CarteProfessionelle { get; set; }

    public bool ContratLeoni { get; set; }

    public bool Horraires { get; set; }

    public bool Comportements { get; set; }

    public string? Commentaires { get; set; }

    public string? Resultat { get; set; }
    [JsonIgnore]

    public virtual Chauffeur? PersonneAuditeeNavigation { get; set; }
}
