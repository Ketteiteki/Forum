namespace Forum.Api.Interfaces;

public interface IFileService
{
	Task CreateFileAsync(string path, IFormFile formFile);

	void RemoveFile(string path);
	
	void RemoveDirectory(string path);
	
	bool IsFileContentType(IFormFile file, params string[] contentTypes);

	bool IsFileContentType(IFormFileCollection files, params string[] contentTypes);
}