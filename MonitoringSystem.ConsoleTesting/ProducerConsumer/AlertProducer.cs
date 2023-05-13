using System.Threading.Tasks;

namespace MonitoringSystem.ConsoleTesting.ProducerConsumer; 

public class AlertProducer {
    public static async Task Main() {
        /*Dictionary<Type, string> collectionNames = new Dictionary<Type, string>();
        collectionNames.Add(typeof(MonitorAlert), "alert_items");
        collectionNames.Add(typeof(ActionItem), "action_items");
        collectionNames.Add(typeof(AnalogChannel), "analog_items");
        collectionNames.Add(typeof(DiscreteChannel), "discrete_items");
        collectionNames.Add(typeof(VirtualChannel), "virtual_items");
        collectionNames.Add(typeof(OutputItem), "output_items");

        collectionNames.Add(typeof(AnalogReading), "analog_readings");
        collectionNames.Add(typeof(DiscreteReading), "discrete_readings");
        collectionNames.Add(typeof(VirtualReading), "virtual_readings");
        collectionNames.Add(typeof(OutputReading), "output_readings");
        collectionNames.Add(typeof(AlertReading), "alert_readings");
        collectionNames.Add(typeof(ActionReading), "action_readings");

        collectionNames.Add(typeof(DeviceReading), "device_readings");
        collectionNames.Add(typeof(MonitorDevice), "device_items");*/

        /*var repo = new MonitorDataService("mongodb://172.20.3.41","epi2_data", collectionNames);
        await repo.LoadAsync();
        var itemAlerts = repo.MonitorAlerts.Select(alert => new AlertRecord(alert, ActionType.Okay)).ToList();
        DateTime now = DateTime.Now;

        EndpointConvention.Map<EmailContract>(new Uri("rabbitmq://172.20.3.28:5672/email/email_processing"));
        var busControl = Bus.Factory.CreateUsingRabbitMq( cfg=> {
            cfg.Host(new Uri("rabbitmq://172.20.3.28:5672/"), host => {
                host.Username("setiadmin");
                host.Password("Sens0r20471#!");
            });
        });

        var source = new CancellationTokenSource(TimeSpan.FromSeconds(10));

        await busControl.StartAsync(source.Token);
        try {
            while (true) {
                var alertRecord=itemAlerts.FirstOrDefault(e => e.ChannelId==85);
                if (alertRecord.ChannelReading == 1) {
                    alertRecord.ChannelReading = 0;
                    alertRecord.CurrentState = ActionType.Okay;
                } else {
                    alertRecord.CurrentState = ActionType.Alarm;
                    alertRecord.ChannelReading = 1;
                }
                /*string value = await Task.Run(() => {
                    Console.WriteLine("Press Enter To Send Alerts");
                    Console.Write(">");
                    return Console.ReadLine();
                });
                if ("quit".Equals(value, StringComparison.OrdinalIgnoreCase)) {
                    break;
                }#1#
                await busControl.Send<EmailContract>(new {TimeStamp=now,Alerts=itemAlerts});
                Console.WriteLine("Alert Sent");
                await Task.Delay(1500);
            }
        } finally {
            await busControl.StopAsync();
        }*/
    }
}