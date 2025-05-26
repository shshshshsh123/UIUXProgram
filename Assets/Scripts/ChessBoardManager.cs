using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Security.Cryptography;

public class ChessBoardManager : MonoBehaviour
{
    public static ChessBoardManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    [Header("# 체스보드")]
    public GameObject tileButtonPrefab; // 프리팹으로 Button 만들기
    public Transform boardParent; // Grid Layout 붙은 오브젝트
    public TileButton[,] tiles = new TileButton[8, 8];

    [Header("# 체스말")]
    public Sprite[] whitePieces;
    public Sprite[] blackPieces;
    public Transform pieceParent; // 체스말 부모 오브젝트4

    [Header("# 이동로직")]
    public ChessPiece selectedPiece;
    public ChessPiece[,] pieces = new ChessPiece[8, 8]; // 체스말 위치 저장

    public void SetChessBoard(int stage)
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
        // 기물 소환
        SetGame(stage);
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

                // 이동 가능한 타일 계산
                List<Vector2Int> moveableTiles = GetAvailableMoves(selectedPiece);
                foreach(var move in moveableTiles)
                {
                    UpdateAvailableMoves(); // 이동 가능한 타일 업데이트
                }
            }
        }
        else
        {   // 기물 이동
            TryMoveChessPieceTo(x, y);
            foreach (var tile in tiles)
            {
                tiles[tile.x, tile.y].ShowCanMove(false);
                tiles[tile.x, tile.y].ShowCanAttack(false); // 이동 가능한 타일 표시 해제
            }
            selectedPiece = null; // 선택 해제
        }
    }

    void TryMoveChessPieceTo(int x, int y)
    {
        // 먼저 이동 가능한 위치인지 계산 후 이동 가능하다면 이동시킨다.
        if (CalculateAbleMovePos(selectedPiece, x, y))
        {
            // 목표 위치에 있는 기물을 제거 (기물 잡기)
            if (pieces[x, y] != null)
            {
                Destroy(pieces[x, y].gameObject);
                pieces[x, y] = null;
            }

            // 목표 위치로 기물을 이동
            pieces[selectedPiece.x, selectedPiece.y] = null; // 기존 위치 비우기
            selectedPiece.MoveTo(x, y); // 기물 이동
            pieces[x, y] = selectedPiece; // 목표 위치에 기물 배치

            //ActionManager.whenPlayerMoved(); // 플레이어가 행동을 마쳤으니 액션 호출
        }

        // 선택 해제
        selectedPiece = null;
    }

    // 이동 목표 지점이 실제로 이동 가능한 지점인지 계산
    bool CalculateAbleMovePos(ChessPiece piece, int targetX, int targetY)
    {
        // 제자리 이동 방지
        if (piece.x == targetX && piece.y == targetY) return false;

        // 이동할 위치에 아군 기물이 있다면 이동 불가
        if (pieces[targetX, targetY] != null)
        {
            if (pieces[targetX, targetY].isWhite == piece.isWhite) return false;
        }

        // 폰
        if (piece.pieceType == PieceType.Pawn)
        {
            // 백색 기물인 경우 위로 한 칸 이동 가능
            if (piece.isWhite)
            {
                if (targetX == piece.x && targetY == piece.y + 1)
                {
                    // 직진으로 이동할 경우, 다른 기물을 잡지 못한다
                    if (pieces[targetX, targetY] == null) return true;
                }
            }
            else // 흑색 기물인 경우 아래로 한 칸 이동 가능
            {
                if (targetX == piece.x && targetY == piece.y - 1)
                {
                    // 직진으로 이동할 경우, 다른 기물을 잡지 못한다
                    if (pieces[targetX, targetY] == null) return true;
                }
            }

            // 흰색 대각선 공격
            if (piece.isWhite)
            {
                if ((targetX == piece.x - 1 || targetX == piece.x + 1) && targetY == piece.y + 1)
                {
                    if (pieces[targetX, targetY] != null && !pieces[targetX, targetY].isWhite)
                        return true;
                }
            }
            else
            {
                if ((targetX == piece.x - 1 || targetX == piece.x + 1) && targetY == piece.y - 1)
                {
                    if (pieces[targetX, targetY] != null && pieces[targetX, targetY].isWhite)
                        return true;
                }
            }
        }

        // 룩
        if (piece.pieceType == PieceType.Rook)
        {
            // 한 축이 고정되어있는 경우에만 이동 가능
            if (piece.x != targetX && piece.y != targetY) return false;

            // x축으로 이동하는 경우 x축 검사
            if (piece.y == targetY)
            {
                int startPoint = piece.x > targetX ? targetX : piece.x;
                int endPoint = piece.x >= targetX ? piece.x : targetX;
                for (int i = startPoint + 1; i < endPoint; i++) if (pieces[i, piece.y] != null) return false;
            }

            // y축으로 이동하는 경우 y축 검사
            if (piece.x == targetX)
            {
                int startPoint = piece.y > targetY ? targetY : piece.y;
                int endPoint = piece.y >= targetY ? piece.y : targetY;
                for (int i = startPoint + 1; i < endPoint; i++) if (pieces[piece.x, i] != null) return false;
            }

            // 이동 불가능한 어느 조건에도 걸리지 않았다면 이동 성공
            return true;
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

            // 두 좌표 차이의 절댓값이 다르다면 대각선 이동이 아닌 것으로 판단하여 이동 불가능
            if (dx != dy) return false;

            // 이동 경로에 다른 기물이 있는지 확인
            int dirX = targetX > piece.x ? 1 : -1;
            int dirY = targetY > piece.y ? 1 : -1;

            int x = piece.x + dirX;
            int y = piece.y + dirY;

            while (x != targetX && y != targetY)
            {
                if (pieces[x, y] != null) return false; // 경로에 기물 있다면 이동 불가
                x += dirX;
                y += dirY;
            }

            // 이동 불가능한 어떤 조건에도 걸리지 않았다면 이동 성공
            return true;
        }

        // 퀸
        if (piece.pieceType == PieceType.Queen)
        {
            // 직선 이동
            if (piece.x == targetX || piece.y == targetY)
            {
                // x축으로 이동하는 경우 x축 검사
                if (piece.y == targetY)
                {
                    int startPoint = piece.x > targetX ? targetX : piece.x;
                    int endPoint = piece.x >= targetX ? piece.x : targetX;
                    for (int i = startPoint + 1; i < endPoint; i++) if (pieces[i, piece.y] != null) return false;
                }

                // y축으로 이동하는 경우 y축 검사
                if (piece.x == targetX)
                {
                    int startPoint = piece.y > targetY ? targetY : piece.y;
                    int endPoint = piece.y >= targetY ? piece.y : targetY;
                    for (int i = startPoint + 1; i < endPoint; i++) if (pieces[piece.x, i] != null) return false;
                }

                // 이동 불가능한 어느 조건에도 걸리지 않았다면 이동 성공
                return true;
            }

            // 대각선 이동
            // 현재 위치와 목표 위치 사이의 차이를 구한다.
            int dx = Mathf.Abs(targetX - piece.x);
            int dy = Mathf.Abs(targetY - piece.y);

            // 두 좌표 차이의 절댓값이 같다면 대각선 이동으로 판단하여 이동 가능
            if (dx == dy)
            {
                // 이동 경로에 다른 기물이 있는지 확인
                int dirX = targetX > piece.x ? 1 : -1;
                int dirY = targetY > piece.y ? 1 : -1;

                int x = piece.x + dirX;
                int y = piece.y + dirY;

                while (x != targetX && y != targetY)
                {
                    if (pieces[x, y] != null) return false; // 경로에 기물 있다면 이동 불가
                    x += dirX;
                    y += dirY;
                }

                // 이동 불가능한 어떤 조건에도 걸리지 않았다면 이동 성공
                return true;
            }

            // 그 외 경로로 이동하려 하면 이동 실패
            return false;
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

    // 이동가능한 타일 계산
    public List<Vector2Int> GetAvailableMoves(ChessPiece piece)
    {
        List<Vector2Int> moves = new List<Vector2Int>();

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (CalculateAbleMovePos(piece, x, y))
                {
                    moves.Add(new Vector2Int(x, y));
                }
            }
        }

        return moves;
    }

    private void SetGame(int stage)
    {
        if( stage == 1 )
        {
            SpawnPiece(blackPieces[4], 0, 0, PieceType.Queen, false);
            SpawnPiece(blackPieces[1], 6, 1, PieceType.Rook, false);
            SpawnPiece(whitePieces[5], 4, 2, PieceType.King, true);
            SpawnPiece(blackPieces[3], 3, 4, PieceType.Bishop, false);
            SpawnPiece(whitePieces[0], 4, 4, PieceType.Pawn, true);
            SpawnPiece(blackPieces[0], 7, 4, PieceType.Pawn, false);
            SpawnPiece(blackPieces[0], 4, 5, PieceType.Pawn, false);
            SpawnPiece(blackPieces[0], 5, 6, PieceType.Pawn, false);
            SpawnPiece(blackPieces[5], 7, 6, PieceType.King, false);
            SpawnPiece(whitePieces[1], 1, 7, PieceType.Rook, true);
            SpawnPiece(whitePieces[1], 2, 7, PieceType.Rook, true);
        }

        if( stage == 2 )
        {
            SpawnPiece(whitePieces[4], 1, 2, PieceType.Queen, true);
            SpawnPiece(blackPieces[5], 3, 3, PieceType.King, false);
            SpawnPiece(blackPieces[0], 4, 3, PieceType.Pawn, false);
            SpawnPiece(whitePieces[0], 0, 4, PieceType.Pawn, true);
            SpawnPiece(blackPieces[2], 1, 5, PieceType.Knight, false);
            SpawnPiece(blackPieces[4], 5, 5, PieceType.Queen, false);
            SpawnPiece(whitePieces[0], 1, 6, PieceType.Pawn, true);
            SpawnPiece(whitePieces[0], 7, 6, PieceType.Pawn, true);
            SpawnPiece(whitePieces[5], 1, 7, PieceType.King, true);
            SpawnPiece(whitePieces[1], 2, 7, PieceType.Rook, true);
        }

        if( stage == 3 )
        {
            SpawnPiece(blackPieces[5], 0, 0, PieceType.King, false);
            SpawnPiece(whitePieces[1], 1, 1, PieceType.Rook, true);
            SpawnPiece(whitePieces[1], 6, 1, PieceType.Rook, true);
            SpawnPiece(blackPieces[0], 0, 2, PieceType.Pawn, false);
            SpawnPiece(whitePieces[5], 5, 4, PieceType.King, true);
            SpawnPiece(whitePieces[0], 1, 4, PieceType.Pawn, true);
            SpawnPiece(blackPieces[1], 0, 5, PieceType.Rook, false);
            SpawnPiece(whitePieces[0], 7, 5, PieceType.Pawn, true);
            SpawnPiece(blackPieces[1], 4, 6, PieceType.Rook, false);
            SpawnPiece(whitePieces[0], 6, 6, PieceType.Pawn, false);
        }

        if( stage == 4 )
        {
            SpawnPiece(blackPieces[0], 0, 2, PieceType.Pawn, false);
            SpawnPiece(blackPieces[1], 1, 2, PieceType.Rook, false);
            SpawnPiece(whitePieces[0], 6, 3, PieceType.Pawn, true);
            SpawnPiece(whitePieces[5], 7, 3, PieceType.King, true);
            SpawnPiece(whitePieces[0], 5, 4, PieceType.Pawn, true);
            SpawnPiece(whitePieces[0], 6, 4, PieceType.Pawn, true);
            SpawnPiece(whitePieces[1], 0, 5, PieceType.Rook, true);
            SpawnPiece(blackPieces[0], 5, 6, PieceType.Pawn, false);
            SpawnPiece(blackPieces[0], 6, 6, PieceType.Pawn, false);
            SpawnPiece(blackPieces[5], 7, 6, PieceType.King, false);
        }

        if( stage == 5 )
        {
            SpawnPiece(whitePieces[0], 0, 1, PieceType.Pawn, true);
            SpawnPiece(whitePieces[0], 5, 1, PieceType.Pawn, true);
            SpawnPiece(whitePieces[5], 6, 1, PieceType.King, true);
            SpawnPiece(whitePieces[0], 2, 3, PieceType.Pawn, true);
            SpawnPiece(blackPieces[2], 6, 3, PieceType.Knight, false);
            SpawnPiece(blackPieces[4], 7, 3, PieceType.Queen, false);
            SpawnPiece(blackPieces[0], 2, 4, PieceType.Pawn, false);
            SpawnPiece(whitePieces[0], 7, 4, PieceType.Pawn, true);
            SpawnPiece(blackPieces[5], 1, 5, PieceType.King, false);
            SpawnPiece(blackPieces[0], 3, 5, PieceType.Pawn, false);
            SpawnPiece(whitePieces[4], 6, 5, PieceType.King, true);
            SpawnPiece(blackPieces[0], 0, 6, PieceType.Pawn, false);
            SpawnPiece(blackPieces[0], 1, 6, PieceType.Pawn, false);
        }

        if( stage == 6 )
        {
            SpawnPiece(whitePieces[0], 3, 1, PieceType.Pawn, true);
            SpawnPiece(whitePieces[1], 4, 2, PieceType.Rook, true);
            SpawnPiece(whitePieces[0], 4, 3, PieceType.Pawn, true);
            SpawnPiece(whitePieces[0], 5, 3, PieceType.Pawn, true);
            SpawnPiece(whitePieces[0], 7, 3, PieceType.Pawn, true);
            SpawnPiece(blackPieces[5], 5, 4, PieceType.King, false);
            SpawnPiece(whitePieces[0], 6, 4, PieceType.Pawn, true);
            SpawnPiece(blackPieces[1], 1, 6, PieceType.Rook, false);
            SpawnPiece(whitePieces[5], 7, 7, PieceType.King, true);
        }
    }

    // 이동 가능한 타일 바닥표시하기
    private void UpdateAvailableMoves()
    {
        if (selectedPiece == null) return;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (CalculateAbleMovePos(selectedPiece, x, y))
                {
                    ChessPiece targetPiece = pieces[x, y];

                    if (targetPiece == null)
                    {
                        tiles[x, y].ShowCanMove(true);
                    }
                    else if (targetPiece.isWhite != selectedPiece.isWhite)
                    {
                        tiles[x, y].ShowCanAttack(true);
                    }
                }
            }
        }
    }

    public void OnStageClear()
    {
        GameManager.instance.OnStageClear(); // 게임매니저에 스테이지 클리어 알림
        foreach (var tile in tiles)
        {
            tile.ShowCanMove(false); // 모든 이동 가능한 타일 표시 해제
            tile.ShowCanAttack(false); // 모든 공격 가능한 타일 표시 해제
        }
        selectedPiece = null; // 선택된 기물 해제

        // 기존 체스보드 초기화 (삭제)
        foreach (Transform child in boardParent)
        {
            Destroy(child.gameObject);
        }
        // 기존 체스말 초기화 (삭제)
        foreach (Transform child in pieceParent)
        {
            Destroy(child.gameObject);
        }
    }

    public void OnStageFailed()
    {
        GameManager.instance.OnGameFailed(); // 게임매니저에 게임 오버 알림
        foreach (var tile in tiles)
        {
            tile.ShowCanMove(false); // 모든 이동 가능한 타일 표시 해제
            tile.ShowCanAttack(false); // 모든 공격 가능한 타일 표시 해제
        }
        selectedPiece = null; // 선택된 기물 해제
        // 기존 체스보드 초기화 (삭제)
        foreach (Transform child in boardParent)
        {
            Destroy(child.gameObject);
        }
        // 기존 체스말 초기화 (삭제)
        foreach (Transform child in pieceParent)
        {
            Destroy(child.gameObject);
        }
    }
}