using Microsoft.Extensions.Configuration;

namespace MonitoringData.Infrastructure.MongoConfiguration;

public class MongoConfigurationSource:IConfigurationSource {

    public IConfigurationProvider Build(IConfigurationBuilder builder) =>
        new MongoConfigurationProvider("mongodb://172.20.3.41");
}