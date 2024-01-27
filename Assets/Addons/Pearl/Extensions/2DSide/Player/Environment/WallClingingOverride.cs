#if PEARL_2DSIDE

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pearl;

namespace Pearl.Side2D
{
    /// <summary>
    /// Use this class on any platform or surface a Character could usually wallcling to.
    /// You'll be able to override the slow factor (close to 0 : very slow fall, 1 : normal fall, larger than 1 : faster fall than normal).
    /// </summary>
    public class WallClingingOverride : MonoBehaviour
    {
        [Information("Use this component on any platform or surface a Character could usually wallcling to. Here you can override the slow factor (close to 0 : very slow fall, " +
            "1 : normal fall, larger than 1 : faster fall than normal), and decide if wallclinging is possible or not.", InformationTypeEnum.Info)]

        /// if this is set to false, a Character won't be able to wallcling to this object
        [Tooltip("if this is set to false, a Character won't be able to wallcling to this object")]
        public bool CanWallClingToThis = true;

        [ConditionalField("@CanWallClingToThis")]
        public bool slowFactorEqualPG = false;

        /// the slow factor to consider when wallclinging to this object
        [ConditionalField("!@slowFactorEqualPG && @CanWallClingToThis"), Range(0, 2), Tooltip("the slow factor to consider when wallclinging to this object")]
        public float WallClingingSlowFactor = 1f;
        [ConditionalField("@CanWallClingToThis")]
        public bool IsColumn = false;
    }
}

#endif