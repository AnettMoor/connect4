using System.Text.Json;
using BLL;

namespace DAL;

public class ConfigRepositoryJson : IRepository<GameConfiguration>
{
    public List<string> List()
    {
        var dir = FilesystemHelpers.GetConfigDirectory();
        var res = new List<string>();

        foreach (var fullFilename in Directory.EnumerateFiles(dir))
        {
            var fileName = Path.GetFileName(fullFilename);
            if (!fullFilename.EndsWith(".json")) continue;
            var fileNameNoExtension = Path.GetFileNameWithoutExtension(fullFilename);
        }

        return res;
    }

    // TODO: what if we just need to update already existing config
    public string Save(GameConfiguration data)
    {
        // data -> json
        var jsonStr = JsonSerializer.Serialize(data);

        // save the data:

        // filename
        // TODO: sanitize data.Name, its unsafe to use it directly
        var filename = $"{data.Name} - {data.BoardWidth}x{data.BoardHeight} - win: {data.WinCondition}" + ".json";

        // file location
        var fullFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + filename;
        
        // save it
        File.WriteAllText(fullFileName, jsonStr);
        return filename;
    }
    
    public  GameConfiguration Load(string id)
    {
        var jsonFileName = FilesystemHelpers.GetConfigDirectory() + Path.DirectorySeparatorChar + id + ".json";
        var jsonText =  File.ReadAllText(jsonFileName);
        var conf =  JsonSerializer.Deserialize<GameConfiguration>(jsonText);
        
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
}