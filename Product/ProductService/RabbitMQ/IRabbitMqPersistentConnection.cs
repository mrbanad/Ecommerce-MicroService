﻿using RabbitMQ.Client;

namespace ProductService.RabbitMQ;

public interface IRabbitMqPersistentConnection
    : IDisposable
{
    bool IsConnected { get; }

    bool TryConnect();

    IModel CreateModel();
}