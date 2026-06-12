using UnityEngine;

namespace UniKaruta.Scripts.Data
{
    [CreateAssetMenu(fileName = "CardData", menuName = "UniKaruta/CardData")]
    public class CardData : ScriptableObject
    {
        [SerializeField]
        private string _displayName;
        [SerializeField]
        private string _description;
        [SerializeField]
        private Sprite _image;

        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Image => _image;
    }
}
