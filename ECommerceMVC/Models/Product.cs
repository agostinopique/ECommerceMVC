using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Models;

public partial class Product
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public double Price { get; set; }

    [ForeignKey("ProductsId")]
    [InverseProperty("Products")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
