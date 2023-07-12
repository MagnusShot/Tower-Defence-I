using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager_MonoB : MonoBehaviour
{
    //--Modulos--
    [SerializeField] GameObject _playerCamera;
    PlayerInput _playerInput;

    CharacterManager_MonoB _characterManager;
    MovementManager_MonoB _movementManager;

    //--Ações--
    InputAction _movementAction;

    InputAction _runAction;
    InputAction _jumpAction;

    InputAction _attackAction;
    InputAction _aimAction;
    InputAction _reloadAction;

    //---Ativadores---
    private void Start()
    {
        _characterManager = GetComponent<CharacterManager_MonoB>();
        _movementManager = GetComponent<MovementManager_MonoB>();
        _playerInput = GetComponent<PlayerInput>();

        _movementAction = _playerInput.actions["Movement"];
        _runAction = _playerInput.actions["Run"];
        _jumpAction = _playerInput.actions["Jump"];
        _attackAction = _playerInput.actions["Fire"];
        _aimAction = _playerInput.actions["Aim"];
        _reloadAction = _playerInput.actions["Reload"];
    }

    public void Update()
    {
        HorizontalMoveImput();
        CameraToFocusDesign(); 
        OthersImput();
    }

    void HorizontalMoveImput()
    {
        //Recebe os Imputs e os transformam em uma direção
        Vector3 horizontal_ = Vector3.right * _movementAction.ReadValue<Vector2>().x;
        Vector3 vertical_ = Vector3.forward * _movementAction.ReadValue<Vector2>().y;
        Vector3 orientation_ = (vertical_ + horizontal_).normalized;

        Vector3 moveDir_ = new Vector3(0, 0, 0);


        //Define Direção relativa a camera   
        if (orientation_.magnitude >= 0.1f)
        {
            float targetAngle_ = Mathf.Atan2(orientation_.x, orientation_.z) * Mathf.Rad2Deg + _playerCamera.transform.eulerAngles.y;
            moveDir_ = Quaternion.Euler(0f, targetAngle_, 0f) * Vector3.forward;
        }
        moveDir_.y = 0;

        //ordenar o movimento
        _movementManager.movementDir = moveDir_;
    }

    private void CameraToFocusDesign()
    {
        _movementManager.visualDir = _playerCamera.transform.rotation.eulerAngles;
    }

    private void OthersImput()
    {
        _characterManager.isAiming = _aimAction.IsPressed();

        //Chama o Ataque
        if (_attackAction.triggered)
            _characterManager.CallAttack();

        //Chama a recarga
        if (_reloadAction.triggered)
            _characterManager.CallReload();

        //Mantem Correndo
        if (_runAction.IsPressed())
            _movementManager.isRunning = true;

        //Ativa o pulo
        if (_jumpAction.triggered)
            _movementManager.Jump();
    }
}
