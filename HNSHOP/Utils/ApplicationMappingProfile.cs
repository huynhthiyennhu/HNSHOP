using AutoMapper;
using HNSHOP.Dtos.Request;
using HNSHOP.Dtos.Response;
using HNSHOP.Models;

namespace HNSHOP.Utils
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            // Mapping từ CartItem sang CartItemResDto
            CreateMap<CartItem, CartItemResDto>()
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
             .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.Image));
            // Product Type Mapping
            CreateMap<ProductType, ProductTypeResDto>();
            CreateMap<ProductTypeReqDto, ProductType>();

            // Shop Mapping
            CreateMap<Shop, ShopResDto>();
            CreateMap<UpdateShopReqDto, Shop>()
                .ForAllMembers(opts => opts
                    .Condition((src, dest, srcMember) =>
                    {
                        if (srcMember is null) return false;
                        if (srcMember is string str) return !string.IsNullOrWhiteSpace(str);
                        return true;
                    }));

            // Product Mapping
            CreateMap<Product, ProductResDto>()
                .ForMember(dest => dest.DiscountPercent, opt => opt.MapFrom(src =>
                    src.SaleEvents
                        .Where(se => DateTime.UtcNow >= se.StartDate && DateTime.UtcNow <= se.EndDate)
                        .Select(se => se.Discount)
                        .FirstOrDefault()))
                .ForMember(dest => dest.LikeQuantity, opt => opt.MapFrom(src => src.Likes.Count))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src =>
                    src.Ratings.Count > 0
                        ? Math.Round((decimal)src.Ratings.Sum(r => r.RatingValue) / src.Ratings.Count, 1)
                        : 0m))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages))
                .ForMember(dest => dest.SoldQuantity, opt => opt.MapFrom(src => src.DetailOrders.Sum(dt => dt.Quantity)));

            CreateMap<ProductReqDto, Product>();
            CreateMap<UpdateProductReqDto, Product>()
                .ForAllMembers(opts => opts
                    .Condition((src, dest, srcMember) =>
                    {
                        if (srcMember is null) return false;
                        if (srcMember is decimal d) return d != default;
                        if (srcMember is int i) return i != default;
                        if (srcMember is string str) return !string.IsNullOrWhiteSpace(str);
                        return true;
                    }));

            CreateMap<Product, DetailProductResDto>()
               .ForMember(dest => dest.DiscountPercent, opt => opt.MapFrom(src =>
                   src.SaleEvents
                       .Where(se => DateTime.UtcNow >= se.StartDate && DateTime.UtcNow <= se.EndDate)
                       .Select(se => se.Discount)
                       .FirstOrDefault()))
               .ForMember(dest => dest.LikeQuantity, opt => opt.MapFrom(src => src.Likes.Count))
               .ForMember(dest => dest.Rating, opt => opt.MapFrom(src =>
                   src.Ratings.Count > 0
                       ? Math.Round((decimal)src.Ratings.Sum(r => r.RatingValue) / src.Ratings.Count, 1)
                       : 0m))
               .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages))
               .ForMember(dest => dest.SoldQuantity, opt => opt.MapFrom(src => src.DetailOrders.Sum(dt => dt.Quantity)))
               .ForMember(dest => dest.Shop, opt => opt.MapFrom(src => src.Shop))
               .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.ProductType));

            // Product Image Mapping
            CreateMap<ProductImage, ProductImageResDto>();
            CreateMap<ProductImageReqDto, ProductImage>();

            // Rating Mapping
            CreateMap<Rating, RatingResDto>();

            // Register User Mapping
            CreateMap<RegisterReqDto, Account>()
                .ForPath(dest => dest.Customer.Name, opt => opt.MapFrom(src => src.Name))
                .ForPath(dest => dest.Customer.CustomerTypeId, opt => opt.MapFrom(src => ConstConfig.CommonCustomerCode))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => ConstConfig.DefaultAvatar))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => ConstConfig.UserRoleCode))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));

            // Register Shop Mapping
            CreateMap<RegisterShopReqDto, Account>()
                .ForPath(dest => dest.Shop.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => ConstConfig.DefaultAvatar))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => ConstConfig.ShopRoleCode))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));

            // Sale Event Mapping
            CreateMap<SaleEvent, SaleEventResDto>();
            CreateMap<SaleEvent, SaleEventProductResDto>();
            CreateMap<CreateSaleEventReqDto, SaleEvent>();
            CreateMap<UpdateSaleEventReqDto, SaleEvent>()
                .ForAllMembers(opts => opts
                    .Condition((src, dest, srcMember) =>
                    {
                        if (srcMember is null) return false;
                        if (srcMember is DateTime date) return date != DateTime.MinValue;
                        if (srcMember is float f) return f != default;
                        if (srcMember is string str) return !string.IsNullOrWhiteSpace(str);
                        return true;
                    }));

            // Customer Type Mapping
            CreateMap<CustomerType, CustomerTypeResDto>();

            // Customer Mapping
            CreateMap<Customer, CustomerResDto>();

            // Checkout Mapping
            CreateMap<HandleDetailOrderReqDto, DetailOrder>();
            CreateMap<Order, OrderResDto>();
            CreateMap<Address, AddressResDto>();

            CreateMap<Product, CompactProductResDto>()
                .ForMember(dest => dest.DiscountPercent, opt => opt.MapFrom(src =>
                    src.SaleEvents
                        .Where(se => DateTime.UtcNow >= se.StartDate && DateTime.UtcNow <= se.EndDate)
                        .Select(se => se.Discount)
                        .FirstOrDefault()))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));

            CreateMap<DetailOrder, DetailOrderResDto>();

            // Account Mapping
            CreateMap<Account, AccountResDto>();

            CreateMap<UpdateAccountReqDto, Account>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForAllMembers(opts => opts
                    .Condition((src, dest, srcMember) =>
                    {
                        if (srcMember is null) return false;
                        if (srcMember is string str) return !string.IsNullOrWhiteSpace(str);
                        return true;
                    }));
        }
    }
}
