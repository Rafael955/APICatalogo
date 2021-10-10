using System;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.Models
{
    public abstract class Entity<T> where T : struct
    {
        [Key]
        public T Id { get; set; }
    }
}
