using UnityEngine;
using UnityEngine.Assertions;
using Utils;

namespace Game
{
    public class StateGameplay : AppStateManager.AppState<StateGameplay>,
        IHandle<PlayerController.EventPlayerDie>,
        IHandle<Centipede.EventCentipedeKilled>
    {
        public class EventLevelComplete
        {
            public int LevelIndex;
        }
        public class SessionData
        {
            public int Scores;
            public int Level;
            public byte PlayerLives = GameConstants.InitialLivesAmount;
        }

        public SimpleGUI GameGUI;
        public Level PrefabLevel;
        public LevelManager LevelManager;

        public Level Level { get; private set; }
        public SessionData PlayerSessionData { get; set; }

        public bool IsWin { get; set; }

        protected override void Awake()
        {
            base.Awake();
            GlobalEventAggregator.EventAggregator.Subscribe(this);
        }

        #region AppStates
        public override void AppStateEnter(bool animated)
        {
            Assert.IsNotNull(GameGUI);
            Assert.IsNotNull(PrefabLevel);
            Assert.IsNotNull(LevelManager);

            gameObject.SetActive(true);
            GameGUI.PushScreen("Screen.GameHUD");

            // instantiate gameplay core objects
            Level = Instantiate(PrefabLevel, this.transform);
            Level.name = PrefabLevel.name;
            Level.Balance = LevelManager.GetNext();
            IsWin = false;
        }

        public override void AppStateLeave(bool animated)
        {
            gameObject.SetActive(false);

            // remove gameplay objects
            Destroy(Level.gameObject);
        }
        #endregion

        #region Event handling
        public void Handle(PlayerController.EventPlayerDie message)
        {
            --PlayerSessionData.PlayerLives;
            if (PlayerSessionData.PlayerLives <= 0) // game over
            {
                // show modal 'game over' window
                GameGUI.PushScreen("Screen.PopupGameOver");
            }
            else // respawn
            {
                Level.RespawnOnlyEnemies();
                Level.SpawnPlayer();
            }
        }

        public void Handle(Centipede.EventCentipedeKilled message)
        {
            var centipedeAmount = Level.Board.TransformEnemies.GetComponentsInChildren<Centipede>().Length;
            if (centipedeAmount == 0)
            {
                IsWin = true;
                GlobalEventAggregator.EventAggregator.Publish(new EventLevelComplete{LevelIndex = LevelManager.GetCurrentLevelIndex()});
                Level.Board.PurgeEnemies();
                GameGUI.PushScreen("Screen.PopupLevelWin");
            }
        }
        #endregion

        public void ResetSessionProgression()
        {
            PlayerSessionData = new SessionData();
            LevelManager.ResetProgression();
        }
    }
}