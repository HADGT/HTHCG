using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HTHCG.Models;

public partial class HthcgContext : DbContext
{
    public HthcgContext()
    {
    }

    public HthcgContext(DbContextOptions<HthcgContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Disease> Diseases { get; set; }

    public virtual DbSet<DiseasesSeq> DiseasesSeqs { get; set; }

    public virtual DbSet<Symptom> Symptoms { get; set; }

    public virtual DbSet<SymptomsDisease> SymptomsDiseases { get; set; }

    public virtual DbSet<SymptomsSeq> SymptomsSeqs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-K1FUUMR\\SQLEXPRESS;Database=HTHCG;uid=sa;pwd=156314@Dh;MultipleActiveResultSets=True; TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Disease>(entity =>
        {
            entity.HasKey(e => e.IdDis).HasName("PK__diseases__3214EC07117CFA80");

            entity.ToTable("diseases", tb => tb.HasTrigger("tg_diseases_insert"));

            entity.Property(e => e.IdDis)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<DiseasesSeq>(entity =>
        {
            entity.HasKey(e => e.Seq).HasName("PK__diseases__DDDFBCBE2D933B21");

            entity.ToTable("diseases_seq");

            entity.Property(e => e.Seq).HasColumnName("seq");
        });

        modelBuilder.Entity<Symptom>(entity =>
        {
            entity.HasKey(e => e.IdSym).HasName("PK__symptoms__3214EC077A6E6A0B");

            entity.ToTable("symptoms", tb => tb.HasTrigger("tg_symptoms_insert"));

            entity.Property(e => e.IdSym)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<SymptomsDisease>(entity =>
        {
            entity.ToTable("symptoms_diseases");

            entity.Property(e => e.IdDis)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.IdSym)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.IdDisNavigation).WithMany(p => p.SymptomsDiseases)
                .HasForeignKey(d => d.IdDis)
                .HasConstraintName("FK__symptoms___IdDis__6754599E");

            entity.HasOne(d => d.IdSymNavigation).WithMany(p => p.SymptomsDiseases)
                .HasForeignKey(d => d.IdSym)
                .HasConstraintName("FK__symptoms___IdSym__68487DD7");
        });

        modelBuilder.Entity<SymptomsSeq>(entity =>
        {
            entity.HasKey(e => e.Seq).HasName("PK__symptoms__DDDFBCBEEE52A226");

            entity.ToTable("symptoms_seq");

            entity.Property(e => e.Seq).HasColumnName("seq");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
