using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Utils
{
    public class MultiFrameSprite : MonoBehaviour
    {
        private enum State
        {
            Stoped,
            Playing
        }

        [Serializable]
        public class FrameDescriptor
        {
            public Sprite Frame;
            public float Delay;
        }

        public SpriteRenderer SpriteRenderer;
        public FrameDescriptor[] Frames;
        public bool IsLooped;
        public bool IsPlayOnAwake;
        private float _currentDelay;
        private int _currentFrame;
        private State _currentState;

        void Awake()
        {
            if (IsPlayOnAwake)
                Play();
        }

        private void Play()
        {
            Assert.IsNotNull(SpriteRenderer);
            SetFrame(0);
            _currentState = State.Playing;
        }

        public void SetFrame(int frameIndex)
        {
            _currentFrame = frameIndex;
            SpriteRenderer.sprite = Frames[frameIndex].Frame;
            _currentDelay = Frames[frameIndex].Delay;
        }

        public Sprite GetSprite(int frameIndex)
        {
            return Frames[frameIndex].Frame;
        }

        public void SetFrameClear()
        {
            _currentFrame = -1;
            SpriteRenderer.sprite = null;
        }

        void Update()
        {
            if (_currentState == State.Stoped)
                return;
            _currentDelay -= Time.deltaTime;
            if (_currentDelay < 0f)
            {
                if (!IsLooped && _currentFrame == Frames.Length - 1)
                {
                    _currentState = State.Stoped;
                    return;
                }
                SetFrame((_currentFrame + 1) % Frames.Length);
            }
        }
    }
}