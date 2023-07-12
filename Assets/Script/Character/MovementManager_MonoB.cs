using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Collections;
using UnityEngine;
using static UnityEngine.UI.ScrollRect;

public class MovementManager_MonoB : MonoBehaviour
{
    //---Variables---
    CharacterController _chController;

    Vector3 _movementDir;
    Vector3 _visualDir;

    Vector3 _currentControlledVelocity;
    Vector3 _physicalSpeed;

    bool _isRunning;

    [SerializeField] float _speedWalking;
    [SerializeField] float _speedRunning;
    [SerializeField] float _accelMod;

    [SerializeField] float _rotationSpeed;

    [SerializeField] AnimationCurve _acAccelFactor;

    [SerializeField] float _gravity;
    [SerializeField] float _maxFallSpeed;
    [SerializeField] float _jumpForce;
    [SerializeField] float _timeToJump;

    [SerializeField] float _fTurnSmoothVelocity1;
    [SerializeField] float _fTurnSmoothVelocity2;
    [SerializeField] float _fTurnSmoothTime;

    //--Util--
    movementType _currentMoveState = movementType.Normal;
    float _currentTimeGrounded;
    private enum movementType
    {
        Normal = 0,
    }

    private float _targetAngle
    { get {
            if (_visualDir == Vector3.one || _isRunning)
                return Mathf.Atan2(_movementDir.x, _movementDir.z) * Mathf.Rad2Deg;
            else
                return _visualDir.y; } }
    
    //--Acesso--
    public Vector3 movementDir { 
        get => _movementDir;
        set => _movementDir = value; }
    public Vector3 visualDir { 
        get => _visualDir;
        set => _visualDir = value; }
    public bool isRunning {
        get => _isRunning;
        set => _isRunning = value;}

    // Start is called before the first frame update
    void Start()
    {
        _chController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //se o personagem ter a direção do movimento zerado, cancela a corrida
        if (movementDir == Vector3.zero)
            isRunning = false;
        //se a direção do movimento é oposta ao movimento atual, cancela a corrida
        if (Vector3.Distance(movementDir, _currentControlledVelocity.normalized) > 1)
            isRunning = false;


        //Aplicação do movimento baseado no estado
        switch (_currentMoveState)
        {
            case movementType.Normal:
                if (_chController.isGrounded)
                    OnGroundNormal();
                else
                    OnAirNormal();
                break;
        }
    }


    //---Estados de Movimento---
    private void OnGroundNormal()
    {
        Vector3 controlledMove = Vector3.zero;

        //verifica o tempo que esta no chão
        _currentTimeGrounded += 1 * Time.deltaTime;
        //Quando não esta correndo
        if (_isRunning)
            controlledMove = ControlledMovement(_movementDir, _currentControlledVelocity, _speedRunning, _speedWalking * _accelMod, _acAccelFactor);
        //Quando esta correndo
        else
        {
            _isRunning = false;
            controlledMove = ControlledMovement(_movementDir, _currentControlledVelocity, _speedWalking, _speedWalking * _accelMod, _acAccelFactor);
        }


        //Faz o jogador girar para a direção do movimento durante o movimento ou durante ataques
        if (_movementDir != Vector3.zero)
        {
            float angle_ = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _fTurnSmoothVelocity1, _rotationSpeed);
            transform.eulerAngles = new Vector3(0, angle_, 0);
        }

        _currentControlledVelocity = controlledMove;


        //Aplica gravidade Fisica
        Gravity();
        HorizontalImpulsePhysics();

        //Soma os Movimentos
        Vector3 nextMove = controlledMove + _physicalSpeed;

        //Efetua o movimento
        _chController.Move(nextMove * Time.deltaTime);
    }

    private void OnAirNormal()
    {
        Vector3 controlledMove = Vector3.zero;

        //Reseta o contador

        if (_currentTimeGrounded > 0)
        {
            //Se a velocidade inicial de queda dele é menor que zero ela volta a ser zero para evitar que quedas sejam bruscas demais
            if (_physicalSpeed.y < 0)
                _physicalSpeed.y = 0;
            _currentTimeGrounded = 0;

        }

        controlledMove = ControlledMovement(_movementDir, _currentControlledVelocity, _speedWalking, _speedWalking / 6, _acAccelFactor);

        Vector3 deltaMove = _physicalSpeed + controlledMove;
        //Faz o jogador girar para a direção do movimento durante o movimento ou durante ataques
        if (deltaMove != Vector3.zero)
        {
            float angle_ = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetAngle, ref _fTurnSmoothVelocity1, _rotationSpeed);
            transform.eulerAngles = new Vector3(0, angle_, 0);
        }

        _currentControlledVelocity = controlledMove;

        //Aplica gravidade Fisica
        Gravity();
        HorizontalImpulsePhysics();

        //Soma os Movimentos
        Vector3 nextMove = controlledMove + _physicalSpeed;

        //Efetua o movimento
        _chController.Move(nextMove * Time.deltaTime);
    }

    private Vector3 ControlledMovement(Vector3 moveDir, Vector3 currentVelocity, float moveSpeed, float moveAccel, AnimationCurve accelCurve)
    {
        //verifica a direção do movimento anterior
        Vector3 lastMoveDir = currentVelocity;

        //Transforma a distancia entre a direção do movimento desejado e a direção do movimento anterior em um float
        float velDot = Vector3.Dot(moveDir, lastMoveDir);
        //Define a acceleração baseada no Factor e Direção anterior
        float accel = moveAccel * accelCurve.Evaluate(velDot);

        //Calcula a velocidade e direção que o jogador quer ir no momento
        Vector3 goalVelocity = moveDir * moveSpeed; //adicionar um speedfactor que n faço a menor ideia do que é ainda

        //progressivamente almenta a velocidade para chegar na velocidade desejada
        Vector3 newVelocity = Vector3.MoveTowards(currentVelocity, goalVelocity, accel * Time.deltaTime);

        return newVelocity;
    }

    private void Gravity()
    {
        //se tocar no chão Y = 0
        if (_chController.isGrounded && _physicalSpeed.y < 0)
        {
            _physicalSpeed.y = -2;
        }
        //se a velocidade em Y for menor que a queda maxima ele continua acelerando
        else if (_physicalSpeed.y > -_maxFallSpeed)
        {
            _physicalSpeed.y -= _gravity * Time.deltaTime;
        }
        //caso nem um dos casos ocorra a gravidade afeta normalmente
        else
        {
            _physicalSpeed.y -= _gravity * Time.deltaTime;
        }
    }

    private void HorizontalImpulsePhysics()
    {
        //consome o impulso no ar
        if (!_chController.isGrounded)
            _physicalSpeed = Vector3.Lerp(new Vector3(_physicalSpeed.x, 0, _physicalSpeed.z), Vector3.zero, 0.5f * Time.deltaTime) + new Vector3(0, _physicalSpeed.y, 0);
        //consome mais o impulso no chão
        else
            _physicalSpeed = Vector3.Lerp(new Vector3(_physicalSpeed.x, 0, _physicalSpeed.z), Vector3.zero, 5 * Time.deltaTime) + new Vector3(0, _physicalSpeed.y, 0);
    }


    //---ImputResponses---
    public void Jump()
    {
        //se estiver no chão e o tempo especificado tiver passado, efetua um pulo
        if (_chController.isGrounded && _timeToJump !< _currentTimeGrounded)
        {
            _physicalSpeed.y = _jumpForce;
        }
    }
}
