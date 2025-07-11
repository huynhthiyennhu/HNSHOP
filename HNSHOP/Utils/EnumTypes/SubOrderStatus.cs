namespace HNSHOP.Utils.EnumTypes
{
    public enum SubOrderStatus
    {
        Pending = 0,      // Chưa được duyệt (chờ admin duyệt đơn tổng)
        
        Shipping = 1,     // Shop đang giao hàng
        Delivered = 2,    // Shop giao hàng xong
        Completed = 3,    // Khách xác nhận đã nhận hàng
        Cancelled = 4     // Shop từ chối giao / đơn bị huỷ riêng lẻ
    }
}
