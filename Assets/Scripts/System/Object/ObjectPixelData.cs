using UnityEngine;

[CreateAssetMenu(menuName ="Pixel/Object Data")]
public class ObjectPixelData : ScriptableObject
{
    public string id;
    public GameObject prefab;
    public Sprite icon;
}
