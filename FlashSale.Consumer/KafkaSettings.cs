using System;
using System.Collections.Generic;
using System.Text;

namespace FlashSale.Consumer
{
    public class KafkaSettings
    {
        public string BootstrapServers { get; set; } = "localhost:9092";
        public string Topic { get; set; } = "purchase_intents";
        public string GroupId { get; set; } = "flashsale-consumer-group";
    }

    public class MongoSettings
    {
        public string ConnectionString { get; set; } = "mongodb://localhost:27017";
        public string Database { get; set; } = "flashsale";
    }
}
