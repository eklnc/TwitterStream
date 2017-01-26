using System.ComponentModel.DataAnnotations;

namespace TwitterStream.Tables
{
    public partial class T_TS_TWEETS
    {
        public int Id { get; set; }

        [Required]
        public string Tweet { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string CreateTime { get; set; }

        [StringLength(50)]
        public string Coordinates { get; set; }

        [StringLength(100)]
        public string Place { get; set; }

        public int CategoryId { get; set; }

        public bool IsActive { get; set; }
    }
}
