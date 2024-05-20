using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GeosampleController : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text sampleScannedText;
    [SerializeField] private TSScConnection tssConn;
    [SerializeField] private LMCCAgent lMCCAgent;
    private int scanned = 0;

    void Update()
    {
        if (!tssConn.isSPECUpdated()) return;
        SPEC spec = tssConn.GetSPEC();

        scanned++;
        sampleScannedText.text = $"Scanned Sample: {scanned}";
        // Debug.Log(spec.eva1.id);
        // Debug.Log(lMCCAgent);
        // Debug.Log(tssConn);
        // lMCCAgent.PostUpdateSample(spec.eva1.id, 0f, 0f);
    }
}
