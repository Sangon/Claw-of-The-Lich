// Example custom importer:
using System;
using System.Collections.Generic;
using Tiled2Unity;
using UnityEngine;

[Tiled2Unity.CustomTiledImporter]
class MyCustomImporter : Tiled2Unity.ICustomTiledImporter
{
    void ICustomTiledImporter.HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties, List<GameObject> collisions)
    {
        bool doubleHeight = false;
        if (customProperties.ContainsKey("DoubleHeight"))
        {
            //AutoLayerSort sorter = gameObject.GetComponent<AutoLayerSort>();
            float offset = -512.0f / 100f;
            if (customProperties["DoubleHeight"].Equals("true"))
            {
                gameObject.transform.position += new Vector3(0, 0, offset);
                doubleHeight = true;
            }
        }
        if (customProperties.ContainsKey("Collision"))
        {
            string collisionName = customProperties["Collision"];
            if (collisionName.Contains("Water"))
                gameObject.layer = Tuner.LAYER_WATER_INT;
            else
                gameObject.layer = Tuner.LAYER_OBSTACLES_INT;

            foreach (GameObject collision in collisions)
            {
                if (collision.name.Equals(collisionName))
                {
                    PolygonCollider2D polygonColliderComponent = collision.GetComponent<PolygonCollider2D>();
                    if (polygonColliderComponent != null)
                    {
                        if (UnityEditorInternal.ComponentUtility.CopyComponent(polygonColliderComponent))
                        {
                            if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(gameObject))
                            {
                                if (doubleHeight)
                                {
                                    PolygonCollider2D polygonCollider = gameObject.GetComponent<PolygonCollider2D>();
                                    polygonCollider.offset -= new Vector2(0, 512.0f);
                                }
                            }
                        }
                    } else
                    {
                        EdgeCollider2D edgeColliderComponent = collision.GetComponent<EdgeCollider2D>();
                        if (UnityEditorInternal.ComponentUtility.CopyComponent(edgeColliderComponent))
                        {
                            if (UnityEditorInternal.ComponentUtility.PasteComponentAsNew(gameObject))
                            {
                                if (doubleHeight)
                                {
                                    EdgeCollider2D edgeCollider = gameObject.GetComponent<EdgeCollider2D>();
                                    edgeCollider.offset -= new Vector2(0, 512.0f);
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    void ICustomTiledImporter.CustomizePrefab(GameObject prefab)
    {
        // Do nothing
    }
}