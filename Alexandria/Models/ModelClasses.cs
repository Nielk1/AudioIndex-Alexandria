using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace Alexandria.Models
{
    // after changing models, nav dev prompt to project folder, run dnx ef migrations add NAME, then run dnx ef database update.

    public class Tag
    {
        [Key]
        public int ID { get; set; }
        [Required]
        //[Index("IX_NameCategory", 1, IsUnique = true)]
        public string Name { get; set; }
        [Required]
        //[Index("IX_NameCategory", 2, IsUnique = true)]
        public TagCategory Category { get; set; }

        //http://stackoverflow.com/questions/10572442/how-to-ignore-a-property-when-using-entity-framework-code-first
        //[NotMapped]
        //public ICollection<Tag> AutoSubTags { get; set; }
    }

    public class File : ITaggable
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public Library Library { get; set; }
        [Required]
        public bool Archival { get; set; }
        [Required]
        public DateTime FileModified { get; set; }
        [Required]
        public DateTime RecordAdded { get; set; }
        public virtual ICollection<TagAssociation> Tags { get; set; }
        [Display(Name = "Tracks")]
        public virtual ICollection<VirtualTrack> vTracks { get; set; }
        [Display(Name = "Albums")]
        public virtual ICollection<VirtualAlbum> vAlbums { get; set; }

        public File()
        {
            Tags = new List<TagAssociation>();
            vTracks = new List<VirtualTrack>();
            vAlbums = new List<VirtualAlbum>();
        }

        public void AddTag(Tag tag, UserAccount user)
        {
            var tagAlreadyThere = Tags.Where(dr => dr.Tag.Equals(tag));
            if(tagAlreadyThere.Any())
            {
                if(!tagAlreadyThere.Where(dr => dr.Added.Where(dx => dx.User.Equals(user)).Any()).Any())
                {
                    TagAdder tagAdderRecord = new TagAdder() { Timestamp = DateTime.UtcNow, User = user };

                    tagAlreadyThere.First().Added.Add(tagAdderRecord);
                }
            }
            else
            {
                TagAdder tagAdderRecord = new TagAdder() { Timestamp = DateTime.UtcNow, User = user };
                TagAssociation newTagAssoc = new TagAssociation() { Tag = tag, Added = new List<TagAdder>() };
                newTagAssoc.Added.Add(tagAdderRecord);
                Tags.Add(newTagAssoc);
            }
        }

        public List<Tag> GetTags()
        {
            return Tags.Select(dr => dr.Tag).ToList();
        }
    }

    public class Library
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Path { get; set; }
        [Required]
        public bool Enabled { get; set; }
        public DateTime? Expire { get; set; }
    }

    public class TagAdder
    {
        [Key]
        public int ID { get; set; }
        public UserAccount User { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class VirtualTrack : ITaggable
    {
        [Key]
        public int ID { get; set; }
        public virtual ICollection<TagAssociation> Tags { get; set; }
        public virtual VirtualAlbum vAlbum { get; set; }
    }

    public class VirtualAlbum : ITaggable
    {
        [Key]
        public int ID { get; set; }
        public virtual ICollection<TagAssociation> Tags { get; set; }
    }

    public class TagCategory
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
    }

    public class TagAssociation
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public Tag Tag { get; set; }
        public virtual ICollection<TagAdder> Added { get; set; }
    }

    public interface ITaggable
    {
        ICollection<TagAssociation> Tags { get; set; }
    }

    public class UserAccount
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public bool System { get; set; }
        [Required]
        public bool Admin { get; set; }
        [Required]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}