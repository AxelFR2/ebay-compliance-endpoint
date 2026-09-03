using EbayComplianceEndpoint;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IEbayDeletionSettings, EnvironmentEbayDeletionSettings>();

var app = builder.Build();

app.MapEbayAccountDeletionEndpoints();
app.MapGet("/health", () => Results.Ok());

app.Run();

public partial class Program;
