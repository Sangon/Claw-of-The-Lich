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
        if (customProperties.ContainsKey("DoubleHeight"))
        {
            //AutoLayerSort sorter = gameObject.GetComponent<AutoLayerSort>();
            float offset = -512.0f / 100f;
            if (customProperties["DoubleHeight"].Equals("true"))
            {
                Debug.Log("Found: " + gameObject.transform.position);
                gameObject.transform.position += new Vector3(0, 0, offset);
                Debug.Log("Fixed: " + gameObject.transform.position);
            }
        }
    }

    void ICustomTiledImporter.CustomizePrefab(GameObject prefab)
    {
        // Do nothing
        //Debug.Log("Toimiiko2? " + prefab.gameObject.name);
    }
}