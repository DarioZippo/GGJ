#if PEARL_2DSIDE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pearl.Side2D
{
    public interface IMovable
    {
        public Vector3 CurrentSpeed { get; }
    }
}

#endif