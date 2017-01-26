using System.ComponentModel.DataAnnotations;

namespace TwitterStream.Tables
{
    public partial class T_TS_EXCEPTION
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ExceptionGuid { get; set; }

        [Required]
        [StringLength(100)]
        public string ExceptionType { get; set; }

        [Required]
        public string ExceptionMessage { get; set; }

        [Required]
        public string StackTrace { get; set; }

        [Required]
        public string ExceptionData { get; set; }

        [Required]
        public string ExceptionSource { get; set; }

        public string ExceptionStatus { get; set; }
    }
}
