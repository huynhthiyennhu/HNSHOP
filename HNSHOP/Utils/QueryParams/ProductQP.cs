using Swashbuckle.AspNetCore.Annotations;

namespace HNSHOP.Utils.QueryParams
{
    public class ProductQP
    {
        private int _page = 1;
        private string _sortBy = "id";
        private string _sortType = "asc";

        [SwaggerParameter(Description = "minPrice_maxPrice (vd: 100_600) (đơn vị nghìn đồng)")]
        public string? FByPrice { get; set; }
        [SwaggerParameter(Description = "FByType: typeId1,typeId2,... (vd: 1,3,8) ")]
        public string? FByType { get; set; }
        [SwaggerParameter(Description = "Pagination: true/false")]
        public string? Search { get; set; }
        public bool Pagination { get; set; } = true;

        public int Page
        {
            get => _page;
            set => _page = value >= 1 ? value : _page;
        }

        [SwaggerParameter(Description = "SortBy: attributeName (vd: rating) " +
            "attributeName{id, rating, like, sold, price} " +
            "rating: tỷ lệ đánh giá " +
            "like: mức độ yêu thích " +
            "sold: doanh số đã bán " +
            "price: mức giá")]
        public string SortBy
        {
            get => _sortBy;
            set => _sortBy = string.IsNullOrEmpty(value) ? _sortBy : value.ToLower();
        }

        [SwaggerParameter(Description = "SortType: type (vd: asc) " +
            "type{asc, desc} " +
            "asc: sắp xếp tăng dần " +
            "desc: sắp xếp giảm dần")]
        public string SortType
        {
            get => _sortType;
            set => _sortType = string.IsNullOrEmpty(value) ? _sortType : value.ToLower();
        }
    }
}
