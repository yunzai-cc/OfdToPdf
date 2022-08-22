namespace OfdToPdf.Responses;

public class OfdToPdfResponse
{
    public int Code { get; set; }
    public string PdfUrl { get; set; }
    public string? Message { get; set; }
}