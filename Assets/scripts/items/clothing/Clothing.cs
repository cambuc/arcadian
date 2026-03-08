using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clothing : Item
{
    public enum Type
    {
       Headwear, Shirt, Jacket, Underwear, Trousers, Shoes, Socks, Back
    }
    public Type type;

    public int coldResist;
}
