using UnityEngine;

public class Order : MonoBehaviour
{
    public GameObject Product01;
    public GameObject Product02;
    public GameObject Product03;

    public GameObject Completed_txt;
    public GameObject FinalScore;


    bool Prod01 = true;
    bool Prod02 = true;
    bool Prod03 = true;

    void Start()
    {
        Prod01 = true;
        Prod02 = true;
        Prod03 = true;
    }

    void Update()
    {
        OrderCompleted();
    }

    public void TurnOffProduct01()
    {
        Product01.SetActive(false);
        Prod01 = false;
    }

    public void TurnOffProduct02()
    {
        Product02.SetActive(false);
        Prod02 = false;
    }

    public void TurnOffProduct03()
    {
        Product03.SetActive(false);
        Prod03 = false;
    }

    void OrderCompleted()
    {
        if(!Prod01 && !Prod02 && !Prod03)
        {
            Completed_txt.SetActive(true);
            FinalScore.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }

}
