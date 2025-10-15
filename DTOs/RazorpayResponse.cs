namespace ShareVolt.DTOs
{
    public class RazorpayResponse
    {
        public int bookingId { get; set; }
        public string razorpay_payment_id { get; set; }
        public string razorpay_order_id { get; set; }
        public string razorpay_signature { get; set; }
    }
}
