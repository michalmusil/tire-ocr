using TireOcr.RunnerPrototype;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddPresentation();

builder.AddServiceDefaults();

var app = builder.Build();
app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();