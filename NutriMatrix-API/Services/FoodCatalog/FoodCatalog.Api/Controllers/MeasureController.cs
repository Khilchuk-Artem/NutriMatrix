using BuildingBlocks.Nutrionix.Refit;
using FoodCatalog.Api.Features.Measures.Queries;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Redis.OM.Modeling;
using Redis.OM.Searching;
using static Google.Protobuf.Compiler.CodeGeneratorResponse.Types;
using FoodCatalog.Application.Dto;

namespace FoodCatalog.Api.Controllers
{
    namespace FoodCatalog.Api.Models.Dto
    {
    }
    [ApiController]
    [Route("api/[controller]")]
    public class MeasureController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MeasureController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:long}", Name = "GetMeasureById")]
        public async Task<IActionResult> Get(long id)
        {
            var query = new GetMeasureByIdQuery { Id = id };
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search([FromBody] string query)
        {
            try
            {
                var searchQuery = new SearchMeasuresQuery { Query = query };
                var result = await _mediator.Send(searchQuery);
                if (result == null || !result.Any())
                {
                    return NotFound("No foods found for the given query.");
                }
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
