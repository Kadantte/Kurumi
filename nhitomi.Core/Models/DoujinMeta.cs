using System.ComponentModel.DataAnnotations;

namespace nhitomi.Core.Models
{
    public enum MetaType
    {
        Artist,
        Group,
        Scanlator,
        Language,
        Parody,
        Character,
        Category,
        Tag
    }

    public class DoujinMeta : ModelBase
    {
        public MetaType Type { get; set; }

        [Required, MaxLength(128)]
        public string Value { get; set; }

        /// <summary>
        /// Number of doujinshi that are associated with this metadata.
        /// </summary>
        public virtual int DoujinCount { get; set; }

        public override string ToString() => $"{Id} {Type} '{Value ?? "<null>"}'";
    }
}