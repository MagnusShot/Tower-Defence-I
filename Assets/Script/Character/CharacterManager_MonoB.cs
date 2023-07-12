using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Cinemachine;
using System.Threading.Tasks;

public class CharacterManager_MonoB : MonoBehaviour
{
    [SerializeField] AttackInstance_Scriptable _basicGunAttack;
    [SerializeField] float _dmgMod = 1;

    [SerializeField] Transform _gunBarrel;
    [SerializeField] Transform _gunBone;
    [SerializeField] Transform _debugTarget;
    [SerializeField] LayerMask _aimMask;
    Vector3 _currentAimTarget;
    Vector3 _desiredAimTarget;


    [SerializeField] CinemachineVirtualCamera _normalCamera;
    [SerializeField] CinemachineVirtualCamera _aimCamera;

    AttackData attackData;

    public bool isAiming;


    // Start is called before the first frame update
    void Start()
    {
        //Setup dos valores do ataque
        attackData = _basicGunAttack.SetupAttack();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAiming)
        {
            AllingGunAndCamera();
            _normalCamera.gameObject.SetActive(false);
            _aimCamera.gameObject.SetActive(true);
        }
        else
        {
            _gunBone.transform.localRotation = Quaternion.Lerp(_gunBone.transform.localRotation, Quaternion.identity, 20 * Time.deltaTime);
            _normalCamera.gameObject.SetActive(true);
            _aimCamera.gameObject.SetActive(false);
        }

        //Update dos valores
        attackData.dmgMod = _dmgMod;
        _basicGunAttack.UpdateAttack(attackData, out attackData);
    }

    void AllingGunAndCamera()
    {
        //Raycast da camera para o target
        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height/2);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, 999, _aimMask))
        {
            _desiredAimTarget = (_debugTarget.position - _gunBone.transform.position).normalized;
            _debugTarget.position = hit.point;
        }
        else
        {
            _desiredAimTarget = Camera.main.transform.forward;
            _debugTarget.position = transform.position + Camera.main.transform.forward*10;
        }

        //Lerp da posição anterior da mira para a posição atual da mira
        _currentAimTarget = Vector3.Lerp(_currentAimTarget, _desiredAimTarget, Time.deltaTime * 20f);

        //gira o barrel para sincronizar com a mira
        _gunBone.transform.forward = _currentAimTarget;
    }

    public void CallAttack()
    {
        if(isAiming)
            if(_basicGunAttack.CallAttack(attackData, _gunBarrel, out attackData))
            {
                CameraShake(0.1f, 1.5f);
            }

    }

    public void CallReload()
    {
        _basicGunAttack.CallReload(attackData, out attackData); 
    }

    public async void CameraShake(float duration, float intensity)
    {
        CinemachineBasicMultiChannelPerlin shakeable = _aimCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        float shakeDuration = duration;
        while (shakeDuration > 0) {
            if(shakeable.m_AmplitudeGain < intensity)
                shakeable.m_AmplitudeGain = intensity;
            shakeDuration -= Time.deltaTime;
            await Task.Yield();
        }
        shakeable.m_AmplitudeGain = 0;
    }
}
