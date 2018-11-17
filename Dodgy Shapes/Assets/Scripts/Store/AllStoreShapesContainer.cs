using UnityEngine;

public class AllStoreShapesContainer : MonoBehaviour
{
    //THIS SHOULD BE ADJUSTED WHEN THE allStoreShapes ARRAY IS CHANGED
    public static int allStoreShapesLength = 44;

    public ShapeChooserController[] allStoreShapes;

    public void Start()
    {
        allStoreShapesLength = allStoreShapes.Length;
    }
}
