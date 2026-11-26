using HikeFlowService;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options => options.ServiceName = "HikeFlowService");
builder.Services.AddSingleton<General>();
builder.Services.AddHostedService<Worker>();
builder.Build().Run();