using HNSHOP.Models;
using HNSHOP.Utils;
using HNSHOP.Utils.EnumTypes;
using HNSHOP.Models;
using HNSHOP.Utils.EnumTypes;
using HNSHOP.Utils;
using Microsoft.EntityFrameworkCore;

namespace HNSHOP.Data
{
    public static class SeedData
    {
        public static ModelBuilder SeedAllData(this ModelBuilder builder)
        {
            builder.SeedProductType()
                .SeedRole()
                .SeedCustomerType()
                .SeedAccount();
                

            return builder;
        }

        public static ModelBuilder SeedProductType(this ModelBuilder builder)
        {
            builder.Entity<ProductType>().HasData(new()
            {
                Id = 1,
                Name = "Mỹ phẩm",
                Description = "Mỹ phẩm là các sản phẩm chăm sóc cá nhân được thiết kế để làm đẹp, bảo vệ hoặc cải thiện vẻ ngoài của da, tóc, móng và cơ thể. Chúng bao gồm nhiều loại sản phẩm như kem dưỡng da, son môi, phấn nền, mascara, dầu gội, và nước hoa. Mỹ phẩm có thể chứa các thành phần tự nhiên hoặc tổng hợp, giúp làm sạch, dưỡng ẩm, chống lão hóa, và bảo vệ da khỏi các tác động xấu từ môi trường."
            },
            new()
            {
                Id = 2,
                Name = "Gia dụng",
                Description = "Sản phẩm gia dụng là các vật dụng được sử dụng hàng ngày trong gia đình, phục vụ cho các nhu cầu cơ bản như nấu nướng, vệ sinh, giặt giũ và trang trí. Chúng bao gồm nhiều loại như đồ điện gia dụng (máy giặt, tủ lạnh, lò vi sóng), đồ dùng nhà bếp (nồi, chảo, bát đĩa), thiết bị vệ sinh (máy hút bụi, cây lau nhà), và các sản phẩm trang trí nội thất. Sản phẩm gia dụng giúp tối ưu hóa công việc trong nhà, mang lại sự tiện nghi và thoải mái cho người dùng."
            },
            new()
            {
                Id = 3,
                Name = "Điện tử",
                Description = "Sản phẩm điện tử là các thiết bị sử dụng công nghệ điện để hoạt động, đáp ứng nhu cầu giải trí, liên lạc, làm việc và học tập. Chúng bao gồm điện thoại di động, máy tính xách tay, máy tính bảng, tivi, tai nghe và các thiết bị gia dụng thông minh. Sản phẩm điện tử thường được trang bị các tính năng tiên tiến như kết nối internet, màn hình cảm ứng, và công nghệ không dây, giúp người dùng dễ dàng tương tác và cải thiện hiệu suất trong cuộc sống hàng ngày."
            },
            new()
            {
                Id = 4,
                Name = "Thời trang",
                Description = "Sản phẩm thời trang bao gồm quần áo, giày dép, phụ kiện như túi xách, mũ, và trang sức, được thiết kế để thể hiện phong cách cá nhân và xu hướng thẩm mỹ. Chúng không chỉ đáp ứng nhu cầu mặc hàng ngày mà còn phản ánh cá tính, văn hóa, và thời đại. Các sản phẩm thời trang đa dạng từ trang phục công sở, dạo phố, đến trang phục dạ tiệc, thường được cập nhật liên tục theo mùa và xu hướng thời trang thế giới."
            });

            return builder;
        }

        public static ModelBuilder SeedRole(this ModelBuilder builder)
        {
            builder.Entity<Role>().HasData(new()
            {
                Id = ConstConfig.AdminRoleCode,
                Name = "Admin",
            },
            new()
            {
                Id = ConstConfig.UserRoleCode,
                Name = "User",
            },
            new()
            {
                Id = ConstConfig.ShopRoleCode,
                Name = "Shop",
            });

            return builder;
        }

      
        public static ModelBuilder SeedCustomerType(this ModelBuilder builder)
        {
            builder.Entity<CustomerType>().HasData(new CustomerType
            {
                Id = ConstConfig.CommonCustomerCode,
                Name = "Khách hàng thông thường",
                Description = "Toàn bộ khách hàng thông thường có mua hàng nhưng ít",
            },
            new CustomerType
            {
                Id = 2,
                Name = "Khách hàng thân thiết",
                Description = "Toàn bộ khách hàng thân thiết, thường xuyên mua hàng",
            });

            return builder;
        }

        public static ModelBuilder SeedAccount(this ModelBuilder builder)
        {
            builder.Entity<Account>().HasData(new Account
            {
                Id = 1,
                Email = "admin@example.com",
                Phone = "0912345678",
                Password = BCrypt.Net.BCrypt.HashPassword("password1"),
                Avatar = "avatar1.jpg",
                VerifyToken = "token123",
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1),
                VerifiedAt = new DateTime(2024, 1, 1),
                RoleId = ConstConfig.AdminRoleCode
            });

            return builder;

        }
    }
}
