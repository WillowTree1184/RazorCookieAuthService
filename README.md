# RazorCookieAuthService
A namespace for the Razor server to login, logout, and get the token written in the cookie.

There was a small accident when I uploaded it. So if you find a problem with the code, please let me know as soon as possible. Thank you very much!

Don't worry about security, I'm very concerned about it. I have updated it and will continue to do so.

> [!IMPORTANT]
> This namespace does not include a login verification part, the login function just writes the token into the cookie. You have to develop this yourself.

## Quick Start
First, add the `Microsoft.AspNetCore.Authorization` nuget package to your project because the AuthService depends on it.

Then, Copy the `CookieAuthService` floder into your project.

Finally, inject AuthService to DI Containers, initialize the ChecksumService, AuthController and TokenDictionaryService, here is an example:

``` c sharp
//In Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

//Here
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthController>();
ChecksumService.ChecksumUpdated += (last, now) => Console.WriteLine($"{DateTime.Now} [ChecksumService] Updated! New checksum is {now}");
ChecksumService.Initialize(60000);
AuthController.Initialize("/", "/login", new KeyValuePair<string, string>("error", "登录超时"), new KeyValuePair<string, string>("error", "Token错误"));
TokenDictionaryService.TokenRegisted += (token, tokenId) => Console.WriteLine($"{DateTime.Now} [TokenDictionaryService] Token {token} has been registed a corresponding tokenId {tokenId}");

var app = builder.Build();

// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
app.UseHsts();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.MapControllers();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

//Here - And it must here
app.UseAuthentication();
app.UseAuthorization();

app.Run();
```

As you see, it's as easy as a pie. Let me tell you some details what we just did.

### Details
Now that you've simply deployed CookieAuthService. There Are 3 services and a tool in CookieAuthService, they are:

- AuthService, inluded [IAuthService.cs](CookieAuthService/IAuthService.cs), [AuthService.cs](CookieAuthService/AuthService.cs) and [AuthController.cs](CookieAuthService/AuthController.cs).
- ChecksumService, included [ChecksumService.cs](CookieAuthService/ChecksumService.cs), a static class
- TokenDictionaryService, included [TokenDictionaryService.cs](CookieAuthService/TokenDictionaryService.cs), a static class
- SHA256Calculator, included [SHA256Calculator.cs](CookieAuthService/SHA256Calculator.cs), a static class

It's easy to deploy the AuthService, you just need to inject It to DI Containers. AuthService depend on HttpContextAccessor, so you must inject it too. You need to configure the AuthController. I have encapsulated a function to help you do that quickly, The definition of it is:

``` c sharp
public static void Initialize(string indexPageURL, string loginPageURL, KeyValuePair<string, string> queryWhenTimeOut, KeyValuePair<string, string> queryWhenTokenIdError)
```

It's a bit hard to discribe these parameters, I have wrote a summary about it, you can see it in [AuthController.cs](CookieAuthService/AuthController.cs). Here is a list about it:

- indexPageURL: The url which will be redirected to when login successful
- loginPageURL: The url which will be redirected to when login unsuccessful or logout successful
- queryWhenTimeOut: The query which will been added when login time out
- queryWhenTokenIdError: The query which will been added if the tokenId is unregisted when login

Hope you understand it :grin:.

For ChecksumService, the checksum always update. but how often? I also encapsulated a function, The definition of it is:

``` c sharp
public static void Initialize(int updateInterval)
```

The `updateInterval` defined the interval between checksum updates, it is measured in milliseconds. The checksum will be generated in each update, a single checksum is valid twice as long as updateInterval. Each update also triggers an event called `ChecksumUpdated`, the sender of it is the last checksum, and e is the new checksum. Both checksums are valid.

There nothing to deploy for TokenDictionaryService. But there are a event called TokenRegisted, it occurs when token get registed. The sender of it is the token, and e it the tokenId which corresponding to the token.

## Configrue your project
In order to make it work, you need to make some configuration to your project.

Add these code into `_Imports.razor`:

``` c sharp
@using Microsoft.AspNetCore.Components.Authorization
@using Microsoft.AspNetCore.Authentication
@using Microsoft.AspNetCore.Authorization
@using CookieAuthService
```

In order to redirect to the login page when the user is not logged in, configure `app.razor` and create `JumpToLogin.razor`

``` c sharp
//app.razor
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
//JumpToLogin.razor
@inject NavigationManager navigation

@code {
    protected override void OnAfterRender(bool firstRender)
    {
        navigation.NavigateTo("/login", false, true);
    }
}

```

Set attribute to your page which you want users can't browse without authentication:

``` c sharp
//Your page (.razor file)
@attribute [Authorize]
```

## How to use?
Inject `IAuthService` to add AuthService

``` c sharp
//Your page (.razor file)
@inject IAuthService authService
```

Then, you can use the functions in interface `IAuthService`, they are:
- `void Login(string token)`: Login with the token. You can also serialize your structs or classes to JSON as token. And the token will be written in the cookie.
- `void Logout()`: Logout. And the cookie will be deleted.
- `Task<string?> GetTokenAsync();`: Get token if user already loggin.
