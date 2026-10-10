namespace ECommerce.UseCases.Common.Models;

public sealed record FileUpload( Func<Stream> OpenReadStream, string FileName, string ContentType,long Length);
