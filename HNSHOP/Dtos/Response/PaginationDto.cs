namespace HNSHOP.Dtos.Response
{
    public class PaginationDto<T>
    {
        public List<T> Data { get; set; } = [];
        public int TotalPage { get; set; }
    }
}
