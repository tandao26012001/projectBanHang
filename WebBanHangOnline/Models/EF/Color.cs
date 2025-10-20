using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHangOnline.Models.EF
{
    [Table("tb_Color")]
    public class Color : CommonAbstract
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }  // Đỏ, Đen, Trắng, Xám...

        [StringLength(10)]
        public string HexColor { get; set; } // #FF0000

        public virtual ICollection<ProductColor> ProductColors { get; set; }
    }
}
