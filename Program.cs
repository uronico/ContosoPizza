using ContosoPizza.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/generate-sales-summary", () =>
{
    var contentRoot = app.Environment.ContentRootPath;
    var dataDirectory = Path.Combine(contentRoot, "SalesData");
    Directory.CreateDirectory(dataDirectory);

    var salesFiles = new[]
    {
        Path.Combine(dataDirectory, "north-store-sales.txt"),
        Path.Combine(dataDirectory, "south-store-sales.txt")
    };

    File.WriteAllLines(salesFiles[0], new[] { "1,234.56", "2,345.67", "$3,456.78" });
    File.WriteAllLines(salesFiles[1], new[] { "4,567.89", "5,678.90", "$6,789.01" });

    var reportFile = Path.Combine(dataDirectory, "SalesSummaryReport.txt");
    SalesSummaryReport.GenerateSalesSummaryReport(reportFile, salesFiles);

    var reportText = File.ReadAllText(reportFile);
    app.Logger.LogInformation("Sales summary report generated at: {ReportPath}\n{Report}", reportFile, reportText);

    return Results.Text(reportText, "text/plain");
});

app.Run();
