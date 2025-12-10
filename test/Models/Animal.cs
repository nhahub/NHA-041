using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test.Models;

public partial class Animal
{
    public int AnimalId { get; set; }
    [MaxLength(100)]
    public string? Name { get; set; }

    public byte? Age { get; set; }
    [MaxLength(50)]
    public string? Type { get; set; }
    [MaxLength(50)]
    public string? Breed { get; set; }

    [MaxLength(10)]
    public string? Gender { get; set; }

    public string? Photo { get; set; }
    public bool IsAdopted { get; set; } = false;
    [MaxLength(600)]
    public string? About { get; set; }

    public string? Userid { get; set; }
    [ForeignKey("Userid")]
    public virtual ApplicationUser? User { get; set; }

    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();


}
