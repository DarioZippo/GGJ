using Pearl;
using UnityEngine;

namespace Game
{
    public class GameoverExplosion : MonoBehaviour
    {

        public void Explosion()
        {
            LevelManager.GameOver();
        }

    }
}
