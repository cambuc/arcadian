using System.Collections.Generic;
using UnityEngine;

public class WorldMapMenu : MonoBehaviour
{
    public Terrain terrain;

    public Camera playerCam;
    public Camera mapCam;

    public float groundTextureMult;
    public bool disableTerrainDetails;

    bool inWorldMap;

    private void Start()
    {
        inWorldMap = false;

        playerCam.gameObject.SetActive(true);
        mapCam.gameObject.SetActive(false);

        if (playerCam.targetTexture) mapCam.targetTexture = playerCam.targetTexture;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!inWorldMap)
                ToWorldMap();
            else
                ToPlayer();
        }
    }

    public void ToWorldMap()
    {
        if (inWorldMap)
            return;
        inWorldMap = true;

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        playerCam.gameObject.SetActive(false);
        mapCam.gameObject.SetActive(true);

        if(disableTerrainDetails)
            terrain.drawTreesAndFoliage = false;

        foreach(TerrainLayer layer in terrain.terrainData.terrainLayers)
        {
            layer.tileSize *= groundTextureMult;
        }

        RenderSettings.fog = false;
    }

    public void ToPlayer()
    {
        if (!inWorldMap)
            return;
        inWorldMap = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        playerCam.gameObject.SetActive(true);
        mapCam.gameObject.SetActive(false);

        if (disableTerrainDetails)
            terrain.drawTreesAndFoliage = true;

        foreach (TerrainLayer layer in terrain.terrainData.terrainLayers)
        {
            layer.tileSize /= groundTextureMult;
        }

        RenderSettings.fog = true;
    }
}
