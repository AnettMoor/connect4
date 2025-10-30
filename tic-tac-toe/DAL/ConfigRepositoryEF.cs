using BLL;

namespace DAL;

// DATABASE SIDE 
public class ConfigRepositoryEF : IRepository<GameConfiguration>
{
    private readonly AppDbContext _dbContext;

    public ConfigRepositoryEF(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    // TODO instead of key display timestamp
    public List<(string id, string description)> List()
    {
        var res = new List<(string id, string description)>();
        foreach (var dbConf in (_dbContext.GameConfigurations))
        {
            res.Add(
                (
                    dbConf.Id.ToString(),
                    dbConf.Name
                )
            );
        }
        return res;
    }

    public string Save(GameConfiguration data)
    {
        var newEntity = new GameConfiguration
        {
            Id = Guid.NewGuid(),
            Name = data.Name,
            BoardWidth = data.BoardWidth,
            BoardHeight = data.BoardHeight,
            WinCondition = data.WinCondition,
        };
        _dbContext.GameConfigurations.Add(newEntity);
        data.Id = newEntity.Id;
        
        _dbContext.SaveChanges();
        
        return data.Id.ToString();
    }

    public GameConfiguration Load(string id)
    {
        if (!Guid.TryParse(id, out var guid))
            throw new ArgumentException("Invalid configuration ID", nameof(id));

        var entity = _dbContext.GameConfigurations.FirstOrDefault(c => c.Id == guid);

        if (entity == null)
            throw new KeyNotFoundException($"No configuration found with ID {id}");

        var conf = new GameConfiguration
        {
            Id = entity.Id,
            Name = entity.Name,
            BoardWidth = entity.BoardWidth,
            BoardHeight = entity.BoardHeight,
            WinCondition = entity.WinCondition,
        };
        return conf;
    }

    public void Delete(string id)
    {
        if (!Guid.TryParse(id, out var guid))
            throw new ArgumentException("Invalid configuration ID", nameof(id));

        var entity = _dbContext.GameConfigurations.FirstOrDefault(c => c.Id == guid);

        if (entity == null)
            throw new KeyNotFoundException($"No configuration found with ID {id}");

        _dbContext.GameConfigurations.Remove(entity);
        _dbContext.SaveChanges();
            
    }

    public string Update(GameConfiguration data, string oldFileName)
    {
        var entity = _dbContext.GameConfigurations.FirstOrDefault(c => c.Id == data.Id);
        if (entity == null)
            throw new KeyNotFoundException($"No configuration found with ID {data.Id}");
        
        entity.Name = data.Name;
        entity.BoardWidth = data.BoardWidth;
        entity.BoardHeight = data.BoardHeight;
        entity.WinCondition = data.WinCondition;

        _dbContext.SaveChanges();

        return entity.Id.ToString();
    }
}
