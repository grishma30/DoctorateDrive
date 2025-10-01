using Microsoft.AspNetCore.Http;
using System;

namespace DoctorateDrive.DTOs
{
    public class StudentDocumentDto
    {
        public int StudentId { get; set; }
        public IFormFile GraduateCertificate { get; set; }
        public IFormFile PostGraduateCertificate { get; set; }
        public IFormFile GateCertificate { get; set; }
        public IFormFile ConversionCertificate { get; set; }
    }
}
