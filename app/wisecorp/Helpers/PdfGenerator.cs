using System.IO;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Quality;
using wisecorp.Models;
using wisecorp.Models.DBModels;

namespace wisecorp.Helpers;

public static class PdfGenerator
{
    /// <summary>
    /// Permets l'export de la current timesheet en pdf
    /// </summary>
    public static void GeneratePdf(Account Account, DateTime currentWeek, List<ProjectTask> ProjectTask)
    {
        using PdfDocument document = new PdfDocument();
        //d�clare mes variable de base
        PdfPage page = document.AddPage();
        page.Orientation = PdfSharp.PageOrientation.Landscape;
        XGraphics gfx = XGraphics.FromPdfPage(page);
        XFont regularFont = new XFont("Arial", 10);
        XFont boldFont = new XFont("Arial", 10, XFontStyleEx.Bold);

        //dessine le logo
        double pageWidth = page.Width.Point;

        XImage image = XImage.FromFile("../../../../../Documentation/logo.png");
        gfx.DrawImage(image, pageWidth - 150, 0, 150, 150);

        //set la marge pis les constante pour ma grid
        double margin = 50;
        double tableWidth = page.Width - (2 * margin);
        double cellWidth = tableWidth / 9; // 9 columns
        double rowHeight = 25;
        double currentY = margin;
        //string pathLogo = "../../../../../Documentation/logo.png";

        //Info emp
        DrawText(gfx, "Employee:", margin, currentY, boldFont);
        DrawText(gfx, Account.FullName, margin + cellWidth, currentY, regularFont);
        currentY += rowHeight;

        //mail emp
        DrawText(gfx, "Email:", margin, currentY, boldFont);
        DrawText(gfx, Account.Email, margin + cellWidth, currentY, regularFont);
        currentY += rowHeight;

        //semaine
        DrawText(gfx, "Week:", margin, currentY, boldFont);
        DrawText(gfx, currentWeek.ToShortDateString() + " to " + currentWeek.AddDays(6).ToShortDateString(), margin + cellWidth, currentY, regularFont);
        currentY += rowHeight * 1.5;

        //Start la grid en bas des infos
        double gridStartY = currentY;

        //headers de colonne
        string[] headers = { "Project", "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Total" };
        for (int i = 0; i < headers.Length; i++)
        {
            DrawTextInCell(gfx, headers[i], margin + (i * cellWidth), currentY, cellWidth, rowHeight, boldFont);
        }
        currentY += rowHeight;

        //Rajoute les totals
        decimal[] dailyTotals = new decimal[7];
        decimal weekTotal = 0;

        //loop les heures dans les tasks
        foreach (ProjectTask project in ProjectTask)
        {
            foreach (Work work in project.Works)
            {
                DrawTextInCell(gfx, work.Project.Name, margin, currentY, cellWidth, rowHeight, regularFont);

                decimal[] dailyHours = new decimal[7]
                {
                        work.HourWorkedSun ?? 0,
                        work.HourWorkedMon ?? 0,
                        work.HourWorkedTue ?? 0,
                        work.HourWorkedWed ?? 0,
                        work.HourWorkedThur ?? 0,
                        work.HourWorkedFri ?? 0,
                        work.HourWorkedSat ?? 0
                };

                for (int i = 0; i < 7; i++)
                {
                    DrawTextInCell(gfx, dailyHours[i].ToString("F1"), margin + cellWidth * (i + 1), currentY, cellWidth, rowHeight, regularFont);
                    dailyTotals[i] += dailyHours[i];
                }

                decimal total = dailyHours.Sum();
                weekTotal += total;
                DrawTextInCell(gfx, total.ToString("F1"), margin + cellWidth * 8, currentY, cellWidth, rowHeight, regularFont);

                currentY += rowHeight;
            }
        }

        //Ajoute les total daily
        DrawTextInCell(gfx, "Daily Totals", margin, currentY, cellWidth, rowHeight, boldFont);
        for (int i = 0; i < 7; i++)
        {
            DrawTextInCell(gfx, dailyTotals[i].ToString("F1"), margin + cellWidth * (i + 1), currentY, cellWidth, rowHeight, boldFont);
        }
        DrawTextInCell(gfx, weekTotal.ToString("F1"), margin + cellWidth * 8, currentY, cellWidth, rowHeight, boldFont);
        currentY += rowHeight;

        //Dessine la grid
        DrawGridLines(gfx, margin, gridStartY, tableWidth, currentY - gridStartY, cellWidth, rowHeight);

        //Save le document
        string downloadsPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        string nom = $"{Account.FullName}_{currentWeek.ToShortDateString()}-{currentWeek.AddDays(6).ToShortDateString()}.pdf";
        string name = $"{downloadsPath}\\{nom}";
        document.Save(name);
        PdfFileUtility.ShowDocument(name);
    }

