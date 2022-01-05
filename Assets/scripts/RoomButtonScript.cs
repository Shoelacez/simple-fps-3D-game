using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomButtonScript : MonoBehaviour
{
    public Text buttonText;
    private RoomInfo info;
    public void setButtonDetails(RoomInfo inputInfo)
    {
        info = inputInfo;
        buttonText.text = info.Name;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
