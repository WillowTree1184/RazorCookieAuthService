# RazorCookieAuthService
A namespace to login, logout and get token in razor.

## How to use?
Fiest of all, we should to add these service to DI Containers and initialize the Checksum service and configue the AuthController.

``` c sharp
//In Program.cs
//Add services to DI
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthController>();

//Initialize the Checksum service
int checksumUpdateInterval = 60000;
Checksum.Initialize(checksumUpdateInterval);

//Comfigue the AuthController
AuthController.loginPageURL = "/login";
```
