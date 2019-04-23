using MassTransit;
using MassTransit.RabbitMqTransport;
using System;

namespace BusHelper
{
    public static class BusFactory
    {
        public static IBusControl CreateRabbitMq(Action<IRabbitMqHost,IRabbitMqBusFactoryConfigurator> configureCallback=null)
        {
            return Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                var host = sbc.Host(new Uri("rabbitmq://localhost:5672/"), h =>
                {
                    h.Username("admin");
                    h.Password("test");
                });

                configureCallback?.Invoke(host,sbc);
            });
        }
    }
}
