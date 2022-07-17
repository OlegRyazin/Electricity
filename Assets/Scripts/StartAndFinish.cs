using UnityEngine;
using System;

public class StartAndFinish : MonoBehaviour
{
    public static int[,] GameMap = new int[7, 9]
    {
        {0, 22, 11, 11, 21, 0, 12, 33, 0},
        {22, 21, 32, 23, 0, 32, 21, 24, 13},
        {11, 14, 0, 12, 0, 0, 13, 21, 14},
        {31, 12, 31, 21, 13, 11, 22, 11, 21},
        {11, 13, 14, 0, 34, 0, 11, 11, 12},
        {12, 22, 0, 11, 21, 11, 21, 11, 21},
        {22, 23, 11, 11, 23, 0, 11, 33, 0}
    }; // 0 - Empty, 1X - Wire, 2X - L-Wire, 3X - T-Wire;   X1 - 0 deg zRot, X2 - 90 deg zRot, X3 - 180 deg zRot, X4 - 270 deg zRot;

    public static GameObject[,] WiresMap = new GameObject[7, 9];//Contains wires GameObjects
    public static bool[,] ElectricityMap = new bool[7, 9];//Contains wires GameObjects

    public GameObject wirePref;
    public GameObject lWirePref;
    public GameObject tWirePref;

    public GameObject RightElectricityForWin;
    public GameObject BlockClicks;

    [Header("Parent transform for wires")]
    public Transform ParentTrans;

    [Header("Distance between wires in pixels on canvas")]
    public float Step = 100;

    static Action End;

    void Start()
    {
        End = EndGame;
        Draw();       
    }

    private void EndGame()
    {
        BlockClicks.SetActive(true);
        RightElectricityForWin.SetActive(true);
        Debug.Log("Победа-победа вместо обеда");
        End -= EndGame;
    }

    private void Draw()
    {
        for (int i = 0; i < GameMap.GetLength(0); i++)
        {
            for (int j = 0; j < GameMap.GetLength(1); j++)
            {
                bool isZero = false;
                GameObject pref = null;
                float angle = 0;
         
                switch(GameMap[i, j] / 10)
                {
                    case 0: isZero = true; break;
                    case 1: pref = wirePref; break;
                    case 2: pref = lWirePref; break;
                    case 3: pref = tWirePref; break;
                }

                switch (GameMap[i, j] % 10)
                {
                    case 1: angle = 0; break;
                    case 2: angle = 90; break;
                    case 3: angle = 180; break;
                    case 4: angle = 270; break;
                }

                if(!isZero)
                {
                    GameObject wire = Instantiate(pref, ParentTrans, false);
                    wire.transform.SetParent(ParentTrans);
                    wire.transform.localPosition = new Vector3(-400 + Step * j , 300 - Step * i, 0);
                    wire.transform.Rotate(new Vector3(0, 0, angle));
                    WiresMap[i, j] = wire;
                }               
            }
        }
    } 

    public static void CheckWinAndElectricity()
    {
        for (int i = 0; i < ElectricityMap.GetLength(0); i++)
        {
            for (int j = 0; j < ElectricityMap.GetLength(1); j++)
            {
                ElectricityMap[i, j] = false;
            }
        }

        Recurs(new int[4] {-1, 50, -1, -1 });

        if ((GameMap[2, 8] == 11 || GameMap[2, 8] == 13) && ElectricityMap[2, 8])
        {
            End.Invoke();           
        }
    }

