using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sample.Models
{
    public class Message
    {

        [Key]
        public int Id { get; set; }

        public string Text { get; set; }

        public string SenderId { get; set; }

        public string ReceiverId { get; set; }

        [Column(TypeName = "datetime2(7)")]
        public DateTime Date { get; set; }

    }
}
