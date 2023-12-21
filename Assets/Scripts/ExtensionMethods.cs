using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace sgg
{
    public static class Math
    {
        public static float InverseLerpUnclamped(float a, float b, float value)
        {
            if (a != b)
                return (value - a) / (b - a);
            else
                return 0.0f;
        }
    }

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

        public static Vector3 SetZ(this Vector3 pos, float z)
        {
            return new Vector3(pos.x, pos.y, z);
        }        

        public static void SetLocalX(this Transform t, float x)
        {
            t.localPosition = t.localPosition.SetX(x);
        }

        public static void SetZ(this Transform t, float z)
        {
            t.position = t.position.SetZ(z);
        }

        public static TweenerCore<Vector3, Vector3, VectorOptions> DOLocalMoveXAtSpeed(this Transform t, float destX, float speedPer100)
        {
            float d = Mathf.Abs(t.localPosition.x - destX);
            var duration = (d / 100) * speedPer100;
            return t.DOLocalMoveX(destX, duration);
        }

        public static Vector2 WorldToCanvasSpace(this Canvas canvas, Camera camera, Vector3 worldPosition)
        {
            //first you need the RectTransform component of your canvas
            RectTransform CanvasRect = canvas.GetComponent<RectTransform>();

            //then you calculate the position of the UI element
            //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

            Vector2 ViewportPosition = camera.WorldToViewportPoint(worldPosition);
            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            //now you can set the position of the ui element
            return WorldObject_ScreenPosition;
        }
    }
}
