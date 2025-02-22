using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HNSHOP.Migrations
{
    /// <inheritdoc />
    public partial class Fix_Rating_Remove_Order : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaleEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Illustration = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Discount = table.Column<float>(type: "real", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleEvents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VerifyToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerTypeSaleEvents",
                columns: table => new
                {
                    CustomerTypeId = table.Column<int>(type: "int", nullable: false),
                    SaleEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTypeSaleEvents", x => new { x.CustomerTypeId, x.SaleEventId });
                    table.ForeignKey(
                        name: "FK_CustomerTypeSaleEvents_CustomerTypes_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "CustomerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerTypeSaleEvents_SaleEvents_SaleEventId",
                        column: x => x.SaleEventId,
                        principalTable: "SaleEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dob = table.Column<DateOnly>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CustomerTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Customers_CustomerTypes_CustomerTypeId",
                        column: x => x.CustomerTypeId,
                        principalTable: "CustomerTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Shops",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shops", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Shops_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressDetail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerNotifications",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerNotifications", x => new { x.CustomerId, x.NotificationId });
                    table.ForeignKey(
                        name: "FK_CustomerNotifications_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerNotifications_Notifications_NotificationId",
                        column: x => x.NotificationId,
                        principalTable: "Notifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(9,1)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    ProductTypeId = table.Column<int>(type: "int", nullable: false),
                    ShopId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_ProductTypes_ProductTypeId",
                        column: x => x.ProductTypeId,
                        principalTable: "ProductTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Shops_ShopId",
                        column: x => x.ShopId,
                        principalTable: "Shops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Total = table.Column<decimal>(type: "decimal(9,1)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => new { x.CustomerId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_Likes_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductSaleEvents",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    SaleEventId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSaleEvents", x => new { x.ProductId, x.SaleEventId });
                    table.ForeignKey(
                        name: "FK_ProductSaleEvents_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSaleEvents_SaleEvents_SaleEventId",
                        column: x => x.SaleEventId,
                        principalTable: "SaleEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetailOrders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(9,1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailOrders", x => new { x.OrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_DetailOrders_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetailOrders_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RatingValue = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Ratings_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Ratings_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "CustomerTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Toàn bộ khách hàng thông thường có mua hàng nhưng ít", "Khách hàng thông thường" },
                    { 2, "Toàn bộ khách hàng thân thiết, thường xuyên mua hàng", "Khách hàng thân thiết" }
                });

            migrationBuilder.InsertData(
                table: "Notifications",
                columns: new[] { "Id", "Body", "CreatedAt", "IsRead", "Title" },
                values: new object[,]
                {
                    { 1, "Hãy kiểm tra tài khoản của bạn để biết thêm thông tin.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Thông báo quan trọng" },
                    { 2, "Hệ thống sẽ bảo trì từ 2 AM đến 4 AM ngày mai.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Cập nhật hệ thống" },
                    { 3, "Nhận ngay 20% giảm giá cho đơn hàng đầu tiên của bạn!", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Tin khuyến mãi" },
                    { 4, "Bạn có một hóa đơn chưa thanh toán. Vui lòng thanh toán ngay.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Nhắc nhở thanh toán" }
                });

            migrationBuilder.InsertData(
                table: "ProductTypes",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Mỹ phẩm là các sản phẩm chăm sóc cá nhân được thiết kế để làm đẹp, bảo vệ hoặc cải thiện vẻ ngoài của da, tóc, móng và cơ thể. Chúng bao gồm nhiều loại sản phẩm như kem dưỡng da, son môi, phấn nền, mascara, dầu gội, và nước hoa. Mỹ phẩm có thể chứa các thành phần tự nhiên hoặc tổng hợp, giúp làm sạch, dưỡng ẩm, chống lão hóa, và bảo vệ da khỏi các tác động xấu từ môi trường.", "Mỹ phẩm" },
                    { 2, "Sản phẩm gia dụng là các vật dụng được sử dụng hàng ngày trong gia đình, phục vụ cho các nhu cầu cơ bản như nấu nướng, vệ sinh, giặt giũ và trang trí. Chúng bao gồm nhiều loại như đồ điện gia dụng (máy giặt, tủ lạnh, lò vi sóng), đồ dùng nhà bếp (nồi, chảo, bát đĩa), thiết bị vệ sinh (máy hút bụi, cây lau nhà), và các sản phẩm trang trí nội thất. Sản phẩm gia dụng giúp tối ưu hóa công việc trong nhà, mang lại sự tiện nghi và thoải mái cho người dùng.", "Gia dụng" },
                    { 3, "Sản phẩm điện tử là các thiết bị sử dụng công nghệ điện để hoạt động, đáp ứng nhu cầu giải trí, liên lạc, làm việc và học tập. Chúng bao gồm điện thoại di động, máy tính xách tay, máy tính bảng, tivi, tai nghe và các thiết bị gia dụng thông minh. Sản phẩm điện tử thường được trang bị các tính năng tiên tiến như kết nối internet, màn hình cảm ứng, và công nghệ không dây, giúp người dùng dễ dàng tương tác và cải thiện hiệu suất trong cuộc sống hàng ngày.", "Điện tử" },
                    { 4, "Sản phẩm thời trang bao gồm quần áo, giày dép, phụ kiện như túi xách, mũ, và trang sức, được thiết kế để thể hiện phong cách cá nhân và xu hướng thẩm mỹ. Chúng không chỉ đáp ứng nhu cầu mặc hàng ngày mà còn phản ánh cá tính, văn hóa, và thời đại. Các sản phẩm thời trang đa dạng từ trang phục công sở, dạo phố, đến trang phục dạ tiệc, thường được cập nhật liên tục theo mùa và xu hướng thời trang thế giới.", "Thời trang" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" },
                    { 3, "Shop" }
                });

            migrationBuilder.InsertData(
                table: "SaleEvents",
                columns: new[] { "Id", "Description", "Discount", "EndDate", "Illustration", "Name", "StartDate" },
                values: new object[,]
                {
                    { 1, "Giảm giá 20% cho tất cả các sản phẩm thời trang trong mùa hè này.", 0.2f, new DateTime(2024, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "image1.jpg", "Khuyến Mãi Mùa Hè", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "Giảm giá 15% cho các sản phẩm điện tử và công nghệ.", 0.15f, new DateTime(2024, 9, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "image2.jpg", "Ngày Hội Công Nghệ", new DateTime(2024, 9, 10, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Giảm giá lên đến 30% cho tất cả các sản phẩm gia dụng và mỹ phẩm.", 0.3f, new DateTime(2024, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "image3.jpg", "Khuyến Mãi Cuối Năm", new DateTime(2024, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Avatar", "CreatedAt", "Email", "Password", "Phone", "RoleId", "UpdatedAt", "VerifiedAt", "VerifyToken" },
                values: new object[,]
                {
                    { 1, "avatar1.jpg", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@example.com", "$2a$11$5gFLfj.bXdEK2KSIhOwh8u2bammAdkjjhc9c3TaQsPQ1qoWiJqQ5W", "0912345678", 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "token123" },
                    { 2, "avatar2.jpg", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user1@example.com", "$2a$11$/vw9fH8V3R3jGvna/0qd7ej4VMeXPLSy1ZNsLeyi3cdNnwgZXResm", "0934567890", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "token456" },
                    { 3, "avatar3.jpg", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user2@example.com", "$2a$11$i5QF0H60zDZ0fyXBKz4Kieo3lALYq0MAc0gg2pMSMdRoUmqDoQ8x2", "0987654321", 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "token789" },
                    { 4, "avatar4.jpg", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "seller1@example.com", "$2a$11$ZOwm6XhayPFsewbgj9AwrOTXvaO6gdqxXLRQbfbeXTIrn3xGt5z/y", "0901234567", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "token101" },
                    { 5, "avatar5.jpg", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "seller2@example.com", "$2a$11$bPKmpaW4vMEeGpNOvWqBp.bb5E7/lAhc.ACpFTeaA8JjS3mUYmSs6", "0912345678", 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "token202" }
                });

            migrationBuilder.InsertData(
                table: "CustomerTypeSaleEvents",
                columns: new[] { "CustomerTypeId", "SaleEventId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "AccountId", "CustomerTypeId", "Description", "Dob", "Name" },
                values: new object[,]
                {
                    { 1, 2, 1, "Mô tả của khách hàng Nguyễn Văn A", new DateOnly(1990, 5, 10), "Nguyễn Văn A" },
                    { 2, 3, 2, "Khách hàng mới", new DateOnly(1985, 3, 15), "Trần Thị B" }
                });

            migrationBuilder.InsertData(
                table: "Shops",
                columns: new[] { "Id", "AccountId", "Description", "Name" },
                values: new object[,]
                {
                    { 1, 4, "\"Elegance Boutique\" là điểm đến lý tưởng cho những tín đồ yêu thích thời trang và mỹ phẩm chất lượng cao. Với sứ mệnh mang đến sự hoàn hảo trong từng sản phẩm, cửa hàng của chúng tôi chuyên cung cấp các mặt hàng thời trang và mỹ phẩm từ những thương hiệu danh tiếng, nổi bật với thiết kế tinh tế và phong cách hiện đại.\n\nTại \"Elegance Boutique\", bạn sẽ tìm thấy những bộ sưu tập thời trang đa dạng, từ các trang phục công sở thanh lịch, váy đầm quyến rũ đến những bộ đồ thể thao năng động, đáp ứng mọi nhu cầu và gu thẩm mỹ của khách hàng. Chúng tôi cam kết mang đến những sản phẩm thời trang được chọn lọc kỹ lưỡng từ các thương hiệu hàng đầu, với chất lượng vải tốt nhất và kiểu dáng thời thượng.\n\nBên cạnh thời trang, cửa hàng của chúng tôi còn nổi bật với bộ sưu tập mỹ phẩm cao cấp. Chúng tôi cung cấp các sản phẩm làm đẹp từ các thương hiệu nổi tiếng, bao gồm kem dưỡng da, son môi, sữa rửa mặt và nhiều sản phẩm chăm sóc sắc đẹp khác. Các sản phẩm mỹ phẩm tại \"Elegance Boutique\" được chọn lựa với tiêu chí an toàn, hiệu quả và phù hợp với mọi loại da, giúp bạn luôn tự tin và rạng rỡ.\n\nChúng tôi hiểu rằng việc mua sắm trực tuyến có thể là một trải nghiệm thú vị và thuận tiện, vì vậy cửa hàng của chúng tôi đã đầu tư vào nền tảng thương mại điện tử hiện đại. Bạn có thể dễ dàng duyệt qua các sản phẩm, thực hiện đơn hàng và nhận hàng ngay tại nhà với dịch vụ giao hàng nhanh chóng và bảo đảm. Hãy ghé thăm \"Elegance Boutique\" và khám phá thế giới thời trang và làm đẹp đỉnh cao ngay hôm nay!", "Elegance Boutique" },
                    { 2, 5, "\"Tech & Home Center\" là cửa hàng trực tuyến hàng đầu cung cấp các sản phẩm gia dụng và điện tử, được thiết kế để nâng cao chất lượng cuộc sống và đáp ứng nhu cầu công nghệ hiện đại của bạn. Chúng tôi tự hào mang đến cho khách hàng một trải nghiệm mua sắm tiện lợi và đa dạng với các sản phẩm chất lượng cao từ những thương hiệu uy tín.\n\nTại \"Tech & Home Center\", bạn sẽ tìm thấy một loạt các sản phẩm gia dụng thiết yếu cho ngôi nhà của bạn. Từ các thiết bị nhà bếp như máy xay sinh tố, nồi chiên không dầu, đến các thiết bị bảo trì và vệ sinh như máy hút bụi và máy lọc không khí, chúng tôi cung cấp những sản phẩm giúp bạn duy trì một môi trường sống sạch sẽ và tiện nghi.\n\nBên cạnh đó, chúng tôi còn cung cấp các thiết bị điện tử tiên tiến, bao gồm máy tính, điện thoại thông minh, TV và các phụ kiện công nghệ khác. Các sản phẩm điện tử của chúng tôi được chọn lựa từ các thương hiệu hàng đầu, đảm bảo hiệu suất vượt trội và tính năng hiện đại, đáp ứng nhu cầu giải trí, làm việc và kết nối của bạn.\n\n\"Tech & Home Center\" cam kết mang đến cho khách hàng một trải nghiệm mua sắm trực tuyến dễ dàng và thuận tiện. Với nền tảng thương mại điện tử tiên tiến, bạn có thể dễ dàng duyệt qua các sản phẩm, so sánh giá cả và đặt hàng chỉ với vài cú click chuột. Chúng tôi cung cấp dịch vụ giao hàng nhanh chóng và đáng tin cậy, cùng với chính sách đổi trả linh hoạt để đảm bảo sự hài lòng tuyệt đối của bạn.\n\nKhám phá \"Tech & Home Center\" ngay hôm nay để tìm kiếm những sản phẩm gia dụng và điện tử chất lượng cao, mang lại sự tiện lợi và hiệu suất tối ưu cho cuộc sống hàng ngày của bạn!", "Tech & Home Center" }
                });

            migrationBuilder.InsertData(
                table: "Addresses",
                columns: new[] { "Id", "AddressDetail", "CustomerId" },
                values: new object[,]
                {
                    { 1, "123 Đường Lê Lợi, Phường Bến Nghé, Quận 1, TP. Hồ Chí Minh", 1 },
                    { 2, "45 Ngõ 19, Phường Hàng Bài, Quận Hoàn Kiếm, Hà Nội", 1 },
                    { 3, "678 Đường Trần Hưng Đạo, Phường Tân Bình, Quận Tân Phú, TP. Hồ Chí Minh", 2 },
                    { 4, "32 Đường Nguyễn Thị Minh Khai, Phường 6, Quận 3, TP. Hồ Chí Minh", 2 }
                });

            migrationBuilder.InsertData(
                table: "CustomerNotifications",
                columns: new[] { "CustomerId", "NotificationId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 3 },
                    { 2, 4 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Description", "Name", "Price", "ProductTypeId", "Quantity", "ShopId" },
                values: new object[,]
                {
                    { 1, "Điện thoại thông minh với màn hình AMOLED và camera 108MP.", "Điện thoại thông minh Galaxy X12", 799.9m, 3, 50, 2 },
                    { 2, "Laptop cao cấp với màn hình 15 inch 4K và hiệu năng mạnh mẽ.", "Laptop Dell XPS 15", 1499.0m, 3, 30, 2 },
                    { 3, "Tai nghe không dây với công nghệ chống ồn và âm thanh chất lượng cao.", "Tai nghe Bluetooth AirPods Pro", 249.5m, 3, 100, 2 },
                    { 4, "Máy ảnh mirrorless với cảm biến full-frame và khả năng quay video 8K.", "Máy ảnh Canon EOS R5", 3899.0m, 3, 15, 2 },
                    { 5, "Áo sơ mi nam chất liệu cotton cao cấp, kiểu dáng cổ điển.", "Áo sơ mi nam Oxford", 59.9m, 4, 200, 1 },
                    { 6, "Váy đầm dành cho văn phòng với thiết kế tinh tế và thanh lịch.", "Váy đầm công sở thanh lịch", 89.0m, 4, 150, 1 },
                    { 7, "Giày thể thao cao cấp với đệm khí Boost, phù hợp cho vận động.", "Giày thể thao Adidas UltraBoost", 129.0m, 4, 80, 1 },
                    { 8, "Áo khoác giữ ấm với lông cừu mềm mại và kiểu dáng thời trang.", "Áo khoác lông cừu nữ", 150.0m, 4, 60, 1 },
                    { 9, "Máy lọc không khí hiệu suất cao với bộ lọc HEPA giúp không khí trong lành.", "Máy lọc không khí Philips", 199.9m, 2, 70, 2 },
                    { 10, "Tủ lạnh tiết kiệm năng lượng với dung tích 400L và công nghệ Inverter.", "Tủ lạnh LG Inverter 400L", 899.0m, 2, 25, 2 },
                    { 11, "Lò vi sóng với nhiều chế độ nấu và công suất mạnh mẽ.", "Lò vi sóng Panasonic", 79.9m, 2, 90, 2 },
                    { 12, "Máy giặt cửa trên với công nghệ giặt sạch hiệu quả và dung tích 7kg.", "Máy giặt Samsung 7kg", 399.0m, 2, 40, 2 },
                    { 13, "Kem dưỡng da ban đêm giúp cấp ẩm và làm sáng da.", "Kem dưỡng da Laneige", 65.0m, 1, 100, 1 },
                    { 14, "Son môi với chất lượng cao, màu sắc bền và không khô môi.", "Son môi MAC Matte", 25.0m, 1, 200, 1 },
                    { 15, "Sữa rửa mặt chiết xuất trà xanh giúp làm sạch da và giảm mụn.", "Sữa rửa mặt Innisfree", 18.0m, 1, 150, 1 }
                });

            migrationBuilder.InsertData(
                table: "Likes",
                columns: new[] { "CustomerId", "ProductId", "CreatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2023, 12, 31, 23, 55, 0, 0, DateTimeKind.Unspecified) },
                    { 1, 2, new DateTime(2023, 12, 31, 23, 57, 0, 0, DateTimeKind.Unspecified) },
                    { 1, 8, new DateTime(2023, 12, 31, 23, 48, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 5, new DateTime(2023, 12, 31, 23, 50, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "AddressId", "CreatedAt", "CustomerId", "PaymentStatus", "Status", "Total", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Pending", "Processing", 250m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 2, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Completed", "Shipping", 120m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Completed", "Delivered", 75.9m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 4, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "Pending", "Cancelled", 450.75m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "Path", "ProductId" },
                values: new object[,]
                {
                    { 1, "product1_image1.jpg", 1 },
                    { 2, "product1_image2.jpg", 1 },
                    { 3, "product2_image1.jpg", 2 },
                    { 4, "product2_image2.jpg", 2 },
                    { 5, "product3_image1.jpg", 3 },
                    { 6, "product3_image2.jpg", 3 },
                    { 7, "product4_image1.jpg", 4 },
                    { 8, "product4_image2.jpg", 4 },
                    { 9, "product5_image1.jpg", 5 },
                    { 10, "product5_image2.jpg", 5 },
                    { 11, "product6_image1.jpg", 6 },
                    { 12, "product6_image2.jpg", 6 },
                    { 13, "product7_image1.jpg", 7 },
                    { 14, "product7_image2.jpg", 7 },
                    { 15, "product8_image1.jpg", 8 },
                    { 16, "product8_image2.jpg", 8 },
                    { 17, "product9_image1.jpg", 9 },
                    { 18, "product9_image2.jpg", 9 },
                    { 19, "product10_image1.jpg", 10 },
                    { 20, "product10_image2.jpg", 10 },
                    { 21, "product11_image1.jpg", 11 },
                    { 22, "product11_image2.jpg", 11 },
                    { 23, "product12_image1.jpg", 12 },
                    { 24, "product12_image2.jpg", 12 },
                    { 25, "product13_image1.jpg", 13 },
                    { 26, "product13_image2.jpg", 13 },
                    { 27, "product14_image1.jpg", 14 },
                    { 28, "product14_image2.jpg", 14 },
                    { 29, "product15_image1.jpg", 15 },
                    { 30, "product15_image2.jpg", 15 }
                });

            migrationBuilder.InsertData(
                table: "ProductSaleEvents",
                columns: new[] { "ProductId", "SaleEventId" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 2, 2 },
                    { 3, 2 },
                    { 4, 2 },
                    { 5, 1 },
                    { 6, 1 },
                    { 7, 1 },
                    { 8, 1 },
                    { 9, 1 },
                    { 10, 1 },
                    { 11, 1 },
                    { 13, 3 },
                    { 14, 3 },
                    { 15, 3 }
                });

            migrationBuilder.InsertData(
                table: "DetailOrders",
                columns: new[] { "OrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 2, 799.9m },
                    { 1, 2, 1, 1499.0m },
                    { 2, 3, 3, 249.5m },
                    { 3, 3, 1, 249.5m },
                    { 4, 5, 1, 59.9m },
                    { 4, 6, 6, 89.0m }
                });

            migrationBuilder.InsertData(
                table: "Ratings",
                columns: new[] { "Id", "Comment", "CreatedAt", "CustomerId", "OrderId", "ProductId", "RatingValue", "UserName" },
                values: new object[,]
                {
                    { 1, "Sản phẩm chất lượng tốt, đáng giá tiền.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 1, 5, "Nguyễn Văn A" },
                    { 2, "Sản phẩm không như mong đợi, cần cải thiện.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 2, 4, "Nguyễn Văn A" },
                    { 3, "Dịch vụ tuyệt vời, sản phẩm hoàn hảo.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 3, 3, "Nguyễn Văn A" },
                    { 4, "Tốt nhưng cần cải thiện đóng gói.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, 3, 4, "Trần Thị B" },
                    { 5, "Sản phẩm bị lỗi, không hài lòng.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, 5, 5, "Trần Thị B" },
                    { 6, "Rất hài lòng với chất lượng và dịch vụ.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 2, 6, 2, "Trần Thị B" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_Email",
                table: "Accounts",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_RoleId",
                table: "Accounts",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustomerId",
                table: "Addresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerNotifications_NotificationId",
                table: "CustomerNotifications",
                column: "NotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_AccountId",
                table: "Customers",
                column: "AccountId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_CustomerTypeId",
                table: "Customers",
                column: "CustomerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTypeSaleEvents_SaleEventId",
                table: "CustomerTypeSaleEvents",
                column: "SaleEventId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailOrders_ProductId",
                table: "DetailOrders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ProductId",
                table: "Likes",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddressId",
                table: "Orders",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_Path",
                table: "ProductImages",
                column: "Path",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductTypeId",
                table: "Products",
                column: "ProductTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ShopId",
                table: "Products",
                column: "ShopId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSaleEvents_SaleEventId",
                table: "ProductSaleEvents",
                column: "SaleEventId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_CustomerId",
                table: "Ratings",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_OrderId",
                table: "Ratings",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ProductId",
                table: "Ratings",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Shops_AccountId",
                table: "Shops",
                column: "AccountId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerNotifications");

            migrationBuilder.DropTable(
                name: "CustomerTypeSaleEvents");

            migrationBuilder.DropTable(
                name: "DetailOrders");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "ProductSaleEvents");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "SaleEvents");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "ProductTypes");

            migrationBuilder.DropTable(
                name: "Shops");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "CustomerTypes");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
