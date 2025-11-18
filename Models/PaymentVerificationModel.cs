namespace DoctorateDrive.Models
{
    public class PaymentVerificationModel
    {
        public string OrderId { get; set; }
        public string PaymentId { get; set; }
        public string Signature { get; set; }
    }
}
