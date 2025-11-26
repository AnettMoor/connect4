using System.Collections.Specialized;

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

}