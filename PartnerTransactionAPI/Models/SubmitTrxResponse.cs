﻿namespace PartnerTransactionAPI.Models
{
    public class SubmitTrxResponse
    {
        public int result {  get; set; }
        public long totalamount { get; set; }
        public long totaldiscount { get; set; }
        public long finalamount { get; set; }
        public string resultmessage { get; set; }
    }
}
