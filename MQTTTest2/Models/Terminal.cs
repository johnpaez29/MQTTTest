using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MQTTTest2.Models
{
    public class Terminal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Serie { get; set; }

        [Required]
        public string Nombre { get; set; }

    }
}
