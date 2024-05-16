using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UpdateGeoFields : MonoBehaviour
{
    private TssStateManger tss;

    [SerializeField]
    private TextMeshProUGUI Title;
    // Start is called before the first frame update
    void Start()
    {
        tss = gameObject.AddComponent<TssStateManger>();


    }

    // Update is called once per frame
    void Update()
    {

        if (tss.Rocks.Any())
        {
            Debug.Log(tss.Rocks[0]);
            Title.text = tss.Rocks[0].name;
        }
    }
}
