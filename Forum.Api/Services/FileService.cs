using Forum.Api.Interfaces;

namespace Forum.Api.Services;

public class FileService : IFileService
{
	public async Task CreateFileAsync(string path, IFormFile formFile)
	{
		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		using (FileStream fileStream = new FileStream(path, FileMode.OpenOrCreate))
		{
			await formFile.CopyToAsync(fileStream);
		}
	}

	public void RemoveFile(string path)
	{
		File.Delete(path);
	}
	
	public void RemoveDirectory(string path)
	{
		Directory.Delete(path, true);
	}

	public bool IsFileContentType(IFormFile file, params string[] contentTypes)
	{
		if (contentTypes.Contains(file.ContentType)) return true;
		
		return false;
	}
	
	public bool IsFileContentType(IFormFileCollection files, params string[] contentTypes)
	{
		foreach (var file in files)
			if (contentTypes.Contains(file.ContentType)) return true;

		return false;
	}
}
