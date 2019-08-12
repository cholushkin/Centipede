using UnityEngine;
using Utils;

namespace Game
{
    public class MushroomVisual : MonoBehaviour
    {
        public MultiFrameSprite MultiSprite;

        public void ShowDamage(float f)
        {
            int frame = (int) ((MultiSprite.Frames.Length - 1) * f);

            if (frame == MultiSprite.Frames.Length)
            {
                MultiSprite.SetFrameClear();
                return;
            }
            MultiSprite.SetFrame(frame);
        }
    }
}