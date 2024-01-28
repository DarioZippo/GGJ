using Pearl;

namespace Game
{
    public enum ModelPG { Cesso, Piano, Flipper, Scala, Tavolo }

    public class SpecificGameManager : GameManager
    {
        public ModelPG modelCurrent;

        public void ChangeModel(ModelPG model)
        {
            modelCurrent = model;
        }

    }
}