    private static void Recurs(int[] mas) //mas contains indexes for further path searching
    {
        bool[] electricityDirect = new bool[4] {true, true, true, true };
        bool connect = false;
        for (int i = 0; i < mas.Length; i++)
        {
            connect = false;
            if (mas[i] == -10) electricityDirect[i] = false;
            else if (mas[i] != -1)
            {
                if (!ElectricityMap[mas[i] / 10, mas[i] % 10])
                {
                    switch (GameMap[mas[i] / 10, mas[i] % 10])
                    {
                        case 0: break;
                        case 11:
                        case 13:
                            if (i == 1)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 3, new bool[4] { false, true, false, false }));
                            }
                            if (i == 3)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 1, new bool[4] { false, false, false, true }));
                            }
                            break;
                        case 12:
                        case 14:
                            if (i == 0)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 2, new bool[4] { true, false, false, false }));
                            }
                            if (i == 2)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 0, new bool[4] { false, false, true, false }));
                            }
                            break;
                        case 21:
                            if (i == 1)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 3, new bool[4] { true, false, false, false }));
                            }
                            if (i == 2)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 0, new bool[4] { false, false, false, true }));
                            }
                            break;
                        case 22:
                            if (i == 0)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 2, new bool[4] { false, false, false, true }));
                            }
                            if (i == 1)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 3, new bool[4] { false, false, true, false }));
                            }
                            break;
                        case 23:
                            if (i == 0)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 2, new bool[4] { false, true, false, false }));
                            }
                            if (i == 3)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 1, new bool[4] { false, false, true, false }));
                            }
                            break;
                        case 24:
                            if (i == 2)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 0, new bool[4] { false, true, false, false }));
                            }
                            if (i == 3)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 1, new bool[4] { true, false, false, false }));
                            }
                            break;
                        case 31:
                            if (i == 0)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 2, new bool[4] { false, true, false, true }));
                            }
                            if (i == 1)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 3, new bool[4] { false, true, true, false }));
                            }
                            if (i == 3)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 1, new bool[4] { false, false, true, true }));
                            }
                            break;
                        case 32:
                            if (i == 0)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 2, new bool[4] { true, true, false, false }));
                            }
                            if (i == 2)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 0, new bool[4] { false, true, true, false }));
                            }
                            if (i == 3)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 1, new bool[4] { true, false, true, false }));
                            }
                            break;
                        case 33:
                            if (i == 1)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 3, new bool[4] { true, true, false, false }));
                            }
                            if (i == 2)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 0, new bool[4] { false, true, false, true }));
                            }
                            if (i == 3)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 1, new bool[4] { true, false, false, true }));
                            }
                            break;
                        case 34:
                            if (i == 0)
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 2, new bool[4] { true, false, false, true }));
                            }
                            if (i == 1)                                
                            {
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 3, new bool[4] { true, false, true, false }));
                            }
                            if (i == 2)
                            {   
                                connect = true;
                                ElectricityMap[mas[i] / 10, mas[i] % 10] = true;
                                Recurs(Mas(mas[i], 0, new bool[4] { false, true, true, false }));
                            }
                            break;
                    }
                    if (connect) electricityDirect[i] = false;
                }
            }
        }
        if((electricityDirect[0] || electricityDirect[1] || electricityDirect[2] || electricityDirect[3]) && mas[1] != 50)
        {
            int index = 0;
            if (mas[0] != -1 && mas[0] != -10) index = mas[0] + 10;
            else if (mas[1] != -1 && mas[1] != -10) index = mas[1] - 1;
            else if (mas[2] != -1 && mas[2] != -10) index = mas[2] - 10;
            else if (mas[3] != -1 && mas[3] != -10) index = mas[3] + 1;    

            for (int i = 0; i < GameMap[index / 10, index % 10] % 10 - 1; i++)
            {
                bool tmp = electricityDirect[electricityDirect.Length - 1];
                for (var j = electricityDirect.Length - 1; j != 0; --j) electricityDirect[j] = electricityDirect[j - 1];
                electricityDirect[0] = tmp;
            }

            if(index != 0) WiresMap[index / 10, index % 10].GetComponent<Wire>().CheckElectricity(electricityDirect);
        }
    }

    private static int[] Mas(int index, int home, bool[] mas)
    {
        int[] result = new int[4];
        for (int i = 0; i < mas.Length; i++)
        {
            if (i == home) result[i] = -10;
            else if (mas[i])
            {
                switch (i)
                {
                    case 0:
                        if (index >= 10) result[i] = index - 10;
                        else result[i] = -1;
                        break;
                    case 1:
                        if (index % 10 < 8) result[i] = index + 1;
                        else result[i] = -1;
                        break;
                    case 2:
                        if (index < 60) result[i] = index + 10;
                        else result[i] = -1;
                        break;
                    case 3:
                        if (index % 10 > 0) result[i] = index - 1;
                        else result[i] = -1;
                        break;
                }
            }
            else result[i] = -1;
        }
        return result;
    }
}
