using UnityEngine;

public class Fire : MonoBehaviour
{
    public enum FireType { None, TypeA, TypeB }
    public enum ExtinguisherType { None, Water, CO2 }

    public FireType currentType = FireType.None;
    private bool alreadyExtinguished = false; // track if scored for current fire

    [Header("Materials")]
    public Material normalMat;
    public Material typeAMat; // Red (wood/paper)
    public Material typeBMat; // Orange (electrical/liquid)

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        SetFire(FireType.None);
    }

    public void SetFire(FireType type)
    {
        currentType = type;
        alreadyExtinguished = false; // reset whenever fire changes
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
