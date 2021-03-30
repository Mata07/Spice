using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Spice.Models
{
    // We want to store user shopping cart order in db

    public class ShoppingCart
    {
        public ShoppingCart()
        {
            //initialize Count to default value of 1
            Count = 1;
        }

        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }


        // Id of menu item in cart
        public int MenuItemId { get; set; }
        [NotMapped]
        [ForeignKey("ApplicationUserId")]
        public virtual MenuItem MenuItem { get; set; }


        // number of menuItems in cart
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a value greater than or equal to {1}")]
        public int Count { get; set; }
    }
}
