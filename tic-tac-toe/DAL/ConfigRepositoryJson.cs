using System.Text.Json;
using BLL;

using System;
using System.IO;
using System.Linq;

namespace DAL;

public class ConfigRepositoryJson : IRepository<GameConfiguration>
{
    public List<string> List()
    {
        var dir = FilesystemHelpers.GetConfigDirectory();
        var res = new List<string>();

        foreach (var fullFileName in Directory.EnumerateFiles(dir))
        {  
            var fileName = Path.GetFileName(fullFileName);
            if (!fileName.EndsWith(".json")) continue;
            res.Add(Path.GetFileNameWithoutExtension(fileName));
        }

        return res;
    }
    
    public string Save(GameConfiguration data)
    {
        var jsonStr = JsonSerializer.Serialize(data);

        // sanitize filename
        var invalidChars = Path.GetInvalidFileNameChars();
        var safeName = new string(data.Name.Where(c => !invalidChars.Contains(c)).ToArray());
        safeName = safeName.Replace(' ', '_').Trim();
        
        var timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"{safeName} - {data.BoardWidth}x{data.BoardHeight} - win_{data.WinCondition}_{timeStamp}" + ".json";
        var fullFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + fileName;
        File.WriteAllText(fullFileName, jsonStr);

        return fileName;
    }

    public GameConfiguration Load(string id)
    {
        var jsonFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + id + ".json";
        var jsonText = File.ReadAllText(jsonFileName);
        var conf = JsonSerializer.Deserialize<GameConfiguration>(jsonText);

        return conf ?? throw new NullReferenceException("Json deserialization returned null. Data: " + jsonText);
    }

    public void Delete(string id)
    {
        var jsonFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + id + ".json";
        if (File.Exists(jsonFileName))
        {
            File.Delete(jsonFileName);
        }
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
        
        var timeStamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var newFileName = $"{safeName} - {data.BoardWidth}x{data.BoardHeight} - win_{data.WinCondition}_{timeStamp}.json";
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