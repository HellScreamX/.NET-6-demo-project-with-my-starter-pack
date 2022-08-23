#nullable disable
using System.Net;

namespace Project.BusinessLogicLayer.CustomModels
{
    public class S3Response
    {
        public HttpStatusCode Status { get; set; }
        public string Message { get; set; }
    }
}
