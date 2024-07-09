using Microsoft.Extensions.DependencyInjection;

ServiceCollection services = new();

services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

services.AddSingleton<App>();

ServiceProvider serviceProvider = services.BuildServiceProvider();

App app = serviceProvider.GetRequiredService<App>();

await app.RunAsync();

