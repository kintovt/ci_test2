using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class ModelSaver : IModelSaver
{
    private const string MODELS_FOLDER = "UserState";
    private JsonSerializerSettings _serializerSettings;

    public ModelSaver()
    {
        _serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }

    public async void Save<T>(T model, string name) where T : class
    {
        var directory = GetDirectory();
        try
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            using (var sw = new StreamWriter(GetPath(directory, name), false))
            {
                var line = JsonConvert.SerializeObject(model, _serializerSettings);
                await sw.WriteAsync(line);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private string GetPath(string directory, string name)
    {
        return Path.Combine(directory, $"{name}.json");
    }

    private string GetDirectory()
    {
        return Path.Combine(Application.persistentDataPath, MODELS_FOLDER);
    }
}
