using DoctorateDrive.Models;
using Microsoft.EntityFrameworkCore;

namespace DoctorateDrive.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly DoctorateDriveContext _context;

        public DocumentRepository(DoctorateDriveContext context)
        {
            _context = context;
        }

        public async Task<Document> AddDocumentAsync(Document document)
        {
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();
            return document;
        }
    }

    public interface IDocumentRepository
    {
        Task<Document> AddDocumentAsync(Document document);
    }
}
