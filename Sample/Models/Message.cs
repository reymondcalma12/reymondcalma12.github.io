using System.ComponentModel.DataAnnotations;

namespace Sample.Models
{
    public class Message
    {

        [Key]
        public int Id { get; set; }

        public string Text { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        public DateTime Date { get; set; }

    }
}
