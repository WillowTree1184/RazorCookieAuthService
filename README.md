# RazorCookieAuthService
A namespace for razor server to login, logout and get token which has been wrote in cookie.

There was a little accident when I uploaded it. So if you notice a problem with the code, please let me know as soon as possible. Thank you very much!

Don't worry about its security, I'm very concerned about it. I've updated its security and will continue to update it in the future.

> [!IMPORTANT]
> This namespace do not included login verification part, it just write the token into the cookie, so you have to develop the login verification part by yourself.

## Quickly deploy it to your project
First of all, add Nuget package `Microsoft.AspNetCore.Authorization` into your project, our service depend on it.

Then, Copy the `CookieAuthService` floder into your project.

Last, inject AuthService to DI Containers, initialize the ChecksumService, AuthController and TokenDictionaryService, here is an example:

``` c sharp
//In Program.cs
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthController>();
AuthController.Initialize("/", "/login", new KeyValuePair<string, string>("error", "登录超时"), new KeyValuePair<string, string>("error", "Token错误"));
ChecksumService.ChecksumUpdated += (last, now) => Console.WriteLine($"{DateTime.Now} [ChecksumService] Updated! New checksum is {now}");
ChecksumService.Initialize(60000);
TokenDictionaryService.TokenRegisted += (token, tokenId) => Console.WriteLine($"{DateTime.Now} [TokenDictionaryService] Token {token} has been registed a corresponding tokenId {tokenId}");
```

As you see, it's as easy as a pie. But, in order to make your deployment work, you also need to make some configuration to your project. Before that, let me tell you what happened.

### What happened?
Now that you've simply deployed CookieAuthService. There Are 3 services and a tool in CookieAuthService, they are:

- AuthService, inluded [IAuthService.cs](CookieAuthService/IAuthService.cs), [AuthService.cs](CookieAuthService/AuthService.cs) and [AuthController.cs](CookieAuthService/AuthController.cs).
- ChecksumService, included [ChecksumService.cs](CookieAuthService/ChecksumService.cs), a static class
- TokenDictionaryService, included [TokenDictionaryService.cs](CookieAuthService/TokenDictionaryService.cs), a static class
- SHA256Calculator, included [SHA256Calculator.cs](CookieAuthService/SHA256Calculator.cs), a static class

It's easy to deploy the AuthService, you just need to inject It to DI Containers. AuthService depend on HttpContextAccessor, so you must inject it too. But you need to configure the AuthController. So I encapsulated a function called Initialize, The definition of this function is as follows:

``` c sharp
public static void Initialize(string indexPageURL, string loginPageURL, KeyValuePair<string, string> queryWhenTimeOut, KeyValuePair<string, string> queryWhenTokenIdError)
```

It's a bit compax to discribe them, so I wrote a summary about it, you can see it in [AuthController.cs](CookieAuthService/AuthController.cs). Here is a list about it:

- indexPageURL: The url which will be redirected to when login successful
- loginPageURL: The url which will be redirected to when login unsuccessful or logout successful
- queryWhenTimeOut: The query which will been added when login time out
- queryWhenTokenIdError: The query which will been added if the tokenId is unregisted when login

Hope you understand it :grin:.

For ChecksumService, the checksum always update. but how often? I also encapsulated a function called Initialize, The definition of this function is as follows too:

``` c sharp
public static void Initialize(int updateInterval)
```

The `updateInterval` defined the interval between checksum updates, it is measured in milliseconds. A new Checksum will be generated in each update, a single checksum is valid twice as long as updateInterval. Each update also triggers an event called `ChecksumUpdated`, where the sender of this event is the previous checksum, and e is the new checksum. Both checksums are valid.

There nothing to deploy for TokenDictionaryService. But there are a event called TokenRegisted, it occurs when token get registed. The sender of it is the token, and e it the tokenId which corresponding to the token.

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
