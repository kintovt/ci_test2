
public interface IModelSaver
{
    void Save<T>(T model, string name) where T : class;
}
