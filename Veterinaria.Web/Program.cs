var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// sirve las paginas de wwwroot (index.html por defecto)
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();
