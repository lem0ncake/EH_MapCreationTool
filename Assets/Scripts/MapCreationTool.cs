using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class MapCreationTool : MonoBehaviour
{
    public MapModel MapConfig { get; set; } = new();

    [SerializeField] string author;
    [SerializeField] string email;

    [SerializeField] string map;
    [TextArea][SerializeField] string description;


    public void Import()
    {
        string json = File.ReadAllText(EditorUtility.OpenFilePanel("Import map", "", "json"));
        MapModel mapModel = JsonConvert.DeserializeObject<MapModel>(json);
        author = mapModel.Author;
        email = mapModel.Email;
        map = mapModel.MapName;
        description = mapModel.MapDescription;
        foreach (MapObject mapObject in mapModel.MapObjects)
        {
            string path = mapObject.AssetPath[17..^7];
            var o = Resources.Load<GameObject>(path);
            var t = Instantiate(o).transform;
            PrefabUtility.ConvertToPrefabInstance(t.gameObject, o, new ConvertToPrefabInstanceSettings(), InteractionMode.AutomatedAction);
            t.SetPositionAndRotation(new Vector3(mapObject.Position.X, mapObject.Position.Y, mapObject.Position.Z), Quaternion.Euler(mapObject.Rotation.X, mapObject.Rotation.Y, mapObject.Rotation.Z));
            t.localScale = new Vector3(mapObject.Scale.X, mapObject.Scale.Y, mapObject.Scale.Z);
        }
    }
    public void Export()
    {
        List<GameObject> objects = FindObjectsOfType(typeof(GameObject)).ToList().ConvertAll((o) => (GameObject)o);
        List<MapObject> mapObjects = new();
        foreach (var obj in objects)
            if (obj.transform.parent == null)
                if (obj.CompareTag("MapObject") | obj.CompareTag("Spawn"))
                    mapObjects.Add(new(obj));

        MapModel mapModel = new MapModel();
        mapModel.Author = author;
        mapModel.Email = email;
        mapModel.MapName = map;
        mapModel.MapDescription = description;
        mapModel.MapObjects = mapObjects;
        string json = JsonConvert.SerializeObject(mapModel);
        File.WriteAllText(EditorUtility.SaveFilePanel("Export map", "~", mapModel.MapName, "json"), json);
    }
    public void ClearAll()
    {
        author = string.Empty;
        email = string.Empty;
        map = string.Empty;
        description = string.Empty;

        List<GameObject> objects = FindObjectsOfType(typeof(GameObject)).ToList().ConvertAll((o) => (GameObject)o);
        foreach (var obj in objects)
            if (obj != gameObject)
                DestroyImmediate(obj);
    }
}
public class MapModel
{
    public string Author { get; set; }
    public string Email { get; set; }

    public string MapName { get; set; }
    public string MapDescription { get; set; }

    public List<MapObject> MapObjects { get; set; }
}
public class MapObject
{
    public string AssetPath { get; }
    public SimpleVector3 Position { get; }
    public SimpleVector3 Rotation { get; }
    public SimpleVector3 Scale { get; }

    [JsonConstructor]
    public MapObject(string assetPath, SimpleVector3 position, SimpleVector3 rotation, SimpleVector3 scale)
    {
        AssetPath = assetPath;
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
    public MapObject(GameObject gameObject)
    {
        AssetPath = AssetDatabase.GetAssetPath(PrefabUtility.GetCorrespondingObjectFromSource(gameObject));
        Position = new(gameObject.transform.position);
        Rotation = new(gameObject.transform.rotation.eulerAngles);
        Scale = new(gameObject.transform.localScale);
    }
}
public class SimpleVector3
{
    public float X { get; }
    public float Y { get; }
    public float Z { get; }
    [JsonConstructor]
    public SimpleVector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public SimpleVector3(Vector3 vector3)
    {
        X = vector3.x;
        Y = vector3.y;
        Z = vector3.z;
    }
}