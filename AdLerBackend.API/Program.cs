using System.Reflection;
using AdLerBackend.API.Filters;
using AdLerBackend.Application;
using AdLerBackend.Infrastructure;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


// This is needed, because wwwroot directory must be present in the beginning to serve files from it
Directory.CreateDirectory("wwwroot");


var builder = WebApplication.CreateBuilder(args);

// If the config file does not exist, create it
if (!File.Exists("./config/config.json"))
{
    File.WriteAllText("./config/config.json", JsonConvert.SerializeObject(new
    {
        useHttps = "false",
        httpPort = 433,
        moodleUrl = "Bitte eine Moodle URL angeben"
    }, Formatting.Indented));

    // shut down programm with message in dialog
    Console.WriteLine("Bitte config.json anpassen und Programm erneut starten");
    Environment.Exit(1);
}

// Use Global AdLer Config File (Most likely coming from a docker volume)
builder.Configuration.AddJsonFile("./config/config.json", false);

// Add HTTPS support
if (!builder.Environment.IsDevelopment())
    builder.WebHost.ConfigureKestrel(options =>
    {
        if (builder.Configuration["useHttps"].ToLower() == "true")
            options.ListenAnyIP(int.Parse(builder.Configuration["httpsPort"] ?? "433"),
                listenOptions =>
                {
                    listenOptions.UseHttps("./config/cert/AdLerBackend.pfx",
                        builder.Configuration["httpsCertificatePassword"]);
                });
        else
            // if builder.Configuration["httpPort"] is not set, use default port 80
            options.ListenAnyIP(int.Parse(builder.Configuration["httpPort"] ?? "80"));
    });


builder.Services.AddControllers(
    options => { options.Filters.Add(new ApiExceptionFilterAttribute()); }
).AddNewtonsoftJson(opts =>
{
    // This converts enum integers to its corresponding string value
    opts.SerializerSettings.Converters.Add(new StringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
}).AddSwaggerGenNewtonsoftSupport();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment.IsDevelopment());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowEverything",
        policy => { policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod(); });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowEverything");

// Disabled for now, because it is not needed
//app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapControllers();

app.Run();