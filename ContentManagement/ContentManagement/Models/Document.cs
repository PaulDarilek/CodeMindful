using ContentManagement.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace ContentManagement.Models
{

    /// <summary>Connector for Storing Content on a Database <see cref="Data"/> or in one or more files <see cref="StoragePaths"/></summary>
    public class Document : IDocument
    {
        [Required]
        public Guid? DocumentGuid { get; set; }

        public DateTime? DateCreatedUtc { get; set; }

        public DateTime? DateUpdatedUtc { get; set; }

        public DateTime? DateRetention { get; set; }


        [DataType(DataType.Text)]
        public string? Notes { get; set; }

        [DataType(DataType.Text)]
        public HashSet<string> Tags { get; set; }

        [DataType(DataType.MultilineText)]
        public Dictionary<string, string> Properties { get; set; }

        public virtual List<DocumentBlob> Blobs { get; set; }


        public Document() 
        {
            EnsureNotNull();
            Blobs ??= [];
            Tags ??= [];
            Properties ??= [];
        }

        /// <summary>Ensure Collections are not null</summary>
        public virtual void EnsureNotNull()
        {
            DocumentGuid ??= Guid.NewGuid();
            DateCreatedUtc ??= DateTime.UtcNow;
            DateUpdatedUtc ??= DateCreatedUtc;
            Notes ??= string.Empty;
            Tags ??= new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            Properties ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Blobs ??= [];
        }

        /// <summary>Combine Tags and Properties and Update Notes if Null or Empty</summary>
        /// <param name="other"></param>
        public void UnionWith(Document other)
        {
            EnsureNotNull();
            if (other != null)
            {
                other.EnsureNotNull();
                Notes = Notes.FirstNotNullOrWhiteSpace(other.Notes);
                Tags.UnionWith(other.Tags);
                Properties.UnionWith(other.Properties, replace: false);
            }
        }


        public override string ToString() => JsonSerializer.Serialize(this);
    }
}
