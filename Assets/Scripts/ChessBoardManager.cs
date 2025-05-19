using UnityEngine;
using UnityEngine.UI;

public class ChessBoardManager : MonoBehaviour
{
    public static ChessBoardManager instance;

    [Header("# 체스보드")]
    public GameObject tileButtonPrefab; // 프리팹으로 Button 만들기
    public Transform boardParent; // Grid Layout 붙은 오브젝트
    private TileButton[,] tiles = new TileButton[8, 8];

    [Header("# 체스말")]
    public Sprite[] whitePieces;
    public Sprite[] blackPieces;
    public Transform pieceParent; // 체스말 부모 오브젝트4

    [Header("# 이동로직")]
    public ChessPiece selectedPiece;
    public ChessPiece[,] pieces = new ChessPiece[8, 8]; // 체스말 위치 저장

    void Start()
    {
        for (int y = 7; y >= 0; y--)
        {
            for (int x = 0; x < 8; x++)
            {
                GameObject tileObj = Instantiate(tileButtonPrefab, boardParent);
                TileButton tile = tileObj.GetComponent<TileButton>();
                tile.Init(x, y, OnTileClicked);
                tiles[x, y] = tile;
            }
        }

        // 기물소환
        SpawnPiece(whitePieces[(int)PieceType.Pawn], 0, 1, PieceType.Pawn, true);
        SpawnPiece(blackPieces[(int)PieceType.Rook], 4, 1, PieceType.Rook, false);
        SpawnPiece(blackPieces[(int)PieceType.King], 3, 4, PieceType.King, false);
        SpawnPiece(blackPieces[(int)PieceType.Queen], 5, 4, PieceType.Queen, false);
    }

    public void OnTileClicked(int x, int y)
    {
        Debug.Log($"Tile clicked: {x}, {y}");
        // 기물 선택
        if (selectedPiece == null)
        {
            ChessPiece piece = pieces[x, y];
            if (piece != null)
            {
                selectedPiece = piece;
                Debug.Log($"Selected piece: {piece.pieceType} at {x}, {y}");
            }
        }
        else
        {   // 기물 이동
            TryMoveChessPieceTo(x, y);
        }
    }

    void TryMoveChessPieceTo(int x, int y)
    {
        // 먼저 이동 가능한 위치인지 계산 후
        if (CalculateAbleMovePos(selectedPiece, x, y))
        {
            // 이동 가능하다면 이동시킨다.
            pieces[selectedPiece.x, selectedPiece.y] = null; // 기존 위치 비우기
            selectedPiece.MoveTo(x, y); // 기물 이동
            pieces[x, y] = selectedPiece; // 새로운 위치에 기물 배치

            selectedPiece = null; // 선택 해제
        }
    }

    // 이동 목표 지점이 실제로 이동 가능한 지점인지 계산
    bool CalculateAbleMovePos(ChessPiece piece, int targetX, int targetY)
    {
        // 제자리 이동 방지
        if (piece.x == targetX && piece.y == targetY) return false;

        // 폰
        if (piece.pieceType == PieceType.Pawn)
        {
            // 백색 기물인 경우 위로 한 칸 이동 가능
            if (piece.isWhite)
            {
                if (targetX == piece.x && targetY == piece.y + 1) return true;
            }
            else // 흑색 기물인 경우 아래로 한 칸 이동 가능
            {
                if (targetX == piece.x && targetY == piece.y - 1) return true;
            }
        }

        // 룩
        if (piece.pieceType == PieceType.Rook)
        {
            // 한 축이 고정되어있다면 어디로든 이동 가능
            if (piece.x == targetX || piece.y == targetY) return true;
        }

        // 나이트
        if (piece.pieceType == PieceType.Knight)
        {
            // 현재 위치와 목표 위치 사이의 차이를 구한다.
            int dx = Mathf.Abs(targetX - piece.x);
            int dy = Mathf.Abs(targetY - piece.y);

            // 수직 2칸 + 수평 1칸 또는 수평 2칸 + 수직 1칸인 경우 이동 가능
            if ((dx == 2 && dy == 1) || (dx == 1 && dy == 2)) return true;
        }
        
        // 비숍
        if (piece.pieceType == PieceType.Bishop)
        {
            // 현재 위치와 목표 위치 사이의 차이를 구한다.
            int dx = Mathf.Abs(targetX - piece.x);
            int dy = Mathf.Abs(targetY - piece.y);

            // 두 좌표 차이의 절댓값이 같다면 대각선 이동으로 판단하여 이동 가능
            if (dx == dy) return true;
        }

        // 퀸
        if (piece.pieceType == PieceType.Queen)
        {
            // 직선 이동
            // 한 축이 고정되어있다면 어디로든 이동 가능
            if (piece.x == targetX || piece.y == targetY) return true;


            // 대각선 이동
            // 현재 위치와 목표 위치 사이의 차이를 구한다.
            int dx = Mathf.Abs(targetX - piece.x);
            int dy = Mathf.Abs(targetY - piece.y);

            // 두 좌표 차이의 절댓값이 같다면 대각선 이동으로 판단하여 이동 가능
            if (dx == dy) return true;
        }

        // 킹
        if (piece.pieceType == PieceType.King)
        {
            // 현재 위치와 목표 위치 사이의 차이를 구한다.
            int dx = Mathf.Abs(targetX - piece.x);
            int dy = Mathf.Abs(targetY - piece.y);

            // 그 차이가 1칸일때만 이동 가능
            if (dx <= 1 && dy <= 1) return true;
        }

        // 어떤 조건에도 걸리지 않았다면 false 리턴하여 이동 실패
        return false;
    }

    void SpawnPiece(Sprite pieceImage, int x, int y, PieceType pieceType, bool isWhite)
    {
        // 새로운 기물 생성
        GameObject pieceObj = new GameObject($"Piece_{x}_{y}");
        
        pieceObj.transform.SetParent(pieceParent);
        pieceObj.transform.localPosition = new Vector3(x * 100, y * 100, 0); // UI에 맞게 조정

        // 기물의 이미지 설정
        Image piece = pieceObj.AddComponent<Image>(); // 기물에 Imgae 컴포넌트 추가
        piece.sprite = pieceImage; // 스프라이트 설정
        piece.color = isWhite ? Color.white : Color.black; // 색상 설정
        piece.rectTransform.sizeDelta = new Vector2(80, 80); // 크기 조정

        ChessPiece chessPiece = pieceObj.AddComponent<ChessPiece>();
        chessPiece.Init(x, y, pieceType, isWhite); // 체스 기물 초기화
        pieces[x, y] = chessPiece; // 기물 위치 저장
    }
}
