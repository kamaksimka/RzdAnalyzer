using Microsoft.AspNetCore.Mvc;
using RZD.Application.Models;
using RZD.Application.Services;
using RZD.Database.Models;

namespace RZD.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingController: ControllerBase
    {
        private readonly TrainService _trainService;

        public TrainingController(TrainService trainService)
        {
            _trainService = trainService;
        }

        [HttpGet("freePlaces")]
        public async Task<List<TrainingFreePlacesModel>> GetTrainingFreePlacesModel(long trackedRouteId)
            => await _trainService.GetTrainingFreePlacesModel(trackedRouteId);
    }
}
