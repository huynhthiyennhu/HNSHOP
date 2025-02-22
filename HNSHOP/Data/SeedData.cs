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
                .SeedAccount()
                .SeedCustomerType()
                .SeedCustomer()
                .SeedNotification()
                .SeedCustomerNotification()
                .SeedAddress()
                .SeedOrder()
                .SeedShop()
                .SeedProduct()
                .SeedDetailOrder()
                .SeedRating()
                .SeedSaleEvent()
                .SeedCustomerTypeSaleEvent()
                .SeedProductSaleEvent()
                .SeedLike()
                .SeedProductImage();

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
            },
            new Account
            {
                Id = 2,
                Email = "user1@example.com",
                Phone = "0934567890",
                Password = BCrypt.Net.BCrypt.HashPassword("password2"),
                Avatar = "avatar2.jpg",
                VerifyToken = "token456",
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1),
                VerifiedAt = null,
                RoleId = ConstConfig.UserRoleCode
            },
            new Account
            {
                Id = 3,
                Email = "user2@example.com",
                Phone = "0987654321",
                Password = BCrypt.Net.BCrypt.HashPassword("password3"),
                Avatar = "avatar3.jpg",
                VerifyToken = "token789",
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1),
                VerifiedAt = new DateTime(2024, 1, 1),
                RoleId = ConstConfig.UserRoleCode
            },
            new Account
            {
                Id = 4,
                Email = "seller1@example.com",
                Phone = "0901234567",
                Password = BCrypt.Net.BCrypt.HashPassword("password4"),
                Avatar = "avatar4.jpg",
                VerifyToken = "token101",
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1),
                VerifiedAt = null,
                RoleId = ConstConfig.ShopRoleCode
            },
            new Account
            {
                Id = 5,
                Email = "seller2@example.com",
                Phone = "0912345678",
                Password = BCrypt.Net.BCrypt.HashPassword("password5"),
                Avatar = "avatar5.jpg",
                VerifyToken = "token202",
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1),
                VerifiedAt = new DateTime(2024, 1, 1),
                RoleId = ConstConfig.ShopRoleCode
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

        public static ModelBuilder SeedCustomer(this ModelBuilder builder)
        {
            builder.Entity<Customer>().HasData(new Customer
            {
                Id = 1,
                Name = "Nguyễn Văn A",
                Dob = new DateOnly(1990, 5, 10),
                Description = "Mô tả của khách hàng Nguyễn Văn A",
                AccountId = 2,
                CustomerTypeId = ConstConfig.CommonCustomerCode
            },
            new Customer
            {
                Id = 2,
                Name = "Trần Thị B",
                Dob = new DateOnly(1985, 3, 15),
                Description = "Khách hàng mới",
                AccountId = 3,
                CustomerTypeId = 2
            });

            return builder;
        }

        public static ModelBuilder SeedNotification(this ModelBuilder builder)
        {
            builder.Entity<Notification>().HasData(new Notification
            {
                Id = 1,
                Title = "Thông báo quan trọng",
                Body = "Hãy kiểm tra tài khoản của bạn để biết thêm thông tin.",
                IsRead = false,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Notification
            {
                Id = 2,
                Title = "Cập nhật hệ thống",
                Body = "Hệ thống sẽ bảo trì từ 2 AM đến 4 AM ngày mai.",
                IsRead = false,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Notification
            {
                Id = 3,
                Title = "Tin khuyến mãi",
                Body = "Nhận ngay 20% giảm giá cho đơn hàng đầu tiên của bạn!",
                IsRead = true,
                CreatedAt = new DateTime(2024, 1, 1)
            },
            new Notification
            {
                Id = 4,
                Title = "Nhắc nhở thanh toán",
                Body = "Bạn có một hóa đơn chưa thanh toán. Vui lòng thanh toán ngay.",
                IsRead = false,
                CreatedAt = new DateTime(2024, 1, 1)
            });

            return builder;
        }

        public static ModelBuilder SeedCustomerNotification(this ModelBuilder builder)
        {
            builder.Entity<CustomerNotification>().HasData(new CustomerNotification
            {
                CustomerId = 1,
                NotificationId = 1
            },
            new CustomerNotification
            {
                CustomerId = 1,
                NotificationId = 2
            },
            new CustomerNotification
            {
                CustomerId = 2,
                NotificationId = 3
            },
            new CustomerNotification
            {
                CustomerId = 2,
                NotificationId = 4
            });

            return builder;
        }


        public static ModelBuilder SeedAddress(this ModelBuilder builder)
        {
            builder.Entity<Address>().HasData(new Address
            {
                Id = 1,
                AddressDetail = "123 Đường Lê Lợi, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh",
                CustomerId = 1
            },
            new Address
            {
                Id = 2,
                AddressDetail = "45 Ngõ 19, Phường Hàng Bài, Quận Hoàn Kiếm, Hà Nội",
                CustomerId = 1
            },
            new Address
            {
                Id = 3,
                AddressDetail = "678 Đường Trần Hưng Đạo, Phường Tân Bình, Quận Tân Phú, TP. Hồ Chí Minh",
                CustomerId = 2
            },
            new Address
            {
                Id = 4,
                AddressDetail = "32 Đường Nguyễn Thị Minh Khai, Phường 6, Quận 3, TP. Hồ Chí Minh",
                CustomerId = 2
            });

            return builder;
        }

        public static ModelBuilder SeedOrder(this ModelBuilder builder)
        {
            builder.Entity<Order>().HasData(new Order
            {
                Id = 1,
                Total = 250m,
                Status = OrderStatus.Processing,
                PaymentStatus = PaymentStatus.Pending,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1),
                CustomerId = 1,
                AddressId = 1
            },
            new Order
            {
                Id = 2,
                Total = 120m,
                Status = OrderStatus.Shipping,
                PaymentStatus = PaymentStatus.Completed,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1),
                CustomerId = 1,
                AddressId = 2
            },
            new Order
            {
                Id = 3,
                Total = 75.9m,
                Status = OrderStatus.Delivered,
                PaymentStatus = PaymentStatus.Completed,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1),
                CustomerId = 2,
                AddressId = 3
            },
            new Order
            {
                Id = 4,
                Total = 450.75m,
                Status = OrderStatus.Cancelled,
                PaymentStatus = PaymentStatus.Pending,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1),
                CustomerId = 2,
                AddressId = 4
            });

            return builder;
        }

        public static ModelBuilder SeedShop(this ModelBuilder builder)
        {
            builder.Entity<Shop>().HasData(new Shop
            {
                Id = 1,
                Name = "Elegance Boutique",
                Description = "\"Elegance Boutique\" là điểm đến lý tưởng cho những tín đồ yêu thích thời trang và mỹ phẩm chất lượng cao. Với sứ mệnh mang đến sự hoàn hảo trong từng sản phẩm, cửa hàng của chúng tôi chuyên cung cấp các mặt hàng thời trang và mỹ phẩm từ những thương hiệu danh tiếng, nổi bật với thiết kế tinh tế và phong cách hiện đại.\n\nTại \"Elegance Boutique\", bạn sẽ tìm thấy những bộ sưu tập thời trang đa dạng, từ các trang phục công sở thanh lịch, váy đầm quyến rũ đến những bộ đồ thể thao năng động, đáp ứng mọi nhu cầu và gu thẩm mỹ của khách hàng. Chúng tôi cam kết mang đến những sản phẩm thời trang được chọn lọc kỹ lưỡng từ các thương hiệu hàng đầu, với chất lượng vải tốt nhất và kiểu dáng thời thượng.\n\nBên cạnh thời trang, cửa hàng của chúng tôi còn nổi bật với bộ sưu tập mỹ phẩm cao cấp. Chúng tôi cung cấp các sản phẩm làm đẹp từ các thương hiệu nổi tiếng, bao gồm kem dưỡng da, son môi, sữa rửa mặt và nhiều sản phẩm chăm sóc sắc đẹp khác. Các sản phẩm mỹ phẩm tại \"Elegance Boutique\" được chọn lựa với tiêu chí an toàn, hiệu quả và phù hợp với mọi loại da, giúp bạn luôn tự tin và rạng rỡ.\n\nChúng tôi hiểu rằng việc mua sắm trực tuyến có thể là một trải nghiệm thú vị và thuận tiện, vì vậy cửa hàng của chúng tôi đã đầu tư vào nền tảng thương mại điện tử hiện đại. Bạn có thể dễ dàng duyệt qua các sản phẩm, thực hiện đơn hàng và nhận hàng ngay tại nhà với dịch vụ giao hàng nhanh chóng và bảo đảm. Hãy ghé thăm \"Elegance Boutique\" và khám phá thế giới thời trang và làm đẹp đỉnh cao ngay hôm nay!",
                AccountId = 4
            },
            new Shop
            {
                Id = 2,
                Name = "Tech & Home Center",
                Description = "\"Tech & Home Center\" là cửa hàng trực tuyến hàng đầu cung cấp các sản phẩm gia dụng và điện tử, được thiết kế để nâng cao chất lượng cuộc sống và đáp ứng nhu cầu công nghệ hiện đại của bạn. Chúng tôi tự hào mang đến cho khách hàng một trải nghiệm mua sắm tiện lợi và đa dạng với các sản phẩm chất lượng cao từ những thương hiệu uy tín.\n\nTại \"Tech & Home Center\", bạn sẽ tìm thấy một loạt các sản phẩm gia dụng thiết yếu cho ngôi nhà của bạn. Từ các thiết bị nhà bếp như máy xay sinh tố, nồi chiên không dầu, đến các thiết bị bảo trì và vệ sinh như máy hút bụi và máy lọc không khí, chúng tôi cung cấp những sản phẩm giúp bạn duy trì một môi trường sống sạch sẽ và tiện nghi.\n\nBên cạnh đó, chúng tôi còn cung cấp các thiết bị điện tử tiên tiến, bao gồm máy tính, điện thoại thông minh, TV và các phụ kiện công nghệ khác. Các sản phẩm điện tử của chúng tôi được chọn lựa từ các thương hiệu hàng đầu, đảm bảo hiệu suất vượt trội và tính năng hiện đại, đáp ứng nhu cầu giải trí, làm việc và kết nối của bạn.\n\n\"Tech & Home Center\" cam kết mang đến cho khách hàng một trải nghiệm mua sắm trực tuyến dễ dàng và thuận tiện. Với nền tảng thương mại điện tử tiên tiến, bạn có thể dễ dàng duyệt qua các sản phẩm, so sánh giá cả và đặt hàng chỉ với vài cú click chuột. Chúng tôi cung cấp dịch vụ giao hàng nhanh chóng và đáng tin cậy, cùng với chính sách đổi trả linh hoạt để đảm bảo sự hài lòng tuyệt đối của bạn.\n\nKhám phá \"Tech & Home Center\" ngay hôm nay để tìm kiếm những sản phẩm gia dụng và điện tử chất lượng cao, mang lại sự tiện lợi và hiệu suất tối ưu cho cuộc sống hàng ngày của bạn!",
                AccountId = 5
            });

            return builder;
        }

        public static ModelBuilder SeedProduct(this ModelBuilder builder)
        {
            builder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Điện thoại thông minh Galaxy X12",
                Description = "Điện thoại thông minh với màn hình AMOLED và camera 108MP.",
                Price = 799.9m,
                Quantity = 50,
                ProductTypeId = 3,
                ShopId = 2
            },
            new Product
            {
                Id = 2,
                Name = "Laptop Dell XPS 15",
                Description = "Laptop cao cấp với màn hình 15 inch 4K và hiệu năng mạnh mẽ.",
                Price = 1499.0m,
                Quantity = 30,
                ProductTypeId = 3,
                ShopId = 2
            },
            new Product
            {
                Id = 3,
                Name = "Tai nghe Bluetooth AirPods Pro",
                Description = "Tai nghe không dây với công nghệ chống ồn và âm thanh chất lượng cao.",
                Price = 249.5m,
                Quantity = 100,
                ProductTypeId = 3,
                ShopId = 2
            },
            new Product
            {
                Id = 4,
                Name = "Máy ảnh Canon EOS R5",
                Description = "Máy ảnh mirrorless với cảm biến full-frame và khả năng quay video 8K.",
                Price = 3899.0m,
                Quantity = 15,
                ProductTypeId = 3,
                ShopId = 2
            },

            // Thời trang
            new Product
            {
                Id = 5,
                Name = "Áo sơ mi nam Oxford",
                Description = "Áo sơ mi nam chất liệu cotton cao cấp, kiểu dáng cổ điển.",
                Price = 59.9m,
                Quantity = 200,
                ProductTypeId = 4,
                ShopId = 1
            },
            new Product
            {
                Id = 6,
                Name = "Váy đầm công sở thanh lịch",
                Description = "Váy đầm dành cho văn phòng với thiết kế tinh tế và thanh lịch.",
                Price = 89.0m,
                Quantity = 150,
                ProductTypeId = 4,
                ShopId = 1
            },
            new Product
            {
                Id = 7,
                Name = "Giày thể thao Adidas UltraBoost",
                Description = "Giày thể thao cao cấp với đệm khí Boost, phù hợp cho vận động.",
                Price = 129.0m,
                Quantity = 80,
                ProductTypeId = 4,
                ShopId = 1
            },
            new Product
            {
                Id = 8,
                Name = "Áo khoác lông cừu nữ",
                Description = "Áo khoác giữ ấm với lông cừu mềm mại và kiểu dáng thời trang.",
                Price = 150.0m,
                Quantity = 60,
                ProductTypeId = 4,
                ShopId = 1
            },

            // Gia dụng
            new Product
            {
                Id = 9,
                Name = "Máy lọc không khí Philips",
                Description = "Máy lọc không khí hiệu suất cao với bộ lọc HEPA giúp không khí trong lành.",
                Price = 199.9m,
                Quantity = 70,
                ProductTypeId = 2,
                ShopId = 2
            },
            new Product
            {
                Id = 10,
                Name = "Tủ lạnh LG Inverter 400L",
                Description = "Tủ lạnh tiết kiệm năng lượng với dung tích 400L và công nghệ Inverter.",
                Price = 899.0m,
                Quantity = 25,
                ProductTypeId = 2,
                ShopId = 2
            },
            new Product
            {
                Id = 11,
                Name = "Lò vi sóng Panasonic",
                Description = "Lò vi sóng với nhiều chế độ nấu và công suất mạnh mẽ.",
                Price = 79.9m,
                Quantity = 90,
                ProductTypeId = 2,
                ShopId = 2
            },
            new Product
            {
                Id = 12,
                Name = "Máy giặt Samsung 7kg",
                Description = "Máy giặt cửa trên với công nghệ giặt sạch hiệu quả và dung tích 7kg.",
                Price = 399.0m,
                Quantity = 40,
                ProductTypeId = 2,
                ShopId = 2
            },

            // Mỹ phẩm
            new Product
            {
                Id = 13,
                Name = "Kem dưỡng da Laneige",
                Description = "Kem dưỡng da ban đêm giúp cấp ẩm và làm sáng da.",
                Price = 65.0m,
                Quantity = 100,
                ProductTypeId = 1,
                ShopId = 1
            },
            new Product
            {
                Id = 14,
                Name = "Son môi MAC Matte",
                Description = "Son môi với chất lượng cao, màu sắc bền và không khô môi.",
                Price = 25.0m,
                Quantity = 200,
                ProductTypeId = 1,
                ShopId = 1
            },
            new Product
            {
                Id = 15,
                Name = "Sữa rửa mặt Innisfree",
                Description = "Sữa rửa mặt chiết xuất trà xanh giúp làm sạch da và giảm mụn.",
                Price = 18.0m,
                Quantity = 150,
                ProductTypeId = 1,
                ShopId = 1
            });

            return builder;
        }

        public static ModelBuilder SeedDetailOrder(this ModelBuilder builder)
        {
            builder.Entity<DetailOrder>().HasData(new DetailOrder
            {
                OrderId = 1,
                ProductId = 1,
                Quantity = 2,
                UnitPrice = 799.9m
            },
            new DetailOrder
            {
                OrderId = 1,
                ProductId = 2,
                Quantity = 1,
                UnitPrice = 1499.0m
            },
            new DetailOrder
            {
                OrderId = 2,
                ProductId = 3,
                Quantity = 3,
                UnitPrice = 249.5m
            },
            new DetailOrder
            {
                OrderId = 3,
                ProductId = 3,
                Quantity = 1,
                UnitPrice = 249.5m
            },
            new DetailOrder
            {
                OrderId = 4,
                ProductId = 5,
                Quantity = 1,
                UnitPrice = 59.9m,
            },
            new DetailOrder
            {
                OrderId = 4,
                ProductId = 6,
                Quantity = 6,
                UnitPrice = 89.0m,
            });

            return builder;
        }
        public static ModelBuilder SeedRating(this ModelBuilder builder)
        {
            builder.Entity<Rating>().HasData(new Rating
            {
                Id= 1,
                CustomerId = 1,
                ProductId = 1,
                OrderId = 1,
                UserName = "Nguyễn Văn A",
                RatingValue = 5,
                Comment = "Sản phẩm chất lượng tốt, đáng giá tiền.",
                CreatedAt = new DateTime(2024, 1, 1),
            },
            new Rating
            {
                Id = 2,
                CustomerId = 1,
                ProductId = 2,
                OrderId = 1,
                UserName = "Nguyễn Văn A",
                RatingValue= 4,
                Comment = "Sản phẩm không như mong đợi, cần cải thiện.",
                CreatedAt = new DateTime(2024, 1, 1),
            },
            new Rating
            {
                Id = 3,
                CustomerId = 1,
                ProductId = 3,
                OrderId = 1,
                UserName= "Nguyễn Văn A",
                RatingValue=3,
                Comment = "Dịch vụ tuyệt vời, sản phẩm hoàn hảo.",
                CreatedAt = new DateTime(2024, 1, 1),
            },
            new Rating
            {
                Id = 4,
                CustomerId = 2,
                ProductId = 3,
                OrderId = 2,
                UserName= "Trần Thị B",
                RatingValue=4,
                Comment = "Tốt nhưng cần cải thiện đóng gói.",
                CreatedAt = new DateTime(2024, 1, 1),
            },
            new Rating
            {   Id = 5,
                CustomerId = 2,
                ProductId = 5,
                OrderId = 2,
                UserName= "Trần Thị B",
                RatingValue=5,
                Comment = "Sản phẩm bị lỗi, không hài lòng.",
                CreatedAt = new DateTime(2024, 1, 1),
            },
            new Rating
            {
                Id= 6,
                CustomerId = 2,
                ProductId = 6,
                OrderId = 2,
                UserName = "Trần Thị B",
                RatingValue=2,
                Comment = "Rất hài lòng với chất lượng và dịch vụ.",
                CreatedAt = new DateTime(2024, 1, 1),
            });

            return builder;
        }

        public static ModelBuilder SeedSaleEvent(this ModelBuilder builder)
        {
            builder.Entity<SaleEvent>().HasData(new SaleEvent
            {
                Id = 1,
                Name = "Khuyến Mãi Mùa Hè",
                Description = "Giảm giá 20% cho tất cả các sản phẩm thời trang trong mùa hè này.",
                Discount = 0.2f,
                Illustration = "image1.jpg",
                StartDate = new DateTime(2024, 6, 1),
                EndDate = new DateTime(2024, 6, 30)
            },
            new SaleEvent
            {
                Id = 2,
                Name = "Ngày Hội Công Nghệ",
                Description = "Giảm giá 15% cho các sản phẩm điện tử và công nghệ.",
                Discount = 0.15f,
                Illustration = "image2.jpg",
                StartDate = new DateTime(2024, 9, 10),
                EndDate = new DateTime(2024, 9, 20)
            },
            new SaleEvent
            {
                Id = 3,
                Name = "Khuyến Mãi Cuối Năm",
                Description = "Giảm giá lên đến 30% cho tất cả các sản phẩm gia dụng và mỹ phẩm.",
                Discount = 0.3f,
                Illustration = "image3.jpg",
                StartDate = new DateTime(2024, 12, 1),
                EndDate = new DateTime(2024, 12, 31)
            });

            return builder;
        }

        public static ModelBuilder SeedCustomerTypeSaleEvent(this ModelBuilder builder)
        {
            builder.Entity<CustomerTypeSaleEvent>().HasData(new CustomerTypeSaleEvent
            {
                CustomerTypeId = ConstConfig.CommonCustomerCode,
                SaleEventId = 1
            },
            new CustomerTypeSaleEvent
            {
                CustomerTypeId = ConstConfig.CommonCustomerCode,
                SaleEventId = 2
            },
            new CustomerTypeSaleEvent
            {
                CustomerTypeId = 2,
                SaleEventId = 3
            });

            return builder;
        }

        public static ModelBuilder SeedProductSaleEvent(this ModelBuilder builder)
        {
            builder.Entity<ProductSaleEvent>().HasData(new ProductSaleEvent
            {
                ProductId = 5,
                SaleEventId = 1
            },
            new ProductSaleEvent
            {
                ProductId = 6,
                SaleEventId = 1
            },
            new ProductSaleEvent
            {
                ProductId = 7,
                SaleEventId = 1
            },
            new ProductSaleEvent
            {
                ProductId = 8,
                SaleEventId = 1
            },
            new ProductSaleEvent
            {
                ProductId = 9,
                SaleEventId = 1
            },
            new ProductSaleEvent
            {
                ProductId = 10,
                SaleEventId = 1
            },
            new ProductSaleEvent
            {
                ProductId = 11,
                SaleEventId = 1
            },
            new ProductSaleEvent
            {
                ProductId = 1,
                SaleEventId = 2
            },
            new ProductSaleEvent
            {
                ProductId = 2,
                SaleEventId = 2
            },
            new ProductSaleEvent
            {
                ProductId = 3,
                SaleEventId = 2
            },
            new ProductSaleEvent
            {
                ProductId = 4,
                SaleEventId = 2
            },
            new ProductSaleEvent
            {
                ProductId = 13,
                SaleEventId = 3
            },
            new ProductSaleEvent
            {
                ProductId = 14,
                SaleEventId = 3
            },
            new ProductSaleEvent
            {
                ProductId = 15,
                SaleEventId = 3
            });

            return builder;
        }

        public static ModelBuilder SeedLike(this ModelBuilder builder)
        {
            builder.Entity<Like>().HasData(new Like
            {
                CustomerId = 1,
                ProductId = 1,
                CreatedAt = new DateTime(2024, 1, 1).AddMinutes(-5),
            },
            new Like
            {
                CustomerId = 1,
                ProductId = 2,
                CreatedAt = new DateTime(2024, 1, 1).AddMinutes(-3),
            },
            new Like
            {
                CustomerId = 2,
                ProductId = 5,
                CreatedAt = new DateTime(2024, 1, 1).AddMinutes(-10),
            },
            new Like
            {
                CustomerId = 1,
                ProductId = 8,
                CreatedAt = new DateTime(2024, 1, 1).AddMinutes(-12),
            });

            return builder;
        }
        public static ModelBuilder SeedProductImage(this ModelBuilder builder)
        {
            builder.Entity<ProductImage>().HasData(
            new ProductImage { Id = 1, Path = "product1_image1.jpg", ProductId = 1 },
            new ProductImage { Id = 2, Path = "product1_image2.jpg", ProductId = 1 },

            new ProductImage { Id = 3, Path = "product2_image1.jpg", ProductId = 2 },
            new ProductImage { Id = 4, Path = "product2_image2.jpg", ProductId = 2 },

            new ProductImage { Id = 5, Path = "product3_image1.jpg", ProductId = 3 },
            new ProductImage { Id = 6, Path = "product3_image2.jpg", ProductId = 3 },

            new ProductImage { Id = 7, Path = "product4_image1.jpg", ProductId = 4 },
            new ProductImage { Id = 8, Path = "product4_image2.jpg", ProductId = 4 },

            new ProductImage { Id = 9, Path = "product5_image1.jpg", ProductId = 5 },
            new ProductImage { Id = 10, Path = "product5_image2.jpg", ProductId = 5 },

            new ProductImage { Id = 11, Path = "product6_image1.jpg", ProductId = 6 },
            new ProductImage { Id = 12, Path = "product6_image2.jpg", ProductId = 6 },

            new ProductImage { Id = 13, Path = "product7_image1.jpg", ProductId = 7 },
            new ProductImage { Id = 14, Path = "product7_image2.jpg", ProductId = 7 },

            new ProductImage { Id = 15, Path = "product8_image1.jpg", ProductId = 8 },
            new ProductImage { Id = 16, Path = "product8_image2.jpg", ProductId = 8 },

            new ProductImage { Id = 17, Path = "product9_image1.jpg", ProductId = 9 },
            new ProductImage { Id = 18, Path = "product9_image2.jpg", ProductId = 9 },

            new ProductImage { Id = 19, Path = "product10_image1.jpg", ProductId = 10 },
            new ProductImage { Id = 20, Path = "product10_image2.jpg", ProductId = 10 },

            new ProductImage { Id = 21, Path = "product11_image1.jpg", ProductId = 11 },
            new ProductImage { Id = 22, Path = "product11_image2.jpg", ProductId = 11 },

            new ProductImage { Id = 23, Path = "product12_image1.jpg", ProductId = 12 },
            new ProductImage { Id = 24, Path = "product12_image2.jpg", ProductId = 12 },

            new ProductImage { Id = 25, Path = "product13_image1.jpg", ProductId = 13 },
            new ProductImage { Id = 26, Path = "product13_image2.jpg", ProductId = 13 },

            new ProductImage { Id = 27, Path = "product14_image1.jpg", ProductId = 14 },
            new ProductImage { Id = 28, Path = "product14_image2.jpg", ProductId = 14 },

            new ProductImage { Id = 29, Path = "product15_image1.jpg", ProductId = 15 },
            new ProductImage { Id = 30, Path = "product15_image2.jpg", ProductId = 15 }
            );

            return builder;
        }
    }
}
