using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using OfficeOpenXml;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using wisecorp.Models;
using wisecorp.Models.DBModels;

namespace wisecorp.Helpers;

public static class XlsxGenerator
{
    /// <summary>
    /// Permets l'export de la current timesheet en excel
    /// </summary>
    public static void GenerateXlsx(Account Account, DateTime currentWeek, List<ProjectTask> ProjectTask)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using var package = new ExcelPackage();
        // Add a new worksheet to the empty workbook
        var worksheet = package.Workbook.Worksheets.Add("TimeSheet");

        // add employee information
        worksheet.Cells[1, 1].Value = "Employee";
        worksheet.Cells[1, 2].Value = Account.FullName;
        worksheet.Cells[2, 1].Value = "Email";
        worksheet.Cells[2, 2].Value = Account.Email;
        worksheet.Cells[3, 1].Value = "Week";
        worksheet.Cells[3, 2].Value = currentWeek.ToShortDateString() + " to " + currentWeek.AddDays(6).ToShortDateString();

        // Add the headers
        worksheet.Cells[6, 1].Value = "Project";
        worksheet.Cells[6, 2].Value = "Sunday";
        worksheet.Cells[6, 3].Value = "Monday";
        worksheet.Cells[6, 4].Value = "Tuesday";
        worksheet.Cells[6, 5].Value = "Wednesday";
        worksheet.Cells[6, 6].Value = "Thursday";
        worksheet.Cells[6, 7].Value = "Friday";
        worksheet.Cells[6, 8].Value = "Saturday";
        worksheet.Cells[6, 9].Value = "Total";
        worksheet.Cells[6, 1, 6, 9].Style.Font.Bold = true;

        // Add the data
        int row = 0;

        foreach (ProjectTask project in ProjectTask)
        {
            foreach (Work work in project.Works)
            {
                worksheet.Cells[row + 7, 1].Value = work.Project.Name;
                worksheet.Cells[row + 7, 2].Value = work.HourWorkedSun ?? 0;
                worksheet.Cells[row + 7, 3].Value = work.HourWorkedMon ?? 0;
                worksheet.Cells[row + 7, 4].Value = work.HourWorkedTue ?? 0;
                worksheet.Cells[row + 7, 5].Value = work.HourWorkedWed ?? 0;
                worksheet.Cells[row + 7, 6].Value = work.HourWorkedThur ?? 0;
                worksheet.Cells[row + 7, 7].Value = work.HourWorkedFri ?? 0;
                worksheet.Cells[row + 7, 8].Value = work.HourWorkedSat ?? 0;
                worksheet.Cells[row + 7, 9].Formula = $"SUM(B{row + 7}:H{row + 7})";
                row++;
            }
        }

        // border
        worksheet.Cells[6, 1, row + 6, 9].Style.Border.Top.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[6, 1, row + 6, 9].Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[6, 1, row + 6, 9].Style.Border.Left.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;
        worksheet.Cells[6, 1, row + 6, 9].Style.Border.Right.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thin;

        // Add the complete total of all week hours
        worksheet.Cells[row + 7, 1].Value = "Total";
        worksheet.Cells[row + 7, 1].Style.Font.Bold = true;

        worksheet.Cells[row + 7, 9].Formula = $"SUM(I7:I{row + 6})";

        // Save the file
        var saveFileDialog = new SaveFileDialog
        {
            Filter = "Excel files|*.xlsx",
            Title = "Save the TimeSheet"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            var file = new FileInfo(saveFileDialog.FileName);
            package.SaveAs(file);
        }
    }


}
