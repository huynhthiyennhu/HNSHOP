$(document).ready(function () {
    // Cập nhật số lượng giỏ hàng khi tải trang
    updateCartCount();

    // Bắt sự kiện click trên tất cả các nút "Thêm vào giỏ hàng"
    $(document).on("click", ".add-to-cart", function (e) {
        e.preventDefault();

        var productId = $(this).data("product-id");
        var quantity = $(this).data("quantity") || 1;

        if (!productId) {
            Swal.fire({
                icon: "error",
                title: "Lỗi!",
                text: "Không tìm thấy ID sản phẩm.",
            });
            return;
        }

        $.ajax({
            url: "/Cart/AddToCart",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ ProductId: productId, Quantity: quantity }),
            success: function (response) {
                if (response.success) {
                    updateCartCount(response.cartCount);

                    Swal.fire({
                        icon: "success",
                        title: "Thêm vào giỏ hàng thành công!",
                        text: "Bạn có muốn xem giỏ hàng không?",
                        showCancelButton: true,
                        confirmButtonText: "Xem giỏ hàng",
                        cancelButtonText: "Tiếp tục mua sắm"
                    }).then((result) => {
                        if (result.isConfirmed) {
                            window.location.href = "/Cart";
                        }
                    });
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Lỗi!",
                        text: response.message,
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: "error",
                    title: "Có lỗi xảy ra!",
                    text: "Vui lòng thử lại sau.",
                });
            }
        });
    });

    // Bắt sự kiện cập nhật số lượng sản phẩm trong giỏ hàng
    $(document).on("click", ".cart_quantity_up, .cart_quantity_down", function (e) {
        e.preventDefault();

        var productId = $(this).data("product-id");
        var action = $(this).data("action");
        var quantityInput = $(this).siblings(".cart_quantity_input");

        var newQuantity = parseInt(quantityInput.val());
        if (action === "increase") {
            newQuantity += 1;
        } else if (action === "decrease" && newQuantity > 1) {
            newQuantity -= 1;
        }

        $.ajax({
            url: "/Cart/UpdateCart",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                ProductId: productId,
                Quantity: newQuantity
            }),
            success: function (response) {
                if (response.success) {
                    quantityInput.val(newQuantity);
                    updateTotalPrice(productId, newQuantity);
                    updateCartCount(response.cartCount);
                } else {
                    Swal.fire({
                        icon: "error",
                        title: "Lỗi!",
                        text: "Không thể cập nhật số lượng.",
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: "error",
                    title: "Có lỗi xảy ra!",
                    text: "Vui lòng thử lại sau.",
                });
            }
        });
    });

    // Bắt sự kiện xóa sản phẩm khỏi giỏ hàng
    $(document).on("click", ".cart-remove-item", function (e) {
        e.preventDefault();

        var productId = $(this).data("product-id");

        Swal.fire({
            title: "Bạn có chắc chắn muốn xóa sản phẩm này?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Xóa",
            cancelButtonText: "Hủy",
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "/Cart/RemoveFromCart",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({ ProductId: productId }),
                    success: function (response) {
                        if (response.success) {
                            $(`#cart-item-${productId}`).fadeOut(500, function () {
                                $(this).remove();
                            });

                            updateCartCount(response.cartCount);
                        } else {
                            Swal.fire({
                                icon: "error",
                                title: "Lỗi!",
                                text: "Không thể xóa sản phẩm.",
                            });
                        }
                    },
                    error: function () {
                        Swal.fire({
                            icon: "error",
                            title: "Có lỗi xảy ra!",
                            text: "Vui lòng thử lại sau.",
                        });
                    }
                });
            }
        });
    });

    // Xóa toàn bộ giỏ hàng
    $("#clear-cart-form").submit(function (e) {
        e.preventDefault();

        Swal.fire({
            title: "Bạn có chắc chắn?",
            text: "Tất cả sản phẩm trong giỏ hàng sẽ bị xóa!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Xóa ngay",
            cancelButtonText: "Hủy bỏ"
        }).then((result) => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "/Cart/ClearCart",
                    type: "POST",
                    success: function (response) {
                        if (response.success) {
                            Swal.fire("Đã xóa!", "Giỏ hàng của bạn đã được làm trống.", "success");
                            $("#cart-items tbody").empty(); // Xóa nội dung bảng
                            $("#cart-count").text(response.cartCount); // Cập nhật số lượng giỏ hàng

                            // Ẩn bảng giỏ hàng nếu rỗng
                            if (response.cartCount === 0) {
                                $(".cart_info").html('<p class="text-center">Giỏ hàng của bạn đang trống. <a href="/">Tiếp tục mua sắm</a></p>');
                            }
                        }
                    },
                    error: function () {
                        Swal.fire("Lỗi!", "Không thể xóa giỏ hàng, vui lòng thử lại.", "error");
                    }
                });
            }
        });
    });

    // Hàm cập nhật số lượng sản phẩm trên icon giỏ hàng
    function updateCartCount(count) {
        if (count > 0) {
            $("#cart-count").text(count).show();
        } else {
            $("#cart-count").text(0).hide();
        }
    }

    // Hàm cập nhật tổng giá trị sản phẩm
    function updateTotalPrice(productId, quantity) {
        var pricePerItem = parseFloat($(`#product-price-${productId}`).data("price"));
        var totalPrice = pricePerItem * quantity;
        $(`#product-total-${productId}`).text(totalPrice.toLocaleString() + " VNĐ");
    }

    // Gọi API để cập nhật số lượng giỏ hàng khi tải trang
    $.get("/Cart/GetCartCount", function (response) {
        updateCartCount(response.count);
    });
});
