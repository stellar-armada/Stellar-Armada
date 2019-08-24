using UnityEngine;
using Zinnia.Extension;

public class MiniMap : MonoBehaviour
{
    public static MiniMap instance;
    public GameObject scene;
    private GameObject miniMapSecene;
    public float yOffset = 1.2f;

    public float startScale = .04f;
    public float minScale;
    public float maxScale;

    public float rotationDamping = 10;

    [SerializeField] private LayerMask uiLayerMask;
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        miniMapSecene = Instantiate(scene);
        SetLayerRecursively(miniMapSecene, LayerUtil.LayerMaskToLayer(uiLayerMask));
        SceneRoot.SceneRootCreated += InitializeMiniMap;
    }

    void InitializeMiniMap()
    {
        // Create mini map scene
        miniMapSecene = Instantiate(scene);
        miniMapSecene.transform.SetParent(MiniMap.instance.transform);
        miniMapSecene.transform.localScale = Vector3.one;
        miniMapSecene.transform.localRotation = Quaternion.identity;
        
        // Put the minimap scene on on our UI ship layer for collision handling
        SetLayerRecursively(miniMapSecene, LayerUtil.LayerMaskToLayer(uiLayerMask));
        
        // Parent the MapTransformRoot to the SceneRoot (bride)
        MapTransformRoot.instance.transform.SetParent(SceneRoot.instance.transform, true);
        MapTransformRoot.instance.transform.localPosition = new Vector3(0, yOffset, 0);
        MapTransformRoot.instance.transform.SetGlobalScale(startScale * Vector3.one);
        
        // Parent the minimap to our MapTransformRoot object
        transform.SetParent(MapTransformRoot.instance.transform);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
    }
    
    
    void LateUpdate()
    {
        if (MapControls.instance == null || MapControls.instance.isActive) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * rotationDamping);
        
    }
    
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
           
        obj.layer = newLayer;
           
        foreach (Transform child in obj.transform)
        {
            if (child != null) SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
