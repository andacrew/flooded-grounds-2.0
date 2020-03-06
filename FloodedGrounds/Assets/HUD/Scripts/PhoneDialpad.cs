using UnityEngine;
using UnityEngine.UI;

public class PhoneDialpad : MonoBehaviour {

    public Text numberText;

    public void AddNumber (string addNumber)
    {
        numberText.text = numberText.text + addNumber;
	}
}
