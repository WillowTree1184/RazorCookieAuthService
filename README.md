# RazorCookieAuthService
A namespace to login, logout and get token in razor.

## How to use?
Fiest of all, we should to add these service to DI Containers, initialize the Checksum service and configure the AuthController.

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

//Configure the AuthController
AuthController.loginPageURL = "/login";
```

Next, add Nuget package `Microsoft.AspNetCore.Authorization` into your project.

Finally, adjust the code to your wishes.

Done! As you see, it's as easy as a pie.
