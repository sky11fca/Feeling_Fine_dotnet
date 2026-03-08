using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WebApi;
using WebApi.Services.Business;
using WebApi.Services.Client;
using WebApi.Services.Reply;
using WebApi.Services.Reviews;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IBusinessService, BusinessService>();
builder.Services.AddScoped<IReviewsService, ReviewsService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IReplyService, ReplyService>();

await builder.Build().RunAsync();