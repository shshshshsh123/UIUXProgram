using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

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

    public int x, y; // 기물의 좌표
    public PieceType pieceType; // 기물의 타입 (폰, 룩, 나이트, 비숍, 퀸, 킹)
    public bool isWhite; // 백색 기물이라면 true

    protected virtual void Start()
    {
        gameObject.GetComponent<Image>().raycastTarget = false;
    }

    // 게임매니저에 의해 기물이 생성될 때 호출되는 초기화 함수
    public void Init(int x, int y, PieceType pieceType, bool isWhite)
    {
        // 기물의 좌표 설정
        this.x = x;
        this.y = y;

        this.pieceType = pieceType; // 이 기물의 타입 설정
        this.isWhite = isWhite; // 백색 기물이라면 true

        transform.localPosition = new Vector3(x * 100, y * 100, 0); // UI에 맞게 조정
    }

    // 이동 함수
    public virtual void MoveTo(int newX, int newY)
    {
        x = newX;
        y = newY;
        transform.localPosition = new Vector3(x * 100, y * 100, 0); // 위치 업데이트
    }
}
