using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;

namespace Vtc.Splunk.Log.Analyzer.Core;

public class FileImportService
{
    public List<string> ImportCodesFromFile(string filePath)
    {
        var codes = new List<string>();
        var fileExtension = Path.GetExtension(filePath).ToLower();

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
                throw new NotSupportedException($"File type {fileExtension} is not supported.");
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