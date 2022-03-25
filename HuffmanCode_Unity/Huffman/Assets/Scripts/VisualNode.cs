using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VisualNode : MonoBehaviour
{
    public TMP_Text Symbol;
    public TMP_Text Frequency;
    public TMP_Text Index;


    public int freq;
    public string symbol;
    public float Leftdepth;
    public float Rightdepth;
    public float shiftcount;
    public int depth;
    public GameObject parent;
    public Node node;
    public float shiftamount;

    private bool render;

    private void Update() {
        Symbol.text = symbol;
        Frequency.text = freq.ToString();

        Vector3 Parentpos = gameObject.transform.parent.transform.position;
        Vector3 Pos = gameObject.transform.position;

        render = true;

        if(Parentpos.x > 200 || Pos.x > 200 ||Parentpos.x < -495 || Pos.x < -495)
        render = false;
        /*
        if(Parentpos.y > 280 || Parentpos.y < -280 || Pos.y > 280 || Pos.y < -280)
        render = false;
        */
        

        if(gameObject.TryGetComponent(out LineRenderer Line) && render)
        {
            Line.enabled = true;
            Line.SetPosition(0,new Vector3((Pos.x),(Pos.y+40),(Pos.z)));
            Line.SetPosition(1,new Vector3((Parentpos.x),(Parentpos.y-20),(Parentpos.z)));
        }
        else if(gameObject.TryGetComponent(out LineRenderer LineL) && !render)
        Line.enabled = false;
    }

    
}
