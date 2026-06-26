using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public GameObject howtoplay;
    public void Jugar()
    {
        SceneManager.LoadScene("Wip_LvL");
    }
    public void Controls()
    {
        howtoplay.SetActive(true);
    }
    public void CloseControls()
    {
        howtoplay.SetActive(false);
    }
    public void Creditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    public void Salir()
    {
        Application.Quit();
    }
}
