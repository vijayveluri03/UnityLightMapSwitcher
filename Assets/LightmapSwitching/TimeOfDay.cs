using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeOfDay : MonoBehaviour
{
    public ChangeLightmap lightmapChanger;
    public string dayTimeLightMapFolderName;
    public string nightTimeLightMapFolderName;

    public Light[] day;
    public Light[] night;

    public void Day()
    {
        lightmapChanger.LoadFrom(dayTimeLightMapFolderName);
        
        foreach( Light light in day )
            light.gameObject.SetActive(true);

        foreach( Light light in night )
            light.gameObject.SetActive(false);
    }

    public void Night()
    {
        lightmapChanger.LoadFrom(nightTimeLightMapFolderName);
        foreach( Light light in day )
            light.gameObject.SetActive(false);

        foreach( Light light in night )
            light.gameObject.SetActive(true);
    }
}