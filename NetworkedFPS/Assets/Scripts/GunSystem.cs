using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;
using static Unity.VisualScripting.Member;

public class GunSystem : NetworkBehaviour
{
    public int damage;
    public float timeBetweenBullets, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsRemaining, shotsRemainingInBurst;

    bool shooting, readyToShoot, reloading;

    public AudioSource audioSource;
    public Camera fpsCam;

    public RaycastHit rayHit;
    public LayerMask enemyLayer;

    public float UpperAudio;
    public float lowerAudio;

    public AudioClip bulletSound;
    public GameObject muzzleReference;
    public Transform muzzleReferencePosition;

    [SyncVar]
    private uint ownerId;

    [SerializeField]
    private ParentConstraint constraint;


    private void Start()
    {
        fpsCam = FindObjectOfType<Camera>();
        bulletsRemaining = magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isOwned)
        {
           Constrain();
        }
    }

    public void AssignOwner(uint ownerId)
    {
        this.ownerId = ownerId;
    }

    public void Constrain()
    {
        transform.parent = FindObjectOfType<Camera>().transform;
        transform.localPosition = NetworkClient.spawned[ownerId].GetComponent<Player>().gunAttachPoint.transform.localPosition;
        transform.localRotation = NetworkClient.spawned[ownerId].GetComponent<Player>().gunAttachPoint.transform.localRotation;
    }

    private void MyInput()
    {
        
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsRemaining < magazineSize && !reloading) Reload();

        if (readyToShoot && shooting && !reloading && bulletsRemaining > 0)
        {
            shotsRemainingInBurst = bulletsPerTap;
            Shoot();
        }

    }

    private void Shoot()
    {
        if (!isLocalPlayer) return;

        PlayGunSound();
        readyToShoot = false;

        PlayGunEffect();

        bulletsRemaining--;

        GetBulletSpreadValues(out Vector2 spreadValues);
        
        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(spreadValues.x, spreadValues.y, 0);

        CmdRaycastForPlayer(direction);

        bulletsRemaining--;
        //Purely for burst or single tap weapons
        shotsRemainingInBurst--;

        Invoke("ResetShot", timeBetweenBullets);

        if (shotsRemainingInBurst > 0 &&  bulletsRemaining > 0) 
            Invoke("Shoot", timeBetweenShots);
    }

    private void GetBulletSpreadValues(out Vector2 spreadValues)
    {
        float x = UnityEngine.Random.Range(-spread, spread);
        float y = UnityEngine.Random.Range(-spread, spread);

        spreadValues = new Vector2(x, y);
    }

    [ClientRpc]
    private void PlayGunEffect()
    {
        GameObject flash = Instantiate(muzzleReference, muzzleReferencePosition.position, Quaternion.identity);
        flash.transform.parent = transform;
    }

    [ClientRpc]
    private void PlayGunSound()
    {
        audioSource.pitch = UnityEngine.Random.Range(lowerAudio, UpperAudio);
        audioSource.PlayOneShot(audioSource.clip);
    }

    [Command]
    private void CmdRaycastForPlayer(Vector3 direction)
    {
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, enemyLayer))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.gameObject.GetComponent<IDamageable>() != null)
            {
                rayHit.collider.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                Debug.Log("HIT OTHER PLAYER");
            }
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void ReloadFinished()
    {
        bulletsRemaining = magazineSize;
        reloading = false;
    }

}
