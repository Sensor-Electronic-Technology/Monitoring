﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MonitoringData.Infrastructure.Services;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringSystem.Shared.Contracts;

namespace MonitoringSystem.ConsoleTesting {
    public class TestAlertConsumer {

        public static async Task Main() {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg => {
                cfg.Host(new Uri("rabbitmq://172.20.3.28:5672/"), host => {
                    host.Username("setiadmin");
                    host.Password("Sens0r20471#!");
                });
                cfg.ReceiveEndpoint("email_processing", e => {
                    e.Consumer<EmailConsumer>();
                });
            });

            var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

            await busControl.StartAsync(source.Token);

            try {
                Console.WriteLine("Press enter to exit");
                await Task.Run(() => Console.ReadLine());
            } finally {
                await busControl.StopAsync();
            }
        }

    }

    public class EmailConsumer : IConsumer<EmailContract> {

        private readonly IEmailService _emailService;

        public EmailConsumer() {
            this._emailService = new EmailService();
        }

        public async Task Consume(ConsumeContext<EmailContract> context) {
            await this._emailService.SendMessageAsync(context.Message.Subject, context.Message.Message);
        }
    }
}