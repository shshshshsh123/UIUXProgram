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
        pieces[selectedPiece.x, selectedPiece.y] = null; // 기존 위치 비우기
        selectedPiece.MoveTo(x, y); // 기물 이동
        pieces[x, y] = selectedPiece; // 새로운 위치에 기물 배치

        selectedPiece = null; // 선택 해제
    }

    void SpawnPiece(Sprite pieceImage, int x, int y, PieceType pieceType, bool isWhite)
    {
        GameObject pieceObj = new GameObject($"Piece_{x}_{y}");
        pieceObj.transform.SetParent(pieceParent);
        pieceObj.transform.localPosition = new Vector3(x * 100, y * 100, 0); // UI에 맞게 조정
        Image piece = pieceObj.AddComponent<Image>();
        piece.sprite = pieceImage;
        piece.color = isWhite ? Color.white : Color.black; // 색상 설정
        piece.rectTransform.sizeDelta = new Vector2(80, 80); // 크기 조정
        ChessPiece chessPiece = pieceObj.AddComponent<ChessPiece>();
        chessPiece.Init(x, y, pieceType, isWhite);
        pieces[x, y] = chessPiece; // 기물 위치 저장
    }
}
