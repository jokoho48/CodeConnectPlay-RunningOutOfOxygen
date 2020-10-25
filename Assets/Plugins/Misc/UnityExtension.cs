using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace Plugins
{
    #region

#if UNITY_EDITOR
    using UnityEditor;

#endif

    #endregion

    public static class UnityExtensions
    {
        #region Camera

        /// <summary>
        ///     Calculates the size of the viewport at a given distance from a perspective camera.
        /// </summary>
        /// <param name="camera">The Camera.</param>
        /// <param name="distance">The positive distance from the camera.</param>
        /// <param name="aspectRatio">Optionally: An aspect ratio to use. If 0 is set, camera.aspect is used.</param>
        /// <returns>The size of the viewport at the given distance.</returns>
        public static Vector2 CalculateViewportWorldSizeAtDistance(this Camera camera, float distance,
            float aspectRatio = 0)
        {
            if (aspectRatio == 0) aspectRatio = camera.aspect;

            var viewportHeightAtDistance = 2.0f * Mathf.Tan(0.5f * camera.fieldOfView * Mathf.Deg2Rad) * distance;
            var viewportWidthAtDistance = viewportHeightAtDistance * aspectRatio;

            return new Vector2(viewportWidthAtDistance, viewportHeightAtDistance);
        }

        #endregion

        #region LayerMask

        /// <summary>
        ///     Is a specific layer actived in the given LayerMask?
        /// </summary>
        /// <param name="mask">The LayerMask.</param>
        /// <param name="layer">The layer to check for.</param>
        /// <returns>True if the layer is activated.</returns>
        public static bool ContainsLayer(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) != 0;
        }

        public static List<int> GetLayers(this LayerMask layerMask)
        {
            var layers = new List<int>();
            for (int mask = layerMask.value, layer = 0; mask != 0; mask = mask >> 1, layer++)
                if ((mask & 1) != 0)
                    layers.Add(layer);

            return layers;
        }

        public static LayerMask Create(params string[] layerNames)
        {
            return NamesToMask(layerNames);
        }

        public static LayerMask Create(params int[] layerNumbers)
        {
            return LayerNumbersToMask(layerNumbers);
        }

        public static LayerMask NamesToMask(params string[] layerNames)
        {
            return layerNames.Aggregate<string, LayerMask>(0,
                (current, name) => (LayerMask) (current | (1 << LayerMask.NameToLayer(name))));
        }

        public static LayerMask LayerNumbersToMask(params int[] layerNumbers)
        {
            return layerNumbers.Aggregate<int, LayerMask>(0, (current, layer) => (LayerMask) (current | (1 << layer)));
        }

        public static LayerMask Inverse(this LayerMask original)
        {
            return ~original;
        }

        public static LayerMask AddToMask(this LayerMask original, params string[] layerNames)
        {
            return original | NamesToMask(layerNames);
        }

        public static LayerMask RemoveFromMask(this LayerMask original, params string[] layerNames)
        {
            LayerMask invertedOriginal = ~original;
            return ~(invertedOriginal | NamesToMask(layerNames));
        }

        public static string[] MaskToNames(this LayerMask original)
        {
            var output = new List<string>();

            for (var i = 0; i < 32; ++i)
            {
                var shifted = 1 << i;
                if ((original & shifted) != shifted) continue;
                var layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName)) output.Add(layerName);
            }

            return output.ToArray();
        }

        public static string MaskToString(this LayerMask original, string delimiter = ", ")
        {
            return string.Join(delimiter, MaskToNames(original));
        }

        #endregion

        #region Transform

        /// <summary>
        ///     Copies the position and rotation from another transform to this transform.
        /// </summary>
        /// <param name="transform">The transform to set the position/rotation at.</param>
        /// <param name="source">The transform to take the position/rotation from.</param>
        public static void CopyPositionAndRotatationFrom(this Transform transform, Transform source)
        {
            transform.position = source.position;
            transform.rotation = source.rotation;
        }

        /// <summary>
        ///     Sets the x/y/z transform.position using optional parameters, keeping all undefined values as they were before. Can
        ///     be
        ///     called with named parameters like transform.SetPosition(x: 5, z: 10), for example, only changing
        ///     transform.position.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.position at.</param>
        /// <param name="x">If this is not null, transform.position.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.position.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.position.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetPosition(this Transform transform, float? x = null, float? y = null, float? z = null)
        {
            transform.position = transform.position.Change3(x, y, z);
            return transform;
        }

        /// <summary>
        ///     Sets the x/y/z transform.localPosition using optional parameters, keeping all undefined values as they were before.
        ///     Can be
        ///     called with named parameters like transform.SetLocalPosition(x: 5, z: 10), for example, only changing
        ///     transform.localPosition.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.localPosition at.</param>
        /// <param name="x">If this is not null, transform.localPosition.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.localPosition.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.localPosition.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLocalPosition(this Transform transform, float? x = null, float? y = null,
            float? z = null)
        {
            transform.localPosition = transform.localPosition.Change3(x, y, z);
            return transform;
        }

        /// <summary>
        ///     Sets the x/y/z transform.localScale using optional parameters, keeping all undefined values as they were before.
        ///     Can be
        ///     called with named parameters like transform.SetLocalScale(x: 5, z: 10), for example, only changing
        ///     transform.localScale.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.localScale at.</param>
        /// <param name="x">If this is not null, transform.localScale.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.localScale.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.localScale.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLocalScale(this Transform transform, float? x = null, float? y = null,
            float? z = null)
        {
            transform.localScale = transform.localScale.Change3(x, y, z);
            return transform;
        }

        /// <summary>
        ///     Sets the x/y/z transform.lossyScale using optional parameters, keeping all undefined values as they were before.
        ///     Can be
        ///     called with named parameters like transform.SetLossyScale(x: 5, z: 10), for example, only changing
        ///     transform.lossyScale.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.lossyScale at.</param>
        /// <param name="x">If this is not null, transform.lossyScale.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.lossyScale.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.lossyScale.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLossyScale(this Transform transform, float? x = null, float? y = null,
            float? z = null)
        {
            var scale = transform.lossyScale;
            var lossyScale = scale.Change3(x, y, z);

            var localScale = new Vector3(lossyScale.x / scale.x,
                lossyScale.y / scale.y,
                lossyScale.z / scale.z);
            transform.localScale = localScale;

            return transform;
        }

        /// <summary>
        ///     Sets the x/y/z transform.eulerAngles using optional parameters, keeping all undefined values as they were before.
        ///     Can be
        ///     called with named parameters like transform.SetEulerAngles(x: 5, z: 10), for example, only changing
        ///     transform.eulerAngles.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.eulerAngles at.</param>
        /// <param name="x">If this is not null, transform.eulerAngles.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.eulerAngles.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.eulerAngles.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetEulerAngles(this Transform transform, float? x = null, float? y = null,
            float? z = null)
        {
            transform.eulerAngles = transform.eulerAngles.Change3(x, y, z);
            return transform;
        }

        /// <summary>
        ///     Sets the x/y/z transform.localEulerAngles using optional parameters, keeping all undefined values as they were
        ///     before. Can be
        ///     called with named parameters like transform.SetLocalEulerAngles(x: 5, z: 10), for example, only changing
        ///     transform.localEulerAngles.x and z.
        /// </summary>
        /// <param name="transform">The transform to set the transform.localEulerAngles at.</param>
        /// <param name="x">If this is not null, transform.localEulerAngles.x is set to this value.</param>
        /// <param name="y">If this is not null, transform.localEulerAngles.y is set to this value.</param>
        /// <param name="z">If this is not null, transform.localEulerAngles.z is set to this value.</param>
        /// <returns>The transform itself.</returns>
        public static Transform SetLocalEulerAngles(this Transform transform, float? x = null, float? y = null,
            float? z = null)
        {
            transform.localEulerAngles = transform.localEulerAngles.Change3(x, y, z);
            return transform;
        }

        public static Transform[] GetChilds(this Transform transform)
        {
            var transforms = new Transform[transform.childCount];
            for (var i = 0; i < transform.childCount; i++) transforms[i] = transform.GetChild(i);

            return transforms;
        }

        #endregion

        #region GameObject

        /// <summary>
        ///     Assigns a layer to this GameObject and all its children recursively.
        /// </summary>
        /// <param name="gameObject">The GameObject to start at.</param>
        /// <param name="layer">The layer to set.</param>
        public static void AssignLayerToHierarchy(this GameObject gameObject, int layer)
        {
            var transforms = gameObject.GetComponentsInChildren<Transform>();
            foreach (var t in transforms)
                t.gameObject.layer = layer;
        }

        /// <summary>
        ///     When <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object)" /> is called on a prefab named
        ///     "Original", the resulting instance will be named "Original(Clone)". This method changes the name
        ///     back to "Original" by stripping everything after and including the first "(Clone)" it finds. If no
        ///     "(Clone)" is found, the name is left unchanged.
        /// </summary>
        /// <param name="gameObject">The GameObject to change the name of.</param>
        public static void StripCloneFromName(this GameObject gameObject)
        {
            gameObject.name = gameObject.GetNameWithoutClone();
        }

        /// <summary>
        ///     When <see cref="UnityEngine.Object.Instantiate(UnityEngine.Object)" /> is called on a prefab named
        ///     "Original", the resulting instance will be named "Original(Clone)". This method returns the name
        ///     without "(Clone)" by stripping everything after and including the first "(Clone)" it finds. If no
        ///     "(Clone)" is found, the name is returned unchanged.
        /// </summary>
        /// <param name="gameObject">The GameObject to return the original name of.</param>
        public static string GetNameWithoutClone(this GameObject gameObject)
        {
            var gameObjectName = gameObject.name;

            var clonePartIndex = gameObjectName.IndexOf("(Clone)", StringComparison.Ordinal);
            return clonePartIndex == -1 ? gameObjectName : gameObjectName.Substring(0, clonePartIndex);
        }

        public static T GetOrAddComponent<T>(this GameObject value) where T : Component
        {
            var c = value.GetComponent<T>();
            if (!c) c = value.AddComponent<T>();
            return c;
        }

        public static bool HasComponentInParent<T>(this GameObject flag) where T : Component
        {
            return flag.GetComponentInParent<T>();
        }

        public static bool HasComponentInChildren<T>(this GameObject flag) where T : Component
        {
            return flag.GetComponentInParent<T>();
        }

        public static bool HasComponentInParent<T>(this Component flag) where T : Component
        {
            return flag.GetComponentInParent<T>();
        }

        public static bool HasComponentInChildren<T>(this Component flag) where T : Component
        {
            return flag.GetComponentInParent<T>();
        }

        #endregion

        #region Vector2/3/4

        /// <summary>
        ///     Makes a copy of the Vector2 with changed x/y values, keeping all undefined values as they were before. Can be
        ///     called with named parameters like vector.Change2(y: 5), for example, only changing the y component.
        /// </summary>
        /// <param name="vector">The Vector2 to be copied with changed values.</param>
        /// <param name="x">If this is not null, the x component is set to this value.</param>
        /// <param name="y">If this is not null, the y component is set to this value.</param>
        /// <returns>A copy of the Vector2 with changed values.</returns>
        public static Vector2 Change2(this Vector2 vector, float? x = null, float? y = null)
        {
            if (x.HasValue) vector.x = x.Value;
            if (y.HasValue) vector.y = y.Value;
            return vector;
        }

        /// <summary>
        ///     Makes a copy of the Vector3 with changed x/y/z values, keeping all undefined values as they were before. Can be
        ///     called with named parameters like vector.Change3(x: 5, z: 10), for example, only changing the x and z components.
        /// </summary>
        /// <param name="vector">The Vector3 to be copied with changed values.</param>
        /// <param name="x">If this is not null, the x component is set to this value.</param>
        /// <param name="y">If this is not null, the y component is set to this value.</param>
        /// <param name="z">If this is not null, the z component is set to this value.</param>
        /// <returns>A copy of the Vector3 with changed values.</returns>
        public static Vector3 Change3(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            if (x.HasValue) vector.x = x.Value;
            if (y.HasValue) vector.y = y.Value;
            if (z.HasValue) vector.z = z.Value;
            return vector;
        }

        /// <summary>
        ///     Makes a copy of the Vector4 with changed x/y/z/w values, keeping all undefined values as they were before. Can be
        ///     called with named parameters like vector.Change4(x: 5, z: 10), for example, only changing the x and z components.
        /// </summary>
        /// <param name="vector">The Vector4 to be copied with changed values.</param>
        /// <param name="x">If this is not null, the x component is set to this value.</param>
        /// <param name="y">If this is not null, the y component is set to this value.</param>
        /// <param name="z">If this is not null, the z component is set to this value.</param>
        /// <param name="w">If this is not null, the w component is set to this value.</param>
        /// <returns>A copy of the Vector4 with changed values.</returns>
        public static Vector4 Change4(this Vector4 vector, float? x = null, float? y = null, float? z = null,
            float? w = null)
        {
            if (x.HasValue) vector.x = x.Value;
            if (y.HasValue) vector.y = y.Value;
            if (z.HasValue) vector.z = z.Value;
            if (w.HasValue) vector.w = w.Value;
            return vector;
        }

        /// <summary>
        ///     Rotates a Vector2.
        /// </summary>
        /// <param name="v">The Vector2 to rotate.</param>
        /// <param name="angleRad">How far to rotate the Vector2 in radians.</param>
        /// <returns>The rotated Vector2.</returns>
        public static Vector2 RotateRad(this Vector2 v, float angleRad)
        {
            // http://answers.unity3d.com/questions/661383/whats-the-most-efficient-way-to-rotate-a-vector2-o.html
            var sin = Mathf.Sin(angleRad);
            var cos = Mathf.Cos(angleRad);

            var tx = v.x;
            var ty = v.y;
            v.x = cos * tx - sin * ty;
            v.y = sin * tx + cos * ty;

            return v;
        }

        /// <summary>
        ///     Rotates a Vector2.
        /// </summary>
        /// <param name="v">The Vector2 to rotate.</param>
        /// <param name="angleDeg">How far to rotate the Vector2 in degrees.</param>
        /// <returns>The rotated Vector2.</returns>
        public static Vector2 RotateDeg(this Vector2 v, float angleDeg)
        {
            return v.RotateRad(angleDeg * Mathf.Deg2Rad);
        }

        /// <summary>
        ///     Creates a Vector2 with a length of 1 pointing towards a certain angle.
        /// </summary>
        /// <param name="angleRad">The angle in radians.</param>
        /// <returns>The Vector2 pointing towards the angle.</returns>
        public static Vector2 CreateVector2AngleRad(float angleRad)
        {
            return new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        /// <summary>
        ///     Creates a Vector2 with a length of 1 pointing towards a certain angle.
        /// </summary>
        /// <param name="angleDeg">The angle in degrees.</param>
        /// <returns>The Vector2 pointing towards the angle.</returns>
        public static Vector2 CreateVector2AngleDeg(float angleDeg)
        {
            return CreateVector2AngleRad(angleDeg * Mathf.Deg2Rad);
        }

        /// <summary>
        ///     Gets the rotation of a Vector2.
        /// </summary>
        /// <param name="vector">The Vector2.</param>
        /// <returns>The rotation of the Vector2 in radians.</returns>
        public static float GetAngleRad(this Vector2 vector)
        {
            return Mathf.Atan2(vector.y, vector.x);
        }

        /// <summary>
        ///     Gets the rotation of a Vector2.
        /// </summary>
        /// <param name="vector">The Vector2.</param>
        /// <returns>The rotation of the Vector2 in degrees.</returns>
        public static float GetAngleDeg(this Vector2 vector)
        {
            return vector.GetAngleRad() * Mathf.Rad2Deg;
        }

        /// <summary>
        ///     Framerate-independent eased lerping to a target value, slowing down the closer it is.
        ///     If you call
        ///     currentValue = UnityHelper.EasedLerpVector3(currentValue, Vector2.one, 0.75f);
        ///     each frame (e.g. in Update()), starting with a currentValue of Vector2.zero, then after 1 second
        ///     it will be approximately (0.75|0.75) - which is 75% of the way between Vector2.zero and Vector2.one.
        ///     Adjusting the target or the percentPerSecond between calls is also possible.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="target">The target value.</param>
        /// <param name="percentPerSecond">How much of the distance between current and target should be covered per second?</param>
        /// <param name="deltaTime">How much time passed since the last call.</param>
        /// <returns>The interpolated value from current to target.</returns>
        public static Vector2 EasedLerpVector2(Vector2 current, Vector2 target, float percentPerSecond,
            float deltaTime = 0f)
        {
            var t = MathHelper.EasedLerpFactor(percentPerSecond, deltaTime);
            return Vector2.Lerp(current, target, t);
        }

        /// <summary>
        ///     Framerate-independent eased lerping to a target value, slowing down the closer it is.
        ///     If you call
        ///     currentValue = UnityHelper.EasedLerpVector3(currentValue, Vector3.one, 0.75f);
        ///     each frame (e.g. in Update()), starting with a currentValue of Vector3.zero, then after 1 second
        ///     it will be approximately (0.75|0.75|0.75) - which is 75% of the way between Vector3.zero and Vector3.one.
        ///     Adjusting the target or the percentPerSecond between calls is also possible.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="target">The target value.</param>
        /// <param name="percentPerSecond">How much of the distance between current and target should be covered per second?</param>
        /// <param name="deltaTime">How much time passed since the last call.</param>
        /// <returns>The interpolated value from current to target.</returns>
        public static Vector3 EasedLerpVector3(Vector3 current, Vector3 target, float percentPerSecond,
            float deltaTime = 0f)
        {
            var t = MathHelper.EasedLerpFactor(percentPerSecond, deltaTime);
            return Vector3.Lerp(current, target, t);
        }

        /// <summary>
        ///     Framerate-independent eased lerping to a target value, slowing down the closer it is.
        ///     If you call
        ///     currentValue = UnityHelper.EasedLerpVector4(currentValue, Vector4.one, 0.75f);
        ///     each frame (e.g. in Update()), starting with a currentValue of Vector4.zero, then after 1 second
        ///     it will be approximately (0.75|0.75|0.75) - which is 75% of the way between Vector4.zero and Vector4.one.
        ///     Adjusting the target or the percentPerSecond between calls is also possible.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="target">The target value.</param>
        /// <param name="percentPerSecond">How much of the distance between current and target should be covered per second?</param>
        /// <param name="deltaTime">How much time passed since the last call.</param>
        /// <returns>The interpolated value from current to target.</returns>
        public static Vector4 EasedLerpVector4(Vector4 current, Vector4 target, float percentPerSecond,
            float deltaTime = 0f)
        {
            var t = MathHelper.EasedLerpFactor(percentPerSecond, deltaTime);
            return Vector4.Lerp(current, target, t);
        }

        public static Vector2 AddX(this Vector2 v, float x)
        {
            v.x = v.x + x;
            return v;
        }

        public static Vector2 AddY(this Vector2 v, float y)
        {
            v.y = v.y + y;
            return v;
        }

        #endregion

        #region Color

        /// <summary>
        ///     Makes a copy of the Color with changed r/g/b/a values, keeping all undefined values as they were before. Can be
        ///     called with named parameters like color.Change(g: 0, a: 0.5), for example, only changing the g and a components.
        /// </summary>
        /// <param name="color">The Color to be copied with changed values.</param>
        /// <param name="r">If this is not null, the r component is set to this value.</param>
        /// <param name="g">If this is not null, the g component is set to this value.</param>
        /// <param name="b">If this is not null, the b component is set to this value.</param>
        /// <param name="a">If this is not null, the a component is set to this value.</param>
        /// <returns>A copy of the Color with changed values.</returns>
        public static Color Change(this Color color, float? r = null, float? g = null, float? b = null, float? a = null)
        {
            if (r.HasValue) color.r = r.Value;
            if (g.HasValue) color.g = g.Value;
            if (b.HasValue) color.b = b.Value;
            if (a.HasValue) color.a = a.Value;
            return color;
        }

        /// <summary>
        ///     Makes a copy of the vector with a changed alpha value.
        /// </summary>
        /// <param name="color">The Color to copy.</param>
        /// <param name="a">The new a component.</param>
        /// <returns>A copy of the Color with a changed alpha.</returns>
        public static Color ChangeAlpha(this Color color, float a)
        {
            color.a = a;
            return color;
        }

        /// <summary>
        ///     Framerate-independent eased lerping to a target value, slowing down the closer it is.
        ///     If you call
        ///     currentValue = UnityHelper.EasedLerpVector3(currentValue, Color.white, 0.75f);
        ///     each frame (e.g. in Update()), starting with a currentValue of Color.black, then after 1 second
        ///     it will be approximately (0.75|0.75|0.75) - which is 75% of the way between Color.white and Color.black.
        ///     Adjusting the target or the percentPerSecond between calls is also possible.
        /// </summary>
        /// <param name="current">The current value.</param>
        /// <param name="target">The target value.</param>
        /// <param name="percentPerSecond">How much of the distance between current and target should be covered per second?</param>
        /// <param name="deltaTime">How much time passed since the last call.</param>
        /// <returns>The interpolated value from current to target.</returns>
        public static Color EasedLerpColor(Color current, Color target, float percentPerSecond, float deltaTime = 0f)
        {
            var t = MathHelper.EasedLerpFactor(percentPerSecond, deltaTime);
            return Color.Lerp(current, target, t);
        }

        /// <summary>
        ///     Calculates the average position of an array of Vector2.
        /// </summary>
        /// <param name="array">The input array.</param>
        /// <returns>The average position.</returns>
        public static Vector2 CalculateCentroid(this Vector2[] array)
        {
            var sum = new Vector2();
            var count = array.Length;
            for (var i = 0; i < count; i++) sum += array[i];
            return sum / count;
        }

        /// <summary>
        ///     Calculates the average position of an array of Vector3.
        /// </summary>
        /// <param name="array">The input array.</param>
        /// <returns>The average position.</returns>
        public static Vector3 CalculateCentroid(this Vector3[] array)
        {
            var sum = new Vector3();
            var count = array.Length;
            for (var i = 0; i < count; i++) sum += array[i];
            return sum / count;
        }

        /// <summary>
        ///     Calculates the average position of an array of Vector4.
        /// </summary>
        /// <param name="array">The input array.</param>
        /// <returns>The average position.</returns>
        public static Vector4 CalculateCentroid(this Vector4[] array)
        {
            var sum = new Vector4();
            var count = array.Length;
            for (var i = 0; i < count; i++) sum += array[i];
            return sum / count;
        }

        /// <summary>
        ///     Calculates the average position of a List of Vector2.
        /// </summary>
        /// <param name="list">The input list.</param>
        /// <returns>The average position.</returns>
        public static Vector2 CalculateCentroid(this List<Vector2> list)
        {
            var sum = new Vector2();
            var count = list.Count;
            for (var i = 0; i < count; i++) sum += list[i];
            return sum / count;
        }

        /// <summary>
        ///     Calculates the average position of a List of Vector3.
        /// </summary>
        /// <param name="list">The input list.</param>
        /// <returns>The average position.</returns>
        public static Vector3 CalculateCentroid(this List<Vector3> list)
        {
            var sum = new Vector3();
            var count = list.Count;
            for (var i = 0; i < count; i++) sum += list[i];
            return sum / count;
        }

        /// <summary>
        ///     Calculates the average position of a List of Vector4.
        /// </summary>
        /// <param name="list">The input list.</param>
        /// <returns>The average position.</returns>
        public static Vector4 CalculateCentroid(this List<Vector4> list)
        {
            var sum = new Vector4();
            var count = list.Count;
            for (var i = 0; i < count; i++) sum += list[i];
            return sum / count;
        }

        #endregion

        #region Rect

        /// <summary>
        ///     Extends/shrinks the rect by extendDistance to each side and gets a random position from the resulting rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>A random position inside the extended rect.</returns>
        public static Vector2 RandomPosition(this Rect rect, float extendDistance = 0f)
        {
            return new Vector2(UnityEngine.Random.Range(rect.xMin - extendDistance, rect.xMax + extendDistance),
                UnityEngine.Random.Range(rect.yMin - extendDistance, rect.yMax + extendDistance));
        }

        /// <summary>
        ///     Gets a random subrect of the given width or height inside this rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="width">The target width of the subrect. Clamped to the width of the given rect.</param>
        /// <param name="height">The target height of the subrect. Clamped to the height of the given rect.</param>
        /// <returns>A random subrect with the given width and height.</returns>
        public static Rect RandomSubRect(this Rect rect, float width, float height)
        {
            width = Mathf.Min(rect.width, width);
            height = Mathf.Min(rect.height, height);

            var halfWidth = width / 2f;
            var halfHeight = height / 2f;

            var centerX = UnityEngine.Random.Range(rect.xMin + halfWidth, rect.xMax - halfWidth);
            var centerY = UnityEngine.Random.Range(rect.yMin + halfHeight, rect.yMax - halfHeight);

            return new Rect(centerX - halfWidth, centerY - halfHeight, width, height);
        }

        /// <summary>
        ///     Extends/shrinks the rect by extendDistance to each side and then restricts the given vector to the resulting rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="position">A position that should be restricted to the rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>The vector, clamped to the Rect.</returns>
        public static Vector2 Clamp2(this Rect rect, Vector2 position, float extendDistance = 0f)
        {
            return new Vector2(Mathf.Clamp(position.x, rect.xMin - extendDistance, rect.xMax + extendDistance),
                Mathf.Clamp(position.y, rect.yMin - extendDistance, rect.yMax + extendDistance));
        }

        /// <summary>
        ///     Extends/shrinks the rect by extendDistance to each side and then restricts the given vector to the resulting rect.
        ///     The z component is kept.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="position">A position that should be restricted to the rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>The vector, clamped to the Rect.</returns>
        public static Vector3 Clamp3(this Rect rect, Vector3 position, float extendDistance = 0f)
        {
            return new Vector3(Mathf.Clamp(position.x, rect.xMin - extendDistance, rect.xMax + extendDistance),
                Mathf.Clamp(position.y, rect.yMin - extendDistance, rect.yMax + extendDistance),
                position.z);
        }

        /// <summary>
        ///     Extends/shrinks the rect by extendDistance to each side.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>The rect, extended/shrunken by extendDistance to each side.</returns>
        public static Rect Extend(this Rect rect, float extendDistance)
        {
            var copy = rect;
            copy.xMin -= extendDistance;
            copy.xMax += extendDistance;
            copy.yMin -= extendDistance;
            copy.yMax += extendDistance;
            return copy;
        }

        /// <summary>
        ///     Extends/shrinks the rect by extendDistance to each side and then checks if a given point is inside the resulting
        ///     rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <param name="position">A position that should be restricted to the rect.</param>
        /// <param name="extendDistance">The distance to extend/shrink the rect to each side.</param>
        /// <returns>True if the position is inside the extended rect.</returns>
        public static bool Contains(this Rect rect, Vector2 position, float extendDistance)
        {
            return position.x > rect.xMin + extendDistance &&
                   position.y > rect.yMin + extendDistance &&
                   position.x < rect.xMax - extendDistance &&
                   position.y < rect.yMax - extendDistance;
        }

        /// <summary>
        ///     Creates an array containing the four corner points of a Rect.
        /// </summary>
        /// <param name="rect">The Rect.</param>
        /// <returns>An array containing the four corner points of the Rect.</returns>
        public static Vector2[] GetCornerPoints(this Rect rect)
        {
            return new[]
            {
                new Vector2(rect.xMin, rect.yMin),
                new Vector2(rect.xMax, rect.yMin),
                new Vector2(rect.xMax, rect.yMax),
                new Vector2(rect.xMin, rect.yMax)
            };
        }

        #endregion

        #region PlayerPrefs

        /// <summary>
        ///     Returns the value corresponding to the key in the preference file if it exists.
        ///     If it doesn't exist, it will return defaultValue.
        ///     (Internally, the value is stored as an int with either 0 or 1.)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The default value if none is given.</param>
        /// <returns>The value corresponding to key in the preference file if it exists, else the default value.</returns>
        public static bool PlayerPrefsGetBool(string key, bool defaultValue = false)
        {
            return PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;
        }

        /// <summary>
        ///     Sets the value of the preference entry identified by the key.
        ///     (Internally, the value is stored as an int with either 0 or 1.)
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to set the preference entry to.</param>
        public static void PlayerPrefsSetBool(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
        }

        #endregion

        #region Physics

        /// <summary>
        ///     Creates a Bounds encapsulating all given colliders bounds.
        /// </summary>
        /// <param name="colliders">The colliders.</param>
        /// <returns>A Bounds encapsulating all given colliders bounds.</returns>
        public static Bounds CombineColliderBounds(Collider[] colliders)
        {
            var bounds = colliders[0].bounds;

            foreach (var colliderComponent in colliders) bounds.Encapsulate(colliderComponent.bounds);

            return bounds;
        }

        /// <summary>
        ///     Given a CharacterController and a point of origin (the lower point of the capsule), this returns the
        ///     point1, point2 and radius needed to fill a CapsuleCast().
        /// </summary>
        /// <param name="characterController">
        ///     The CharacterController to use as the capsule, providing scale, radius, height and
        ///     center offset.
        /// </param>
        /// <param name="origin">The capsule cast starting point at the lower end of the capsule.</param>
        /// <param name="point1">Outputs the point1 parameter to be used in the CapsuleCast()</param>
        /// <param name="point2">Outputs the point2 parameter to be used in the CapsuleCast()</param>
        /// <param name="radius">Outputs the radius parameter to be used in the CapsuleCast()</param>
        public static void GetCapsuleCastData(CharacterController characterController, Vector3 origin,
            out Vector3 point1,
            out Vector3 point2, out float radius)
        {
            var scale = characterController.transform.lossyScale;
            radius = characterController.radius * scale.x;
            var height = characterController.height * scale.y - radius * 2;
            var center = characterController.center;
            center.Scale(scale);
            point1 = origin + center + Vector3.down * (height / 2f);
            point2 = point1 + Vector3.up * height;
        }

        #endregion

        #region Random

        /// <summary>
        ///     Gets a random Vector2 of length 1 pointing in a random direction.
        /// </summary>
        public static Vector2 RandomOnUnitCircle()
        {
            var angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }

        public static Vector2 RandomOnUnitCircle(this Random random)
        {
            var angle = (float) random.NextDouble(Mathf.PI * 2);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        }


        /// <summary>
        ///     Returns -1 or 1 with equal change.
        /// </summary>
        public static int RandomSign()
        {
            return UnityEngine.Random.value < 0.5f ? -1 : 1;
        }

        public static int RandomSign(this Random random)
        {
            return random.NextDouble(1) < 0.5f ? -1 : 1;
        }

        /// <summary>
        ///     Returns true or false with equal chance.
        /// </summary>
        public static bool RandomBool()
        {
            return UnityEngine.Random.value < 0.5f;
        }

        public static bool RandomBool(this Random random)
        {
            return random.NextDouble(1) < 0.5f;
        }

        public static double NextDouble(this Random random, double maxValue)
        {
            return random.NextDouble() % maxValue;
        }

        public static double NextDouble(this Random random, double minValue, double maxValue)
        {
            return random.NextDouble() * (maxValue - minValue) + minValue;
        }

        public static T SelectRandom<T>(this List<T> data)
        {
            var index = UnityEngine.Random.Range(0, data.Count);
            return data[index];
        }

        public static T SelectRandom<T>(this List<T> data, Random random)
        {
            var index = random.Next(data.Count);
            return data[index];
        }

        public static T SelectRandom<T>(this T[] data)
        {
            var index = UnityEngine.Random.Range(0, data.Length);
            return data[index];
        }

        public static T SelectRandom<T>(this T[] data, Random random)
        {
            var index = random.Next(data.Length);
            return data[index];
        }

        #endregion

        #region Misc

        public static void SaveMesh(Mesh mesh, string path, bool optimizeMesh = true, bool autoSave = true)
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetProjectRelativePath(path);
            if (optimizeMesh)
                MeshUtility.Optimize(mesh);
            AssetDatabase.CreateAsset(mesh, path);
            if (autoSave) AssetDatabase.SaveAssets();
#endif
        }

        public static GameObject Instantiate(GameObject prefab, Vector3 position = default,
            Quaternion rotation = default,
            Transform parent = default)
        {
            GameObject go;
            if (!Application.isPlaying && Application.isEditor)
            {
#if UNITY_EDITOR
                go = (GameObject) PrefabUtility.InstantiatePrefab(prefab); // Create Spawn Tile
                go.transform.parent = parent;
                go.transform.position = position;
                go.transform.rotation = rotation;
#else
            go = Object.Instantiate(prefab, position, rotation, parent); // Create Spawn Tile
#endif
            }
            else
            {
                go = Object.Instantiate(prefab, position, rotation, parent); // Create Spawn Tile
            }

            return go;
        }

        public static void Destory(GameObject target, float time = 0f)
        {
            if (Application.isPlaying && !Application.isEditor)
            {
                if (time == 0f)
                    Object.Destroy(target);
                else
                    Object.Destroy(target, time);
            }
            else
            {
                Object.DestroyImmediate(target);
            }
        }

        public static void Destory(Transform target, float time = 0f)
        {
            if (Application.isPlaying && !Application.isEditor)
                Object.Destroy(target.gameObject, time);
            else
                Object.DestroyImmediate(target.gameObject);
        }

        public static void Destroy(this Object target)
        {
            if (Application.isPlaying && !Application.isEditor)
                Object.Destroy(target);
            else
                Object.DestroyImmediate(target);
        }

        public static void Destroy(this Transform target)
        {
            target.gameObject.Destroy();
        }

        public static bool Raycast(Vector3 startPosition, Vector3 direction, Transform ignoredObject,
            LayerMask layerMask,
            float distance, out RaycastHit outHit, bool checkParent = false)
        {
            var allHits = Physics.RaycastAll(startPosition, direction, distance, layerMask).ToList();

            allHits = checkParent
                ? allHits.OrderBy(hit => hit.distance).Where(hit => !hit.transform.IsChildOf(ignoredObject)).ToList()
                : allHits.OrderBy(hit => hit.distance).Where(hit => hit.transform != ignoredObject).ToList();

            outHit = allHits.Any() ? allHits[0] : default;

            return allHits.Any();
        }

        public static bool Raycast(Vector3 startPosition, Vector3 direction, Transform ignoredObject,
            float distance, out RaycastHit outHit, bool checkParent = false)
        {
            var allHits = Physics.RaycastAll(startPosition, direction, distance).ToList();

            allHits = checkParent
                ? allHits.OrderBy(hit => hit.distance).Where(hit => !hit.transform.IsChildOf(ignoredObject)).ToList()
                : allHits.OrderBy(hit => hit.distance).Where(hit => hit.transform != ignoredObject).ToList();


            outHit = allHits.Any() ? allHits[0] : default;

            return allHits.Any();
        }

        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }

        #endregion
    }
}