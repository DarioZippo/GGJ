using Pearl;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PearlInitialize
{
    //Init at start scene
    [RuntimeInitializeOnLoadMethod]
    private static void Start()
    {
        CreateGameManager();
    }

    private static void CreateGameManager()
    {
        if (!GameObject.FindFirstObjectByType<GameManager>())
        {
            bool create = true;

            var gameManagerCreation = AssetManager.LoadAsset<GameManagerScriptableObject>("GameManagerCreation");
            if (gameManagerCreation != null)
            {
                string[] scenes = gameManagerCreation.ScenesExclude;
                if (scenes != null && scenes.Contains(SceneManager.GetActiveScene().name))
                {
                    create = false;
                }
                else
                {
                    var gameManager = gameManagerCreation.GameManager;
                    if (gameManager != null)
                    {
                        GameObject.Instantiate(gameManager);
                        create = false;
                    }
                }
            }

            if (create)
            {
                GameObjectExtend.CreateGameObject("GameManager", components: typeof(GameManager));
            }
        }
    }
}
