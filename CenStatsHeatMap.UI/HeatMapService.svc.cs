using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using AdamRoderick.HeatMap;

namespace CenStatsHeatMap.UI
{

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class HeatMapService : IHeatMapService
    {

        public Guid GenerateHeatMapOverlayToken(HeatPoint[] heatPoints, HeatmapSettings settings)
        {
            //until javascript sets the intensity on each point, it comes through as 0, which renders transparent
            //this sets the intensity to the default intensity so the heat points render with color
            foreach (HeatPoint h in heatPoints) h.Intensity = HeatmapSettings.DEFAULT_INTENSITY;

            // for now, force each of the tile images to have a border with the Border = true setting
            if (null == settings) settings = new HeatmapSettings() {Border = true};

            var cacheObject = new HeatmapSettingsCache.SettingsCacheObject(settings, new HeatPointList(heatPoints));

            return HeatmapSettingsCache.Instance.Store(cacheObject);
        }

        public Guid UpdateHeatmapOverlaySettings(Guid token, HeatmapSettings settings)
        {
            var cacheObject = HeatmapSettingsCache.Instance.Retrieve(token);
            cacheObject.Settings = settings;
            return HeatmapSettingsCache.Instance.Store(token, cacheObject);
        }

        public Guid UpdateHeatmapOverlayHeatPoints(Guid token, HeatPoint[] heatPoints)
        {
            foreach (HeatPoint h in heatPoints) h.Intensity = HeatmapSettings.DEFAULT_INTENSITY;

            var cacheObject = HeatmapSettingsCache.Instance.Retrieve(token);
            cacheObject.HeatPoints = new HeatPointList(heatPoints);
            return HeatmapSettingsCache.Instance.Store(token, cacheObject);
        }

        public HeatmapSettings RetrieveHeatMapOverlaySettings(Guid token)
        {
            HeatmapSettings settings = null;
            try
            {
                settings = HeatmapSettingsCache.Instance.Retrieve(token).Settings;
            } catch (Exception ex)
            {
                //TODO: log
            }
            return settings;
        }

        public void DoWork(Person p)
        {
            throw new Exception("testing exceptions");
            return;
        }

        public Person GetPerson()
        {
            Person p = new Person()
                           {Age = 27, Name = "Adam", Shoes = new List<string>(new string[] {"asdf", "eeee", "poeir"})};
            return p;
        }

    }
}
