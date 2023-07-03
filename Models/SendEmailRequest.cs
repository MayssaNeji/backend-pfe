using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace LTMS.Models;
public class SendEmailRequest
{
    public string Mail { get; set; }
    public IFormFile Facture { get; set; }
}

