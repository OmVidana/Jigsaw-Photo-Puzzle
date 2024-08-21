using UnityEngine;

namespace Game
{
    public class PuzzlePiece : MonoBehaviour
    {
        private PuzzleHolder _pieceHolder;
        private Vector3 _startPosition;
        private Vector3 _offset;
        protected internal bool IsPlaced;

        private void Start()
        {
            _pieceHolder = GameObject.Find(gameObject.name).GetComponent<PuzzleHolder>();
        }
    
        private void OnMouseDown()
        {
            if (!IsPlaced){
                _startPosition = transform.position;
                _offset = _startPosition - Camera.main!.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        private void OnMouseDrag()
        {
            if (!IsPlaced)
            {
                Vector3 currentPosition = Camera.main!.ScreenToWorldPoint(Input.mousePosition) + _offset;
                transform.position = currentPosition;
            }
        }

        private void OnMouseUp()
        {
            if (!IsPlaced)
            {
                _pieceHolder.CheckForSnap(transform);
            }
        }
    }
}