using AthliQ.Core.DTOs.Child;
using AthliQ.Core.Service.Contract;
using iText.IO.Font.Constants;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;
using SkiaSharp;

namespace AthliQ.Service.Services.Report
{
    public class ReportService : IReportGenerationService
    {
        private readonly ILogger<ReportService> _logger;

        public ReportService(ILogger<ReportService> logger)
        {
            _logger = logger;
        }

        public async Task<byte[]> GenerateChartImageAsync(ReturnedEvaluateChildDto data)
        {
            try
            {
                const int width = 800;
                const int height = 600;
                const int centerX = width / 2;
                const int centerY = height / 2;
                var radius = Math.Min(width, height) / 3;

                using var surface = SKSurface.Create(new SKImageInfo(width, height));
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);

                // Calculate angles for pie chart
                var totalScore = data.ChildResultIntegratedDto.Sum(x => x.Score);
                var colors = new[]
                {
                    SKColors.Red,
                    SKColors.Blue,
                    SKColors.Green,
                    SKColors.Orange,
                    SKColors.Purple,
                    SKColors.Brown,
                    SKColors.Pink,
                    SKColors.Gray,
                };

                float startAngle = 0;
                var paint = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true };
                var textPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = 16,
                    IsAntialias = true,
                    Typeface = SKTypeface.Default,
                };

                // Draw pie chart
                for (int i = 0; i < data.ChildResultIntegratedDto.Count; i++)
                {
                    var item = data.ChildResultIntegratedDto[i];
                    var sweepAngle = (float)(item.Score * 360.0 / totalScore);

                    paint.Color = colors[i % colors.Length];

                    var rect = new SKRect(
                        centerX - radius,
                        centerY - radius,
                        centerX + radius,
                        centerY + radius
                    );
                    canvas.DrawArc(rect, startAngle, sweepAngle, true, paint);

                    // Draw labels
                    var labelAngle = (startAngle + sweepAngle / 2) * Math.PI / 180;
                    var labelX = centerX + (radius + 30) * Math.Cos(labelAngle);
                    var labelY = centerY + (radius + 30) * Math.Sin(labelAngle);

                    var percentage =
                        data.ChildResultWithPercentagesDtos.FirstOrDefault(p =>
                            p.Category == item.Category
                        )?.Percentage ?? "N/A";

                    canvas.DrawText(
                        $"{item.Category}: {percentage}",
                        (float)labelX,
                        (float)labelY,
                        textPaint
                    );

                    startAngle += sweepAngle;
                }

                // Draw title
                var titlePaint = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = 24,
                    IsAntialias = true,
                    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
                };
                canvas.DrawText("Physical Attributes Distribution", centerX - 150, 50, titlePaint);

                using var image = surface.Snapshot();
                using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
                return encoded.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating chart image");
                throw;
            }
        }

        public async Task<byte[]> GeneratePdfReportAsync(
            ReturnedEvaluateChildDto data,
            string childName
        )
        {
            try
            {
                using var memoryStream = new MemoryStream();

                // Create PDF writer and document
                using var writer = new PdfWriter(memoryStream);
                using var pdf = new PdfDocument(writer);
                var document = new Document(pdf, PageSize.A4);

                // Set margins
                document.SetMargins(50, 50, 50, 50);

                // Fonts
                var titleFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                var headerFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);
                var cellFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

                // Title
                var title = new Paragraph($"Child Physical Evaluation Report - {childName}")
                    .SetFont(titleFont)
                    .SetFontSize(18)
                    .SetFontColor(ColorConstants.BLUE)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20);
                document.Add(title);

                // Final Result
                var finalResult = new Paragraph(data.FinalResult)
                    .SetFont(headerFont)
                    .SetFontSize(14)
                    .SetFontColor(ColorConstants.GREEN)
                    .SetMarginBottom(20);
                document.Add(finalResult);

                // Scores Header
                var scoresHeader = new Paragraph("Physical Attributes Scores:")
                    .SetFont(headerFont)
                    .SetFontSize(12)
                    .SetMarginBottom(10);
                document.Add(scoresHeader);

                // Scores Table
                var table = new Table(UnitValue.CreatePercentArray(new float[] { 3f, 1f, 1f }))
                    .SetWidth(UnitValue.CreatePercentValue(100))
                    .SetMarginBottom(20);

                // Table Headers
                table.AddHeaderCell(
                    new Cell()
                        .Add(new Paragraph("Category").SetFont(headerFont).SetFontSize(12))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetPadding(8)
                );

                table.AddHeaderCell(
                    new Cell()
                        .Add(new Paragraph("Score").SetFont(headerFont).SetFontSize(12))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetPadding(8)
                );

                table.AddHeaderCell(
                    new Cell()
                        .Add(new Paragraph("Percentage").SetFont(headerFont).SetFontSize(12))
                        .SetBackgroundColor(ColorConstants.LIGHT_GRAY)
                        .SetPadding(8)
                );

                // Table Data
                foreach (var item in data.ChildResultIntegratedDto)
                {
                    var percentage =
                        data.ChildResultWithPercentagesDtos.FirstOrDefault(p =>
                            p.Category == item.Category
                        )?.Percentage ?? "N/A";

                    table.AddCell(
                        new Cell()
                            .Add(new Paragraph(item.Category).SetFont(cellFont).SetFontSize(10))
                            .SetPadding(8)
                    );

                    table.AddCell(
                        new Cell()
                            .Add(
                                new Paragraph(item.Score.ToString())
                                    .SetFont(cellFont)
                                    .SetFontSize(10)
                            )
                            .SetPadding(8)
                    );

                    table.AddCell(
                        new Cell()
                            .Add(new Paragraph(percentage).SetFont(cellFont).SetFontSize(10))
                            .SetPadding(8)
                    );
                }

                document.Add(table);

                // Recommended Sports Header
                var sportsHeader = new Paragraph("Recommended Sports:")
                    .SetFont(headerFont)
                    .SetFontSize(12)
                    .SetMarginBottom(15);
                document.Add(sportsHeader);

                // Recommended Sports Content
                foreach (var sport in data.MatchedSports)
                {
                    var sportTitle = new Paragraph(sport.Name)
                        .SetFont(headerFont)
                        .SetFontSize(12)
                        .SetFontColor(ColorConstants.BLUE)
                        .SetMarginBottom(5);
                    document.Add(sportTitle);

                    var sportDescription = new Paragraph(sport.Description)
                        .SetFont(cellFont)
                        .SetFontSize(10)
                        .SetTextAlignment(TextAlignment.JUSTIFIED)
                        .SetMarginBottom(15);
                    document.Add(sportDescription);
                }

                // Footer
                var footer = new Paragraph($"Generated on {DateTime.Now:dd/MM/yyyy HH:mm}")
                    .SetFont(cellFont)
                    .SetFontSize(9)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginTop(30);
                document.Add(footer);

                // Close document
                document.Close();

                return memoryStream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF report with iText 8");
                throw;
            }
        }
    }
}
