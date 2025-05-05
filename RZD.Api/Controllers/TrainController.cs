using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpPost("freePlacesPlot")]
        public async Task<Dictionary<DateTime,int>> GetFreePlacesPlot(TrainRequest request)
            => await _trainService.GetFreePlacesPlot(request);

        [HttpPost("carPlaceTypes")]
        public async Task<List<CarPlaceTypeModel>> GetCarPlaceTypes(TrainRequest request)
            => await _trainService.GetCarPlaceTypes(request);

        [HttpPost("minPricePlacesPlot")]
        public async Task<Dictionary<DateTime, decimal>> GetMinPricePlacesPlot(GetPricePlacesPlotRequest request)
            => await _trainService.GetMinPricePlacesPlot(request);

        [HttpPost("maxPricePlacesPlot")]
        public async Task<Dictionary<DateTime, decimal>> GetMaxPricePlacesPlot(GetPricePlacesPlotRequest request)
            => await _trainService.GetMaxPricePlacesPlot(request);

        [HttpPost("freePlacesByCarTypePlot")]
        public async Task<Dictionary<DateTime, int>> GetFreePlacesPlotByCarPlaceType(GetPricePlacesPlotRequest request)
            => await _trainService.GetFreePlacesPlotByCarPlaceType(request);

        [HttpPost("train")]
        public async Task<TrainModel> GetTrainAsync([FromBody] TrainRequest request)
            => await _trainService.GetTrainAsync(request);
    }
}
