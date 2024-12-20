using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models {
    [NotMapped]
    public class EmailMessage
    {
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public List<EmailUser> UsersTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        
        //only for EmailService in SendEmail
        public string ToEmail { get; set; }
        public string ToName { get; set; }
    }

    [NotMapped]
    public class EmailUser
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
