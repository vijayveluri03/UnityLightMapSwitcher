//using System;
//using UnityEngine;

//public enum TimeOfDay 
//{
//    DAY,
//    NIGHT,

//    SUNSET,


//}

//public enum WeatherConditions 
//{
//    CLEAR,
//    RAINY, 
//    OVERCAST
//}

//public static class PlayerPrefKeys 
//{
//    public static string TimeOfDayPreference = "DAY";
//}
//public class LightingScenarioSwitcher : MonoBehaviour
//{
//    public static LightingScenarioSwitcher Instance
//    {
//        get 
//        {
//            return m_instance;
//        }
//    }
//    private static LightingScenarioSwitcher m_instance = null;
//    public LevelLightmapData LocalLevelLightmapData;
//    public GameObject DayLighting;
//    public GameObject NightLighting;

//    public static TimeOfDay? LightingPreference;
//    public static TimeOfDay RealTimeLightingScenario
//    {
//        get
//        {
//            if (LightingPreference == null)
//            {
//                var hourOfLocalDay = DateTime.Now.ToLocalTime().Hour;
//                if (hourOfLocalDay > 8 && hourOfLocalDay < 18)
//                {
//                    return TimeOfDay.DAY;
//                }
//                else return TimeOfDay.NIGHT;
//            }

//            return LightingPreference.Value;
//        }
//    }

//    private void OnGUI()
//    {
//        if ( GUI.Button( new Rect(0,0,100,50), "Day"))
//        {
//            LoadLighting(TimeOfDay.DAY, WeatherConditions.CLEAR);
//        }
//        if (GUI.Button(new Rect(0, 50, 100, 50), "Night"))
//        {
//            LoadLighting(TimeOfDay.NIGHT, WeatherConditions.CLEAR);
//        }

//    }
//    private void Awake()
//    {
//        m_instance = this;
        
//        RenderSettings.fog = true;

//        if (!PlayerPrefs.HasKey(PlayerPrefKeys.TimeOfDayPreference))
//        {
//            SetTimeOfDayPreference(null);
//        }
//        else if (Enum.TryParse<TimeOfDay>(PlayerPrefs.GetString(PlayerPrefKeys.TimeOfDayPreference), out var preference))
//        {
//            SetTimeOfDayPreference(preference);
//        }
//        else
//        {
//            SetTimeOfDayPreference(null);
//        }

//        //LoadLighting(TimeOfDay.DAY, WeatherConditions.CLEAR);
//    }

//    public static void LoadLighting(TimeOfDay timeOfDay, WeatherConditions weatherConditions)
//    {
//        SetAtmosphere(timeOfDay, weatherConditions);

//        if (timeOfDay == TimeOfDay.DAY) Instance.LocalLevelLightmapData.LoadLightingScenario(0, Instance);
//        else if (timeOfDay == TimeOfDay.NIGHT) Instance.LocalLevelLightmapData.LoadLightingScenario(1, Instance);

//        if(Instance.DayLighting != null)Instance.DayLighting.SetActive(timeOfDay == TimeOfDay.DAY);
//        if (Instance.NightLighting != null ) Instance.NightLighting.SetActive (timeOfDay == TimeOfDay.NIGHT);
//    }

//    public static void SetTimeOfDayPreference(TimeOfDay? preference)
//    {
//        LightingPreference = preference;

//        if (preference.HasValue)
//        {
//            if (preference == TimeOfDay.SUNSET)
//            {
//                throw new ArgumentOutOfRangeException("No lighting available for sunset.");
//            }

//            PlayerPrefs.SetString(PlayerPrefKeys.TimeOfDayPreference, preference.Value.ToString());
//        }
//        else
//        {
//            // Basically no-op to recalculate
//            PlayerPrefs.DeleteKey(PlayerPrefKeys.TimeOfDayPreference);
//        }
//    }

//    private static void SetAtmosphere(TimeOfDay timeOfDay, WeatherConditions weatherConditions)
//    {
//        RenderSettings.fogDensity = 0.0015f;
//        // switch (weatherConditions)
//        // {
//        //     case WeatherConditions.RAINY:
//        //         if (timeOfDay == TimeOfDay.NIGHT)
//        //         {
//        //             RenderSettings.skybox = Resources.Load<Material>("Environment/Skyboxes/Rainy");
//        //             RenderSettings.fogColor = new Color(0.156f, 0.17f, .4f);
//        //         }
//        //         else
//        //         {
//        //             RenderSettings.skybox = Resources.Load<Material>("Environment/Skyboxes/RainyDay");
//        //             RenderSettings.fogColor = new Color(.5f, .54f, .57f);
//        //         }
//        //         break;

//        //     case WeatherConditions.OVERCAST:
//        //         RenderSettings.fogColor = new Color(0, .1f, .4f);
//        //         if (timeOfDay == TimeOfDay.NIGHT)
//        //         {
//        //             RenderSettings.skybox = Resources.Load<Material>("Environment/Skyboxes/Rainy");
//        //             RenderSettings.fogColor = new Color(0, .1f, .4f);
//        //         }
//        //         else
//        //         {
//        //             RenderSettings.skybox = Resources.Load<Material>("Environment/Skyboxes/RainyDay");
//        //             RenderSettings.fogColor = new Color(.5f, .54f, .57f);
//        //         }
//        //         break;
//        //     case WeatherConditions.CLEAR:
//        //         if (timeOfDay == TimeOfDay.NIGHT)
//        //         {
//        //             RenderSettings.skybox = Resources.Load<Material>("Environment/Skyboxes/Night");
//        //             RenderSettings.fogColor = new Color(.22f, .29f, .47f);

//        //         }
//        //         else
//        //         {
//        //             RenderSettings.fogColor = new Color(.95f, .98f, 1f);
//        //             RenderSettings.skybox = Resources.Load<Material>("Environment/Skyboxes/Sunny");
//        //             RenderSettings.fogDensity = 0.001f;
//        //         }
//        //         break;
//        // }
//        DynamicGI.UpdateEnvironment();
//    }

//    //private static readonly ThirdTime.Common.Logging.ILogger _logger = new UnityLogger("LightingSwitcher");
//}