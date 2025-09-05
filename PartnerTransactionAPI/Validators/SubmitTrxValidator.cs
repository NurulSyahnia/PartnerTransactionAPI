using Microsoft.Extensions.Options;
using PartnerTransactionAPI.Controllers;
using PartnerTransactionAPI.Models;
using PartnerTransactionAPI.Utilities;
using System;
using System.Linq;
using System.Text;

namespace PartnerTransactionAPI.Validators
{
    public class SubmitTrxValidator
    {
        private readonly ILogger<TransactionsController> _logger;
        private readonly List<PartnerConfig> _allowedPartners;

        public SubmitTrxValidator(ILogger<TransactionsController> logger, IOptions<List<PartnerConfig>> allowedPartners)
        {
            _logger = logger;
            _allowedPartners = allowedPartners.Value;
        }

        public string Validate(SubmitTrxRequest req)
        {
            if (string.IsNullOrEmpty(req.partnerkey)) return "Partner Key is Required.";
            if (string.IsNullOrEmpty(req.partnerrefno)) return "Partner Ref No is Required.";
            if (string.IsNullOrEmpty(req.partnerpassword)) return "Partner Password is Required.";
            if (req.totalamount <= 0) return "Invalid Total Amount.";

            if (req.items != null && req.items.Count > 0)
            {
                foreach (var item in req.items)
                {
                    if (string.IsNullOrEmpty(item.partneritemref)) return "Partner Item Ref is Required.";
                    if (string.IsNullOrEmpty(item.name)) return "Name is Required.";
                    if (item.qty <= 0 || item.qty > 5) return "Invalid Qty for item.";
                    if (item.unitprice <= 0) return "Invalid Unit Price for item.";
                }

                var sumItems = req.items.Sum(i => i.qty * i.unitprice);
                if (sumItems != req.totalamount) return "Invalid Total Amount.";
            }

            //if (!DateTime.TryParse(req.timestamp, out var ts)) return "Invalid Timestamp format.";
            //var utcNow = DateTime.UtcNow;
            //if (Math.Abs((utcNow - ts).TotalMinutes) > 5) return "Expired.";

            // Validate partner info from config
            var partnerInfo = _allowedPartners.FirstOrDefault(p =>
                p.PartnerRefNo == req.partnerrefno &&
                p.PartnerKey == req.partnerkey
            );
            if (partnerInfo == null) return "Access Denied!";

            var passwordBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(partnerInfo.PartnerPassword));
            if (passwordBase64 != req.partnerpassword) return "Access Denied!";

            // Compute signature
            var sigExpected = SignatureHelper.GenerateSignature(
                DateTime.Parse(req.timestamp).ToString("yyyyMMddHHmmss"),
                req.partnerkey,
                req.partnerrefno,
                req.totalamount,
                req.partnerpassword
            );

            if (sigExpected != req.sig) return "Access Denied.";

            return null;
        }
    }
}
