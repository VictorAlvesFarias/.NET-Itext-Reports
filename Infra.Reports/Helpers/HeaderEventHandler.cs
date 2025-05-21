using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Kernel.Pdf.Xobject;

public class HeaderEventHandler : AbstractPdfDocumentEventHandler
{
    private PdfFormXObject HeaderForm;
    private float HeaderHeight;

    public HeaderEventHandler(PdfFormXObject headerForm, float headerHeight)
    {
        this.HeaderForm = headerForm;
        this.HeaderHeight = headerHeight;
    }

    protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
    {
        var docEvent = (PdfDocumentEvent)@event;
        var pdfDoc = docEvent.GetDocument();
        var page = docEvent.GetPage();
        var canvas = new PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDoc);
        var pageSize = page.GetPageSize();

        canvas.SaveState();
        canvas.SetFillColor(ColorConstants.BLUE);

        // Desenha o fundo azul no cabeçalho
        canvas.Rectangle(0, pageSize.GetTop() - HeaderHeight, pageSize.GetWidth(), HeaderHeight);
        canvas.Fill();
        canvas.RestoreState();

        canvas.AddXObjectAt(HeaderForm, 0, pageSize.GetTop() - HeaderHeight);
        canvas.Release();
    }
}
