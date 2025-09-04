using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using Microsoft.Extensions.Logging;

namespace Vtc.Splunk.Log.Analyzer.Core;

public class FileImportService
{
    private readonly ILogger<FileImportService> _logger;

    public FileImportService(ILogger<FileImportService> logger)
    {
        _logger = logger;
    }

    public List<string> ImportCodesFromFile(string filePath)
    {
        var codes = new List<string>();
        var fileExtension = Path.GetExtension(filePath).ToLower();
        _logger.LogInformation("Attempting to import codes from {FilePath} with extension {FileExtension}", filePath, fileExtension);

        try
        {
            switch (fileExtension)
            {
                case ".csv":
                    codes = ImportFromCsv(filePath);
                    break;
                case ".xlsx":
                case ".xls":
                    codes = ImportFromExcel(filePath);
                    break;
                case ".txt":
                    codes = ImportFromTxt(filePath);
                    break;
                default:
                    var ex = new NotSupportedException($"File type {fileExtension} is not supported.");
                    _logger.LogError(ex, "Unsupported file type for import: {FileExtension}", fileExtension);
                    throw ex;
            }
            _logger.LogInformation("Successfully imported {CodeCount} codes from {FilePath}", codes.Count, filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while importing codes from {FilePath}", filePath);
            throw; // Re-throw the exception to be handled by the ViewModel
        }

        return codes.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
    }

    private List<string> ImportFromCsv(string filePath)
    {
        return File.ReadAllLines(filePath)
                   .SelectMany(line => line.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
                   .Select(code => code.Trim())
                   .ToList();
    }

    private List<string> ImportFromExcel(string filePath)
    {
        var codes = new List<string>();
        using (var workbook = new XLWorkbook(filePath))
        {
            foreach (var worksheet in workbook.Worksheets)
            {
                foreach (var row in worksheet.RowsUsed())
                {
                    foreach (var cell in row.CellsUsed())
                    {
                        codes.Add(cell.GetValue<string>());
                    }
                }
            }
        }
        return codes;
    }

    private List<string> ImportFromTxt(string filePath)
    {
        return File.ReadAllLines(filePath)
                   .Select(line => line.Trim())
                   .ToList();
    }
}