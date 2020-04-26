using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Core.Services;
using System.IO;


namespace Assets.Scripts.Core.Controllers
{
    public class FloorController : MonoBehaviour
    {

        private SettingsService settingsService;

        private static readonly ILogger Logger = Debug.unityLogger;

        public void Init(SettingsService settingsService)
        {
            this.settingsService = settingsService;
        }

        public static Texture2D LoadTextureFromFile(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(2, 2);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            } else
            {
                Logger.Log("File path for floor texture don't exists");
            }
            return tex;
        }

        // Start is called before the first frame update
        void Start()
        {
            var currentSettings = settingsService.GetRandomizedValues();
            Texture2D floorTexture = LoadTextureFromFile(currentSettings.PathToFloorTexture);
            floorTexture.wrapMode = TextureWrapMode.Repeat;
            floorTexture.Apply();
            //Find the Standard Shader
            Material floorMaterial = new Material(Shader.Find("Diffuse"));
            floorMaterial.SetTexture("_MainTex", floorTexture);

            gameObject.GetComponent<Renderer>().material = floorMaterial;
            gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(15, 15));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
