using UnityEngine;

namespace Game
{
    public class PuzzleHolder : MonoBehaviour
    {
        public void CheckForSnap(Transform pieceTransform)
        {
            float distance = Vector3.Distance(pieceTransform.position, transform.position);

            if (distance <= 1.25f)
            {
                pieceTransform.position = transform.position;
                pieceTransform.GetComponent<PuzzlePiece>().IsPlaced = true;
                PuzzleManager.PiecePlaced();
            }
        }
    }
}