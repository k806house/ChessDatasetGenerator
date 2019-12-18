using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using LightType = UnityEngine.LightType;
using Random = UnityEngine.Random;

public class LightProcess : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        var rangeValues = JsonConvert.DeserializeObject<RangeValues>(File.ReadAllText($"Intervals.json"));
        GameObject lightGameObject = new GameObject("The Light");
        Light lightComp = lightGameObject.AddComponent<Light>();
        lightComp.type = LightType.Spot;
        lightComp.color = Color.blue;
        lightGameObject.transform.position = new Vector3(0, 5, 0);
    }

    //List<GameObject> InitSpotLights(Object resource)
    //{
    //    Quaternion rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
    //    Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
    //    SpotLight light = new SpotLight();
    //    var go = GameObject.Instantiate(light, position, rotation);

    //   // var go = new GameObject();
    //    //go.AddComponent<SpotLight>(light);
    //   // lightC.name = "discolight";
    //   // lightC.transform.position = position;



    //    //Light chessFiguresResource = (Light)Instantiate(light, position, rotation);

    //    //Transform[] chessFiguresTs = chessFiguresResource.GetComponentsInChildren<Transform>();

    //    //List<GameObject> chessFigures = new List<GameObject>();
    //    //foreach (Transform child in chessFiguresTs)
    //    //{
    //    //    chessFigures.Add(child.gameObject);
    //    //}

    //   // return chessFigures;
    //}

    public static RandomizedValues IntervalRandomizer(RangeValues values)
    {
        var randomized = new RandomizedValues
        {
            AmbientLightBrightness = Random.Range(values.AmbientLightBrightness.Start, values.AmbientLightBrightness.End),
            CameraPhi = Random.Range(values.CameraPhi.Start, values.CameraPhi.End),
            CameraTheta = Random.Range(values.CameraTheta.Start, values.CameraTheta.End),
            ChessBoardWidth = Random.Range(values.ChessBoardWidth.Start, values.ChessBoardWidth.End),
            ChessmanOffset = Random.Range(values.ChessmanOffset.Start, values.ChessmanOffset.End),
            PathToModels = values.PathToModels,
            PositionX = Random.Range(values.PositionX.Start, values.PositionX.End),
            PositionY = Random.Range(values.PositionY.Start, values.PositionY.End),
            SnapshotPerFen = values.SnapshotPerFen,
            SpotLightBrightness = Random.Range(values.SpotLightBrightness.Start, values.SpotLightBrightness.End),
            SpotLightNumber = Random.Range(values.SpotLightNumber.Start, values.SpotLightNumber.End),
            SpotLightPositionZ = Random.Range(values.SpotLightPositionZ.Start, values.SpotLightPositionZ.End),
            SpotLightPositions = new List<(float x, float y)>()
        };

        for (var i = 0; i < randomized.SpotLightNumber; i++)
        {
            var spotLightPositionX =
                Random.Range(values.SpotLightPositionX.Start, values.SpotLightPositionX.End);
            var spotLightPositionY = Random.Range(values.SpotLightPositionY.Start, values.SpotLightPositionY.End);
            randomized.SpotLightPositions.Add((spotLightPositionX, spotLightPositionY));
        }
        return randomized;
    }


    public class RangeValues
    {
        public Interval<float> PositionX { get; set; }
        public Interval<float> PositionY { get; set; }
        public Interval<float> CameraPhi { get; set; }
        public Interval<float> CameraTheta { get; set; }
        public Interval<int> SpotLightNumber { get; set; }
        public Interval<float> SpotLightPositionZ { get; set; }
        public Interval<float> SpotLightPositionX { get; set; }
        public Interval<float> SpotLightPositionY { get; set; }
        public Interval<float> SpotLightBrightness { get; set; }
        public Interval<float> AmbientLightBrightness { get; set; }
        public Interval<float> ChessBoardWidth { get; set; }
        public Interval<float> ChessmanOffset { get; set; }
        public int SnapshotPerFen { get; set; }
        public string PathToModels { get; set; }
    }


    public class Interval<T> where T : IComparable<T>
    {
        public T Start { get; set; }
        public T End { get; set; }
    }


    public class RandomizedValues
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float CameraPhi { get; set; }
        public float CameraTheta { get; set; }
        public int SpotLightNumber { get; set; }
        public float SpotLightPositionZ { get; set; }
        public List<(float x, float y)> SpotLightPositions { get; set; }
        public float SpotLightBrightness { get; set; }
        public float AmbientLightBrightness { get; set; }
        public float ChessBoardWidth { get; set; }
        public float ChessmanOffset { get; set; }
        public int SnapshotPerFen { get; set; }
        public string PathToModels { get; set; }
    }
}
