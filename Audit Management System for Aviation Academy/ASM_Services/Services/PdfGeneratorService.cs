using ASM_Repositories.Entities;
using ASM_Repositories.Models.AuditDTO;
using ASM_Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace ASM_Services.Services
{
    public class PdfGeneratorService : IPdfGeneratorService
    {
        public byte[] GeneratePdf(ViewAuditSummary summary, List<Finding> findings, List<Attachment> attachments, byte[]? logo)
        {
            var pdf = QuestPDF.Fluent.Document.Create(container =>
            {
                // =============== 1️ COVER PAGE ===============
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Content().AlignMiddle().AlignCenter().Column(col =>
                    {
                        if (logo != null)
                            col.Item().AlignCenter().Width(120).Image(logo).FitWidth();

                        col.Item().Height(30);
                        col.Item().Text(summary.Title)
                            .FontSize(22).Bold().FontColor("#1B4965").AlignCenter();

                        col.Item().Text($"{summary.Type} — {summary.Scope}")
                            .FontSize(13).FontColor("#5C677D").AlignCenter();

                        col.Item().Height(20);
                        col.Item().Text($"Audit Period: {summary.StartDate:dd MMM yyyy} - {summary.EndDate:dd MMM yyyy}")
                            .FontSize(11).AlignCenter();

                        col.Item().Height(20);
                        col.Item().Text("Prepared by Audit Department")
                            .FontSize(11).Italic().FontColor("#999999").AlignCenter();

                        col.Item().Height(60);
                        col.Item().Text("CONFIDENTIAL DOCUMENT")
                            .FontSize(10).FontColor("#D90429").Bold().AlignCenter();
                    });

                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.DefaultTextStyle(x => x.FontSize(10));
                        txt.Span("Page ");
                        txt.CurrentPageNumber();
                        txt.Span(" / ");
                        txt.TotalPages();
                    });
                });

                // =============== 2️ SUMMARY ONLY PAGE ===============
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(24);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Header().Text("1. Summary Tables").Bold().FontSize(12).AlignCenter();

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Spacing(10);
                        var headerColor = "#E9ECEF";
                        var borderColor = "#999999";
                        var rowBorder = "#CCCCCC";

                        col.Item().Column(section =>
                        {
                            section.Spacing(12);

                            // 1.1 Severity
                            section.Item().Text("1.1 Findings by Severity").Bold().FontSize(11);
                            section.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(1);
                                });

                                t.Header(h =>
                                {
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).Text("Severity").Bold();
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).Text("Count").Bold();
                                });

                                foreach (var kv in summary.SeverityBreakdown.OrderByDescending(k => k.Value))
                                {
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).Text(kv.Key);
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).AlignLeft().Text(kv.Value.ToString());
                                }
                            });

                            // 1.2 Department
                            section.Item().Text("1.2 Findings by Department").Bold().FontSize(11);

                            var deptSummary = summary.ByDepartment
                                .GroupBy(d => d.DeptName)
                                .Select(g => new
                                {
                                    DeptName = g.Key,
                                    Count = g.Sum(x => x.Count)
                                })
                                .OrderByDescending(x => x.Count)
                                .ToList();

                            section.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(1);
                                });

                                t.Header(h =>
                                {
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5)
                                        .Text("Department").Bold();
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5)
                                        .Text("Count").Bold();
                                });

                                foreach (var d in deptSummary)
                                {
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).Text(d.DeptName);
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).Text(d.Count.ToString());
                                }
                            });

                            // 1.3 Root Cause
                            section.Item().Text("1.3 Findings by Root Cause").Bold().FontSize(11);
                            section.Item().Table(t =>
                            {
                                t.ColumnsDefinition(cd =>
                                {
                                    cd.RelativeColumn(3);
                                    cd.RelativeColumn(1);
                                });

                                t.Header(h =>
                                {
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).Text("Root Cause").Bold();
                                    h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).Text("Count").Bold();
                                });

                                foreach (var r in summary.ByRootCause)
                                {
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).Text(r.RootCause);
                                    t.Cell().Border(0.5f).BorderColor(rowBorder).Padding(5).AlignLeft().Text(r.Count.ToString());
                                }
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.DefaultTextStyle(x => x.FontSize(10));
                        txt.Span("Page ");
                        txt.CurrentPageNumber();
                        txt.Span(" / ");
                        txt.TotalPages();
                    });
                });

                // =============== 3️ DETAILED FINDINGS PAGE ===============
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(24);
                    page.DefaultTextStyle(x => x.FontSize(10));
                    page.Header().Text("2. Detailed Findings").Bold().FontSize(12).AlignCenter();

                    page.Content().PaddingTop(15).Column(col =>
                    {
                        col.Spacing(12);

                        col.Item().Table(t =>
                        {
                            t.ColumnsDefinition(cd =>
                            {
                                cd.ConstantColumn(28);   
                                cd.ConstantColumn(190);  
                                cd.ConstantColumn(110);  
                                cd.ConstantColumn(60);   
                                cd.ConstantColumn(60);  
                                cd.ConstantColumn(80);   
                            });

                            string headerColor = "#E9ECEF";
                            string borderColor = "#999999";
                            string rowBorder = "#CCCCCC";

                            t.Header(h =>
                            {
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).AlignCenter().Text("#").Bold();
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).AlignCenter().Text("Title").Bold();
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).AlignCenter().Text("Department Name").Bold();
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).AlignCenter().Text("Severity").Bold();
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).AlignCenter().Text("Status").Bold();
                                h.Cell().Background(headerColor).Border(0.5f).BorderColor(borderColor).Padding(5).AlignCenter().Text("Deadline").Bold();
                            });

                            int idx = 1;
                            bool alt = false;

                            foreach (var f in findings)
                            {
                                string bg = alt ? "#F9FAFB" : "#FFFFFF";
                                alt = !alt;

                                t.Cell().Background(bg).Border(0.5f).BorderColor(rowBorder).Padding(4).AlignCenter().Text(idx++.ToString());
                                t.Cell().Background(bg).Border(0.5f).BorderColor(rowBorder).Padding(4).Text(f.Title ?? "-").LineHeight(1.25f);
                                t.Cell().Background(bg).Border(0.5f).BorderColor(rowBorder).Padding(4).Text(f.Dept.Name?.ToString() ?? "-").LineHeight(1.25f);
                                t.Cell().Background(bg).Border(0.5f).BorderColor(rowBorder).Padding(4).Text(f.Severity ?? "-");
                                t.Cell().Background(bg).Border(0.5f).BorderColor(rowBorder).Padding(4).Text(f.Status ?? "-");
                                t.Cell().Background(bg).Border(0.5f).BorderColor(rowBorder).Padding(4).AlignCenter().Text(f.Deadline?.ToString("yyyy-MM-dd") ?? "-");
                            }
                        });
                    });

                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.DefaultTextStyle(x => x.FontSize(10));
                        txt.Span("Page ");
                        txt.CurrentPageNumber();
                        txt.Span(" / ");
                        txt.TotalPages();
                    });
                });


                // =============== 4 SIGNATURE PAGE ==============
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);

                    page.Content().Column(col =>
                    {
                        col.Spacing(20);
                        col.Item().Text("Approval and Signatures").Bold().FontSize(14).AlignCenter();

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Border(1).Padding(20).Column(c =>
                            {
                                c.Item().Text("Prepared by:").Bold();
                                c.Item().Height(60);
                                c.Item().Text("Signature: ____________________________");
                                c.Item().Text("Name: ____________________________");
                                c.Item().Text("Date: ____________________________");
                            });

                            row.RelativeItem().Border(1).Padding(20).Column(c =>
                            {
                                c.Item().Text("Reviewed by (Lead Auditor):").Bold();
                                c.Item().Height(60);
                                c.Item().Text("Signature: ____________________________");
                                c.Item().Text("Name: ____________________________");
                                c.Item().Text("Date: ____________________________");
                            });
                        });

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Border(1).Padding(20).Column(c =>
                            {
                                c.Item().Text("Approved by (Director QA):").Bold();
                                c.Item().Height(60);
                                c.Item().Text("Signature: ____________________________");
                                c.Item().Text("Name: ____________________________");
                                c.Item().Text("Date: ____________________________");
                            });
                        });
                    });

                    page.Footer().AlignCenter().Text(txt =>
                    {
                        txt.DefaultTextStyle(x => x.FontSize(10));
                        txt.Span("Page ");
                        txt.CurrentPageNumber();
                        txt.Span(" / ");
                        txt.TotalPages();
                    });
                });

            });

            return pdf.GeneratePdf();
        }
    }
}
