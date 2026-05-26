using System;
using System.Collections.Generic;

namespace PruebaTecnica.Models
{
    public class Venta
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public int ClienteId { get; set; }

        public Cliente Cliente { get; set; }

        public List<Producto> Producto { get; set; }

        public decimal Total { get; set; }

    }
}
