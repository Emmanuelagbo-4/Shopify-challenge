using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeChallenge.Entities
{
    public class UsersFiles
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int Id {get; set;}
        public string Name {get; set;}
        public string File {get; set;}
        public string FileType {get; set;}
        public string ContentType {get; set;}
        public string ApplicationUserId {get; set;}
        [ForeignKey ("ApplicationUserId")]
        public ApplicationUser ApplicationUser {get; set;}
    }
}