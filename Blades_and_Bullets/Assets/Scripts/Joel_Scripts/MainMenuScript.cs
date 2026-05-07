using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    void Start()
    {
       
    }
    
    public void LoadGame()
    {
        
        StartCoroutine(_LoadGame());
        
        IEnumerator _LoadGame()
        {
            yield return new WaitForSeconds(.5f);
            SceneManager.LoadSceneAsync("BladesAndBullets_Ethan_withWaves");
           
        }
    }

}
