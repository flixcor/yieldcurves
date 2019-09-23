using System.Threading.Tasks;
using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CalculationEngine.Query.Service.Features.Common
{
    [ApiController]
    [Route("api")]
    public class Controller : ControllerBase
    {
        private readonly IRequestMediator _requestMediator;

        public Controller(IRequestMediator requestMediator)
        {
            _requestMediator = requestMediator;
        }

        [HttpGet]
        public async Task<IActionResult> Handle([FromQuery] GetCalculatedCurveDetail.Query query)
        {
            var result = await _requestMediator.Send(query);
            return this.ComponentActionResult(result, "get-calculated-curve-detail");
        }
    }
}
