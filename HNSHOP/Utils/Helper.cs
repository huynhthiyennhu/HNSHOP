using HNSHOP.Dtos.Response;

namespace HNSHOP.Utils
{
    public static class Helper
    {
        public static PaginationDto<T> GetPaginationResult<T>(List<T> dataList, int count)
        {

            if (dataList == null)
                throw new ArgumentException("Data is invalid.");

            int totalPage = (int)Math.Ceiling((double)count / ConstConfig.PageSize);

            var result = new PaginationDto<T>
            {
                Data = dataList,
                TotalPage = totalPage
            };

            return result;
        }

        public static PaginationDto<T> GetPaginationResult<T>(List<T> dataList, int count, int pageSize)
        {
            if (dataList == null || pageSize <= 0)
                throw new ArgumentException("Data or page size is invalid.");

            int totalPage = (int)Math.Ceiling((double)count / pageSize);

            var result = new PaginationDto<T>
            {
                Data = dataList,
                TotalPage = totalPage
            };

            return result;
        }

        public static ErrorDto ErrorResponse(string message)
        {
            return new ErrorDto
            {
                Message = message
            };
        }

        public static string GetEmailSaleEventContent(string customerName, string evenntName, DateOnly startDate, DateOnly endDate, float discountPercent)
        {


            return $"<!DOCTYPE html><html lang=\"vi\"><head><meta charset=\"UTF-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" /><title>Sự Kiện Khuyến Mãi</title><style>body {{font-family: Arial, sans-serif;margin: 0;padding: 0;background-color: #f4f4f4;}}.container {{width: 100%;max-width: 600px;margin: 0 auto;background-color: #ffffff;padding: 20px;}}h1, h2, h3 {{color: #333333;}}p {{font-size: 16px;line-height: 1.6;color: #555555;}}a {{color: #ffffff;text-decoration: none;}}.button {{display: inline-block;padding: 10px 20px;background-color: #ff6f61;color: #ffffff;border-radius: 5px;text-decoration: none;font-weight: bold;}}.social-media a {{color: #ff6f61;margin-right: 10px;text-decoration: none;}}.footer {{font-size: 12px;color: #999999;text-align: center;padding: 20px;}}</style></head><body><div class=\"container\"><h1>🎉 Sự Kiện Khuyến Mãi Lớn Dành Riêng Cho Bạn!</h1><p>Xin chào <strong>{customerName}</strong>,</p><p>Chúng tôi rất vui mừng thông báo về sự kiện <strong>{evenntName}</strong> đặc biệt, nơi bạn có thể nhận được những ưu đãi tuyệt vời cho các sản phẩm yêu thích của mình! 🎉</p><h2>Từ ngày <strong>{startDate}</strong> đến ngày <strong>{endDate}</strong>, chúng tôi đang cung cấp giảm giá lên đến <strong>{Convert.ToInt64(discountPercent * 100)}%</strong> cho nhiều mặt hàng khác nhau. Dù bạn đang tìm kiếm các thiết bị công nghệ mới nhất, phụ kiện nhà cửa thời trang, hay quần áo phong cách, chúng tôi đều có sản phẩm phù hợp cho bạn.</h2><h3>Những gì bạn có thể mong đợi:</h3><ul><li>🛍️ <strong>Giảm giá lên đến {Convert.ToInt64(discountPercent * 100)}%</strong> trên các sản phẩm được chọn</li><li>💥 <strong>Ưu đãi có thời gian giới hạn</strong> trên những sản phẩm bán chạy nhất</li><li>📦 <strong>Miễn phí vận chuyển</strong> cho đơn hàng trên 200.000 VNĐ</li><li>🎁 <strong>Quà tặng độc quyền</strong> cho 100 khách hàng đầu tiên</li></ul><h3>Mua sắm ngay hôm nay</h3><p>Đừng bỏ lỡ những ưu đãi tuyệt vời này! Hãy truy cập cửa hàng trực tuyến của chúng tôi ngay để chọn mua những sản phẩm yêu thích của bạn trước khi hết hàng. Số lượng có hạn, hãy nhanh tay!</p><a href=\"http://localhost:5500\" class=\"button\">Mua Ngay</a><h3>Cần hỗ trợ?</h3><p>Nếu bạn có bất kỳ câu hỏi nào hoặc cần trợ giúp, đội ngũ chăm sóc khách hàng của chúng tôi luôn sẵn sàng! Vui lòng liên hệ với chúng tôi qua email <a href=\"mailto:tatln@support.com\">tatln@support.com</a> hoặc gọi đến số 0932849673.</p><p>Chúc bạn mua sắm vui vẻ!</p><p>Trân trọng,<br />Đội ngũ <strong>TATLN</strong></p><div class=\"social-media\"><p>Theo dõi chúng tôi trên mạng xã hội:</p><a href=\"#\">Facebook</a> | <a href=\"#\">Instagram</a> | <a href=\"#\">Twitter</a></div><div class=\"footer\"><p><strong>Điều Khoản & Điều Kiện:</strong><br />Ưu đãi có hiệu lực từ ngày <strong>{startDate}</strong> đến ngày <strong>{endDate}</strong>. Giảm giá được áp dụng tự động khi thanh toán. Không áp dụng cùng các chương trình khuyến mãi khác.</p></div></div></body></html>\r\n";
        }
    }
}
