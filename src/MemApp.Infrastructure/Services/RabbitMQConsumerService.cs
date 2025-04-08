using MemApp.Application.Interfaces;
using MemApp.Application.Models.DTOs;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace MemApp.Infrastructure.Services
{
    public class RabbitMQConsumerService : BackgroundService
    {
        private readonly IConnection _rabbitMQConnection;
        private readonly IModel _rabbitMQChannel;

        private readonly IBroadcastHandler _broadcastHandler;


        public RabbitMQConsumerService(IConnection rabbitMQConnection, IBroadcastHandler broadcastHandler)
        {
            _rabbitMQConnection = rabbitMQConnection;
            _rabbitMQChannel = _rabbitMQConnection.CreateModel();
            _rabbitMQChannel.QueueDeclare("Venue", exclusive: false);
            _rabbitMQChannel.QueueDeclare("SSlCommerzValidatorSMS", exclusive: false);
            _rabbitMQChannel.QueueDeclare("SSlCommerzValidatorMail", exclusive: false);
            _rabbitMQChannel.QueueDeclare("VenueBookingMail", exclusive: false);

            _broadcastHandler = broadcastHandler;


        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var consumer = new EventingBasicConsumer(_rabbitMQChannel);
                consumer.Received += async (model, ea) =>
                {

                    var body = ea.Body.ToArray();
                    var data = Encoding.UTF8.GetString(body);

                    if (ea.RoutingKey == "Venue")
                    {
                        await ProcessVenueDataAsync(data);
                        var SMSInfo = JsonConvert.SerializeObject(data);
                        Log.Information("Venue : " + SMSInfo);
                    }
                    else if (ea.RoutingKey == "VenueBookingMail")
                    {
                        var emailInfo = JsonConvert.DeserializeObject<EmailInfo>(data);
                        Log.Information("SSlCommerzValidatorMail : " + JsonConvert.SerializeObject(emailInfo));
                        await _broadcastHandler.SendEmail(emailInfo.MailId, emailInfo.MailSubject, emailInfo.MailBody);
                    }
                    else if (ea.RoutingKey == "SSlCommerzValidatorSMS")
                    {
                        var SMSInfo = JsonConvert.DeserializeObject<SMSInfo>(data);
                        Log.Information("SSlCommerzValidatorSMS : " + JsonConvert.SerializeObject(SMSInfo));
                        await _broadcastHandler.SendSms(SMSInfo.Phone, SMSInfo.Message, "English");
                    }
                    else if (ea.RoutingKey == "SSlCommerzValidatorMail")
                    {
                        var emailInfo = JsonConvert.DeserializeObject<EmailInfo>(data);
                        Log.Information("SSlCommerzValidatorMail : " + JsonConvert.SerializeObject(emailInfo));
                        await _broadcastHandler.SendEmail(emailInfo.MailId, emailInfo.MailSubject, emailInfo.MailBody);
                    }

                    _rabbitMQChannel.BasicAck(ea.DeliveryTag, false);
                };

                _rabbitMQChannel.BasicConsume(queue: "Venue", autoAck: false, consumer: consumer);
                _rabbitMQChannel.BasicConsume(queue: "SSlCommerzValidatorSMS", autoAck: false, consumer: consumer);
                _rabbitMQChannel.BasicConsume(queue: "SSlCommerzValidatorMail", autoAck: false, consumer: consumer);


                await Task.Delay(5000, stoppingToken); // Delay for 1 second before checking for new messages
            }
        }

        private async Task ProcessVenueDataAsync(string productData)
        {
            // Implement logic to process and save the product data
            // Use dependency injection to get required services

            var data = JsonConvert.DeserializeObject<VenueBookingMessage>(productData);
            string message = "";
            string subject = "";
            if (data.Phone != null)
            {
                message = "Venue Booking Amount : " + data.Amount + ", Vat Amount : " + data.VatAmount + ", Service Charge Amount : " + data.ServiceAmount;
                await _broadcastHandler.SendSms(data.Phone, message, "English");
                message = "";
            }


        }
        //private async Task ProcessItemDataAsync(string itemData)
        //{
        //    // Implement logic to process and save the product data
        //    // Use dependency injection to get required services
        //    using (var scope = _serviceProvider.CreateScope())
        //    {
        //        var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
        //        var deserializedProduct = JsonConvert.DeserializeObject<Item>(itemData);
        //        productService.AddItem(deserializedProduct);
        //    }
        //}
    }
}