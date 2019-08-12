using UnityEngine;
using Utils;

namespace Game
{
    public class CentipedeVisual : MonoBehaviour
    {
        public MultiFrameSprite MultiSprite;

        public GameObject CreateVisualSegment(bool isHead)
        {
            var visSegment = new GameObject("Segment");
            visSegment.transform.SetParent(transform);
            var spriteRenderer = visSegment.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = isHead ? MultiSprite.GetSprite(0) : MultiSprite.GetSprite(1);
            visSegment.transform.localScale *= isHead ? 1.6f : 1.7f;
            return visSegment;
        }
    }
}