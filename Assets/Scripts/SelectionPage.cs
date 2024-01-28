using Pearl;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class SelectionPage : MonoBehaviour
    {
        public Camera cam;


        // Start is called before the first frame update
        void Start()
        {

        }

        public void ChangeCesso()
        {
            SpecificGameManager.ChangeModel(ModelPG.Cesso);
            SceneSystemManager.EnterNewScene("Game");
        }

        public void ChangePiano()
        {
            SpecificGameManager.ChangeModel(ModelPG.Piano);
            SceneSystemManager.EnterNewScene("Game");
        }

        public void ChangeScala()
        {
            SpecificGameManager.ChangeModel(ModelPG.Scala);
            SceneSystemManager.EnterNewScene("Game");
        }

        public void ChangeFlipper()
        {
            SpecificGameManager.ChangeModel(ModelPG.Flipper);
            SceneSystemManager.EnterNewScene("Game");
        }

        public void ChangeTavolo()
        {
            SpecificGameManager.ChangeModel(ModelPG.Tavolo);
            SceneSystemManager.EnterNewScene("Game");
        }

        // Update is called once per frame
        void Update()
        {
            var images = transform.GetComponentsInHierarchy<Image>();
            var texts = transform.GetComponentsInHierarchy<TMP_Text>();
            foreach (var image in images)
            {
                image.color = ColorExtend.Complementary(cam.backgroundColor);
            }

            foreach (var text in texts)
            {
                text.color = cam.backgroundColor;
            }
        }
    }
}
