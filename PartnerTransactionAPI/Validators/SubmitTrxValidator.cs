using Microsoft.Extensions.Options;
using PartnerTransactionAPI.Controllers;
using PartnerTransactionAPI.Models;
using PartnerTransactionAPI.Utilities;
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
        private ValidationResult Fail(string userMessage, string logMessage)
        {
            var messageToLog = string.IsNullOrWhiteSpace(logMessage) ? userMessage : logMessage;
            _logger.LogWarning(messageToLog);
            return ValidationResult.Fail(userMessage);
        }
        public ValidationResult Validate(SubmitTrxRequest req)
        {
            if (string.IsNullOrEmpty(req.partnerkey)) 
                return Fail("Partner Key is Required.","");
            
            if (string.IsNullOrEmpty(req.partnerrefno))
                return Fail("Partner Ref No is Required.", "");

            if (string.IsNullOrEmpty(req.partnerpassword))
                return Fail("Partner Password is Required.", "");

            if (req.totalamount <= 0)
                return Fail("Invalid Total Amount.", "");

            if (string.IsNullOrEmpty(req.timestamp))
                return Fail("Timestamp is Required.", "");

            if (string.IsNullOrEmpty(req.sig))
                return Fail("Signature is Required.", "");

            if (req.items != null && req.items.Count > 0)
            {
                foreach (var item in req.items)
                {
                    if (string.IsNullOrEmpty(item.partneritemref))
                        return Fail("Partner Item Ref is Required.", "");

                    if (string.IsNullOrEmpty(item.name))
                        return Fail("Name is Required.", "");

                    if (item.qty <= 0 || item.qty > 5)
                        return Fail("Invalid Qty for item.", "");

                    if (item.unitprice <= 0)
                        return Fail("Invalid Unit Price for item.", "");
                }

                var sumItems = req.items.Sum(i => i.qty * i.unitprice);
                if (sumItems != req.totalamount)
                    return Fail("Invalid Total Amount.", "The total value stated in itemDetails array not equal to value in totalamount.");
            }

            if (!DateTime.TryParse(req.timestamp, out var ts))
                Fail("Invalid Timestamp format.", "");

            var utcNow = DateTime.UtcNow;
            if (Math.Abs((utcNow - ts).TotalMinutes) > 5)
                return Fail("Expired.", "Provided timestamp exceed server time +-5min.");

            // Validate partner info from config
            var partnerInfo = _allowedPartners.FirstOrDefault(p =>
                p.PartnerRefNo == req.partnerrefno &&
                p.PartnerKey == req.partnerkey
            );
            if (partnerInfo == null)
                return Fail("Access Denied!", "Unauthorized Partner!");

            //validate partner password
            var passwordBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(partnerInfo.PartnerPassword));
            if (passwordBase64 != req.partnerpassword)
                return Fail("Access Denied!", "Partner Password Validation failed");

            // validate signature
            var sigExpected = SignatureHelper.GenerateSignature(
                DateTime.Parse(req.timestamp).ToString("yyyyMMddHHmmss"),
                req.partnerkey,
                req.partnerrefno,
                req.totalamount,
                req.partnerpassword
            );

            if (sigExpected != req.sig)
                return Fail("Access Denied.", $"Signature Validation failed, Expected Signature : [{sigExpected}], Request Signature : [{req.sig}]");

            return ValidationResult.Success();
        }
    }
}
