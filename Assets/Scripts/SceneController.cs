using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    Animator an;
    public void SceneChange(string name)
    {
       // an.SetTrigger("start");
        SceneManager.LoadScene(name);
        Time.timeScale = 1;
    }
}
