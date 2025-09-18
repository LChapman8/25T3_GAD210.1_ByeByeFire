using UnityEngine;

public class Fire : MonoBehaviour
{
    public enum FireType { None, TypeA, TypeB }
    public FireType currentType = FireType.None;

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

    public bool TryExtinguish(string extinguisherType)
    {
        if ((currentType == FireType.TypeA && extinguisherType == "Water") ||
            (currentType == FireType.TypeB && extinguisherType == "CO2"))
        {
            SetFire(FireType.None);
            return true;
        }
        return false;
    }
}
