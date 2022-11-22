using System.Threading.Tasks;

public interface IModelLoader
{
    Task<T> Load<T>(string name) where T : class;
    Task<T> LocalLoading<T>(string name) where T : class;
    Task<T> LoadFromResources<T>(string name) where T : class;
}
