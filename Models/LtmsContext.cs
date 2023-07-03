using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using LTMS.Models;

namespace LTMS.Models;

public partial class LtmsContext : DbContext
{
    public LtmsContext()
    {
    }

    public LtmsContext(DbContextOptions<LtmsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agence> Agences { get; set; }

    public virtual DbSet<Audit> Audits { get; set; }

    public virtual DbSet<Chauffeur> Chauffeurs { get; set; }

    public virtual DbSet<Circuit> Circuits { get; set; }

    public virtual DbSet<Compte> Comptes { get; set; }

    public virtual DbSet<Cotisation> Cotisations { get; set; }

    public virtual DbSet<Employe> Employes { get; set; }

    public virtual DbSet<Facture> Factures { get; set; }

    public virtual DbSet<HistoriqueImport> HistoriqueImports { get; set; }

    public virtual DbSet<PlanAgence> PlanAgences { get; set; }

    public virtual DbSet<PlanHebdo> PlanHebdos { get; set; }

    public virtual DbSet<PlanSegment> PlanSegments { get; set; }

    public virtual DbSet<Segment> Segments { get; set; }

    public virtual DbSet<Shift> Shifts { get; set; }

    public virtual DbSet<Station> Stations { get; set; }

    public virtual DbSet<Vehicule> Vehicules { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=MAYNEJI\\SQLEXPRESS;Database=LTMS;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Agence>(entity =>
        {
            entity.HasKey(e => e.Nom);

            entity.ToTable("Agence");

            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Adresse)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("email");
            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("ID");
            entity.Property(e => e.MatriculeFiscale).HasColumnName("matricule_fiscale");
            entity.Property(e => e.SiteInternet)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("site_internet");
        });

        modelBuilder.Entity<Audit>(entity =>
        {
            entity.ToTable("audit");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Assurance).HasColumnName("assurance");
            entity.Property(e => e.Bus)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("bus");
            entity.Property(e => e.CarteProfessionelle).HasColumnName("carteProfessionelle");
            entity.Property(e => e.Chaises).HasColumnName("chaises");
            entity.Property(e => e.Commentaires)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("commentaires");
            entity.Property(e => e.Comportements).HasColumnName("comportements");
            entity.Property(e => e.DateAudit)
                .HasColumnType("datetime")
                .HasColumnName("dateAudit");
            entity.Property(e => e.Feux).HasColumnName("feux");
            entity.Property(e => e.Maintenance).HasColumnName("maintenance");
            entity.Property(e => e.NomAuditeur)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("nomAuditeur");
            entity.Property(e => e.PersonneAuditee)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("personneAuditee");
            entity.Property(e => e.Pneux).HasColumnName("pneux");
            entity.Property(e => e.Resultat)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Vitres).HasColumnName("vitres");

            entity.HasOne(d => d.PersonneAuditeeNavigation).WithMany(p => p.Audits)
                .HasPrincipalKey(p => p.Nom)
                .HasForeignKey(d => d.PersonneAuditee)
                .HasConstraintName("FK_audit_Chauffeurs");
        });

        modelBuilder.Entity<Chauffeur>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Chauffeu__3214EC27A2F25F26");

            entity.HasIndex(e => e.Nom, "UQ_Chauffeurs_Nom").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Agence)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.DateDeNaissance)
                .HasColumnType("date")
                .HasColumnName("Date_de_naissance");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Prenom)
                .HasMaxLength(100)
                .IsFixedLength();

            entity.HasOne(d => d.AgenceNavigation).WithMany(p => p.Chauffeurs)
                .HasForeignKey(d => d.Agence)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chauffeurs_Agence");
        });

        modelBuilder.Entity<Circuit>(entity =>
        {
            entity.ToTable("circuit");

            entity.HasIndex(e => e.RefSapLeoni, "UQ_circuit_REF_SAP_LEONI").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Agence)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.ContributionEmploye).HasColumnName("contribution_employe");
            entity.Property(e => e.CoutKm).HasColumnName("Cout_Km");
            entity.Property(e => e.NbKm).HasColumnName("Nb_Km");
            entity.Property(e => e.PointArrivee)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("point_arrivee");
            entity.Property(e => e.RefChemin)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Ref_Chemin");
            entity.Property(e => e.RefSapLeoni)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("REF_SAP_LEONI");

            entity.HasOne(d => d.AgenceNavigation).WithMany(p => p.Circuits)
                .HasForeignKey(d => d.Agence)
                .HasConstraintName("FK_circuit_Agence");

            entity.HasOne(d => d.RefSapLeoniNavigation).WithOne(p => p.Circuit)
                .HasPrincipalKey<Station>(p => p.RefSapLeoni)
                .HasForeignKey<Circuit>(d => d.RefSapLeoni)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_circuit_station");
        });

        modelBuilder.Entity<Compte>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Compte__3214EC2745C4BCE7");

            entity.ToTable("Compte");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.DateDeNaissance)
                .HasColumnType("date")
                .HasColumnName("date_de_naissance");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Matricule).HasColumnName("matricule");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Role).HasMaxLength(255);
        });

        modelBuilder.Entity<Cotisation>(entity =>
        {
            entity.ToTable("cotisation");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Annee)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Circuit)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("circuit");
            entity.Property(e => e.Cotisation1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("cotisation");
            entity.Property(e => e.Employe)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("employe");
            entity.Property(e => e.Mois)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mois");
            entity.Property(e => e.Organization)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ps)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PS");
            entity.Property(e => e.Segment)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("segment");
        });

        modelBuilder.Entity<Employe>(entity =>
        {
            entity.HasKey(e => e.Matricule);

            entity.ToTable("employes");

            entity.HasIndex(e => e.Matricule, "UQ_employe_Matricule").IsUnique();

            entity.Property(e => e.Matricule).ValueGeneratedNever();
            entity.Property(e => e.CentreDeCout)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ContreMaitre)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("contreMaitre");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.NomDuGroupe)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ps)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PS");
            entity.Property(e => e.Segment)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("segment");
            entity.Property(e => e.Shift)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("shift");
            entity.Property(e => e.Station)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("station");

            entity.HasOne(d => d.SegmentNavigation).WithMany(p => p.Employes)
                .HasPrincipalKey(p => p.Nom)
                .HasForeignKey(d => d.Segment)
                .HasConstraintName("FK_employes_segment");

            entity.HasOne(d => d.ShiftNavigation).WithMany(p => p.Employes)
                .HasForeignKey(d => d.Shift)
                .HasConstraintName("FK_employes_shift");

            entity.HasOne(d => d.StationNavigation).WithMany(p => p.Employes)
                .HasPrincipalKey(p => p.RefSapLeoni)
                .HasForeignKey(d => d.Station)
                .HasConstraintName("FK_employes_station");
        });

        modelBuilder.Entity<Facture>(entity =>
        {
            entity.ToTable("Facture");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agence)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Annee)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("annee");
            entity.Property(e => e.Circuit)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("circuit");
            entity.Property(e => e.Mois)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mois");
        });

        modelBuilder.Entity<HistoriqueImport>(entity =>
        {
            entity.ToTable("HistoriqueImport");

            entity.Property(e => e.Creater)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.DateImport).HasColumnType("datetime");
            entity.Property(e => e.NomFichier)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PlanAgence>(entity =>
        {
            entity.ToTable("PlanAgence");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Agence)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("agence");
            entity.Property(e => e.Annee)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("annee");
            entity.Property(e => e.BesoinEnBus).HasColumnName("besoinEnBus");
            entity.Property(e => e.Circuit)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("circuit");
            entity.Property(e => e.Destination)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("destination");
            entity.Property(e => e.Effectif).HasColumnName("effectif");
            entity.Property(e => e.Mois)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mois");
            entity.Property(e => e.Navette).HasColumnName("navette");
            entity.Property(e => e.RefSemaine)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("refSemaine");
        });

        modelBuilder.Entity<PlanHebdo>(entity =>
        {
            entity.ToTable("PlanHebdo");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Annee)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Circuit)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dimanche)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Jeudi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Lundi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Mardi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Mercredi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Mois)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Organization)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ps)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("PS");
            entity.Property(e => e.RefSemaine)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Samedi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Segment)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Shift)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Station)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Vendredi)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PlanSegment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_PlanSeg");

            entity.ToTable("PlanSegment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Dimanche)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("dimanche");
            entity.Property(e => e.Jeudi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("jeudi");
            entity.Property(e => e.Lundi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lundi");
            entity.Property(e => e.Mardi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mardi");
            entity.Property(e => e.Matricule).HasColumnName("matricule");
            entity.Property(e => e.Mercredi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mercredi");
            entity.Property(e => e.Mois).HasColumnName("mois");
            entity.Property(e => e.Nom)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Prenom)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.RefSemaine)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("refSemaine");
            entity.Property(e => e.Samedi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Segment)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("segment");
            entity.Property(e => e.Shift)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("shift");
            entity.Property(e => e.Vendredi)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("vendredi");

            entity.HasOne(d => d.MatriculeNavigation).WithMany(p => p.PlanSegments)
                .HasForeignKey(d => d.Matricule)
                .HasConstraintName("FK_PlanSegment_employes");

            entity.HasOne(d => d.SegmentNavigation).WithMany(p => p.PlanSegments)
                .HasPrincipalKey(p => p.Nom)
                .HasForeignKey(d => d.Segment)
                .HasConstraintName("FK_PlanSegment_Segment");
        });

        modelBuilder.Entity<Segment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Segment__3214EC274B8BC47C");

            entity.ToTable("Segment");

            entity.HasIndex(e => e.Nom, "UQ_segment_Nom").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CentreDeCout)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("centre_de_cout");
            entity.Property(e => e.ChefDeSegment)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("chef_de_segment");
            entity.Property(e => e.Nom)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.NomSegSapRef)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("Nom_seg_SAP_Ref");
            entity.Property(e => e.RhSegment)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("RH_Segment");
        });

        modelBuilder.Entity<Shift>(entity =>
        {
            entity.HasKey(e => e.ReferenceShift);

            entity.ToTable("shift");

            entity.Property(e => e.ReferenceShift)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Dimanche)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Jeudi)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Lundi)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Mardi)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Mercredi)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Samedi)
                .HasMaxLength(11)
                .IsUnicode(false);
            entity.Property(e => e.Vendredi)
                .HasMaxLength(11)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Station>(entity =>
        {
            entity.ToTable("Station");

            entity.HasIndex(e => e.RefSapLeoni, "UQ_station[ref.SAP LEONI").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.RefSapLeoni)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("ref.SAP LEONI");
            entity.Property(e => e.ReferenceRegion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Reference Region");
        });

        modelBuilder.Entity<Vehicule>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Vehicule__3214EC27A34BDA8F");

            entity.ToTable("Vehicule");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Agence)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.DateDeMiseEnRoute)
                .HasColumnType("date")
                .HasColumnName("Date_de_mise_en_route");
            entity.Property(e => e.NomDeReference)
                .HasMaxLength(100)
                .IsFixedLength()
                .HasColumnName("Nom_de_reference");
            entity.Property(e => e.NumSerie)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("numSerie");
            entity.Property(e => e.Type)
                .HasMaxLength(100)
                .IsFixedLength();

            entity.HasOne(d => d.AgenceNavigation).WithMany(p => p.Vehicules)
                .HasForeignKey(d => d.Agence)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vehicule_Agence");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    public DbSet<LTMS.Models.Véhicule>? Véhicule { get; set; }
}