    /// <summary>
    /// Fonction utilisee pour ecrire le text dans le pdf
    /// </summary>
    /// <param name="gfx">Objet XGraphics de la page</param>
    /// <param name="text">LE texte a �crire</param>
    /// <param name="x">position en x</param>
    /// <param name="y">position en y</param>
    /// <param name="font">Un objet Xfont pour le font</param>
    private static void DrawText(XGraphics gfx, string text, double x, double y, XFont font)
    {
        gfx.DrawString(text, font, XBrushes.Black, x, y);
    }

    /// <summary>
    /// Dessine a l'interieur de la grid
    /// </summary>
    /// <param name="gfx">Objet XGraphics de la page</param>
    /// <param name="text">LE texte a ecrire</param>
    /// <param name="x">position en x</param>
    /// <param name="y">position en y</param>
    /// <param name="width">Largeur de la cell</param>
    /// <param name="height">hauteur de la cell</param>
    /// <param name="font">Un objet Xfont pour le font</param>
    private static void DrawTextInCell(XGraphics gfx, string text, double x, double y, double width, double height, XFont font)
    {
        XSize textSize = gfx.MeasureString(text, font);

        if (textSize.Width > width - 4)
        {
            text = TruncateWithEllipsis(gfx, text, font, width - 4);
        }

        double xPos = x + (width - textSize.Width) / 2;
        double yPos = y + (height - textSize.Height) / 2 + font.Height * 0.75;

        gfx.DrawString(text, font, XBrushes.Black, xPos, yPos);
    }

    /// <summary>
    /// Methode qui s'assure que le texte est de la bonne longeur pour la cellule
    /// </summary>
    /// <param name="gfx">Objet XGraphics de la page</param>
    /// <param name="text">LE texte a ecrire</param>
    /// <param name="font">Un objet Xfont pour le font</param>
    /// <param name="maxWidth">La largeur maximale que le texte peux avoir</param>
    /// <returns></returns>
    private static string TruncateWithEllipsis(XGraphics gfx, string text, XFont font, double maxWidth)
    {
        string ellipsis = "...";
        if (gfx.MeasureString(text, font).Width <= maxWidth)
            return text;

        int length = text.Length;
        while (length > 0 && gfx.MeasureString(text.Substring(0, length) + ellipsis, font).Width > maxWidth)
        {
            length--;
        }

        return text.Substring(0, length) + ellipsis;
    }

    /// <summary>
    /// Rajoute les lignes de la grid 
    /// </summary>
    /// <param name="gfx">Objet XGraphics de la page</param>
    /// <param name="x">points de debut x de la grid</param>
    /// <param name="y"> points de debut y de la grid</param>
    /// <param name="width">Largeur de la grid</param>
    /// <param name="height">Hauteur de la grid</param>
    /// <param name="cellWidth">largeur des cellules</param>
    /// <param name="rowHeight">Hauteur des cellules</param>
    private static void DrawGridLines(XGraphics gfx, double x, double y, double width, double height, double cellWidth, double rowHeight)
    {
        XPen pen = new(XColors.Black, 0.5);


        for (double i = x; i <= x + width + 0.1; i += cellWidth)
        {
            gfx.DrawLine(pen, i, y, i, y + height);
        }


        for (double i = y; i <= y + height + 0.1; i += rowHeight)
        {
            gfx.DrawLine(pen, x, i, x + width, i);
        }
    }

}
