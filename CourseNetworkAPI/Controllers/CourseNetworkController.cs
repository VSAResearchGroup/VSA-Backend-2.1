using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
namespace CourseNetworkAPI.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class CourseNetworkController : Controller
    {
        private const int CACHE_TIMEOUT = 5;
        private IMemoryCache _cache; // Cache Object to help us find and maintain
                                     // a course network object.

        public CourseNetworkController(IMemoryCache memCache) {
            _cache = memCache;
        }

        // GET: api/CourseNetwork
        [HttpGet]
        public IEnumerable<string> Get(int course)
        {
            CourseNetwork cn;
            if (!_cache.TryGetValue(CacheKeys.CourseNetwork, out cn))
            {
                // Key Not in cache, so lets create new data.
                cn = new CourseNetwork();
                cn.BuildNetwork();

                // Set cache Options
                var cOptions = new MemoryCacheEntryOptions()
                    // Set the expiration
                    .SetSlidingExpiration(TimeSpan.FromSeconds(CACHE_TIMEOUT));

                // Save Data in cache
                _cache.Set(CacheKeys.CourseNetwork, cn, cOptions);
            }
            cn.FindShortPath(course);

            return new string[] {"HELLO", "THERE", "test" };
        }
    }
}
