using Pearl;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RoadManager : PearlBehaviour, ISingleton
    {

        public List<GameObject> roads;

        public static bool GetIstance(out RoadManager result)
        {
            return Singleton<RoadManager>.GetIstance(out result);
        }


        public GameObject GetNextRoad()
        {
            var element = RandomExtend.GetRandomElement(roads);
            return element;
        }

    }
}
