﻿namespace Hamkare.Services;

public interface ITokenService
{
    Task<string> GetToken();
    Task RemoveToken();
    Task SetToken(string token);
}