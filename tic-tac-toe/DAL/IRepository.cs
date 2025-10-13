using System.Collections.Specialized;

namespace DAL;

/// "TData" either Game or Configuration
public interface IRepository<TData>
{
    List<string> List();
    
    //crud
    string Save(TData data);
    TData Load(string id);
    void Delete(string id);
}