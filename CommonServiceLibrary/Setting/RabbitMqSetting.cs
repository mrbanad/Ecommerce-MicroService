﻿namespace CommonServiceLibrary.Setting;

public class RabbitMqSetting
{
    public string HostName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string RetryCount { get; set; } = null!;
}