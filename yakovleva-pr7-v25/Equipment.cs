using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yakovleva_pr7_v25
{
    public class Equipment
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string InventoryNumber { get; set; } = null!;

        public decimal Price { get; set; }

        public DateOnly PurchaseDate { get; set; }

        public bool IsOperational { get; set; }
    }
}
