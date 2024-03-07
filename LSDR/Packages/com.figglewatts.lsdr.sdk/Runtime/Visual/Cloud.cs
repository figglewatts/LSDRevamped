using System;
using LSDR.SDK.Util;
using UnityEngine;

namespace LSDR.SDK.Visual
{
    public class Cloud : MonoBehaviour
    {
        protected const float TIME_BETWEEN_EVENTS_SECONDS = 5f;

        protected float _t = 0;
        protected Vector3 _moveVec;
        protected float _rotationAmount;
        protected bool _canMove = false;

        public void Start()
        {
            if (RandUtil.OneIn(4))
            {
                _canMove = true;
            }
        }

        public void Update()
        {
            if (!gameObject.activeInHierarchy || !_canMove) return;

            if (_t > TIME_BETWEEN_EVENTS_SECONDS)
            {
                _t = 0;

                int choice = RandUtil.Int(4);
                if (choice == 0)
                {
                    reset();
                }
                else if (choice == 1)
                {
                    rotate();
                }
                else if (choice == 2)
                {
                    move();
                }
                else if (choice == 3)
                {
                    rotate();
                    move();
                }
            }

            if (_moveVec != Vector3.zero) transform.localPosition += _moveVec;
            if (_rotationAmount > 0) transform.Rotate(Vector3.up, _rotationAmount, Space.World);

            _t += Time.deltaTime;
        }

        protected void move()
        {
            _moveVec = new Vector3(RandUtil.Float(-1, 1), 0, RandUtil.Float(-1, 1)).normalized;
            _moveVec *= Time.deltaTime * RandUtil.Float(1, 3);
        }

        protected void rotate()
        {
            _rotationAmount = RandUtil.Float(20, 50) * Time.deltaTime;
        }

        protected void reset()
        {
            _moveVec = Vector3.zero;
            _rotationAmount = 0;
        }
    }
}
