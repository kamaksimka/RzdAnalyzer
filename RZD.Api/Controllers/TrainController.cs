using Microsoft.AspNetCore.Mvc;
using RZD.Application.Models;
using RZD.Application.Services;

namespace RZD.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainController
    {
        private readonly TrainService _trainService;

        public TrainController(TrainService trainService)
        {
            _trainService = trainService;
        }

        [HttpPost("trains")]
        public async Task<List<TrainTableModel>> GetTrains([FromBody] GetTrainsRequest request)
            => await _trainService.GetTrainsByTrackedRouteId(request);

        [HttpPost("trainGridInitModel")]
        public async Task<TrainGridInitModel> GetTrainGridInitModel(TrainGridInitRequest request)
            => await _trainService.GetTrainGridInitModel(request);
    }
}
