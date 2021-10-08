using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IsometricCoordTest : MonoBehaviour
{
    public GameObject trackingObject;
    public Text coordinateText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        coordinateText.text = IsometricCoordinateUtilites.TranslateSceneToIso(trackingObject.transform.position).ToString();
    }
}
