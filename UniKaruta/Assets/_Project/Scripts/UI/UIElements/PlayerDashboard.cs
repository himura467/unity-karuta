using System;
using R3;
using UnityEngine.UIElements;

namespace UniKaruta.Scripts.UI.UIElements
{
    public class PlayerDashboard : VisualElement
    {
        private const string RootClassName = "player-dashboard";
        private const string NameLabelClassName = RootClassName + "__name";
        private const string ScoreLabelClassName = RootClassName + "__score";
        private const string PenaltyModifierClassName = RootClassName + "--penalty";

        private readonly Label _nameLabel;
        private readonly Label _scoreLabel;

        public PlayerDashboard()
        {
            AddToClassList(RootClassName);

            _nameLabel = new Label();
            _nameLabel.AddToClassList(NameLabelClassName);
            Add(_nameLabel);

            _scoreLabel = new Label();
            _scoreLabel.AddToClassList(ScoreLabelClassName);
            Add(_scoreLabel);
        }

        public PlayerDashboard(string playerName) : this()
        {
            SetPlayerName(playerName);
            SetScore(0);
            SetPenalty(false);
        }

        public void SetPlayerName(string name) => _nameLabel.text = name;

        public void SetScore(int score) => _scoreLabel.text = score.ToString();

        public void SetPenalty(bool isInPenalty) => EnableInClassList(PenaltyModifierClassName, isInPenalty);

        public IDisposable BindScore(Observable<int> score) => score.Subscribe(SetScore);

        public IDisposable BindPenalty(Observable<bool> penalty) => penalty.Subscribe(SetPenalty);

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
