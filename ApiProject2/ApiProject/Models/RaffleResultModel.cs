using System.ComponentModel.DataAnnotations.Schema;

namespace ApiProject.Models
{
    public class RaffleResultModel
    {
        public int Id { get; set; }
        public int GiftId { get; set; }
        public GiftModel Gift { get; set; }

        [ForeignKey("UserModel")]
        public int WinnerUserId { get; set; }
    }    
}