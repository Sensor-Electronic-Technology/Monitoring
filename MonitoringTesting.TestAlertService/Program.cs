// See https://aka.ms/new-console-template for more information
using ConsoleTables;
using MassTransit;
using MonitoringData.Infrastructure.Services.AlertServices;
using MonitoringSystem.Shared.Contracts;
namespace MonitoringTesting.TestAlertService;
public class Program {
    static async Task Main(string[] args) {
        Console.WriteLine("Starting EmailConsumer");
        await TestAlertBus();
    }
    static async Task TestAlertBus() {
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
    /*private readonly IEmailService _emailService;*/
    public EmailConsumer() {
        /*this._emailService = new EmailService();*/
    }
    public async Task Consume(ConsumeContext<EmailContract> context) {
        Console.Clear();
        ConsoleTable table = new ConsoleTable("Alert","Status","Reading");
        foreach (var alert in context.Message.Alerts) {
            table.AddRow(alert.DisplayName, alert.CurrentState, alert.ChannelReading.ToString());
        }
        Console.WriteLine(table.ToString());
        //await this._emailService.SendMessageAsync(context.Message.Subject, context.Message.Message);
    }
}