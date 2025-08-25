using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System.Net;
using HNSHOP.Utils;
using Microsoft.Extensions.Options;
using HNSHOP.Dtos.Response;

public class PayPalService
{
    private readonly PayPalConfig _config;

    public PayPalService(IOptions<PayPalConfig> config)
    {
        _config = config.Value;
    }

    private PayPalEnvironment GetEnvironment() =>
        _config.Mode.ToLower() == "sandbox"
        ? new SandboxEnvironment(_config.ClientId, _config.Secret)
        : new LiveEnvironment(_config.ClientId, _config.Secret);

    private PayPalHttpClient GetClient() => new PayPalHttpClient(GetEnvironment());


    public async Task<string?> CreateOrder(
    decimal total,
    string currency,
    string returnUrl,
    string cancelUrl,
    string invoiceNote
)
    {
        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");
        request.RequestBody(new OrderRequest
        {
            CheckoutPaymentIntent = "CAPTURE",
            ApplicationContext = new ApplicationContext
            {
                ReturnUrl = returnUrl,
                CancelUrl = cancelUrl,
                LandingPage = "BILLING",
                ShippingPreference = "NO_SHIPPING"
            },
            PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new PurchaseUnitRequest
            {
                Description = invoiceNote,
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = currency,
                    Value = total.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)
                }
            }
        }
        });

        var response = await GetClient().Execute(request);
        var result = response.Result<PayPalCheckoutSdk.Orders.Order>();
        return result.Links.FirstOrDefault(l => l.Rel == "approve")?.Href;
    }


    public async Task<bool> CaptureOrder(string token)
    {
        var request = new OrdersCaptureRequest(token);
        request.RequestBody(new OrderActionRequest());
        var response = await GetClient().Execute(request);
        return response.StatusCode == HttpStatusCode.Created;
    }
}
