$(document).ready(function () {
    $(".add-to-cart-btn").click(function (e) {
        e.preventDefault();
        var productId = $(this).data("id");

        $.ajax({
            url: "/Cart/AddToCart",
            type: "POST",
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    updateCartCount();
                }
            },
            error: function () {
                alert("Có lỗi xảy ra, vui lòng thử lại.");
            }
        });
    });

    function updateCartCount() {
        $.get("/Cart/GetCartCount", function (count) {
            $("#cart-count").text(count);
        });
    }
});
