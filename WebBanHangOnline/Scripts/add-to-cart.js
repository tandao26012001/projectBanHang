$(document).ready(function () {
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();
        var id = $(this).data("id");

        $.ajax({
            url: '/ShoppingCart/AddToCart',
            type: 'POST',
            data: { id: id, quantity: 1 },
            success: function (res) {
                if (res.success) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Đã thêm vào giỏ hàng!',
                        showConfirmButton: false,
                        timer: 1500
                    });

                    // Cập nhật số lượng hiển thị trên icon giỏ hàng nếu có
                    if ($('.cart-count').length) {
                        $('.cart-count').text(res.count);
                    }
                } else {
                    Swal.fire('Lỗi!', res.msg || 'Không thể thêm vào giỏ hàng.', 'error');
                }
            },
            error: function () {
                Swal.fire('Lỗi!', 'Không thể kết nối đến máy chủ.', 'error');
            }
        });
    });
});
