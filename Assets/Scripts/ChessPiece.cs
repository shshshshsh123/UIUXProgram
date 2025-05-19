using UnityEngine;

public enum PieceType
{
    Pawn,
    Rook,
    Knight,
    Bishop,
    Queen,
    King
}

public class ChessPiece : MonoBehaviour
{

    public int x, y;
    public PieceType pieceType;
    public bool isWhite;

    public void Init(int x, int y, PieceType pieceType, bool isWhite)
    {
        this.x = x;
        this.y = y;
        this.pieceType = pieceType;
        this.isWhite = isWhite;
        transform.localPosition = new Vector3(x * 100, y * 100, 0); // UI에 맞게 조정
    }

    public void MoveTo(int newX, int newY)
    {
        x = newX;
        y = newY;
        transform.localPosition = new Vector3(x * 100, y * 100, 0); // 위치 업데이트
    }
}
