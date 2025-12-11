using System.Text.Json;
using BLL;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace DAL;

public class ConfigRepositoryJson : IRepository<GameConfiguration>
{
    private string GetFilePath(GameConfiguration data)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = new string(data.Name.Where(c => !invalidChars.Contains(c)).ToArray());
        safeName = safeName.Replace(' ', '_').Trim();
        var fileName = $"{safeName} - {data.BoardWidth}x{data.BoardHeight} - win_{data.WinCondition} - {data.CreatedAt}.json";
        return Path.Combine(FilesystemHelpers.GetConfigDirectory(), fileName);
    }

    private string GetFilePathById(string id)
    {
        var dir = FilesystemHelpers.GetConfigDirectory();
        foreach (var fullFileName in Directory.EnumerateFiles(dir, "*.json"))
        {
            var jsonText = File.ReadAllText(fullFileName);
            var conf = JsonSerializer.Deserialize<GameConfiguration>(jsonText);
            if (conf != null && conf.Id.ToString() == id)
                return fullFileName;
        }
        throw new FileNotFoundException($"No configuration found with Id {id}");
    }

    public List<(string id, string description)> List()
    {
        var dir = FilesystemHelpers.GetConfigDirectory();
        var res = new List<(string id, string description)>();

        foreach (var fullFileName in Directory.EnumerateFiles(dir, "*.json"))
        {
            var jsonText = File.ReadAllText(fullFileName);
            var conf = JsonSerializer.Deserialize<GameConfiguration>(jsonText);
            if (conf == null) continue;
            res.Add((conf.Id.ToString(), $"{conf.Name} - {conf.BoardWidth}x{conf.BoardHeight} - win_{conf.WinCondition} - {conf.CreatedAt}"));
        }

        return res;
    }

    public string Save(GameConfiguration data)
    {
        if (data.Id == Guid.Empty)
            data.Id = Guid.NewGuid();

        data.BoardData = data.Board == null ? null : JsonSerializer.Serialize(data.Board);
        var path = GetFilePath(data);
        File.WriteAllText(path, JsonSerializer.Serialize(data));
        return data.Id.ToString();
    }

    public GameConfiguration Load(string id)
    {
        var path = GetFilePathById(id);
        var jsonText = File.ReadAllText(path);
        var conf = JsonSerializer.Deserialize<GameConfiguration>(jsonText);
        if (conf == null) throw new FileNotFoundException($"No configuration found with Id {id}");

        if (!string.IsNullOrEmpty(conf.BoardData))
            conf.Board = JsonSerializer.Deserialize<List<List<ECellState>>>(conf.BoardData);

        return conf;
    }

    public void Delete(string id)
    {
        var path = GetFilePathById(id);
        File.Delete(path);
    }

    public string Update(GameConfiguration data, string oldFileName)
    {
        var oldFullPath = Path.Combine(FilesystemHelpers.GetConfigDirectory(), oldFileName);
        if (!File.Exists(oldFullPath))
            throw new FileNotFoundException("Config file not found.", oldFullPath);

        data.CreatedAt = DateTime.Now.ToString("HH_mm_ddMMyyyy");
        data.BoardData = data.Board == null ? null : JsonSerializer.Serialize(data.Board);

        var newFullPath = GetFilePath(data);

        if (!oldFullPath.Equals(newFullPath, StringComparison.OrdinalIgnoreCase))
            File.Delete(oldFullPath);

        File.WriteAllText(newFullPath, JsonSerializer.Serialize(data));
        return data.Id.ToString();
    }

    // ASYNC CRUD
    public async Task<List<(string id, string description)>> ListAsync()
    {
        return await Task.Run(() => List());
    }

    public async Task<GameConfiguration> LoadAsync(string id)
    {
        return await Task.Run(() => Load(id));
    }

    public async Task<string> SaveAsync(GameConfiguration data)
    {
        return await Task.Run(() => Save(data));
    }

    public async Task DeleteAsync(string id)
    {
        await Task.Run(() => Delete(id));
    }

    public async Task<string> UpdateAsync(GameConfiguration data, string oldFileName)
    {
        // Ensure the BoardData is up-to-date
        if (data.Board != null)
            data.BoardData = JsonSerializer.Serialize(data.Board);

        // Update timestamp
        data.CreatedAt = DateTime.Now.ToString("HH_mm_ddMMyyyy");

        var oldFullPath = Path.Combine(FilesystemHelpers.GetConfigDirectory(), oldFileName);
        var newFullPath = GetFilePath(data);

        if (!oldFullPath.Equals(newFullPath, StringComparison.OrdinalIgnoreCase))
            File.Delete(oldFullPath);

        await File.WriteAllTextAsync(newFullPath, JsonSerializer.Serialize(data));

        return data.Id.ToString();
    }

}
