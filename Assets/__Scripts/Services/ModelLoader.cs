using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class ModelLoader : IModelLoader
{
    private const string MODELS_FOLDER = "UserState";
    private JsonSerializerSettings _serializerSettings;

    public ModelLoader()
    {
        _serializerSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
    }

    public async Task<T> Load<T>(string name) where T : class
    {
        var result = await LocalLoading<T>(name);
        
        if(result == null)
        {
            result = await LoadFromResources<T>(name);
        }

        if (result == null)
        {
            Debug.LogWarning($"Failed to load {name}");
        }

        return result;
    }

    public async Task<T> LoadFromResources<T>(string name) where T: class
    {
        try
        {
            var textAsset = Resources.Load<TextAsset>(Path.Combine(MODELS_FOLDER, name));
            var model = JsonConvert.DeserializeObject<T>(textAsset.text);
            return model;
        }
        catch(Exception e)
        {
            Debug.LogWarning(e);
            return null;
        }
    }

    public async Task<T> LocalLoading<T>(string name) where T : class
    {
        T result = null;
        var path = Path.Combine(Application.persistentDataPath, MODELS_FOLDER, $"{name}.json");
        try
        {
            result = await Loading<T>(path);
        }
        catch(Exception e)
        {
            Debug.LogWarning(e);
        }
        return result;
    }

    private async Task<T> Loading<T>(string path) where T : class
    {
        try
        {
            using (StreamReader sr = new StreamReader(new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read)))
            {

                var modelString = await sr.ReadToEndAsync();
                var model = JsonConvert.DeserializeObject<T>(modelString, _serializerSettings);
                return model;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }

        return null;
    }
}
