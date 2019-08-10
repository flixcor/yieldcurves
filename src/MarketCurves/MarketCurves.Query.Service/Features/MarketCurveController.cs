using Common.Core;
using Common.Infrastructure.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketCurves.Query.Service.Features
{
    [Route("api")]
    [ApiController]
    public class MarketCurveController : ControllerBase
    {
        private readonly IRequestMediator _requestMediator;
        private readonly IFileProvider _fileProvider;

        public MarketCurveController(IRequestMediator requestMediator, IFileProvider fileProvider)
        {
            _requestMediator = requestMediator ?? throw new ArgumentNullException(nameof(requestMediator));
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        }

        // GET api/values
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions),nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<IEnumerable<GetMarketCurveList.Dto>>> Get()
        {
            var result = await _requestMediator.Send(new GetMarketCurveList.Query());
            var script = GetUrlToScript("get-marketcurves");

            var component = FrontendComponent.Create(result, script);
            return Ok(component);
        }

        [HttpGet("{id}")]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        public async Task<ActionResult<GetMarketCurve.Dto>> Get(Guid id)
        {
            var result = await _requestMediator.Send(new GetMarketCurve.Query { Id = id });
            var script = GetUrlToScript("get-marketcurve");

            return result.ToComponentActionResult(script);
        }

        private string GetUrlToScript(string fileNamePart)
        {
            var contents = _fileProvider.GetDirectoryContents("wwwroot");
            var fileName = contents
                .AsEnumerable()
                .Where(x=> x.Name.Contains($"{fileNamePart}."))
                .OrderByDescending(f => f.LastModified)
                .Select(x=> x.Name)
                .FirstOrDefault();

            return fileName == null
                ? null
                : $"{BaseUrl}/{fileName}";
        }

        private string BaseUrl => $"{Request.Scheme}://{Request.Host}{Request.PathBase}";
    }
}
