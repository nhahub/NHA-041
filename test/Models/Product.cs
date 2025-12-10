using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace test.Models;

public partial class Product
{
    
    [Key]
    [Column("productid")]
    public int Productid { get; set; }


    [Required(ErrorMessage = "Product Name is required.")]
    [StringLength(100, ErrorMessage = "Product Name cannot exceed 100 characters.")]
    public string Name { get; set; }

    public string? Userid { get; set; }
    [ForeignKey("Userid")]
    public ApplicationUser? User { get; set; }

    [Column("type")]
    [StringLength(50, ErrorMessage = "Type cannot exceed 50 characters.")]
    [Unicode(false)]
    public string? Type { get; set; }

    [Required(ErrorMessage = "Quantity is required.")]
    [Range(0, int.MaxValue)]
    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("price")]

    public decimal Price { get; set; }

    [Column("disc")]
    [StringLength(255, ErrorMessage = "Description cannot exceed 500 characters.")]
    [Unicode(false)]
    public string? Disc { get; set; }

    [Column("photo")]
    [StringLength(500)]
    [Unicode(false)]
    public string? Photo { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; }


}
