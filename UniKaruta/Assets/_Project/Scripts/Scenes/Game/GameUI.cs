using System.Collections.Generic;
using R3;
using UniKaruta.Framework.Scripts.Scene;
using UniKaruta.Scripts.UI.UIElements;
using UnityEngine.UIElements;

namespace UniKaruta.Scripts.Scenes.Game
{
    public class GameUI : AbstractSceneUI
    {
        private const string ReadingCueLabelName = "reading-cue";
        private const string PlayersContainerName = "players-container";
        private const string CardFieldName = "card-field";

        private readonly Label _readingCueLabel;
        private readonly VisualElement _playersContainer;
        private readonly VisualElement _cardField;

        private readonly Subject<int> _cardPointerDown = new();
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<int, PlayerDashboard> _playerDashboards = new();

        public GameUI(VisualElement root)
        {
            _readingCueLabel = root.Q<Label>(ReadingCueLabelName);
            _playersContainer = root.Q<VisualElement>(PlayersContainerName);
            _cardField = root.Q<VisualElement>(CardFieldName);
        }

        public Observable<int> OnCardPointerDown => _cardPointerDown;

        public void OnReadingCueFired(string phraseText) => _readingCueLabel.text = phraseText;

        public void InitPlayerDashboards(IReadOnlyList<int> playerIds)
        {
            foreach (var playerId in playerIds)
            {
                var dashboard = new PlayerDashboard($"Player {playerId + 1}");
                _playersContainer.Add(dashboard);
                _playerDashboards[playerId] = dashboard;
            }
        }

        public void UpdatePlayerScore(int playerId, int score)
        {
            if (_playerDashboards.TryGetValue(playerId, out var dashboard))
                dashboard.SetScore(score);
        }

        public void UpdatePlayerPenalty(int playerId, bool isInPenalty)
        {
            if (_playerDashboards.TryGetValue(playerId, out var dashboard))
                dashboard.SetPenalty(isInPenalty);
        }

        public void AddCard(int cardId)
        {
            var card = new Card(cardId);
            _cardField.Add(card);
            _disposables.Add(card.OnPointerDown.Subscribe(_ => _cardPointerDown.OnNext(cardId)));
        }

        public override void Dispose()
        {
            _disposables.Dispose();
            _cardPointerDown.Dispose();
            base.Dispose();
        }
    }
}
