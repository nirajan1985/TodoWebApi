using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models.Dto
{
    public class TodoUpdateDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }
        public bool Reminder { get; set; }
    }
}
