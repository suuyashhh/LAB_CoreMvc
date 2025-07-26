using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Lab_Mvc.Repositries;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using static Lab_Mvc.Controllers.LoginController;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IAdmin, AdminRepository>();
builder.Services.AddScoped<ITest, TestRepository>();
builder.Services.AddScoped<IDoctor, DoctorRepository>();
builder.Services.AddScoped<ICasePaper, CasePaperRepository>();
builder.Services.AddScoped<ILogin, LoginRepository>();
builder.Services.AddScoped<IEmployee, EmployeeRepository>();
builder.Services.AddScoped<ILabMaterials, LabMaterialsRepository>();
builder.Services.AddScoped<IBikeFule, BikeFuleRepository>();
builder.Services.AddScoped<IEmployeeSalary, EmployeeSalaryRepository>();
builder.Services.AddScoped<IElectricityBill, ElectricityBillRepository>();
builder.Services.AddScoped<IOtherExpense, OtherExpenseRepository>();
builder.Services.AddScoped<IDoctorCommission, DoctorCommissionRepository>();
builder.Services.AddScoped<IHome, HomeRepository>();
builder.Services.AddScoped<IFinance, FinanceRepository>();

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false")
);

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddDbContext<LabMvcDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("myTestDB")));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt"); // ✅ FIXED

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false, // ⛔ Token never expires unless manually blacklisted
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            )
        };
    });



var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();                        // ✅ Auth first
app.UseMiddleware<Lab_Mvc.Controllers.LoginController.TokenValidationMiddleware>(); // ✅ Then your Redis session middleware
app.UseAuthorization();


app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapControllers();

app.Run();
