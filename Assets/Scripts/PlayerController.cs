using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test.Player
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField] private CharacterController _controller;
        [SerializeField] private float _movementSpeed;

        private void FixedUpdate()
        {
            var inputX = Input.GetAxis("Horizontal");
            var inputY = Input.GetAxis("Vertical");

            if (inputX == 0 && inputY == 0)
            {
                _controller.Move(Physics.gravity);
                return;
            }

            var direction = new Vector3(inputX, 0.1f, inputY);
            _controller.Move(direction.normalized * _movementSpeed + Physics.gravity);
        }
    }
}