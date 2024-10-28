using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Audio;

public class GunSystem : NetworkBehaviour
{
    public int damage;
    public float timeBetweenBullets, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    public bool allowButtonHold;
    int bulletsLeft, bulletsShot;

    bool shooting, readyToShoot, reloading;

    public AudioSource[] audioSourcePool;
    public Camera fpsCam;

    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

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
        bulletsLeft = magazineSize;
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
           //Constrain();
        }
    }

    public void AssignOwner(uint ownerId)
    {
        this.ownerId = ownerId;
    }

    public void Constrain()
    {
        Transform ap = NetworkClient.spawned[ownerId].GetComponent<Player>().gunAttachPoint;
        constraint.constraintActive = true;
        constraint.SetSource(0, new ConstraintSource { sourceTransform = ap, weight = 1 });
    }

    private void MyInput()
    {
        if (allowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !reloading) Reload();

        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }

    }

    [Command]
    private void Shoot()
    {
        Debug.Log("Shooting on server");

        int source = 0;

        audioSourcePool[source].pitch = UnityEngine.Random.Range(lowerAudio, UpperAudio);
        audioSourcePool[source].PlayOneShot(audioSourcePool[source].clip);
        source++;

        if (source == 3) source = 0;
        readyToShoot = false;

        GameObject flash = Instantiate(muzzleReference, muzzleReferencePosition.position, Quaternion.identity);
        flash.transform.parent = transform;

        bulletsLeft--;

        //Spread
        float x = UnityEngine.Random.Range(-spread, spread);
        float y = UnityEngine.Random.Range(-spread, spread);

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);


        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.gameObject.GetComponent<IDamageable>() != null)
            {
                rayHit.collider.gameObject.GetComponent<IDamageable>().TakeDamage(damage);
                Debug.Log("HIT OTHER PLAYER");
            }
        }
        bulletsLeft--;
        bulletsShot--;

        Invoke("ResetShot", timeBetweenBullets);

        if (bulletsShot > 0 &&  bulletsLeft > 0) 
            Invoke("Shoot", timeBetweenShots);
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
        bulletsLeft = magazineSize;
        reloading = false;
    }

}
