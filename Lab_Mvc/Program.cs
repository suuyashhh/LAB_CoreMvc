using Lab_Mvc.Contest;
using Lab_Mvc.Interfaces;
using Lab_Mvc.Interfaces.DairyFarm;
using Lab_Mvc.Interfaces.Farm;
using Lab_Mvc.Interfaces.Shop;
using Lab_Mvc.Repositries;
using Lab_Mvc.Repositries.DairyFarm;
using Lab_Mvc.Repositries.Farm;
using Lab_Mvc.Repositries.Shop;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SmartParking.Interfaces;
using SmartParking.Repositories;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
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

builder.Services.AddScoped<ILoginDairyFarm, LoginDairyFarmRepository>();
builder.Services.AddScoped<IDairyMasters, DairyMastersRepository>();
builder.Services.AddScoped<IFeeds, FeedsRepository>();
builder.Services.AddScoped<IDoctorDairy, DoctorDairyRepository>();
builder.Services.AddScoped<IOtherFeeds, OtherFeedsRepository>();
builder.Services.AddScoped<IBillDairy, BillDairyRepository>();
builder.Services.AddScoped<IHistoryDairy, HistoryDairyRepository>();
builder.Services.AddScoped<IAnimalHealthHistory, AnimalHealthHistoryRepository>();
builder.Services.AddScoped<IBreedingDateCheck, BreedingDateCheckRepository>();
builder.Services.AddScoped<IMonthlyPERepository, MonthlyPERepository>();
builder.Services.AddScoped<IDatePERepository, DatePERepository>();
builder.Services.AddScoped<INotification, NotificationRepository>();
builder.Services.AddHostedService<DailyBreedingNotificationService>();

builder.Services.AddScoped<ILoginFarm, LoginFarmRepository>();
builder.Services.AddScoped<IHomeFarm, HomeFarmRepository>();
builder.Services.AddScoped<IFarmEntry, FarmEntryRepository>();
builder.Services.AddScoped<IShopLogin, ShopLoginRepository>();

builder.Services.AddScoped<IParkingLogin, ParkingLoginRepository>();
builder.Services.AddScoped<IParkingProvider, ParkingProviderRepository>();
builder.Services.AddScoped<IParkingRegistration, ParkingRegistrationRepository>();

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect("localhost:6379,abortConnect=false")
);

builder.Services.AddSingleton<DapperContext>();
builder.Services.AddDbContext<LabMvcDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("connString")));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("Jwt");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = false,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!)
            )
        };
    });

var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseMiddleware<Lab_Mvc.Controllers.LoginController.TokenValidationMiddleware>();
app.UseMiddleware<SmartParking.Controllers.ParkingLoginController.ParkingTokenValidationMiddleware>();
app.UseAuthorization();

app.UseStaticFiles();

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
