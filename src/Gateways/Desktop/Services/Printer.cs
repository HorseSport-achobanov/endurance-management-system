﻿using EnduranceJudge.Core.ConventionalServices;
using EnduranceJudge.Core.Services;
using System.IO;
using System.Printing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Xps.Packaging;

namespace EnduranceJudge.Gateways.Desktop.Services;

public class Printer : IPrinter
{
    private readonly IFileService fileService;

    public Printer(IFileService fileService)
    {
        this.fileService = fileService;
    }

    public void Print(Visual visual)
    {
        var filePath = Path.GetRandomFileName();
        try
        {
            var printDialog = new PrintDialog
            {
                PageRangeSelection = PageRangeSelection.AllPages,
                UserPageRangeEnabled = true
            };
            var shouldPrint = printDialog.ShowDialog();
            if (shouldPrint ?? false)
            {
                using var writeDoc = new XpsDocument(filePath, FileAccess.ReadWrite);
                var writer = XpsDocument.CreateXpsDocumentWriter(writeDoc);
                writer.Write(visual);
                var fds = writeDoc.GetFixedDocumentSequence();
                var paginator = fds.DocumentPaginator;
                var pageMediaSize = new PageMediaSize(PageMediaSizeName.ISOA4);
                printDialog.PrintTicket.PageMediaSize = pageMediaSize;
                printDialog.PrintDocument(paginator, "Print");
            }
        }
        finally
        {
            this.fileService.Delete(filePath);
        }
    }
}

public interface IPrinter : ITransientService
{
    void Print(Visual visual);
}
