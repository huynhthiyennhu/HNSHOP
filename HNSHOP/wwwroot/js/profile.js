$(document).ready(function () {
    // 🟢 Thêm sản phẩm bằng AJAX
    $("#addProductForm").submit(function (e) {
        e.preventDefault();
        var formData = new FormData(this);
        $.ajax({
            url: "/Product/Create",
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (res) {
                alert(res.message);
                location.reload();
            }
        });
    });

    // 🟠 Chỉnh sửa sản phẩm (Tải dữ liệu vào modal)
    $("#editProductModal").on("show.bs.modal", function (event) {
        var button = $(event.relatedTarget);
        var productId = button.data("id");
        var productName = button.data("name");
        var productPrice = button.data("price");
        var productDescription = button.data("description");

        var modal = $(this);
        modal.find("#editProductId").val(productId);
        modal.find("#editProductName").val(productName);
        modal.find("#editProductPrice").val(productPrice);
        modal.find("#editProductDescription").val(productDescription);
    });

    // 🟠 Cập nhật sản phẩm bằng AJAX
    $("#editProductForm").submit(function (e) {
        e.preventDefault();
        var formData = new FormData(this);
        var productId = $("#editProductId").val();

        $.ajax({
            url: "/Product/Edit/" + productId,
            type: "POST",
            data: formData,
            contentType: false,
            processData: false,
            success: function (res) {
                alert(res.message);
                location.reload();
            }
        });
    });

    // 🔴 Xóa sản phẩm bằng AJAX
    $(".deleteProductBtn").click(function () {
        var productId = $(this).data("id");

        if (confirm("Bạn có chắc chắn muốn xóa sản phẩm này?")) {
            $.ajax({
                url: "/Product/Delete/" + productId,
                type: "POST",
                success: function (res) {
                    alert(res.message);
                    location.reload();
                }
            });
        }
    });
});
