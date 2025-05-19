using UnityEngine;
using UnityEngine.UI;

public class ChessBoardManager : MonoBehaviour
{
    public static ChessBoardManager instance;

    [Header("# ü������")]
    public GameObject tileButtonPrefab; // ���������� Button �����
    public Transform boardParent; // Grid Layout ���� ������Ʈ
    private TileButton[,] tiles = new TileButton[8, 8];

    [Header("# ü����")]
    public Sprite[] whitePieces;
    public Sprite[] blackPieces;
    public Transform pieceParent; // ü���� �θ� ������Ʈ4

    [Header("# �̵�����")]
    public ChessPiece selectedPiece;
    public ChessPiece[,] pieces = new ChessPiece[8, 8]; // ü���� ��ġ ����

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

        // �⹰��ȯ
        SpawnPiece(whitePieces[(int)PieceType.Pawn], 0, 1, PieceType.Pawn, true);
        SpawnPiece(blackPieces[(int)PieceType.Rook], 4, 1, PieceType.Rook, false);
        SpawnPiece(blackPieces[(int)PieceType.King], 3, 4, PieceType.King, false);
        SpawnPiece(blackPieces[(int)PieceType.Queen], 5, 4, PieceType.Queen, false);
    }

    public void OnTileClicked(int x, int y)
    {
        Debug.Log($"Tile clicked: {x}, {y}");
        // �⹰ ����
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
        {   // �⹰ �̵�
            TryMoveChessPieceTo(x, y);
        }
    }

    void TryMoveChessPieceTo(int x, int y)
    {
        pieces[selectedPiece.x, selectedPiece.y] = null; // ���� ��ġ ����
        selectedPiece.MoveTo(x, y); // �⹰ �̵�
        pieces[x, y] = selectedPiece; // ���ο� ��ġ�� �⹰ ��ġ

        selectedPiece = null; // ���� ����
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
        pieces[x, y] = chessPiece; // �⹰ ��ġ ����
    }
}
