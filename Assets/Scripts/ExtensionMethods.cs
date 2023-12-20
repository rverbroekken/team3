using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public static class ExtensionMethods
{
    public static Texture2D toTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.ARGB32, false);

        var old_rt = RenderTexture.active;
        RenderTexture.active = rTex;

        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();

        RenderTexture.active = old_rt;
        return tex;
    }


    public static void Move<T>(this List<T> list, int oldIndex, int newIndex)
    {
        T item = list[oldIndex];
        list.RemoveAt(oldIndex);
        list.Insert(newIndex, item);
    }

    public static Vector3 SetX(this Vector3 pos, float x)
    {
        return new Vector3(x, pos.y, pos.z);
    }

    public static void SetLocalX(this Transform t, float x)
    {
        t.localPosition = t.localPosition.SetX(x);
    }

    public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalMoveXAtSpeed(this Transform t, float destX, float speedPer100)
    {
        float d = Mathf.Abs(t.localPosition.x - destX);
        var duration = (d / 100) * speedPer100;
        return t.DOLocalMoveX(destX, duration);
    }
    
}
