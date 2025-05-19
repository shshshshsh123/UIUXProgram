using UnityEngine;
using UnityEngine.UI;

public class ChessBoardManager : MonoBehaviour
{
    [Header("# 체스보드")]
    public GameObject tileButtonPrefab; // 프리팹으로 Button 만들기
    public Transform boardParent; // Grid Layout 붙은 오브젝트
    private TileButton[,] tiles = new TileButton[8, 8];

    [Header("# 체스말")]
    public Sprite[] whitePieces;
    public Sprite[] blackPieces;
    public Transform pieceParent; // 체스말 부모 오브젝트
       
    void Start()
    {
        for (int y = 0; y < 8; y++)
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

    void OnTileClicked(int x, int y)
    {
        Debug.Log($"Tile clicked: {x}, {y}");
        // 이후 말 이동이나 오답 처리 추가 예정
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
    }
}
