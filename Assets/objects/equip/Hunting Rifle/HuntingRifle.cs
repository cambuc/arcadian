using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class HuntingRifle : Equippable
{
    public FirearmItem item;

    public int magazineCapacity;

    public GameObject chamberedCartridgeObject;
    public GameObject chamberedCasingObject;
    public GameObject ejectionBullet;

    public bool chambered;

    public FirearmMagazine magazine;
    public Bullet bulletPrefab;
    public Transform bulletPositioner;
    public Item cartridgeItem;

    public int chamberingTime;

    public int reloadBeginTime;
    public int reloadSingleTime;
    public int reloadEndTime;

    public float recoilRotation;
    public float recoilPushback;

    [Header("Controls")]
    public string chamberControl = "r";
    public string reloadControl = "t";
    public string inspectChamberControl = "f";
    public string inspectMagazineControl = "g";

    [Header("Audio Settings")]
    public AudioSource chamberingSound;
    public float chamberingSplitTime;
    public AudioSource magOut;
    public AudioSource magIn;
    public AudioClip fireSound;
    public AudioReverbPreset firingEcho;
    public AudioClip ringingSound;

    public List<AudioClip> loadSounds = new List<AudioClip>();

    //[Header("Camera Recoil")]
    //public float recoilLength;
    //public float recoilReturnLength;
    //public float recoilMagnitude;
    //Vector3 camStartPos;

    bool busy;

    public override void Equip(UnityAction onEquipped)
    {
        Initialize();
        HuntingRifleControls.runtime.root.SetActive(true);
        CallOnAnimationEnd(onEquipped);
    }
    public override void Unequip(UnityAction onUnequipped)
    {
        anim.Play("Unequip");
        HuntingRifleControls.runtime.root.SetActive(false);
        CallOnAnimationEnd(onUnequipped);
    }

    private void Initialize()
    {
        magazine.Initialize(item);

        chamberedCartridgeObject.SetActive(chambered);
        chamberedCasingObject.SetActive(false);
        ejectionBullet.SetActive(false);
    }

    private void Update()
    {
        if (PlayerMovement.runtime.crouching) anim.SetBool("Stable", true);
        else anim.SetBool("Stable", false);

        //Chamber a Round
        if (Input.GetKeyDown(chamberControl) && !busy)
        {
            Chamber();
        }

        //Reload Magazine
        if (Input.GetKeyDown(reloadControl) && !busy)
        {
            Reload();
        }


        //Aiming
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            PlayerHud.runtime.DisableCrosshair();
            Aim();
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            PlayerHud.runtime.EnableCrosshair();
            ReleaseAim();
        }

        //Firing
        if (Input.GetKey(KeyCode.Mouse1) && Input.GetKeyDown(KeyCode.Mouse0) && chambered)
        {
            Fire();
        }

        //Inspect Chamber
        if (Input.GetKeyDown(inspectChamberControl) && !chamberedCasingObject.activeSelf)
        {
            InspectChamber();
        }
        if (Input.GetKeyUp(inspectChamberControl) && !chamberedCasingObject.activeSelf)
        {
            UninspectChamber();
        }

        //Inspect Magazine
        if (Input.GetKeyDown(inspectMagazineControl))
        {
            anim.SetBool("Reloading", true);
            magOut.Play();
        }
        if (Input.GetKeyUp(inspectMagazineControl))
        {
            anim.SetBool("Reloading", false);
        }
    }

    async void Chamber()
    {
        busy = true;
        anim.SetTrigger("Chambering");
        chamberingSound.time = 0;
        chamberingSound.Play();

        chamberedCartridgeObject.SetActive(magazine.count > 0);
        if (chambered)
        {
            chamberedCasingObject.SetActive(true);
            ejectionBullet.SetActive(true);
            PlayerInventory.runtime.AddItem(cartridgeItem);
        }

        await Task.Delay(chamberingTime);

        chamberedCasingObject.SetActive(false);
        busy = false;

        if (magazine.count > 0)
        {
            magazine.RemoveCartridge();
            chambered = true;
            item.chambered = chambered;
        }
        else
        {
            chambered = false;
            item.chambered = chambered;
        }
    }

    void Aim()
    {
        anim.SetBool("Aiming", true);
    }
    void ReleaseAim()
    {
        anim.SetBool("Aiming", false);
    }

    void Fire()
    {
        chambered = false;
        item.chambered = chambered;
        FaunaBehavior.spookEvent.Invoke(true);

        anim.Play("Fire");
        PlayerMovement.runtime.RecoilCamera(recoilRotation, recoilPushback);

        Bullet bullet = Instantiate(bulletPrefab.gameObject, bulletPositioner).GetComponent<Bullet>();
        bullet.transform.localPosition = Vector3.zero;
        bullet.transform.localEulerAngles = Vector3.zero;
        bullet.transform.localScale = Vector3.one;
        bullet.transform.parent = null;
        bullet.Shoot();

        new UniversalPlayer(fireSound, firingEcho, MixerGroupHolder.runtime.sfx);
        new UniversalPlayer(ringingSound, MixerGroupHolder.runtime.sfx);

        chamberedCartridgeObject.SetActive(false);
        chamberedCasingObject.SetActive(true);
        ejectionBullet.SetActive(false);
    }

    async void Reload()
    {
        busy = true;
        anim.SetBool("Reloading", true);
        magOut.Play();
        await Task.Delay(reloadBeginTime);

        while (magazine.count < magazineCapacity && PlayerInventory.runtime.HasItem(cartridgeItem))
        {
            anim.Play("ReloadSingle");
            new UniversalPlayer(loadSounds[Random.Range(0, loadSounds.Count)], MixerGroupHolder.runtime.sfx);
            await magazine.AnimateAddCartridge(reloadSingleTime);
            PlayerInventory.runtime.RemoveItem(cartridgeItem);
        }

        anim.SetBool("Reloading", false);
        await Task.Delay(reloadEndTime);
        busy = false;
    }

    public void PlayMagInSound()
    {
        magIn.Play();
    }

    async void InspectChamber()
    {
        anim.SetBool("InspectingChamber", true);

        chamberingSound.time = 0;
        chamberingSound.Play();
        await Task.Delay((int)(chamberingSplitTime * 1000));
        chamberingSound.Stop();
    }
    void UninspectChamber()
    {
        anim.SetBool("InspectingChamber", false);

        chamberingSound.time = chamberingSplitTime;
        chamberingSound.Play();
    }

    //async void RecoilCamera()
    //{
    //    camStartPos = Camera.main.transform.position;
    //    Vector3 targetPos = Camera.main.transform.position + Camera.main.transform.forward * -recoilMagnitude;

    //    for(float i = 0; i < recoilLength; i += 0.01f)
    //    {
    //        Camera.main.transform.position = Vector3.Lerp(camStartPos, targetPos, i / recoilLength);
    //        await Task.Delay(10);
    //    }
    //    for (float i = 0; i < recoilReturnLength; i += 0.01f)
    //    {
    //        Camera.main.transform.position = Vector3.Lerp(targetPos, camStartPos, i / recoilReturnLength);
    //        await Task.Delay(10);
    //    }

    //    Camera.main.transform.position = camStartPos;
    //}
}
