using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;

namespace ApiProject.Models
{
    public class GiftModel
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        public int TicketPrice { get; set; }
        public string Image { get; set; }

        public int DonorModelId { get; set; }
        public DonorModel DonorModel { get; set; }//אם יש בעיה...

        public int CategoryModelId { get; set; }
        public CategoryModel CategoryModel { get; set; }
        public bool? isRaffleDone { get; set; }

        public int? UserModelId { get; set; }
        public UserModel UserModel { get; set; }
    }
}












