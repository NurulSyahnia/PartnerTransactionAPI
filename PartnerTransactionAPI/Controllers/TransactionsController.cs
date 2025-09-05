using Microsoft.AspNetCore.Mvc;
using PartnerTransactionAPI.Models;
using PartnerTransactionAPI.Services;
using PartnerTransactionAPI.Utilities;
using PartnerTransactionAPI.Validators;

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
            _logger.LogInformation("Incoming request: {Request}", LogSanitizer.Sanitize(req));

            var validation = _validator.Validate(req);
            if (!validation.IsValid)
            {
                var responseError = new SubmitTrxResponse { result = 0, resultmessage = validation.ErrorMessage };
                _logger.LogWarning("Validation failed. Response: {Response}", LogSanitizer.Sanitize(responseError));
                return BadRequest(responseError); 
            }

            var (totalDiscount, finalAmount) = DiscountService.CalculateDiscount(req.totalamount);

            var response = new SubmitTrxResponse
            {
                result = 1,
                totalamount = req.totalamount,
                totaldiscount = totalDiscount,
                finalamount = finalAmount
            };

            _logger.LogInformation("Success response: {Response}", LogSanitizer.Sanitize(response));

            return Ok(response);
        }
    }
}
