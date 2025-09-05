using Microsoft.AspNetCore.Mvc;
using PartnerTransactionAPI.Models;
using PartnerTransactionAPI.Services;
using PartnerTransactionAPI.Utilities;
using PartnerTransactionAPI.Validators;
using System.Text.Json;

namespace PartnerTransactionAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ILogger<TransactionsController> _logger;
        private readonly SubmitTrxValidator _validator;
        public TransactionsController(ILogger<TransactionsController> logger, SubmitTrxValidator validator)
        {
            _logger = logger;
            _validator = validator;
        }

        [HttpPost]
        public IActionResult Post([FromBody] SubmitTrxRequest req)
        {
            //_logger.LogInformation("Request: {Request}", JsonSerializer.Serialize(req));
            //_logger.LogInformation("Incoming request: {@Request}", req);
            _logger.LogInformation("Incoming request: {Request}", LogSanitizer.Sanitize(req));

            var error = _validator.Validate(req);
            if (error != null)
            {
                var responseError = new SubmitTrxResponse { result = 0, resultmessage = error };
                //_logger.LogWarning("Validation failed. Response: {@Response}", responseError);
                _logger.LogWarning("Validation failed. Response: {Response}", LogSanitizer.Sanitize(responseError));
                return Ok(responseError);
            }

            var (totalDiscount, finalAmount) = DiscountService.CalculateDiscount(req.totalamount);

            var response = new SubmitTrxResponse
            {
                result = 1,
                totalamount = req.totalamount,
                totaldiscount = totalDiscount,
                finalamount = finalAmount
            };

            //_logger.LogInformation("Response: {Response}", JsonSerializer.Serialize(response));
            //_logger.LogInformation("Success response: {@Response}", response);
            _logger.LogInformation("Success response: {Response}", LogSanitizer.Sanitize(response));

            return Ok(response);
        }
    }
}
