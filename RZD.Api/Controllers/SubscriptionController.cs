using Microsoft.AspNetCore.Mvc;
using RZD.Application.Models;
using RZD.Application.Services;
using System.Security.Claims;

namespace RZD.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionService _subscriptionService;

        public SubscriptionController(SubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("subscriptions")]
        public async Task<List<SubscriptionModel>> GetSubscriptions() 
            => await _subscriptionService.GetSubscriptionsAsync(User);

        [HttpPost("create")]
        public async Task CreateAsync(PickUpTrainRequest request)
            => await _subscriptionService.CreateSubscriptionAsync(request, User);
    }
}
