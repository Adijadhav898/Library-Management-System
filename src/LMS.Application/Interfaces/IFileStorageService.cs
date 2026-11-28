using Microsoft.AspNetCore.Http;

namespace LMS.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    Task<byte[]?> GetFileAsync(string fileUrl, CancellationToken cancellationToken = default);
    string GetFileUrl(string relativePath);
}
