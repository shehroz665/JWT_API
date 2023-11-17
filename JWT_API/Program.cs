using JWT_API.Data;
using JWT_API.Logging;
using JWT_API.Repository_Pattern.Category;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICategory, CategoryRepository>();
builder.Services.AddSingleton<LoggingInterface, Logging>();
builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBuser"));
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => {
    policy.RequireClaim("RoleId", "1");
    policy.RequireClaim("UserId");
    policy.RequireClaim("UserEmail");
    });
    options.AddPolicy("Student", policy =>
    {
        policy.RequireClaim("RoleId", "2");
        policy.RequireClaim("UserId");
        policy.RequireClaim("UserEmail");
    });
    options.AddPolicy("Teacher", policy =>
    {
        policy.RequireClaim("RoleId", "3");
        policy.RequireClaim("UserId");
        policy.RequireClaim("UserEmail");
    });

});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken   = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,          
            ValidateAudience = true,        
            ValidIssuer = builder.Configuration["Jwt:Issuer"],    
            ValidAudience = builder.Configuration["Jwt:Audience"], 
            IssuerSigningKey = new SymmetricSecurityKey(
                     Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) 
        };
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "corspolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseCors("corspolicy");
app.Run();
