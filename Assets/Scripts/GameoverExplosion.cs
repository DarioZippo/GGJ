using UnityEngine;

namespace Game
{
    public class GameoverExplosion : MonoBehaviour
    {

        public void Explosion()
        {
            SpecificLevelManager.GetSpecificIstance().ExplosionScene();
        }

    }
}
