using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Game._Scripts.Runtime.Models;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models;
using UnityEngine;

namespace Game._Scripts.Runtime.UnityServices.Save
{
    public class CloudSaveClient : ISaveClient
    {
        public async Task Save(string key, object value)
        {
            try
            {
                var data = new Dictionary<string, object>{{key.Replace(" ", "_"), value}};
                
                // Saving the data without write lock validation by passing the data as an object instead of a SaveItem
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);

                Debug.Log(
                    $"Successfully saved Unit: {key}"
                );
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        public async Task Save(params (string key, object value)[] values)
        {
            try
            {
                var data = values.ToDictionary(item => item.key.Replace(" ", "_"), item => item.value);
                
                // Saving the data without write lock validation by passing the data as an object instead of a SaveItem
                await CloudSaveService.Instance.Data.Player.SaveAsync(data);

                Debug.Log(
                    $"Successfully saved Units"
                );
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }
        }

        public async Task<T> Load<T>(string key)
        {
            try
            {
                var results = await CloudSaveService.Instance.Data.Player.LoadAsync(
                    new HashSet<string> { key.Replace(" ", "_") }
                );

                if (results.TryGetValue(key, out var item))
                    return item.Value.GetAs<T>();
                else
                    Debug.Log($"There is no such key as {key}!");
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }

            return default;
        }

        public async Task<IEnumerable<T>> Load<T>(params string[] keys)
        {
            try
            {
                Dictionary<string, Item> results = await CloudSaveService.Instance.Data.Player.LoadAsync(keys.Select(k => k.Replace(" ", "_")).ToHashSet());

                List<UnitSaveData> data = new();

                foreach (var key in keys)
                {
                    if(results.Keys.Contains(key))
                    {
                        var unitData = results[key].Value.GetAs<T>();
                        data.Add(unitData as UnitSaveData);
                    }
                    else
                    {
                        data.Add(null);
                    }
                }
                
                /*foreach (var value in results.Values)
                {
                    var unitData = value.Value.GetAs<T>();
                    data.Add(unitData as UnitSaveData);
                }*/

                return (IEnumerable<T>)data;
            }
            catch (CloudSaveValidationException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveRateLimitedException e)
            {
                Debug.LogError(e);
            }
            catch (CloudSaveException e)
            {
                Debug.LogError(e);
            }

            return default;
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