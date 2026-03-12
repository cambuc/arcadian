using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class FirearmMagazine : MonoBehaviour
{
    public int count;

    public List<GameObject> cartridges = new List<GameObject>();

    public GameObject push;
    public float pushSingleHeight;
    public Transform flipParent;

    public GameObject animateCartridge;
    public Vector3 animateStartLocalPos;

    Vector3 pushStartPos;

    FirearmItem item;

    public void Initialize(FirearmItem item)
    {
        this.item = item;

        pushStartPos = push.transform.localPosition;
        animateCartridge.SetActive(false);
        SetCartridges(count);
    }
    public void Initialize(int count)
    {
        pushStartPos = push.transform.localPosition;
        animateCartridge.SetActive(false);
        SetCartridges(count);
    }

    public void SetCartridges(int count)
    {
        this.count = count;
        if(item) item.loadedCount = count;

        if (count % 2 == 1) flipParent.localScale = Vector3.one;
        else flipParent.localScale = new Vector3(-1, 1, 1);

        foreach (GameObject cartridge in cartridges) cartridge.SetActive(false);
        for(int i = 0; i < count; i++)
        {
            cartridges[i].SetActive(true);
        }
        push.transform.localPosition = new Vector3(pushStartPos.x, pushStartPos.y, pushStartPos.z - count * pushSingleHeight);
    }

    public void AddCartridge()
    {
        SetCartridges(count + 1);
    }
    public void RemoveCartridge()
    {
        SetCartridges(count - 1);
    }

    public async Task AnimateAddCartridge(int reloadSingleTime)
    {
        float length = reloadSingleTime / 1000f;

        animateCartridge.SetActive(true);
        for (float i = 0; i <= 1; i += 0.01f / length)
        {
            animateCartridge.transform.localPosition = Vector3.Lerp(animateStartLocalPos, cartridges[count % 2 == 0 ? 0 : 1].transform.localPosition, i);
            await Task.Delay(10);
        }
        animateCartridge.SetActive(false);
        AddCartridge();
    }
}
