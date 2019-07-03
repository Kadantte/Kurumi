using System;
using System.ComponentModel.DataAnnotations;

namespace nhitomi.Core.Models
{
    public class DoujinSource : ModelBase
    {
        public Guid DoujinId { get; set; }

        /// <summary>
        /// Identifier of this source (e.g. nhentai = 1, Hitomi = 2, etc.).
        /// </summary>
        [Required, MaxLength(16)]
        public int Source { get; set; }

        /// <summary>
        /// Identifier used by this source (e.g. gallery number for nhentai).
        /// </summary>
        [Required, MaxLength(16)]
        public string SourceId { get; set; }

        /// <summary>
        /// Time at which the doujinshi from this source was uploaded.
        /// </summary>
        public DateTime UploadTime { get; set; }

        /// <summary>
        /// Time at which this source was created or last updated.
        /// </summary>
        public DateTime ProcessTime { get; set; }

        /// <summary>
        /// Number of pages in the doujinshi from this source.
        /// </summary>
        public int PageCount { get; set; }
    }
}