using UnityEngine;
using UnityEngine.UI;

public class ChessBoardManager : MonoBehaviour
{
    [Header("# ü������")]
    public GameObject tileButtonPrefab; // ���������� Button �����
    public Transform boardParent; // Grid Layout ���� ������Ʈ
    private TileButton[,] tiles = new TileButton[8, 8];

    [Header("# ü����")]
    public Sprite[] whitePieces;
    public Sprite[] blackPieces;
    public Transform pieceParent; // ü���� �θ� ������Ʈ
       
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

        // �⹰��ȯ
        SpawnPiece(whitePieces[(int)PieceType.Pawn], 0, 1, PieceType.Pawn, true);
        SpawnPiece(blackPieces[(int)PieceType.Rook], 4, 1, PieceType.Rook, false);
        SpawnPiece(blackPieces[(int)PieceType.King], 3, 4, PieceType.King, false);
        SpawnPiece(blackPieces[(int)PieceType.Queen], 5, 4, PieceType.Queen, false);
    }

    void OnTileClicked(int x, int y)
    {
        Debug.Log($"Tile clicked: {x}, {y}");
        // ���� �� �̵��̳� ���� ó�� �߰� ����
    }

    void SpawnPiece(Sprite pieceImage, int x, int y, PieceType pieceType, bool isWhite)
    {
        GameObject pieceObj = new GameObject($"Piece_{x}_{y}");
        pieceObj.transform.SetParent(pieceParent);
        pieceObj.transform.localPosition = new Vector3(x * 100, y * 100, 0); // UI�� �°� ����
        Image piece = pieceObj.AddComponent<Image>();
        piece.sprite = pieceImage;
        piece.color = isWhite ? Color.white : Color.black; // ���� ����
        piece.rectTransform.sizeDelta = new Vector2(80, 80); // ũ�� ����
        ChessPiece chessPiece = pieceObj.AddComponent<ChessPiece>();
        chessPiece.Init(x, y, pieceType, isWhite);
    }
}
