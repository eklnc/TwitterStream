using System.ComponentModel.DataAnnotations;

namespace TwitterStream.Tables
{
    public partial class T_TS_PARAMETERS
    {
        public int Id { get; set; }

        [Required]
        public string ParamaterName { get; set; }

        [Required]
        public string ParameterValue { get; set; }
    }
}
