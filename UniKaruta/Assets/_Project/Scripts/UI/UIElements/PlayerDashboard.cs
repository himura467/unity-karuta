using UnityEngine.UIElements;

namespace UniKaruta.Scripts.UI.UIElements
{
    public class PlayerDashboard : VisualElement
    {
        private const string RootClassName = "player-dashboard";
        private static readonly string NameLabelClassName = $"{RootClassName}__name";
        private static readonly string ScoreLabelClassName = $"{RootClassName}__score";
        private static readonly string PenaltyLabelClassName = $"{RootClassName}__penalty";

        private readonly Label _nameLabel;
        private readonly Label _scoreLabel;
        private readonly Label _penaltyLabel;

        public PlayerDashboard()
        {
            AddToClassList(RootClassName);

            _nameLabel = new Label();
            _nameLabel.AddToClassList(NameLabelClassName);
            Add(_nameLabel);

            _scoreLabel = new Label();
            _scoreLabel.AddToClassList(ScoreLabelClassName);
            Add(_scoreLabel);

            _penaltyLabel = new Label();
            _penaltyLabel.AddToClassList(PenaltyLabelClassName);
            Add(_penaltyLabel);
        }

        public PlayerDashboard(string playerName) : this()
        {
            SetPlayerName(playerName);
            SetScore(0);
        }

        public void SetPlayerName(string name) => _nameLabel.text = name;

        public void SetScore(int score) => _scoreLabel.text = score.ToString();

        public void SetPenalty(bool isInPenalty) => _penaltyLabel.text = isInPenalty ? "Penalty" : string.Empty;

        public new class UxmlFactory : UxmlFactory<PlayerDashboard, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlStringAttributeDescription _playerName = new() { name = "player-name", defaultValue = string.Empty };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var playerName = _playerName.GetValueFromBag(bag, cc);
                var dashboard = (PlayerDashboard)ve;
                if (!string.IsNullOrEmpty(playerName))
                    dashboard.SetPlayerName(playerName);
            }
        }
    }
}
