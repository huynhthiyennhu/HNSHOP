using Swashbuckle.AspNetCore.Annotations;

namespace HNSHOP.Utils.QueryParams
{
    public class ShopReqQuery
    {
        private int _page = 1;
        private string _sortBy = "id";
        private string _sortType = "asc";

        public string? Search { get; set; }

        public int Page
        {
            get => _page;
            set => _page = value >= 1 ? value : _page;
        }

        [SwaggerParameter(Description = "SortBy: attributeName (vd: id) " +
            "attributeName{id, name} " +
            "id: mã cửa hàng" +
            "name: tên cửa hàng")]
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
