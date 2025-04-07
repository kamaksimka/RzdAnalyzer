using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RZD.Application.Models;
using RZD.Application.Services;
using RZD.Common.Enums;

namespace RZD.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrackedRouteController : ControllerBase
    {
        private readonly TrackedRouteService _trackedRouteService;

        public TrackedRouteController(TrackedRouteService trackedRouteService)
        {
            _trackedRouteService = trackedRouteService;
        }

        [HttpGet("all")] 
        public async Task<List<TrackedRouteModel>> GetAll() 
            => await _trackedRouteService.GetAllAsync();

        [HttpPost("create")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task Create([FromBody] CreateTrackedRouteRequest request)
            => await _trackedRouteService.CreateAsync(request);

        [HttpPost("delete")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task Delete([FromBody] DeleteTrackedRouteRequest request)
            => await _trackedRouteService.DeleteAsync(request);
    }
}
