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
				const int width = 900;
				const int height = 700;
				const int centerX = width / 2;
				const int centerY = height / 2 + 30; // Offset down to make room for title
				var radius = Math.Min(width, height) / 4;

				using var surface = SKSurface.Create(new SKImageInfo(width, height));
				var canvas = surface.Canvas;
				canvas.Clear(SKColors.White);

				// Calculate angles for pie chart
				var totalScore = data.ChildResultIntegratedDto.Sum(x => x.Score);

				// Improved color palette
				var colors = new[]
				{
			SKColor.Parse("#FF6B6B"), // Red
            SKColor.Parse("#4ECDC4"), // Teal
            SKColor.Parse("#45B7D1"), // Blue
            SKColor.Parse("#96CEB4"), // Green
            SKColor.Parse("#FFEAA7"), // Yellow
            SKColor.Parse("#DDA0DD"), // Plum
            SKColor.Parse("#98D8C8"), // Mint
            SKColor.Parse("#F7DC6F"), // Light Gold
        };

				float startAngle = -90; // Start from top
				var paint = new SKPaint { Style = SKPaintStyle.Fill, IsAntialias = true };

				// Improved text styling
				var labelPaint = new SKPaint
				{
					Color = SKColors.Black,
					TextSize = 14,
					IsAntialias = true,
					Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Normal),
				};

				var percentagePaint = new SKPaint
				{
					Color = SKColors.Black,
					TextSize = 12,
					IsAntialias = true,
					Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
				};

				// Store slice information for better label positioning
				var sliceInfo = new List<(string category, string percentage, float midAngle, SKColor color)>();

				// Draw pie chart and collect slice information
				for (int i = 0; i < data.ChildResultIntegratedDto.Count; i++)
				{
					var item = data.ChildResultIntegratedDto[i];
					var sweepAngle = (float)(item.Score * 360.0 / totalScore);
					var color = colors[i % colors.Length];
					paint.Color = color;

					var rect = new SKRect(
						centerX - radius,
						centerY - radius,
						centerX + radius,
						centerY + radius
					);

					canvas.DrawArc(rect, startAngle, sweepAngle, true, paint);

					// Store slice info for label drawing
					var midAngle = startAngle + sweepAngle / 2;
					var percentage = data.ChildResultWithPercentagesDtos
						.FirstOrDefault(p => p.Category == item.Category)?.Percentage ?? "N/A";

					sliceInfo.Add((item.Category, percentage, midAngle, color));
					startAngle += sweepAngle;
				}

				// Draw labels with better positioning and connection lines
				foreach (var slice in sliceInfo)
				{
					var labelAngle = slice.midAngle * Math.PI / 180;

					// Position for the end of the connection line
					var lineEndX = centerX + (radius + 40) * Math.Cos(labelAngle);
					var lineEndY = centerY + (radius + 40) * Math.Sin(labelAngle);

					// Position for the label text (further out)
					var labelDistance = radius + 80;
					var labelX = centerX + labelDistance * Math.Cos(labelAngle);
					var labelY = centerY + labelDistance * Math.Sin(labelAngle);

					// Adjust label position based on quadrant to avoid overlap
					var textBounds = new SKRect();
					var labelText = $"{slice.category}: {slice.percentage}";
					labelPaint.MeasureText(labelText, ref textBounds);

					// Adjust horizontal positioning
					if (labelX > centerX) // Right side
					{
						labelX += 10;
					}
					else // Left side
					{
						labelX -= textBounds.Width + 10;
					}

					// Adjust vertical positioning
					if (labelY < centerY) // Top half
					{
						labelY -= 10;
					}
					else // Bottom half
					{
						labelY += 15;
					}

					// Draw connection line
					var linePaint = new SKPaint
					{
						Color = SKColors.Gray,
						StrokeWidth = 1,
						Style = SKPaintStyle.Stroke,
						IsAntialias = true
					};

					// Line from pie edge to label
					var pieEdgeX = centerX + radius * Math.Cos(labelAngle);
					var pieEdgeY = centerY + radius * Math.Sin(labelAngle);

					canvas.DrawLine((float)pieEdgeX, (float)pieEdgeY,
								   (float)lineEndX, (float)lineEndY, linePaint);
					canvas.DrawLine((float)lineEndX, (float)lineEndY,
								   (float)labelX, (float)labelY, linePaint);

					// Draw label background for better readability
					var bgRect = new SKRect(
						(float)labelX - 5,
						(float)labelY - textBounds.Height - 2,
						(float)labelX + textBounds.Width + 5,
						(float)labelY + 5
					);

					var bgPaint = new SKPaint
					{
						Color = SKColors.White.WithAlpha(230),
						Style = SKPaintStyle.Fill,
						IsAntialias = true
					};

					canvas.DrawRoundRect(bgRect, 3, 3, bgPaint);

					// Draw border around label background
					var borderPaint = new SKPaint
					{
						Color = slice.color.WithAlpha(100),
						Style = SKPaintStyle.Stroke,
						StrokeWidth = 1,
						IsAntialias = true
					};
					canvas.DrawRoundRect(bgRect, 3, 3, borderPaint);

					// Draw the label text
					canvas.DrawText(labelText, (float)labelX, (float)labelY, labelPaint);
				}

				// Draw improved title
				var titlePaint = new SKPaint
				{
					Color = SKColor.Parse("#2C3E50"),
					TextSize = 28,
					IsAntialias = true,
					Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold),
				};

				var titleText = "Physical Attributes Distribution";
				var titleBounds = new SKRect();
				titlePaint.MeasureText(titleText, ref titleBounds);
				var titleX = centerX - titleBounds.Width / 2;

				canvas.DrawText(titleText, titleX, 40, titlePaint);

				// Add a subtle shadow/border effect to the pie chart
				var shadowPaint = new SKPaint
				{
					Color = SKColors.Black.WithAlpha(30),
					Style = SKPaintStyle.Fill,
					IsAntialias = true,
					ImageFilter = SKImageFilter.CreateDropShadow(2, 2, 4, 4, SKColors.Black.WithAlpha(50))
				};

				var shadowRect = new SKRect(
					centerX - radius - 2,
					centerY - radius - 2,
					centerX + radius + 2,
					centerY + radius + 2
				);

				// Draw shadow circle behind the pie
				canvas.DrawOval(shadowRect, shadowPaint);

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
