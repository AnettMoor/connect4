using System.Text.Json;
using BLL;
using Microsoft.EntityFrameworkCore;

namespace DAL;

// DATABASE SIDE 
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
                    $"{dbConf.Name} - {dbConf.BoardWidth}x{dbConf.BoardHeight} - win_{dbConf.WinCondition} - {dbConf.CreatedAt}"
            
                )
            );
        }
        return res;
    }
    
    public async Task<List<(string id, string description)>> ListAsync()
    {
        var res = new List<(string id, string description)>();

        foreach (var dbConf in await _dbContext.GameConfigurations.ToListAsync())
        {
            res.Add(
                (
                    dbConf.Id.ToString(),
                    $"{dbConf.Name} - {dbConf.BoardWidth}x{dbConf.BoardHeight} - win_{dbConf.WinCondition} - {dbConf.CreatedAt}"
                )
            );
        }
        return res;
    }


    public string Save(GameConfiguration data)
    {
        data.BoardData = data.Board == null ? null : JsonSerializer.Serialize(data.Board);
        
        if (data.Id == Guid.Empty)
            data.Id = Guid.NewGuid();
        
        _dbContext.GameConfigurations.Add(data);
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

        if (!string.IsNullOrEmpty(entity.BoardData))
            entity.Board = JsonSerializer.Deserialize<List<List<ECellState>>>(entity.BoardData);
        return entity;
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
        
        entity.BoardData = data.Board == null ? null : JsonSerializer.Serialize(data.Board);
        entity.CreatedAt = DateTime.Now.ToString("HH_mm_ddMMyyyy");

        _dbContext.SaveChanges();

        return entity.Id.ToString();
    }



      public async Task<GameConfiguration> LoadAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                throw new ArgumentException("Invalid configuration ID", nameof(id));

            var entity = await _dbContext.GameConfigurations.FirstOrDefaultAsync(c => c.Id == guid);
            if (entity == null)
                throw new KeyNotFoundException($"No configuration found with ID {id}");

            if (!string.IsNullOrEmpty(entity.BoardData))
                entity.Board = JsonSerializer.Deserialize<List<List<ECellState>>>(entity.BoardData);

            return entity;
        }

        public async Task<string> SaveAsync(GameConfiguration data)
        {
            data.BoardData = data.Board == null ? null : JsonSerializer.Serialize(data.Board);

            if (data.Id == Guid.Empty)
                data.Id = Guid.NewGuid();

            await _dbContext.GameConfigurations.AddAsync(data);
            await _dbContext.SaveChangesAsync();

            return data.Id.ToString();
        }

        public async Task DeleteAsync(string id)
        {
            if (!Guid.TryParse(id, out var guid))
                throw new ArgumentException("Invalid configuration ID", nameof(id));

            var entity = await _dbContext.GameConfigurations.FirstOrDefaultAsync(c => c.Id == guid);
            if (entity == null)
                throw new KeyNotFoundException($"No configuration found with ID {id}");

            _dbContext.GameConfigurations.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> UpdateAsync(GameConfiguration data, string oldFileName)
        {
            var entity = await _dbContext.GameConfigurations.FirstOrDefaultAsync(c => c.Id == data.Id);
            if (entity == null)
                throw new KeyNotFoundException($"No configuration found with ID {data.Id}");

            entity.BoardData = data.Board == null ? null : JsonSerializer.Serialize(data.Board);
            entity.CreatedAt = DateTime.Now.ToString("HH_mm_ddMMyyyy");

            await _dbContext.SaveChangesAsync();

            return entity.Id.ToString();
        }
    }

