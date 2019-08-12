﻿using UnityEngine;
using UnityEngine.Assertions;
using Utils;

namespace Game
{
    public class StateGameplay : AppStateManager.AppState<StateGameplay>, IHandle<PlayerController.EventPlayerDie>
    {
        public class SessionData
        {
            public long Scores;
            public byte PlayerLives = GameConstants.InitialLivesAmount;
        }

        public SimpleGUI GameGUI;
        public Level PrefabLevel;
        public LevelManager LevelManager;

        public Level Level { get; private set; }
        public SessionData PlayerSessionData { get; set; }

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
                Debug.Log("game over");
            }
            else // respawn
            {
                Level.RespawnOnlyEnemies();
                Level.SpawnPlayer();
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