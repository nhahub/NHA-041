using Braintree;
using Microsoft.Extensions.Options;
using test.Helpers;

namespace test.Services
{
    public class BraintreeService
    {
        BraintreeGateway _gateway;

        public BraintreeService(IOptions<BraintreeSettings> options)
        {
            _gateway = new BraintreeGateway
            {
                Environment = Braintree.Environment.SANDBOX,
                MerchantId = options.Value.MerchantId,
                PublicKey = options.Value.PublicKey,
                PrivateKey = options.Value.PrivateKey,
            }; 
        }
        public string GetClientToken()
        {
            return _gateway.ClientToken.Generate();
        }
        public (bool Success, string? Token, string? Last4, string? CardType, string? ExpiryMonth, string? ExpiryYear, string? Error)
    CreatePaymentMethod(string nonce, string userId)
        {
            try { _gateway.Customer.Find(userId); }
            catch { _gateway.Customer.Create(new CustomerRequest { Id = userId }); }

            var result = _gateway.PaymentMethod.Create(new PaymentMethodRequest
            {
                CustomerId = userId,
                PaymentMethodNonce = nonce
            });

            // Step 3: Return the card info
            if (result.IsSuccess())
            {
                var card = result.Target as CreditCard;
                if (card != null)
                {
                    // Normalize card type string to match what the view expects
                    string cardTypeRaw = card.CardType.ToString();
                    string cardType = cardTypeRaw.ToUpper() switch
                    {
                        "VISA" => "Visa",
                        "MASTERCARD" => "MasterCard",
                        "MASTER_CARD" => "MasterCard",
                        "AMERICANEXPRESS" => "AmericanExpress",
                        "AMERICAN_EXPRESS" => "AmericanExpress",
                        "DISCOVER" => "Discover",
                        _ => cardTypeRaw.StartsWith("V", StringComparison.OrdinalIgnoreCase) ? "Visa" :
                             cardTypeRaw.StartsWith("M", StringComparison.OrdinalIgnoreCase) ? "MasterCard" : 
                             cardTypeRaw
                    };
                    
                    return (true, card.Token, card.LastFour, cardType,
                            card.ExpirationMonth, card.ExpirationYear, null);
                }
            }

            return (false, null, null, null, null, null, result.Message);
        }
        public (bool Success, string? TransactionId, string? ErrorMessage) Sale(string paymentMethodToken, decimal amount)
        {
            var request = new TransactionRequest
            {
                Amount = amount,
                PaymentMethodToken = paymentMethodToken,
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var result = _gateway.Transaction.Sale(request);

            if (result.IsSuccess())
            {
                return (true, result.Target.Id, null);
            }

            return (false, null, result.Message);
        }
    }
}
