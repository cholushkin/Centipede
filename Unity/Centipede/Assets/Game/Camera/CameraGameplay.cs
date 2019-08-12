using UnityEngine;
using UnityEngine.Assertions;
using Utils;

namespace Game
{
    public class CameraGameplay : Singleton<CameraGameplay>
    {
        public Follower Follower;

        public void FocusOnBoardCenter()
        {
            Assert.IsNotNull(StateGameplay.Instance.Level); // in real game there should be many cameras (gameplay, menu, etc.)
            var gridSize = StateGameplay.Instance.Level.Balance.GridSize;
            var focusPos = new Vector3(
                gridSize.x * GameConstants.CellWidth * 0.5f,
                gridSize.y * GameConstants.CellHeight * 0.5f,
                0f
            );

            GetComponent<Camera>().orthographicSize = gridSize.y / 16f;
            Follower.Follow(focusPos);
        }
    }
}