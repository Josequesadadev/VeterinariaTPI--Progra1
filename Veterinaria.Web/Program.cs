var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Proyecto web: sirve las paginas HTML, CSS y JavaScript de la carpeta wwwroot.
// Toda la informacion la obtiene llamando a la Web API (Veterinaria.API).
app.UseDefaultFiles();   // abre index.html por defecto
app.UseStaticFiles();

app.Run();
