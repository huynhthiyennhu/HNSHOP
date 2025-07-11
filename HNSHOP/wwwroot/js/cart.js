$(document).ready(function () {
    updateCartCount()

    // Thêm vào giỏ hàng
    $(document).on("click", ".add-to-cart", function (e) {
        e.preventDefault()
        const productId = $(this).data("product-id")
        const quantity = $(this).data("quantity") || 1

        if (!productId) {
            Swal.fire({ icon: "error", title: "Lỗi!", text: "Không tìm thấy ID sản phẩm." })
            return
        }

        $.ajax({
            url: "/Cart/AddToCart",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ ProductId: productId, Quantity: quantity }),
            success: function (res) {
                if (res.success) {
                    updateCartCount(res.cartCount)
                    Swal.fire({
                        icon: "success",
                        title: "Thêm vào giỏ hàng thành công!",
                        text: "Bạn có muốn xem giỏ hàng không?",
                        showCancelButton: true,
                        confirmButtonText: "Xem giỏ hàng",
                        cancelButtonText: "Tiếp tục mua sắm"
                    }).then(result => {
                        if (result.isConfirmed) window.location.href = "/Cart"
                    })
                } else {
                    Swal.fire({ icon: "error", title: "Lỗi!", text: res.message })
                }
            },
            error: () => {
                Swal.fire({ icon: "error", title: "Có lỗi xảy ra!", text: "Vui lòng thử lại sau." })
            }
        })
    })

    // Cập nhật số lượng
    $(document).on("click", ".cart_quantity_up, .cart_quantity_down", function (e) {
        e.preventDefault()
        const productId = $(this).data("product-id")
        const action = $(this).data("action")
        const input = $(this).siblings(".cart_quantity_input")
        let quantity = parseInt(input.val())

        if (action === "increase") quantity++
        else if (action === "decrease" && quantity > 1) quantity--

        $.ajax({
            url: "/Cart/UpdateCart",
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({ ProductId: productId, Quantity: quantity }),
            success: function (res) {
                if (res.success) {
                    input.val(quantity)
                    updateTotalPrice(productId, quantity)
                    updateCartCount(res.cartCount)       
                    updateCartTotals()

                } else {
                    Swal.fire({ icon: "error", title: "Lỗi!", text: "Không thể cập nhật số lượng." })
                }
            },
            error: () => {
                Swal.fire({ icon: "error", title: "Có lỗi xảy ra!", text: "Vui lòng thử lại sau." })
            }
        })
    })

    // Xóa sản phẩm
    $(document).on("click", ".cart-remove-item", function (e) {
        e.preventDefault()
        const productId = $(this).data("product-id")
        const shopId = $(this).data("shop-id")

        Swal.fire({
            title: "Bạn có chắc chắn muốn xóa sản phẩm này?",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Xóa",
            cancelButtonText: "Hủy"
        }).then(result => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "/Cart/RemoveFromCart",
                    type: "POST",
                    contentType: "application/json",
                    data: JSON.stringify({ ProductId: productId }),
                    success: function (res) {
                        if (res.success) {
                            const $row = $(`#cart-item-${productId}`)
                            const $shop = $(`#shop-${shopId}`)

                            $row.fadeOut(200, function () {
                                $(this).remove()

                                const hasOtherItems = $shop.find("tbody tr").length > 0
                                if (!hasOtherItems) {
                                    $shop.fadeOut(200, function () {
                                        $(this).remove()

                                        if ($(".shop-group").length === 0) {
                                            $("#cart-container").html(`
                                                <h2 class="title text-center">
                                                    Giỏ hàng của bạn đang trống.
                                                    <a href="/">Tiếp tục mua sắm</a>
                                                </h2>
                                            `)
                                        }
                                    })
                                }
                            })

                            updateCartCount(res.cartCount)
                        } else {
                            Swal.fire({ icon: "error", title: "Lỗi!", text: "Không thể xóa sản phẩm." })
                        }
                    },
                    error: () => {
                        Swal.fire({ icon: "error", title: "Có lỗi xảy ra!", text: "Vui lòng thử lại." })
                    }
                })
            }
        })
    })

    // Xóa toàn bộ giỏ hàng
    $("#clear-cart-form").submit(function (e) {
        e.preventDefault()

        Swal.fire({
            title: "Bạn có chắc chắn?",
            text: "Tất cả sản phẩm trong giỏ hàng sẽ bị xóa!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Xóa ngay",
            cancelButtonText: "Hủy"
        }).then(result => {
            if (result.isConfirmed) {
                $.ajax({
                    url: "/Cart/ClearCart",
                    type: "POST",
                    success: function (res) {
                        if (res.success) {
                            Swal.fire("Đã xóa!", "Giỏ hàng đã được làm trống.", "success")
                            $(".shop-group").remove()
                            updateCartCount(0)
                            $("#cart-container").html(`
                                <h2 class="title text-center">
                                    Giỏ hàng của bạn đang trống.
                                    <a href="/">Tiếp tục mua sắm</a>
                                </h2>
                            `)
                        }
                    },
                    error: () => {
                        Swal.fire("Lỗi!", "Không thể xóa giỏ hàng, vui lòng thử lại.", "error")
                    }
                })
            }
        })
    })

    // Tổng tiền mỗi sản phẩm
    function updateTotalPrice(productId, quantity) {
        const price = parseFloat($(`#product-price-${productId}`).data("price"))
        const total = price * quantity
        $(`#product-total-${productId}`).text(total.toLocaleString() + " VNĐ")

    }

    // Cập nhật số sản phẩm trong biểu tượng
    function updateCartCount(count) {
        const $cart = $("#cart-count")
        if (count > 0) $cart.text(count).show()
        else $cart.text(0).hide()
    }
    function updateCartTotals() {
        let total = 0
        let totalDiscount = 0 // Nếu cần hỗ trợ giảm giá từng sản phẩm thì xử lý tại đây

        $(".cart_total_price").each(function () {
            const raw = $(this).text().replace(/[^\d]/g, "")
            const value = parseInt(raw)
            if (!isNaN(value)) total += value
        })

        const final = total - totalDiscount
        $("#cart-total").text(total.toLocaleString("vi-VN") + " VNĐ")
        $("#cart-discount").text("- " + totalDiscount.toLocaleString("vi-VN") + " VNĐ")
        $("#cart-total-final").text(final.toLocaleString("vi-VN") + " VNĐ")
        $("#finalTotal").val(final)
    }


    // Gọi khi load trang
    $.get("/Cart/GetCartCount", function (res) {
        updateCartCount(res.count)
    })
})
