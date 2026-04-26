using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Lab_Mvc.Interfaces.DairyFarm;
using Lab_Mvc.Interfaces.Farm;
using Lab_Mvc.Repositries;
using Lab_Mvc.Repositries.DairyFarm;
using Lab_Mvc.Repositries.Farm;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;
using static Lab_Mvc.Controllers.LoginController;
using SmartParking.Interfaces;
using SmartParking.Repositories;


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


//DairyFARM Project
builder.Services.AddScoped<ILoginDairyFarm, LoginDairyFarmRepository>();
builder.Services.AddScoped<IDairyMasters, DairyMastersRepository>();
builder.Services.AddScoped<IFeeds, FeedsRepository>();
builder.Services.AddScoped<IDoctorDairy, DoctorDairyRepository>();
builder.Services.AddScoped<IOtherFeeds, OtherFeedsRepository>();
builder.Services.AddScoped < IBillDairy, BillDairyRepository>();
builder.Services.AddScoped < IHistoryDairy, HistoryDairyRepository>();
builder.Services.AddScoped <IAnimalHealthHistory, AnimalHealthHistoryRepository>();
builder.Services.AddScoped <IBreedingDateCheck, BreedingDateCheckRepository>();
builder.Services.AddScoped <IMonthlyPERepository, MonthlyPERepository>();
builder.Services.AddScoped <IDatePERepository, DatePERepository>();
builder.Services.AddScoped<INotification, NotificationRepository>();

builder.Services.AddHostedService<DailyBreedingNotificationService>();



//FARM Project
builder.Services.AddScoped<ILoginFarm, LoginFarmRepository>();
builder.Services.AddScoped<IHomeFarm, HomeFarmRepository>();
builder.Services.AddScoped<IFarmEntry, FarmEntryRepository>();

// SMART PARKING Project
builder.Services.AddScoped<IParkingLogin, ParkingLoginRepository>();
builder.Services.AddScoped<IParkingProvider, ParkingProviderRepository>();




builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false")
);

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddDbContext<LabMvcDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("connString")));


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

app.UseStaticFiles();
// Add custom static file serving for FarmImgs folder at root
var farmImgsPath = Path.Combine(app.Environment.ContentRootPath, "FarmImgs");
if (!Directory.Exists(farmImgsPath))
{
    Directory.CreateDirectory(farmImgsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(farmImgsPath),
    RequestPath = "/FarmImgs"
});

// Add custom static file serving for ParkingImages folder at root
var parkingImgsPath = Path.Combine(app.Environment.ContentRootPath, "ParkingImages");
if (!Directory.Exists(parkingImgsPath))
{
    Directory.CreateDirectory(parkingImgsPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(parkingImgsPath),
    RequestPath = "/ParkingImages"
});



app.UseCors(policy => policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

app.MapControllers();

app.Run();
