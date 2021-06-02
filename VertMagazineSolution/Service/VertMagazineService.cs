using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VertMagazineSolution.Model;
using VertMagazineSolution.Utils;

namespace VertMagazineSolution.Service
{
    public class VertMagazineService
    {
        private readonly MemoryCache _cache;
        private readonly HttpClientWrapper _client;
        private const string _categoryCacheName = "category";
        private const string _magazinesCacheName = "magazinnes";
        public VertMagazineService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
            _client = new HttpClientWrapper();
        }

        /// <summary>
        /// 1. get category list and subscriber list
        /// 2. get magazine for each category
        /// 3. assuming that maganize id is unique across categories
        /// 4. looping each subscribers and adding category into dictionory of each subscribers from match maganize id
        /// 5. retrive subscribers who has atleat one magazine from each category by comparing count
        /// 6. check answer are corect and timing from answers api
        /// </summary>
        /// <returns></returns>
        public async Task<Answers> GetResults()
        {
            List<Magazines> magazines = new List<Magazines>();
            AnswerRequest request = new AnswerRequest();
            Dictionary<string, List<string>> subscriberCategories = new Dictionary<string, List<string>>();

            // get category list and subscriber list
            var categorylist = await getCategoriesAsync();
            var subscrbers = await getSubscribersAsync();

            //get magazine for each category
            categorylist.ForEach( c => {

                var magResult =  getMagazinesAsync(c).Result;
                magazines.AddRange(magResult);
            });

            //assuming that maganize id is unique across categories
            // looping each subscribers and adding category into dictionory of each subscribers from match maganize id
            subscrbers.ForEach(s => {
                s.magazineIds?.ForEach(id => {
                    var category=magazines.FirstOrDefault(m => m.id == id)?.category;
                    if(!string.IsNullOrEmpty(category))
                    {
                        if(subscriberCategories.TryGetValue(s.id,out List<string> sCategories))
                        {
                            if(!sCategories.Any(sc=>sc == category))
                            {
                                sCategories.Add(category);
                                subscriberCategories[s.id] = sCategories;
                            }
                            
                        }
                        else
                        {
                            sCategories = new List<string>
                            {
                                category
                            };
                            subscriberCategories.TryAdd(s.id, sCategories);
                        }
                        
                    }
                                   
                });
            });

            //retrive subscribers who has atleat one magazine from each category by comparing count
            request.subscribers=subscriberCategories.Where(s => s.Value.Count() == categorylist.Count()).Select(k=>k.Key).ToList();
            //check answer are corect and timing from answers api
            var result= await getAnswersAsync(request);
            return result;

        }


        #region private
        private void UpsertCache<T>(string key,T obj)
        {
           if( _cache.TryGetValue(key, out T _))
            {
                _cache.Remove(key);
            }

           //set cache with absolute expire of 10 sec
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromSeconds(10));
            _cache.Set(key, obj, cacheEntryOptions);
        }


        private async Task<List<string>> getCategoriesAsync()
        {
            if(!_cache.TryGetValue(_categoryCacheName,out List<string> result))
            {
                var response = await _client.Get<string>(API.Categories);
                result = response.success ? response.data : throw new Exception($"Error while getting categories with message: {response.message}");
                UpsertCache(_categoryCacheName, result);
            }
            return result;
        }
        private async Task<List<Magazines>> getMagazinesAsync(string category)
        {
            var cacheName = $"{_magazinesCacheName}-{category}";
            if (!_cache.TryGetValue(cacheName, out List<Magazines> result))
            {
                var response = await _client.Get<Magazines>($"{API.Magazines}{category}");
                result = response.success ? response.data : throw new Exception($"Error while getting magazines with message: {response.message}");
                UpsertCache(cacheName, result);
            }
            return result;
        }

        private async Task<List<Subscriber>> getSubscribersAsync()
        {
                var response = await _client.Get<Subscriber>(API.Subscriber);
                return response.success ? response.data : throw new Exception($"Error while getting subscriber with message: {response.message}");                
        }

        private async Task<Answers> getAnswersAsync(AnswerRequest request)
        {
            var response = await _client.Post<Answers, AnswerRequest>(API.Answers,request);
            return response.success ? response.data : throw new Exception($"Error while getting answers with message: {response.message} and answer should be {string.Join(',',response.data.shouldBe)}");
        }

        #endregion
    }
}
