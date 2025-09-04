using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using Vtc.Splunk.Log.Analyzer.Models;

namespace Vtc.Splunk.Log.Analyzer.Core;

public class FileExportService
{
    private readonly ILogger<FileExportService> _logger;

    public FileExportService(ILogger<FileExportService> logger)
    {
        _logger = logger;
    }

    public void ExportToCsv(List<TransactionInfo> data, string filePath)
    {
        _logger.LogInformation("Attempting to export {DataCount} records to CSV file: {FilePath}", data.Count, filePath);
        try
        {
            using (var writer = new StreamWriter(filePath))
            {
                // Write header
                writer.WriteLine("SearchCode,IssuerBankName,RemitterName,RemitterAccountNumber,SearchStatus,Timestamp");

                // Write data
                foreach (var item in data)
                {
                    writer.WriteLine($"{item.SearchCode},{item.IssuerBankName},{item.RemitterName},{item.RemitterAccountNumber},{item.SearchStatus},{item.Timestamp}");
                }
            }
            _logger.LogInformation("Successfully exported data to CSV: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while exporting data to CSV: {FilePath}", filePath);
            throw;
        }
    }

    public void ExportToExcel(List<TransactionInfo> data, string filePath)
    {
        _logger.LogInformation("Attempting to export {DataCount} records to Excel file: {FilePath}", data.Count, filePath);
        try
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Transaction Data");

                // Write header
                worksheet.Cell(1, 1).Value = "SearchCode";
                worksheet.Cell(1, 2).Value = "IssuerBankName";
                worksheet.Cell(1, 3).Value = "RemitterName";
                worksheet.Cell(1, 4).Value = "RemitterAccountNumber";
                worksheet.Cell(1, 5).Value = "SearchStatus";
                worksheet.Cell(1, 6).Value = "Timestamp";

                // Write data
                for (int i = 0; i < data.Count; i++)
                {
                    var row = i + 2; // Start from row 2 for data
                    worksheet.Cell(row, 1).Value = data[i].SearchCode;
                    worksheet.Cell(row, 2).Value = data[i].IssuerBankName;
                    worksheet.Cell(row, 3).Value = data[i].RemitterName;
                    worksheet.Cell(row, 4).Value = data[i].RemitterAccountNumber;
                    worksheet.Cell(row, 5).Value = data[i].SearchStatus;
                    worksheet.Cell(row, 6).Value = data[i].Timestamp;
                }

                workbook.SaveAs(filePath);
            }
            _logger.LogInformation("Successfully exported data to Excel: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while exporting data to Excel: {FilePath}", filePath);
            throw;
        }
    }
}