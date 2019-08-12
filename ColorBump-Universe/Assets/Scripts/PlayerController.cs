using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
	public class PlayerController : MonoBehaviour
	{
		public event Action GetToFinish = ()=> { };

		[SerializeField]
		private float _moveSpeed = 0.035f;
		private Transform _transform;
		private Rigidbody _rigidbody;
		private bool _startInput = false;
		private Vector2 _startInputPosition;
		private Vector2 _lastInputPosition;
		private Vector2 _beforeLastInputPosition;
		private float _swipeSpeed = 1f;
		private const float _forseLimit = 10f;
		private const float _speedLimit = 1f;
		private const float _xBorderLimit = 4.5f;
		private Vector3 _newForce;
		private Vector3 _newMove;

		private void Start()
		{
			Input.multiTouchEnabled = false;
			_transform = transform;
			_rigidbody = GetComponent<Rigidbody>();
		}

		public void HandleForce(Vector3 vectorForce)
		{
			_newForce = new Vector3(Mathf.Clamp(vectorForce.x, -_forseLimit, _forseLimit), 0, Mathf.Clamp(vectorForce.z, -_forseLimit, _forseLimit));
		}

		public void HandleMove(Vector3 vectorMove)
		{
			_newMove = new Vector3(Mathf.Clamp(vectorMove.x, -_speedLimit, _speedLimit), 0, Mathf.Clamp(vectorMove.z, -_speedLimit, _speedLimit));
		}

		private void FixedUpdate()
		{
			if (_newForce != Vector3.zero)
			{
				_rigidbody.AddForce(_newForce, ForceMode.VelocityChange);
				if (_rigidbody.position.x > _xBorderLimit || _rigidbody.position.x < -_xBorderLimit) // avoid falling
				{
					_rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, _rigidbody.velocity.z);
				}
			} else if (_newMove != Vector3.zero)
			{
				_rigidbody.velocity = Vector3.zero;
				var newPosition = _rigidbody.position + _newMove;
				_rigidbody.MovePosition(new Vector3(Mathf.Clamp(newPosition.x, -_xBorderLimit, _xBorderLimit), newPosition.y, newPosition.z));
				if (_rigidbody.position.x > _xBorderLimit || _rigidbody.position.x < -_xBorderLimit)
				{
					_rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, _rigidbody.velocity.z);
				}
			}


			_newForce = Vector3.zero;
			_newMove = Vector3.zero;
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Finish"))
			{
				GetToFinish();
			}
		}
	}
}