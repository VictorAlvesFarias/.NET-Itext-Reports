using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Kernel.Pdf.Xobject;

public class FooterEventHandler : AbstractPdfDocumentEventHandler
{
    private PdfFormXObject FooterForm;
    private float FooterHeight;

    public FooterEventHandler(PdfFormXObject footerForm, float footerHeight)
    {
        this.FooterForm = footerForm;
        this.FooterHeight = footerHeight;
    }
    protected override void OnAcceptedEvent(AbstractPdfDocumentEvent currentEvent)
    {
        var docEvent = (PdfDocumentEvent)currentEvent;
        var pdfDoc = docEvent.GetDocument();
        var page = docEvent.GetPage();
        var canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
        var pageSize = page.GetPageSize();

        canvas.SaveState();
        canvas.SetFillColor(ColorConstants.BLUE);

        // Desenha o fundo azul no rodapé
        canvas.Rectangle(0, pageSize.GetBottom(), pageSize.GetWidth(), FooterHeight);
        canvas.Fill();
        canvas.RestoreState();

        canvas.AddXObjectAt(FooterForm, 0, pageSize.GetBottom());
        canvas.Release();

    }
}
