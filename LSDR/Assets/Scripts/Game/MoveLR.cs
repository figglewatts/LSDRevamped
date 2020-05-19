using System;
using UnityEngine;

namespace LSDR.Game
{
    [RequireComponent(typeof(Rigidbody))]
    public class MoveLR : MonoBehaviour
    {
        public float Speed = 1f;
        public float Amount = 5f;

        private Rigidbody _body;
        private Vector3 _anchor;

        public void Awake()
        {
            _body = GetComponent<Rigidbody>();
            _anchor = transform.position;
        }

        public void FixedUpdate()
        {
            float pos = Mathf.Sin(Time.fixedTime * Speed) * Amount;
            _body.MovePosition(_anchor + Vector3.forward * pos);
        }
    }
}
