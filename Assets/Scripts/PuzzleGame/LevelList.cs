using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    [CreateAssetMenu(fileName = "LevelList", menuName = "Scriptable Objects/LevelList")]
    public class LevelList : ScriptableObject
    {
        public List<LevelData> Levels;
    }

}
