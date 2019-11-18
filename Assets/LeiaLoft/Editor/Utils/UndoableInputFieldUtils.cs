/****************************************************************
*
* Copyright 2019 © Leia Inc.  All rights reserved.
*
* NOTICE:  All information contained herein is, and remains
* the property of Leia Inc. and its suppliers, if any.  The
* intellectual and technical concepts contained herein are
* proprietary to Leia Inc. and its suppliers and may be covered
* by U.S. and Foreign Patents, patents in process, and are
* protected by trade secret or copyright law.  Dissemination of
* this information or reproduction of this materials strictly
* forbidden unless prior written permission is obtained from
* Leia Inc.
*
****************************************************************
*/
using System;
using UnityEngine;
using UnityEditor;

namespace LeiaLoft
{
    /// <summary>
    /// Set of methods that display input fields of different types and record objects for Undo
    /// </summary>
    public static class UndoableInputFieldUtils
    {
        public static void ImmediateVector2Field(Func<Vector2> getter, Action<Vector2> setter, string label, Vector2 min, Vector2 max, UnityEngine.Object obj = null)
        {
            var value = getter();
            var newValue = EditorGUILayout.Vector2Field(label, value);

            newValue.x = Mathf.Clamp(newValue.x, min.x, max.x);
            newValue.y = Mathf.Clamp(newValue.y, min.y, max.y);

            if (!value.Equals(newValue))
            {
                if (obj != null) Undo.RecordObject(obj, label);
                setter(newValue);
                if (obj != null) EditorUtility.SetDirty(obj);
            }
        }

        public static void ImmediateFloatField(Func<float> getter, Action<float> setter, string label, UnityEngine.Object obj = null)
        {
            var value = getter();
            var newValue = EditorGUILayout.FloatField(label, value);

            if (!value.Equals(newValue))
            {
                if (obj != null) Undo.RecordObject(obj, label);
                setter(newValue);
                if (obj != null) EditorUtility.SetDirty(obj);
            }
        }

        public static void ImmediateIntField(Func<int> getter, Action<int> setter, string label, UnityEngine.Object obj = null)
        {
            var value = getter();
            var newValue = EditorGUILayout.IntField(label, value);

            if (!value.Equals(newValue))
            {
                if (obj != null) Undo.RecordObject(obj, label);
                setter(newValue);
                if (obj != null) EditorUtility.SetDirty(obj);
            }
        }

        public static void EnumField(Func<Enum> getter, Action<Enum> setter, string label, UnityEngine.Object obj = null)
        {
            var value = getter();
            var newValue = EditorGUILayout.EnumPopup(label, value);

            if (!value.Equals(newValue))
            {
                if (obj != null) Undo.RecordObject(obj, label);
                setter(newValue);
                if (obj != null) EditorUtility.SetDirty(obj);
            }
        }

        public static void Popup(Action<int> setter, string label, int index, string[] options, UnityEngine.Object obj = null)
        {
            var newIndex = EditorGUILayout.Popup(index, options);

            if (index >= 0 && index != newIndex)
            {
                if (obj != null) Undo.RecordObject(obj, label);
                setter(newIndex);
                if (obj != null) EditorUtility.SetDirty(obj);
            }
        }

        public static void PopupLabeled(Action<int> setter, string label, int index, string[] options, UnityEngine.Object obj = null)
        {
            var newIndex = EditorGUILayout.Popup(label, index, options);

            if (index >= 0 && index != newIndex)
            {
                if (obj != null) Undo.RecordObject(obj, label);
                setter(newIndex);
                if (obj != null) EditorUtility.SetDirty(obj);
            }
        }

        public static void BoolField(Func<bool> getter, Action<bool> setter, string label, UnityEngine.Object obj = null)
        {
            var value = getter();
            var newValue = GUILayout.Toggle(value, label);

            if (!value.Equals(newValue))
            {
                if (obj != null) Undo.RecordObject(obj, label);
                setter(newValue);
                if (obj != null) EditorUtility.SetDirty(obj);
            }
        }

        public static void BoolFieldWithTooltip(Func<bool> getter, Action<bool> setter, string label, string tooltip, UnityEngine.Object obj = null)
        {
            var value = getter();
            var newValue = GUILayout.Toggle(value, new GUIContent(label, tooltip));

            if (!value.Equals(newValue))
            {
                if (obj != null) Undo.RecordObject(obj, label);
                setter(newValue);
                if (obj != null) EditorUtility.SetDirty(obj);
            }
        }
    }
}