using System.ComponentModel.DataAnnotations;

namespace TwitterStream.Tables
{
    public partial class T_TS_CATEGORY
    {
        public int Id { get; set; }

        [Required]
        public string CategoryName { get; set; }

        [Required]
        public string CategoryTrackKeywords { get; set; }

        public bool IsActive { get; set; }
    }
}
