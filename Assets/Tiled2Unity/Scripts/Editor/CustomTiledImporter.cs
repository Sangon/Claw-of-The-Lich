// Example custom importer:
using System;
using System.Collections.Generic;
using Tiled2Unity;
using UnityEngine;

[Tiled2Unity.CustomTiledImporter]
class MyCustomImporter : Tiled2Unity.ICustomTiledImporter
{
    void ICustomTiledImporter.HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties)
    {
        if (customProperties.ContainsKey("CanGetBehind"))
        {
            AutoLayerSort sorter = gameObject.GetComponent<AutoLayerSort>();
            if (customProperties["CanGetBehind"].Equals("0"))
                sorter.offset = -512.0f / 100.0f;
            else
                sorter.offset = 128.0f / 100.0f;
        }
    }

    void ICustomTiledImporter.CustomizePrefab(GameObject prefab)
    {
        // Do nothing
        //Debug.Log("Toimiiko2? " + prefab.gameObject.name);
    }
}