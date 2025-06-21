
$(document).ready(function () {
	// Cập nhật danh sách sản phẩm
	function updateProductList(formData) {
		$.ajax({
			url: "@Url.Action("Index", "Home")",
			type: "GET",
			data: formData,
			success: function (data) {
				$("#productContainer").html($(data).find("#productContainer").html());
				$(".pagination").html($(data).find(".pagination").html());
			},
			error: function () {
				alert("Lỗi khi tải dữ liệu sản phẩm.");
			}
		});
	}

	// Lấy dữ liệu lọc hiện tại từ form
	function getFilterData(additionalData = {}) {
		var formData = $("#filterForm").serializeArray();
		$.each(additionalData, function (key, value) {
			formData.push({ name: key, value: value });
		});
		return formData;
	}

	// Xử lý chọn loại sản phẩm
	$(document).on("click", ".filter-category", function (e) {
		e.preventDefault();
		var formData = getFilterData({
			productTypeId: $(this).data("product-type"),
			page: 1
		});
		updateProductList(formData);
	});

	// Xử lý khi chọn thuộc tính sắp xếp
	$("#sortBy").on("change", function () {
		var formData = getFilterData({ page: 1 });
		updateProductList(formData);
	});

	// Xử lý đổi chiều sắp xếp khi nhấn vào icon
	$("#toggleSortType").on("click", function () {
		let currentType = $(this).attr("data-sort-type");

		if (currentType === "desc") {
			$(this).attr("data-sort-type", "asc");
			$("#sortType").val("asc");
			$(this).find('i').removeClass("fa-sort-amount-desc").addClass("fa-sort-amount-asc");
		} else {
			$(this).attr("data-sort-type", "desc");
			$("#sortType").val("desc");
			$(this).find('i').removeClass("fa-sort-amount-asc").addClass("fa-sort-amount-desc");
		}

		var formData = getFilterData({ page: 1 });
		updateProductList(formData);
	});

	// Xử lý phân trang
	$(document).on("click", ".pagination-link", function (e) {
		e.preventDefault();
		var formData = getFilterData({
			page: $(this).data("page")
		});
		updateProductList(formData);
	});

	// Xử lý tìm kiếm
	$('#searchForm').on('submit', function (e) {
		e.preventDefault();
		var formData = getFilterData({ page: 1 });
		updateProductList(formData);
	});
});