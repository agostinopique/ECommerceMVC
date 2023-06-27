using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Models;

[Microsoft.EntityFrameworkCore.Index("ClientId", Name = "IX_Orders_ClientId")]
public partial class Order
{
    [Key]
    public int Id { get; set; }

    public int IdentificationNumber { get; set; }

    public int ClientId { get; set; }

    public double? TotalPrice { get; set; }

    [ForeignKey("ClientId")]
    [InverseProperty("Orders")]
    public virtual Client Client { get; set; } = null!;

    [ForeignKey("OrdersId")]
    [InverseProperty("Orders")]
    public virtual List<Product> Products { get; set; } = new List<Product>();
    public Order()
    {
        Random rnd = new Random();
        IdentificationNumber = rnd.Next();
    }
}
