// private readonly ILogger<AuthenticationHttpClient> logger;
// private readonly HttpClient http;
// private readonly ITokenService tokenService;
// private readonly CustomAuthenticationStateProvider myAuthenticationStateProvider;
//
// public AuthenticationHttpClient(ILogger<AuthenticationHttpClient> logger,
//     HttpClient http,
//     ITokenService tokenService,
//     CustomAuthenticationStateProvider myAuthenticationStateProvider)
// {
//     this.logger = logger;
//     this.http = http;
//     this.tokenService = tokenService;
//     this.myAuthenticationStateProvider = myAuthenticationStateProvider;
// }
//
// public async Task<UserLoginResultDTO> LoginUser(UserLoginDTO userLoginDTO)
// {
//     try
//     {
//         var response = await http.PostAsJsonAsync("user/login", userLoginDTO);
//         var result = await response.Content.ReadFromJsonAsync<UserLoginResultDTO>();
//         await tokenService.SetToken(result.Token);
//         myAuthenticationStateProvider.StateChanged();
//         return result;
//     }
//     catch (Exception ex)
//     {
//         logger.LogError(ex.Message);
//
//         return new UserLoginResultDTO
//         {
//             Succeeded = false,
//             Message = "Sorry, we were unable to log you in at this time. Please try again shortly."
//         };
//     }
// }

