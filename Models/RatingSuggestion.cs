using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Guide_Me.Models
{
    public class RatingSuggestion
    {
        [Key]
        public int RateSuggID { get; set; }

        public string  Discription { get; set; }

        [ForeignKey("Rating")]
        public int rateID { get; set; }
        public Rating Rating { get; set; }
    }
}
