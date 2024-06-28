# RazorCookieAuthService
A namespace for razor to login, logout and get token which has been wrote in cookie.

> [!IMPORTANT]
> This namespace do not included login verification part, it just write the token into the cookie, so you have to develop the login verification part by yourself.

## Quickly deploy it to your project
First of all, add Nuget package `Microsoft.AspNetCore.Authorization` into your project, our service depend on it.

Then, Copy the `CookieAuthService` floder into your project.

Last, add these service to DI Containers, initialize the ChecksumService, AuthController and TokenDictionaryService, here is an example:

``` c sharp
//In Program.cs
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthController>();
ChecksumService.ChecksumUpdated += (last, now) => Console.WriteLine($"{DateTime.Now} [ChecksumService] Updated! New checksum is {now}");
ChecksumService.Initialize(60000);
AuthController.Initialize("/", "/login", new KeyValuePair<string, string>("error", "登录超时"), new KeyValuePair<string, string>("error", "Token错误"));
TokenDictionaryService.TokenRegisted += (token, tokenId) => Console.WriteLine($"{DateTime.Now} [TokenDictionaryService] Token {token} has been registed a corresponding tokenId {tokenId}");
```

As you see, it's as easy as a pie.

## Configrue your project
Add namespace to `_Imports.razor`

``` c sharp
//In _Imports.razor
@using CookieAuthService
```

Configure `app.razor` and create `JumpToLogin.razor`

``` c sharp
//In app.razor
<Router AppAssembly="@typeof(App).Assembly">
    <Found Context="routeData">
        <AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
            <NotAuthorized>
                <JumpToLogin></JumpToLogin>
            </NotAuthorized>
        </AuthorizeRouteView>
    </Found>
    <NotFound>
        <PageTitle>Not found</PageTitle>
        <LayoutView Layout="@typeof(MainLayout)">
            <p role="alert">Sorry, there's nothing at this address.</p>
        </LayoutView>
    </NotFound>
</Router>
```

``` c sharp
//In JumpToLogin.razor
@inject NavigationManager navigation

@code {
    protected override void OnAfterRender(bool firstRender)
    {
        navigation.NavigateTo("/login");
    }
}

```

Set attribute to your page which you want users can't browse without authentication

``` c sharp
//In your page
@attribute [Authorize]
```

## How to use?
Inject `IAuthService`

``` c sharp
//In your page
@inject IAuthService authService
```

Functions in interface `IAuthService`:
- `Task LoginAsync(string token)`: Login with the token. You can also serialize your structs or classes to JSON as token. The token will be written in the cookie.
- `Task LogoutAsync()`: Logout. The cookie will be deleted.
- `Task<string> GetTokenAsync()`: Get token if you already loggin. Or you can use `(await authenticationStateProvider.GetAuthenticationStateAsync()).User.FindFirst(c => c.Type == "token").Value;`.
