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
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace LeiaLoft
{
    public sealed class LeiaPostEffectsController : MonoBehaviour 
    {
        private LeiaCamera _leiaCamera;
        private List<MonoBehaviour> _effects = new List<MonoBehaviour>();
        private int _lastViewCount = 0;

        public void Update()
        {
            if (_leiaCamera == null)
            {
                _leiaCamera = GetComponent<LeiaCamera>();
                if (_leiaCamera == null)
                {
                    DestroyScript(this);
                    return;
                }
            }

            if (LeiaDisplay.InstanceIsNull) return;

            if (IsEffectsChanged())
            {
                CopyEffectsToLeiaViews();
            }
        }

        public void ForceUpdate()
        {
            this.Debug("ForceUpdate");

            if (_leiaCamera == null)
                _leiaCamera = GetComponent<LeiaCamera>();

            if (LeiaDisplay.InstanceIsNull) return;

            IsEffectsChanged();
            CopyEffectsToLeiaViews();
        }

        private bool IsEffectsChanged()
        {
            bool isEffectsChanged = false;

            //Views count changed
            if (_lastViewCount != _leiaCamera.GetViewCount())
            {
                _lastViewCount = _leiaCamera.GetViewCount();
                isEffectsChanged = true;
            }

            //Effects removed
            for (int i = 0; i < _effects.Count; i++)
            {
                if (_effects[i] == null)
                {
                    _effects.RemoveAt(i);
                    i--;
                    isEffectsChanged = true;
                }
            }

            //Effects added or enabled
            foreach (var effect in GetEffectsBehaviors(gameObject))
            {
                if (effect.enabled)
                {
                    if (!_effects.Contains(effect))
                    {
                        _effects.Add(effect);
                        isEffectsChanged = true;
                    }
                }
            }

            //Effects disabled
            for (int i = 0; i < _effects.Count; i++)
            {
                if (!_effects[i].enabled)
                {
                    _effects.Remove(_effects[i]);
                    isEffectsChanged = true;
                    i--;
                }
            }

            return isEffectsChanged;
        }

        public void RestoreEffects()
        {
            this.Debug("RestoreEffects");
            _effects.Clear();

            if (_leiaCamera == null)
                _leiaCamera = GetComponent<LeiaCamera>();

            RemoveEffectsFromLeiaViews();
        }

        private void RemoveEffectsFromLeiaViews()
        {
            for (int i = 0; i < _leiaCamera.GetViewCount(); i++)
            {
                RemoveEffects(GetEffectsBehaviors(_leiaCamera.GetView(i).Object));
            }
        }

        private void CopyEffectsToLeiaViews()
        {
            for (int i = 0; i < _leiaCamera.GetViewCount(); i++)
            {
                CopyEffectsToView(_effects, _leiaCamera.GetView(i).Object);
            }
        }

        private static void CopyEffectsToView(List<MonoBehaviour> effects, GameObject view)
        {
            if (view == null)
                return;

            var oldEffects = GetEffectsBehaviors(view);

            foreach (var effect in effects)
            {
                if (effect == null)
                    continue;

                Component copy = null;

                for (int j = 0; j < oldEffects.Count; j++)
                {
                    if (effect.GetType() == oldEffects[j].GetType())
                    {
                        copy = oldEffects[j];
                        oldEffects.RemoveAt(j);
                        break;
                    }
                }

                if (copy == null)
                {
                    copy = view.AddComponent(effect.GetType());
                }

                if (copy != null)
                {
                    var fields = effect.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);

                    for (int j = 0; j < fields.Length; j++)
                    {
                        fields[j].SetValue(copy, fields[j].GetValue(effect));
                    }

                    //Generate onEnable
                    var behavior = copy as MonoBehaviour;
                    behavior.enabled = false;
                    behavior.enabled = true;
                }
            }

            RemoveEffects(oldEffects);
        }

        private static void RemoveEffects(List<MonoBehaviour> effects)
        {
            foreach (var effect in effects)
            {
                DestroyScript(effect);
            }
        }

        private static List<MonoBehaviour> GetEffectsBehaviors(GameObject gameObject)
        {
            if (gameObject == null)
                return new List<MonoBehaviour>();

            var methodFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            var effects = new List<MonoBehaviour>();
            foreach (var item in gameObject.GetComponents<MonoBehaviour>())
            {
                if (!item)
                    continue;

                if (item.GetType().GetMethod("OnRenderImage", methodFlags) != null)
                    effects.Add(item);
            }

            return effects;
        }

        private static void DestroyScript(Object o)
        {
            if (!Application.isPlaying)
                DestroyImmediate(o);
            else
                Destroy(o);
        }
    }
}