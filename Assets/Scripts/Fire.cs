using UnityEngine;

/// <summary>
/// This script controls the fire cubes, by setting their material to a specific colour (firetype) and allows them to be estingushiable
/// </summary>
public class Fire : MonoBehaviour
{
    public enum FireType { None, TypeA, TypeB }
    public enum ExtinguisherType { None, Water, CO2 }

    public FireType currentType = FireType.None;
    private bool alreadyExtinguished = false; 

    // declaring my material types
    [Header("Materials")]
    public Material normalMat;
    public Material typeAMat; // my red matieral = Class A fire 
    public Material typeBMat; // my orange material = Class B fire

    private Renderer rend;

    // on start get the renderer and chage all fires to no fire material
    void Start()
    {
        rend = GetComponent<Renderer>();
        SetFire(FireType.None);
    }

    // function for setting the cubes to different fire types
    public void SetFire(FireType type)
    {
        currentType = type;
        alreadyExtinguished = false;
        switch (type)
        {
            case FireType.None:
                rend.material = normalMat;
                break;
            case FireType.TypeA:
                rend.material = typeAMat;
                break;
            case FireType.TypeB:
                rend.material = typeBMat;
                break;
        }
    }

    // function for attempting to extinguish fires if the correct type of extinguisher is used
    public bool TryExtinguish(ExtinguisherType extinguisherType)
    {
        if (currentType == FireType.None || alreadyExtinguished)
            return false;

        if ((currentType == FireType.TypeA && extinguisherType == ExtinguisherType.Water) ||
            (currentType == FireType.TypeB && extinguisherType == ExtinguisherType.CO2))
        {
            SetFire(FireType.None);
            alreadyExtinguished = true;
            return true;
        }

        return false;
    }
}
