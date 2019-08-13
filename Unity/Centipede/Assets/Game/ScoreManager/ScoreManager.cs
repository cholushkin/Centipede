using UnityEngine;
using Utils;

namespace Game
{
    public class ScoreManager : MonoBehaviour,
        IHandle<Spider.EventSpiderDied>,
        IHandle<Centipede.EventDestroyCentipedeSegment>,
        IHandle<StateGameplay.EventLevelComplete>
    {
        private static readonly int ScoreCentipedeHead = 100;
        private static readonly int ScoreCentipedeSegment = 10;
        private static readonly int ScoreSpider = 300;
        private static readonly int ScoreLevelComplete = 1000;

        public StateGameplay Gameplay;

        public void Awake()
        {
            GlobalEventAggregator.EventAggregator.Subscribe(this);
        }

        public void Handle(Spider.EventSpiderDied message)
        {
            Gameplay.PlayerSessionData.Scores += ScoreSpider;
        }

        public void Handle(Centipede.EventDestroyCentipedeSegment message)
        {
            Gameplay.PlayerSessionData.Scores += message.IsHead ? ScoreCentipedeHead : ScoreCentipedeSegment;
        }

        public void Handle(StateGameplay.EventLevelComplete message)
        {
            var scores = message.LevelIndex * ScoreLevelComplete;
            Gameplay.PlayerSessionData.Scores += scores;
        }
    }
}