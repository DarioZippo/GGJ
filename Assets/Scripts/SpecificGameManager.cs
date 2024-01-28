using Pearl;

namespace Game
{
    public enum ModelPG { Cesso, Piano, Flipper, Scala, Tavolo }

    public class SpecificGameManager : GameManager
    {
        public static ModelPG modelCurrent;

        public static void ChangeModel(ModelPG model)
        {
            modelCurrent = model;
        }

    }
}
