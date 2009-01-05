using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdamRoderick.HeatMap
{
    public class HeatmapSettingsCache
    {

        HeatmapSettingsCache()
        {
            Cache = new Dictionary<Guid, SettingsCacheObject>();
        }

        public static HeatmapSettingsCache Instance
        {
            get
            {
                return Nested.Instance;
            }
        }
        private Dictionary<Guid, SettingsCacheObject> Cache { get; set; }

        /// <summary>
        /// Adds a settings object to the cache
        /// </summary>
        /// <param name="settings"></param>
        /// <returns>Guid representing token (key in cache dictionary)</returns>
        public Guid Store(SettingsCacheObject cacheObject)
        {
            Guid token = Guid.NewGuid();
                Cache.Add(token, cacheObject);

            // asynchronously remove stale cached items
            RemoveStaleCachedItemsDelegate remover = (RemoveStaleCachedItems);
            remover.BeginInvoke(null, null);

            return token;
        }
        public Guid Store(Guid token, SettingsCacheObject cacheObject)
        {
            Guid retval;
            var existingKeyValuePair = from kvp in Cache where kvp.Key.Equals(token) select kvp;
            if (0 == existingKeyValuePair.Count()) // new 
            {
                retval = Store(cacheObject);
            } else // existing
            {
                retval = token;
                Cache[retval] = cacheObject;
                new RemoveStaleCachedItemsDelegate(RemoveStaleCachedItems).BeginInvoke(null, null);
            }
            return retval;
        }

        public SettingsCacheObject Retrieve(Guid token)
        {
            SettingsCacheObject cacheObject = Cache[token];
            new RemoveStaleCachedItemsDelegate(RemoveStaleCachedItems).BeginInvoke(null, null);
            return cacheObject;
        }

        private delegate void RemoveStaleCachedItemsDelegate();
        private void RemoveStaleCachedItems()
        {
            var staleKeyValuePairs =
                from kvp in Cache
                where DateTime.Now.Subtract(kvp.Value.LastTouched).Seconds > 300
                select kvp;

            foreach (KeyValuePair<Guid, SettingsCacheObject> kvp in staleKeyValuePairs)
                Cache.Remove(kvp.Key);
        }

        class Nested
        {
            //Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly HeatmapSettingsCache Instance = new HeatmapSettingsCache();
        }

        public class SettingsCacheObject
        {
            public SettingsCacheObject(HeatmapSettings settings, HeatPointList heatPoints)
            {
                Settings = settings;
                HeatPoints = heatPoints;
            }
            private DateTime _lastTouched;
            internal DateTime LastTouched
            {
                get
                {
                    return _lastTouched;
                }
            }

            private HeatmapSettings _settings;
            public HeatmapSettings Settings
            {
                get
                {
                    _lastTouched = DateTime.Now;
                    return _settings;
                }
                set
                {
                    _settings = value;
                    _lastTouched = DateTime.Now;
                }
            }

            private HeatPointList _heatPoints;
            public HeatPointList HeatPoints
            {
                get
                {
                    _lastTouched = DateTime.Now;
                    return _heatPoints;
                }
                set
                {
                    _heatPoints = value;
                    _lastTouched = DateTime.Now;
                }
            }
        }

    }
}
