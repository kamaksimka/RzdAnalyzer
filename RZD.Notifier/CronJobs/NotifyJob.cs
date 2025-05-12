using Quartz;
using RZD.Application.Services;
using RZD.MailSender;
using RZD.MailSender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RZD.Notifier.CronJobs
{
    [DisallowConcurrentExecution]
    public class NotifyJob : IJob
    {
        private readonly SubscriptionService _notifyService;
        private readonly MailSender.MailSender _mailSender;

        public NotifyJob(SubscriptionService notifyService, MailSender.MailSender mailSender)
        {
            _notifyService = notifyService;
            _mailSender = mailSender;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var subscriptions = await _notifyService.GetSubscriptionsAsync();

            foreach (var sub in subscriptions)
            {
                var trains = await _notifyService.GetTrainsForSubsctription(sub);

                if (trains.Any())
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"<h2>Найдены поезда по вашему маршруту:</h2>");
                    sb.AppendLine($"<p><b>Маршрут:</b> {sub.OriginStationName} - {sub.DestinationStationName}</p>");
                    sb.AppendLine("<ul>");

                    foreach (var train in trains)
                    {
                        sb.AppendLine("<li>");
                        sb.AppendLine($"<b>Поезд:</b> {train.TrainNumber} ({train.TrainDescription})<br>");
                        sb.AppendLine($"<b>Отправление:</b> {train.DepartureDateTime}<br>");
                        sb.AppendLine($"<b>Прибытие:</b> {train.ArrivalDateTime}<br>");
                        sb.AppendLine($"<b>Длительность:</b> {train.TripDuration}<br>");
                        sb.AppendLine("</li>");
                    }

                    sb.AppendLine("</ul>");
                    sb.AppendLine("<p>Спасибо, что пользуетесь нашим сервисом!</p>");

                    var message = new EmailMessage
                    {
                        To = sub.UserEmail,
                        Subject = $"Найдено {trains.Count} поезд(ов) для маршрута {sub.OriginStationName} - {sub.DestinationStationName}",
                        Body = sb.ToString(),
                        IsHtml = true
                    };

                    _mailSender.SendEmail(message);
                }
            }
        }
    }
}
