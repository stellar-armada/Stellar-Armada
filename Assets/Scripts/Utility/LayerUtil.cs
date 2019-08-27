using UnityEngine;

public static class LayerUtil
{
    public static int LayerMaskToLayer(LayerMask layerMask) {
        int layerNumber = 0;
        int layer = layerMask.value;
        while(layer > 0) {
            layer = layer >> 1;
            layerNumber++;
        }
        return layerNumber - 1;
    }
}
