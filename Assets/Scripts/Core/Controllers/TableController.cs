using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Core.Services;
using System.IO;


namespace Assets.Scripts.Core.Controllers
{
    public class TableController : MonoBehaviour
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
                tex = new Texture2D(2, 2, TextureFormat.BGRA32, false);
                tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            else
            {
                Logger.Log("File path for table texture don't exists");
            }
            return tex;
        }

        // Start is called before the first frame update
        void Start()
        {
            var currentSettings = settingsService.GetRandomizedValues();
            Texture2D tableTexture = LoadTextureFromFile(currentSettings.PathToTableTexture);
            //Find the Standard Shader
            Material tableMaterial = new Material(Shader.Find("Standard"));
            tableMaterial.SetTexture("_MainTex", tableTexture);
            gameObject.GetComponent<Renderer>().material = tableMaterial;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
