using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Game._Scripts.Runtime.UnityServices.Save
{
    public class PlayerPrefClient : ISaveClient
    {
        public Task Save(string key, object value)
        {
            if (value is string s)
            {
                PlayerPrefs.SetString(key.Replace(" ", "_"), s);
            }
            else
            {
                var data = JsonConvert.SerializeObject(value);
                PlayerPrefs.SetString(key.Replace(" ", "_"), data);
            }

            return Task.CompletedTask;
        }

        public async Task Save(params (string key, object value)[] values)
        {
            foreach (var (key, value) in values)
            {
                await Save(key, value);
            }
        }

        public Task<T> Load<T>(string key)
        {
            var data = PlayerPrefs.GetString(key.Replace(" ", "_"));
            if (!string.IsNullOrEmpty(data)) return Task.FromResult(JsonConvert.DeserializeObject<T>(data));
            return Task.FromResult<T>(default);
        }

        public async Task<IEnumerable<T>> Load<T>(params string[] keys)
        {
            return await Task.WhenAll(keys.Select(Load<T>));
        }

        public Task Delete(string key)
        {
            throw new System.NotImplementedException();
        }

        public Task DeleteAll()
        {
            throw new System.NotImplementedException();
        }
    }
}