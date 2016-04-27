using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FlikrClone.Models
{
    [Table("Images")]
    public class Image
    {
        public Image()
        {
            this.Comments = new HashSet<Comment>();
        }
        [Key]
        public int ImageId { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
    }
}
