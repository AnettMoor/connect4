using System.Collections.Specialized;
using BLL;

namespace DAL;

/// "TData" either Game or Configuration
public interface IRepository<TData>
{
    List<(string id, string description)> List();
    Task<List<(string id, string description)>> ListAsync();
    
    //crud
    string Save(TData data);
    TData Load(string id);
    void Delete(string id);
    string Update(TData data, string oldFileName);
    
    // async crud
    Task<GameConfiguration> LoadAsync(string id);
    Task<string> SaveAsync(TData data);
    Task DeleteAsync(string id);
    Task<string> UpdateAsync(TData data, string oldFileName);
    

}