using System.Text.Json;
using BLL;

using System;
using System.IO;
using System.Linq;

namespace DAL;

public class ConfigRepositoryJson : IRepository<GameConfiguration>
{
    public List<(string id, string description)> List()
    {
        var dir = FilesystemHelpers.GetConfigDirectory();
        var res = new List<(string id, string description)>();

        foreach (var fullFileName in Directory.EnumerateFiles(dir))
        {  
            var fileName = Path.GetFileName(fullFileName);
            if (!fileName.EndsWith(".json")) continue;
            res.Add(
                (Path.GetFileName(fileName),
            Path.GetFileNameWithoutExtension(fileName))
            );
        }

        return res;
    }
    
    public async Task<List<(string id, string description)>> ListAsync()
    {
        // TODO finish method?
        return List();
    }

    
    public string Save(GameConfiguration data)
    {
        var jsonStr = JsonSerializer.Serialize(data);

        // sanitize filename
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = new string(data.Name.Where(c => !invalidChars.Contains(c)).ToArray());
        safeName = safeName.Replace(' ', '_').Trim();
        
        var fileName = $"{safeName} - {data.BoardWidth}x{data.BoardHeight} - win_{data.WinCondition} - {data.CreatedAt}" + ".json";
        var fullFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + fileName;
        File.WriteAllText(fullFileName, jsonStr);
        

        return fileName;
    }

    public GameConfiguration Load(string id)
    {
        var fileName = id.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            ? id
            : id + ".json";
        var jsonFileName = Path.Combine(FilesystemHelpers.GetConfigDirectory(), fileName);
        var jsonText = File.ReadAllText(jsonFileName);
        var conf = JsonSerializer.Deserialize<GameConfiguration>(jsonText);

        return conf ?? throw new NullReferenceException("Json deserialization returned null. Data: " + jsonText);
    }

    public void Delete(string id)
    {
        if (!id.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            id += ".json";

        var jsonFileName = Path.Combine(FilesystemHelpers.GetConfigDirectory(), id);
        if (File.Exists(jsonFileName))
            File.Delete(jsonFileName);
    }

    public string Update(GameConfiguration data, string oldFileName)
    {
        var configDir = FilesystemHelpers.GetConfigDirectory();
        var oldFullPath = Path.Combine(configDir, oldFileName);
        
        if (!oldFullPath.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            oldFullPath += ".json";
        }
        
        if (!File.Exists(oldFullPath))
            throw new FileNotFoundException("Config file not found.", oldFullPath);
        
        var jsonStr = JsonSerializer.Serialize(data);

        // sanitize filename
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = new string(data.Name.Where(c => !invalidChars.Contains(c)).ToArray());
        safeName = safeName.Replace(' ', '_').Trim();
        
        data.CreatedAt = DateTime.Now.ToString("HH_mm_ddMMyyyy");
        
        var newFileName = $"{safeName} - {data.BoardWidth}x{data.BoardHeight} - win_{data.WinCondition}_{data.CreatedAt}.json";
        var newFullPath = Path.Combine(configDir, newFileName);

        // delete old file
        if (!oldFullPath.Equals(newFullPath, StringComparison.OrdinalIgnoreCase))
        {
            File.Delete(oldFullPath);
        }

        // Save new
        File.WriteAllText(newFullPath, jsonStr);

        return newFileName;
    }
}