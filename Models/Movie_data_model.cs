using System.ComponentModel.DataAnnotations;

namespace All_type_input_database.Models
{
    public class Movie_data_model
    {
        [Key]
        public int Id { get; set; }

        [Display(Name ="Movie Name")]
        [Required(ErrorMessage = "This field is require")]
        public string MovieName { get; set; }

        [Display(Name = "Movie Date and time")]
        [Required(ErrorMessage = "This field is require")]
        public DateTime MovieDateTime { get; set; }

        [Display(Name = "Movie Image")]
        [Required(ErrorMessage = "This field is require")]
        public byte[] Image { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "This field is require")]
        public string email { get; set; }

    }
}
