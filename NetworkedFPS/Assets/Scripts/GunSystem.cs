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
        if (NetworkClient.active)
        {
            if (ownerId == NetworkClient.localPlayer.netId)
            {
                MonitorForShooting();
            }
        }
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

    private void MonitorForShooting()
    {
        //Check if burst or auto and set check accordingly
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        //Check if can reload
        if (Input.GetKeyDown(KeyCode.R) && bulletsRemaining < magazineSize && !reloading) Reload();

        //Check if all requirements met to shoot
        if (readyToShoot && shooting && !reloading && bulletsRemaining > 0)
        {
            shotsRemainingInBurst = bulletsPerTap;
            Shoot();
        }

    }

    //I think this has to be a command as we are running client RPCs inside? Maybe better to separate everything out into its own method
    [Command]
    private void Shoot()
    {

        RpcPlayGunSound(muzzleReferencePosition.position);
        readyToShoot = false;

        RpcPlayGunEffect(muzzleReferencePosition.position);

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
    private void RpcPlayGunEffect(Vector3 effectPos)
    {
        GameObject flash = Instantiate(muzzleReference, effectPos, Quaternion.identity);
        flash.transform.parent = transform;
    }

    [ClientRpc]
    private void RpcPlayGunSound(Vector3 soundPos)
    {
        //audioSource.pitch = UnityEngine.Random.Range(lowerAudio, UpperAudio);
        //audioSource.PlayOneShot(audioSource.clip);

        AudioSource.PlayClipAtPoint(audioSource.clip, soundPos);
    }

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
