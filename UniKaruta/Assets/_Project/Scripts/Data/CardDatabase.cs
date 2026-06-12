using System.Collections.Generic;
using UnityEngine;

namespace UniKaruta.Scripts.Data
{
    [CreateAssetMenu(fileName = "CardDatabase", menuName = "UniKaruta/CardDatabase")]
    public class CardDatabase : ScriptableObject
    {
        [SerializeField]
        private CardData[] _cards;

        public IReadOnlyList<CardData> Cards => _cards;
    }
}
