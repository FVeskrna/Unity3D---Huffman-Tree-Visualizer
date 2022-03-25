using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class AddNext : MonoBehaviour
{
    public GameObject Char;
    private int count = 1;
    private int distance = 50;

    public GameObject NewChar;

    public List<GameObject> GOinputs = new List<GameObject>();
    public List<TMP_InputField> Chars = new List<TMP_InputField>();
    public List<TMP_InputField> Freq = new List<TMP_InputField>();


    private void Start()
    {
        GOinputs.Add(Char);
        Chars.Add(Char.transform.Find("InputField_Char").GetComponent<TMP_InputField>());
        Freq.Add(Char.transform.Find("InputField_Freq").GetComponent<TMP_InputField>());
    }
    public void add()
    {
        if(count < 32)
        {
            Vector3 charposition = new Vector3(Char.transform.position.x,Char.transform.position.y - distance,Char.transform.position.z);
            NewChar = Instantiate(Char,charposition,Quaternion.identity);
            NewChar.transform.SetParent(Char.gameObject.transform.parent);
            NewChar.transform.localScale = new Vector3(1,1,1);
            
            GOinputs.Add(NewChar);
            Chars.Add(NewChar.transform.Find("InputField_Char").GetComponent<TMP_InputField>());
            Freq.Add(NewChar.transform.Find("InputField_Freq").GetComponent<TMP_InputField>());

            distance += 50;
            count++;
        }
    }

    public void remove()
    {
        if(GOinputs.Count > 1)
        {
            var ObjectToRemove = GOinputs.Last();
            GOinputs.RemoveAt(GOinputs.Count -1);
            Chars.RemoveAt(Chars.Count-1);
            Freq.RemoveAt(Freq.Count-1);
            Destroy(ObjectToRemove);
            count--;
        }   
    }
}
