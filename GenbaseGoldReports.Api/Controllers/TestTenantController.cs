using GenbaseGoldReports.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GenbaseGoldReports.Api.Controllers
{
    [ApiController]
    [Route("api/test-tenant")]
    public class TestTenantController : ControllerBase
    {
        private readonly ITenantContext _tenantContext;
        private readonly IDbConnectionFactory _connectionFactory;

        public TestTenantController(ITenantContext tenantContext, IDbConnectionFactory connectionFactory)
        {
            _tenantContext = tenantContext;
            _connectionFactory = connectionFactory;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentTenant()
        {
            if (string.IsNullOrEmpty(_tenantContext.ConnectionString))
            {
                return BadRequest(new { Message = "Tenant context is not resolved or invalid." });
            }

            // Validate actual connectivity to the dynamic client database via Dapper connection factory
            using var connection = _connectionFactory.CreateConnection();
            connection.Open();
            var databaseName = connection.Database;

            return Ok(new
            {
                TenantId = _tenantContext.TenantId,
                Identifier = _tenantContext.Identifier,
                ResolvedDatabase = databaseName,
                Status = "Successfully connected to tenant isolated database!"
            });
        }
    }
}
