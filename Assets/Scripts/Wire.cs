using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wire : MonoBehaviour
{
    int index;
    bool isRotate = false;
    private Animator anim;
    public static List<GameObject> Wires = new List<GameObject>();
    public List<GameObject> Electricity;

    void Start()
    {
        Wires.Add(gameObject);
        anim = GetComponent<Animator>();
        for (int i = 0; i < StartAndFinish.WiresMap.GetLength(0); i++)
        {
            for (int j = 0; j < StartAndFinish.WiresMap.GetLength(1); j++)
            {
                if (StartAndFinish.WiresMap[i, j] == gameObject)
                {
                    index = i * 10 + j;
                }
            }
        }
    }

    public void WireRotate()
    {
        if (!isRotate)
        {
            isRotate = true;
            StartCoroutine(AfterRotate());
            anim.SetTrigger("Rotate");
            foreach (var wire in Wires)
            {
                foreach (var electricity in wire.GetComponent<Wire>().Electricity)
                {
                    electricity.SetActive(false);
                }
            }
        }
    }

    private IEnumerator AfterRotate()
    {
        yield return new WaitForSeconds(0.35f);

        if (StartAndFinish.GameMap[index / 10, index % 10] % 10 != 4) StartAndFinish.GameMap[index / 10, index % 10]++;
        else StartAndFinish.GameMap[index / 10, index % 10] -= 3;       

        StartAndFinish.CheckWinAndElectricity();
        isRotate = false;
    }

    public void CheckElectricity(bool[] direct)
    {
        for (int i = 0; i < direct.Length; i++)
        {
            if(direct[i])
                switch(StartAndFinish.GameMap[index / 10, index % 10] / 10)
                {
                    case 1:
                        if (i == 1) Electricity[0].SetActive(true);
                        if (i == 3) Electricity[1].SetActive(true);
                        break;
                    case 2:
                        if (i == 0) Electricity[0].SetActive(true);
                        if (i == 3) Electricity[1].SetActive(true);
                        break;
                    case 3:
                        if (i == 1) Electricity[0].SetActive(true);
                        if (i == 2) Electricity[1].SetActive(true);
                        if (i == 3) Electricity[2].SetActive(true);
                        break;
                }
        }
    }
}
