using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace nhitomi.Core.Models
{
    public class Doujin : ModelBase
    {
        /// <summary>
        /// Prettified name of the doujinshi.
        /// This is usually English.
        /// </summary>
        [Required, MaxLength(256)]
        public string PrettyName { get; set; }

        /// <summary>
        /// Original name of the doujinshi.
        /// This is usually the original language of the doujinshi (i.e. Japanese).
        /// </summary>
        [Required, MaxLength(256)]
        public string OriginalName { get; set; }

        /// <summary>
        /// Time at which this doujinshi was created or last updated.
        /// </summary>
        public DateTime ProcessTime { get; set; }

        /// <summary>
        /// Sources from which this doujinshi was available.
        /// </summary>
        public ICollection<DoujinSource> Sources { get; set; }

        /// <summary>
        /// Metadata objects that further describes this doujinshi.
        /// </summary>
        public virtual IEnumerable<DoujinMeta> Metas { get; set; }
    }
}