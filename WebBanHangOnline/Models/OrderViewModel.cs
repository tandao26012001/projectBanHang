using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebBanHangOnline.Models
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "Tên khách hàng không để trống")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Số điện thoại không để trống")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Địa chỉ không để trống")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        public string CustomerId { get; set; }

        [Range(1, 3, ErrorMessage = "Phương thức thanh toán không hợp lệ")]
        public int TypePayment { get; set; }

        public int TypePaymentVN { get; set; }

        public string Note { get; set; } // Ghi chú đơn hàng
    }
}
