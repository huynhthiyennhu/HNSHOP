namespace HNSHOP.Utils
{
    public class ConstConfig
    {
        // Pagination
        public const int PageSize = 2;
        public const int NumberRelatedItem = 4;

        // Default field
        public const string DefaultAvatar = "no-avatar.jpg";
        public const string DefaultImage = "no-image.jpg";

        // Default Code
        public const int AdminRoleCode = 1;
        public const int UserRoleCode = 2;
        public const int ShopRoleCode = 3;
        public const int CommonCustomerCode = 1;

        // Error message
        public const string InvalidId = "Id is invalid!";
        public const string InvalidBody = "Data submit is invalid!";
        public const string NotFound = "Not found!";
        public const string InternalServer = "Something went wrong!";

        // Field's Length
        public const int DisplayNameLength = 32;
        public const int ShortNameLength = 32;
        public const int MediumNameLength = 64;
        public const int LongNameLength = 128;
        public const int ImagePathLength = 256;
        public const int MinPasswordLength = 8;
        public const int MaxPasswordLength = 20;
        public const int DescriptionLength = 4096;
        public const int LongDescriptionLength = 8000;

        // Jwt
        public const int ExperyTimeJwtToken = 60 * 24 * 15;

        // Common
        public const int VerifyEmailTokenLength = 5;

        // CORS
        public const string AllowSpecificOrigins = "AllowSpecific";
        public const string AllowAllOrigins = "AllowAll";
        public const int ClientPort = 3000;

        //Claims
        public const string AccountIdClaimType = "accountId";
        public const string AvatarClaimType = "avatar";
        public const string UserIdClaimType = "userId";


        // Authorization
        public const string AdminPolicy = "AdminPolicy";
        public const string ShopPolicy = "ShopPolicy";
        public const string UserPolicy = "UserPolicy";
        public const string AdminRoleName = "Admin";
        public const string UserRoleName = "User";
        public const string ShopRoleName = "Shop";
    }
}
