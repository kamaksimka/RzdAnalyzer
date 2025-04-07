using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RZD.Application.Models;
using RZD.Application.Services;
using RZD.Common.Enums;
using RZD.Database.Models;
using System.Security.Claims;

namespace RZD.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly FeedbackService _feedbackService;

        public FeedbackController(FeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task CreateAsync([FromBody]CreateFeedbackRequest request)
            => await _feedbackService.CreateAsync(request, User);

        [HttpGet("all")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<List<FeedbackModel>> GetAllAsync()
            => await _feedbackService.GetAllAsync();


    }
}
