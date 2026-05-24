using UnityEngine;

public class ChangeImage : MonoBehaviour
{
    public GameObject isOn;
    public GameObject isOff;

    public void SetImageOn(bool isValueChanged)
    {
        isOn.SetActive(isValueChanged);
        isOff.SetActive(!isValueChanged);
    }
}