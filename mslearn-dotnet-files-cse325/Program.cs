using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

// See https://aka.ms/new-console-template for more information

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

// File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

GenerateSalesSummaryReport(salesTotal, salesFiles, salesTotalDir);

IEnumerable<string> FindFiles(string folderName)
{
    List<string> salesFiles = new List<string>();

    var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

    foreach (var file in foundFiles)
    {
        var extension = Path.GetExtension(file);
        if (extension == ".json")
        {
            salesFiles.Add(file);
        }
    }

    return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
    double salesTotal = 0;

    // Loop over each file path in salesFiles
    foreach (var file in salesFiles)
    {
        // Read the contents of the file
        string salesJson = File.ReadAllText(file);

        // Parse the contents as JSON
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

        // Add the amount found in the Total field to the salesTotal variable
        salesTotal += data?.Total ?? 0;
    }

    return salesTotal;
}

void GenerateSalesSummaryReport(double total, IEnumerable<string> salesFiles, string outputDir)
{
    StringBuilder report = new StringBuilder();

    // Add header
    report.AppendLine("Sales Summary");
    report.AppendLine("---------------------------");
    report.AppendLine($"Total Sales: {total:C}");
    report.AppendLine("Details:");

    // Add details for each sales file
    foreach (var file in salesFiles)
    {
        string salesJson = File.ReadAllText(file);
        SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);
        report.AppendLine($"  {Path.GetFileName(file)}: {data?.Total:C}");
    }

    // Write report to file
    File.WriteAllText(Path.Combine(outputDir, "sales_summary_report.txt"), report.ToString());
}

record SalesData(double Total);
