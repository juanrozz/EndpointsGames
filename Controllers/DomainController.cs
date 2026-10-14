using EndpointsPage.DTOs.Response;
using EndpointsPage.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EndpointsPage.Controllers
{
	[Route("Api")]
	[ApiController]
	public class DomainController : ControllerBase
	{
		private readonly InternsDbContext _dbContext;

		public DomainController(InternsDbContext dbContext)
		{
			_dbContext = dbContext;
		}

		[HttpGet("Domains")]
		public async Task<ActionResult> GetDomains()
		{
			var domains = await _dbContext.DomainJdrs
				.Select(d => new { d.DomainId, d.Domain })
				.ToListAsync();

			return Ok(GeneralResponseDto<object>.SuccesResult(domains, "Domains retrieved successfully"));
		}
	}
}