using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BuchShop.Models
{
    public class CustomerOrder
    {
        [Key]
        public int OrderId { get; set; } // Primärschlüssel für die Bestellung

        [Required]
        public string UserEmail { get; set; } // Fremdschlüssel, um die Bestellung mit einem Benutzer zu verknüpfen

        [ForeignKey("UserEmail")]
        public User User { get; set; } // Navigationseigenschaft zum Benutzer

        [Required]
        public DateTime Date { get; set; } // Datum und Uhrzeit der Bestellung

        public ICollection<OrderItem> OrderItems { get; set; } // Liste der bestellten Artikel

        [Required]
        public decimal TotalAmount { get; set; } // Gesamtsumme der Bestellung
    }
}
