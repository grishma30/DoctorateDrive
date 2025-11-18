using Razorpay.Api;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoctorateDrive.Services
{
    public interface IPaymentService
    {
        Order CreateOrder(decimal amount, string applicationId, string receipt);
        bool VerifyPaymentSignature(string orderId, string paymentId, string signature);
    }

    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;
        private readonly RazorpayClient _client;

        public PaymentService(IConfiguration configuration, ILogger<PaymentService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            var keyId = _configuration["RazorpaySettings:KeyId"];
            var keySecret = _configuration["RazorpaySettings:KeySecret"];

            _client = new RazorpayClient(keyId, keySecret);
        }

        public Order CreateOrder(decimal amount, string applicationId, string receipt)
        {
            try
            {
                var amountInPaise = (int)(amount * 100);

                var options = new Dictionary<string, object>
                {
                    { "amount", amountInPaise },
                    { "currency", "INR" },
                    { "receipt", receipt },
                    { "payment_capture", 1 }
                };

                Order order = _client.Order.Create(options);

                // ✅ Fix: Extract to string before logging
                string orderId = order["id"]?.ToString() ?? "N/A";
                _logger.LogInformation("Order created: OrderId={OrderId}, Amount={Amount}", orderId, amountInPaise);

                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create order");
                throw;
            }
        }

        public bool VerifyPaymentSignature(string orderId, string paymentId, string signature)
        {
            try
            {
                var attributes = new Dictionary<string, string>
                {
                    { "razorpay_order_id", orderId },
                    { "razorpay_payment_id", paymentId },
                    { "razorpay_signature", signature }
                };

                Utils.verifyPaymentSignature(attributes);

                // ✅ Already strings, safe to log
                _logger.LogInformation("Payment verified: OrderId={OrderId}, PaymentId={PaymentId}",
                    orderId, paymentId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment verification failed: OrderId={OrderId}", orderId ?? "N/A");
                return false;
            }
        }
    }
}
