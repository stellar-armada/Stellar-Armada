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

using System.Collections;

using UnityEngine;


namespace LeiaLoft.Examples {

    public class Spin : MonoBehaviour {

        public float start =  0.0f;
        public float end = 180.0f;

        public float speed = 1.0f;

        private Quaternion _start;
        private Quaternion _end;

        private float _rotate;


        protected void Start ( ) {
            _start = transform.rotation * Quaternion.Euler(transform.up * start);
            _end = transform.rotation * Quaternion.Euler(transform.up * end);
        }

        protected void Update ( ) {
            _rotate = Mathf.PingPong(Time.time * speed, 1.0f);
            transform.localRotation = Quaternion.Slerp(_start, _end, _rotate);
        }
    }
}