using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace test.Models;

[Table("medical_record")]
public partial class MedicalRecord
{
    [Key]
    
    public int Recordid { get; set; }

   
    public int Animalid { get; set; }
    [ForeignKey("Animalid")]
    public virtual Animal? Animal { get; set; }

   
   
    [MaxLength(500)]
    public string? injurys { get; set; }

    [Column("status")]
    [StringLength(20)]
    [Unicode(false)]
    public string? Status { get; set; }
    public virtual ICollection<VaccinationNeeded> VaccinationNeededs { get; set; } = new List<VaccinationNeeded>();
}