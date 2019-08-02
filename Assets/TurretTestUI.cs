using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SpaceCommander.Weapons.Tests
{
    public class TurretTestUI : MonoBehaviour
    {
        public Text WeaponTypeText;

        // GUI captions
        private string[] _fxTypeName =
        {
            "Vulcan", "Sniper", "Shotgun", "Guardian beam", "Pulse laser"
        };
        private void Start()
        {
            SetWeaponTypeText();
        }

        void SetWeaponTypeText()
        {
            WeaponTypeText.text = _fxTypeName[(int) TurretTest.instance.turret.defaultWeaponType];
        }
        
        public void OnButtonNext()
        {
            ToggleWeaponType(true);
        }

        public void OnButtonPrevious()
        {
            ToggleWeaponType(false);
        }

        private void ToggleWeaponType(bool next)
        {
            if (next) TurretTest.instance.turret.NextWeapon();
                else TurretTest.instance.turret.PrevWeapon();
            TurretTest.instance.turret.Stop();
            
            SetWeaponTypeText();
        }

        private void Update()
        {
            // Switch weapon types using keyboard keys
            if (Input.GetKeyDown(KeyCode.E))
                OnButtonNext();
            else if (Input.GetKeyDown(KeyCode.Q))
                OnButtonPrevious();
        }
    }
}