<script>
    // Tạo namespace để tránh xung đột
    var CartManager = (function() {
        // Private variables/functions
        var cartCountElement = $('#cartCount');
        
        // Hàm cập nhật số lượng giỏ hàng
        function updateCartCount() {
            return $.get('@Url.Action("ShowCount", "ShoppingCart")')
                .done(function(data) {
                    cartCountElement.text(data.Count);
                })
                .fail(function() {
                    console.error('Không thể cập nhật số lượng giỏ hàng');
                });
        }
        
        // Hàm thêm sản phẩm vào giỏ hàng
        function addToCart(productId, quantity) {
            return $.ajax({
                url: '@Url.Action("AddToCart", "Products")',
                type: 'POST',
                data: { 
                    id: productId, 
                    quantity: quantity || 1 
                }
            }).done(function(response) {
                if (response.success) {
                    updateCartCount();
                    showSuccessMessage('Đã thêm sản phẩm vào giỏ hàng');
                } else {
                    showErrorMessage(response.message || 'Không thể thêm sản phẩm');
                }
            }).fail(function() {
                showErrorMessage('Lỗi kết nối, vui lòng thử lại');
            });
        }
        
        // Hiển thị thông báo thành công
        function showSuccessMessage(message) {
            Swal.fire({
                icon: 'success',
                title: 'Thành công!',
                text: message,
                timer: 1500,
                showConfirmButton: false
            });
        }
        
        // Hiển thị thông báo lỗi
        function showErrorMessage(message) {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: message
            });
        }
        
        // Public API
        return {
            init: function() {
                updateCartCount();
                $(document).on('click', '.btn-add-to-cart', function() {
                    var productId = $(this).data('id');
                    addToCart(productId);
                });
            },
            updateCount: updateCartCount,
            addItem: addToCart
        };
    })();

    // Khởi tạo khi trang tải xong
    $(document).ready(function() {
        CartManager.init();
    });
</script>