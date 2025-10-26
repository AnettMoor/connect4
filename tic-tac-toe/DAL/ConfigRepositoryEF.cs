using BLL;

namespace DAL;

public class ConfigRepositoryEF : IRepository<GameConfiguration>
{
    private readonly AppDbContext _dbContext;

    public ConfigRepositoryEF(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
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
        throw new NotImplementedException();
    }

    public GameConfiguration Load(string id)
    {
        throw new NotImplementedException();
    }

    public void Delete(string id)
    {
        throw new NotImplementedException();
    }

    public string Update(GameConfiguration data, string oldFileName)
    {
        throw new NotImplementedException();
    }
}