using System;
using System.ComponentModel.DataAnnotations;

namespace nhitomi.Core.Models
{
    public class DoujinCollection : ModelBase
    {
        [Required, MinLength(1), MaxLength(64)]
        public string Name { get; set; }

        public virtual Guid OwnerId { get; set; }

        public virtual int ItemCount { get; set; }
    }
}