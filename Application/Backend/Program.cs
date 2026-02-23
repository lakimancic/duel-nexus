using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Backend.Data.Context;
using Backend.Application.Services.Interfaces;
using Backend.Application.Services;
using Backend.Data.UnitOfWork;
using Backend.Application.Mappings;
using Backend.Domain.Commands;
using Backend.Domain.Commands.Handlers;
using Backend.Domain.Engine;
using Backend.Utils.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DuelNexusDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.Value!.Contains("/gameHub", StringComparison.OrdinalIgnoreCase))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddAuthorization();

builder.Services.AddSingleton<Backend.Utils.Security.PasswordHasher>();
builder.Services.AddSingleton<Backend.Utils.Security.JwtTokenGenerator>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<IDeckService, DeckService>();
builder.Services.AddScoped<IEffectService, EffectService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IGameRoomService, GameRoomService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IGameEngine, GameEngine>();
builder.Services.AddSingleton<IGameCommandLock, GameCommandLock>();
builder.Services.AddScoped<IGameCommandHandler<DrawActionCommand, DrawActionResult>, DrawActionCommandHandler>();
builder.Services.AddScoped<IGameCommandHandler<SkipDrawActionCommand, DrawPhaseProgressResult>, SkipDrawActionCommandHandler>();
builder.Services.AddScoped<IGameCommandHandler<PlaceCardActionCommand, PlaceCardResult>, PlaceCardActionCommandHandler>();
builder.Services.AddScoped<IGameCommandHandler<SendCardToGraveyardActionCommand, GameCardUpdateResult>, SendCardToGraveyardActionCommandHandler>();
builder.Services.AddScoped<IGameCommandHandler<ToggleDefensePositionActionCommand, GameCardUpdateResult>, ToggleDefensePositionActionCommandHandler>();
builder.Services.AddScoped<IGameCommandHandler<RevealCardActionCommand, GameCardUpdateResult>, RevealCardActionCommandHandler>();
builder.Services.AddScoped<IGameCommandHandler<AdvancePhaseActionCommand, PhaseAdvanceResult>, AdvancePhaseActionCommandHandler>();
builder.Services.AddScoped<IConnectionService, ConnectionService>();
builder.Services.AddSingleton<ConnectionTracker>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var mapperConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new MappingProfile());
});

builder.Services.AddSingleton(mapperConfig.CreateMapper());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration["AppSettings:Frontend"]!)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend API v1");
        c.RoutePrefix = string.Empty;
    });
}

// app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<GameHub>("/gameHub");

app.Run();
