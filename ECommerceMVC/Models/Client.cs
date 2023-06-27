using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommerceMVC.Models;

public partial class Client
{
    [Key]
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    [InverseProperty("Client")]
    public virtual List<Order> Orders { get; set; } = new List<Order>();
}
