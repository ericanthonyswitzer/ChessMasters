﻿//Most of this game logic is much more complicated than I had predicted.
//I have the basic logic of the game, but I am working on studying chessprogramming.wikispaces.com to learn more about algorithms to work with chess

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : Singleton<Board>
{
    public enum PlayerE
    {
        White = 0,
        Black = 1
    };

    public enum AIE: int
    {
        NONE = 0,
        EASY = 1,
        NORMAL = 2,
        HARD = 3
    };

    bool gameActive;
    int turn = (int) PlayerE.White;
    bool piecesUpdated = false;
    Piece[,] boardPieces;
    List<Piece> whiteList;
    List<Piece> blackList;
    History firstHistory, lastHistory;
    Point enPassant;
    Piece[] kings;
    public AIE ai;
    public GameObject whitePawn;
    public GameObject blackPawn;
    public GameObject whiteRook;
    public GameObject blackRook;
    public GameObject whiteKnight;
    public GameObject blackKnight;
    public GameObject whiteBishop;
    public GameObject blackBishop;
    public GameObject whiteQueen;
    public GameObject blackQueen;
    public GameObject whiteKing;
    public GameObject blackKing;

    // Use this for initialization
    void Start()
    {
        boardPieces = new Piece[8, 8];
        kings = new Piece[2];
        whiteList = new List<Piece>();
        blackList = new List<Piece>();
        setupBoard();
    }

    // Update is called once per frame
    void Update()
    {
        if (piecesUpdated)
        {
            piecesUpdated = false;
            gameActive = isCheckmate();
            switch (ai)
            {
                case AIE.NONE:
                    break;
                case AIE.EASY:
                    runEasyAI();
                    break;
                case AIE.NORMAL:
                    runNormalAI();
                    break;
                default:
                    break;

            }
        }
        //During Milestone 2, there will be tiles once we integrate the graphics with this code
        //I will use a similar detection of click as for the Piece class
        //When I detect a click, if the tile clicked is highlighted, I will move currentPiece to the new location
        //Next, I will remove any piece currently on that tile from the game
        //I will then update the location of the piece
        //Finally, I will make piecesUpdated true
    }

    private void switchTurn()
    {
        turn += 1;
        turn %= 2;
    }

    //This is effectively acting as the constructor for Board
    void setupBoard()
    {
        generatePiece(PlayerE.White, new Point(0, 0), Piece.PieceTypeE.ROOK, whiteRook, "Rook");
        generatePiece(PlayerE.White, new Point(0, 1), Piece.PieceTypeE.KNIGHT, whiteKnight, "Knight");
        generatePiece(PlayerE.White, new Point(0, 2), Piece.PieceTypeE.BISHOP, whiteBishop, "Bishop");
        generatePiece(PlayerE.White, new Point(0, 3), Piece.PieceTypeE.QUEEN, whiteQueen, "Queen");
        generatePiece(PlayerE.White, new Point(0, 4), Piece.PieceTypeE.KING, whiteKing, "King");
        generatePiece(PlayerE.White, new Point(0, 5), Piece.PieceTypeE.BISHOP, whiteBishop, "Bishop");
        generatePiece(PlayerE.White, new Point(0, 6), Piece.PieceTypeE.KNIGHT, whiteKnight, "Knight");
        generatePiece(PlayerE.White, new Point(0, 7), Piece.PieceTypeE.ROOK, whiteRook, "Rook");
        generatePiece(PlayerE.White, new Point(1, 0), Piece.PieceTypeE.PAWN, whitePawn, "Pawn");
        generatePiece(PlayerE.White, new Point(1, 1), Piece.PieceTypeE.PAWN, whitePawn, "Pawn");
        generatePiece(PlayerE.White, new Point(1, 2), Piece.PieceTypeE.PAWN, whitePawn, "Pawn");
        generatePiece(PlayerE.White, new Point(1, 3), Piece.PieceTypeE.PAWN, whitePawn, "Pawn");
        generatePiece(PlayerE.White, new Point(1, 4), Piece.PieceTypeE.PAWN, whitePawn, "Pawn");
        generatePiece(PlayerE.White, new Point(1, 5), Piece.PieceTypeE.PAWN, whitePawn, "Pawn");
        generatePiece(PlayerE.White, new Point(1, 6), Piece.PieceTypeE.PAWN, whitePawn, "Pawn");
        generatePiece(PlayerE.White, new Point(1, 7), Piece.PieceTypeE.PAWN, whitePawn, "Pawn");

        generatePiece(PlayerE.Black, new Point(6, 0), Piece.PieceTypeE.PAWN, blackPawn, "Pawn");
        generatePiece(PlayerE.Black, new Point(6, 1), Piece.PieceTypeE.PAWN, blackPawn, "Pawn");
        generatePiece(PlayerE.Black, new Point(6, 2), Piece.PieceTypeE.PAWN, blackPawn, "Pawn");
        generatePiece(PlayerE.Black, new Point(6, 3), Piece.PieceTypeE.PAWN, blackPawn, "Pawn");
        generatePiece(PlayerE.Black, new Point(6, 4), Piece.PieceTypeE.PAWN, blackPawn, "Pawn");
        generatePiece(PlayerE.Black, new Point(6, 5), Piece.PieceTypeE.PAWN, blackPawn, "Pawn");
        generatePiece(PlayerE.Black, new Point(6, 6), Piece.PieceTypeE.PAWN, blackPawn, "Pawn");
        generatePiece(PlayerE.Black, new Point(6, 7), Piece.PieceTypeE.PAWN, blackPawn, "Pawn");
        generatePiece(PlayerE.Black, new Point(7, 0), Piece.PieceTypeE.ROOK, blackRook, "Rook");
        generatePiece(PlayerE.Black, new Point(7, 1), Piece.PieceTypeE.KNIGHT, blackKnight, "Knight");
        generatePiece(PlayerE.Black, new Point(7, 2), Piece.PieceTypeE.BISHOP, blackBishop, "Bishop");
        generatePiece(PlayerE.Black, new Point(7, 3), Piece.PieceTypeE.QUEEN, blackQueen, "Queen");
        generatePiece(PlayerE.Black, new Point(7, 4), Piece.PieceTypeE.KING, blackKing, "King");
        generatePiece(PlayerE.Black, new Point(7, 5), Piece.PieceTypeE.BISHOP, blackBishop, "Bishop");
        generatePiece(PlayerE.Black, new Point(7, 6), Piece.PieceTypeE.KNIGHT, blackKnight, "Knight");
        generatePiece(PlayerE.Black, new Point(7, 7), Piece.PieceTypeE.ROOK, blackRook, "Rook");

        firstHistory = null;
        lastHistory = null;
        enPassant = null;
    }

    public void generatePiece(PlayerE player, Point p, Piece.PieceTypeE piece, GameObject prefab, string str)
    {
        GameObject go = Instantiate(prefab, new Vector3(p.turnToWorld()[0], 0.25f, p.turnToWorld()[1]), Quaternion.identity);
        ((Piece)go.GetComponent(str)).initialize((int)player, p, this, piece);
        boardPieces[p.getX(), p.getY()] = (Piece)go.GetComponent(str);
        if (player == PlayerE.White)
            whiteList.Add(boardPieces[p.getX(), p.getY()]);
        else
            blackList.Add(boardPieces[p.getX(), p.getY()]);
        if(piece == Piece.PieceTypeE.KING)
        {
            if (player == PlayerE.White)
                kings[0] = boardPieces[p.getX(), p.getY()];
            else
                kings[1] = boardPieces[p.getX(), p.getY()];
        }
    }

    //Returns the piece located at the point p (null if no piece)
    public Piece pieceAt(Point p)
    {
        return boardPieces[p.getX() , p.getY()];
    }

    //Returns the piece located at (x,y) (null if no piece)
    public Piece pieceAt(int x, int y)
    {
        return boardPieces[x, y];
    }

    //Moves the piece located at the point p to the point pt
    public void placePieceAt(Piece p, Point pt)
    {
        if (boardPieces[pt.getX(), pt.getY()] != null)
        {
            if (boardPieces[pt.getX(), pt.getY()].getAllegiance() == 0)
                whiteList.Remove(boardPieces[pt.getX(), pt.getY()]);
            else
                blackList.Remove(boardPieces[pt.getX(), pt.getY()]);
        }
        boardPieces[pt.getX(), pt.getY()] = p;
    }

    //Moves the piece at the point p1 to p2 (calls the 3 paramater function with the third point null)
    public void Move(Point p1, Point p2)
    {
        Move(p1, p2, null);
    }

    //Moves the piece at the point p1 to p2 and sets enpassant to ep
    //Updates the game history
    //Switches the current turn
    public void Move(Point p1, Point p2, Point ep)
    {
        History temp_hist = new History(p1, p2, this, lastHistory);
        lastHistory.setNext(temp_hist);
        lastHistory = temp_hist;
        enPassant = ep;
        switchTurn();
        piecesUpdated = true;
        placePieceAt(pieceAt(p1), p2);
        boardPieces[p1.getX(), p1.getY()] = null;
    }

    //Calls tryToMove for the piece at p1 to move to p2
    public void tryToMove(Point p1, Point p2)
    {
        Piece temp_piece = pieceAt(p1);
        if (temp_piece != null)
        {
            temp_piece.tryToMove(p2);
        }
    }

    //Kills the piece at enPassant
    public void killEnPassant()
    {
        if(boardPieces[enPassant.getX(), enPassant.getY()] != null)
        {
            if (boardPieces[enPassant.getX(), enPassant.getY()].getAllegiance() == 0)
                whiteList.Remove(boardPieces[enPassant.getX(), enPassant.getY()]);
            else
                blackList.Remove(boardPieces[enPassant.getX(), enPassant.getY()]);
        }
        boardPieces[enPassant.getX(), enPassant.getY()] = null;
    }

    //Tests if moving a piece from start to finish would put the current turn's king in check
    public bool inCheck(Point start, Point finish)
    {
        Piece startPiece = boardPieces[start.getX(), start.getY()];
        Piece finishPiece = boardPieces[finish.getX(), finish.getY()];

        boardPieces[finish.getX(), finish.getY()] = startPiece;

        bool flag = inCheck(kings[turn].getLoc());
        boardPieces[start.getX(), start.getY()] = startPiece;
        boardPieces[finish.getX(), finish.getY()] = finishPiece;

        return flag;
    }

    //Tests if any enemy piece can move to the current space (where the king is)
    public bool inCheck(Point p)
    {
        for(int i = 0; i < 7; i++)
            for(int j = 0; j < 7; j++)
                if(boardPieces[i,j] != null && boardPieces[i,j].getAllegiance() != turn)
                    if (boardPieces[i, j].canMove(p) != Piece.MoveTypesE.ILLEGAL)
                        return true;
        return false;
    }

    //Tests if you pick up notKing and king, if the king is placed at (xloc, yloc) then if the king is in check
    //Used for castling
    public bool inCheck(Piece notKing, Piece king, int xloc, int yloc)
    {
        boardPieces[notKing.getLoc().getX(), notKing.getLoc().getY()] = null;
        boardPieces[king.getLoc().getX(), king.getLoc().getY()] = null;
        bool flag = inCheck(new Point(xloc, yloc));
        boardPieces[notKing.getLoc().getX(), notKing.getLoc().getY()] = notKing;
        boardPieces[king.getLoc().getX(), king.getLoc().getY()] = king;
        return flag;
    }
    
    //Promotes the pawn at p
    //Currently promotes it to queen until we figure out how to prompt the user
    public void promotePawn(Point p)
    {
        if (turn == 0)
            whiteList.Remove(boardPieces[p.getX(), p.getY()]);
        else
            blackList.Remove(boardPieces[p.getX(), p.getY()]);
        boardPieces[p.getX(), p.getY()] = new Queen(turn, p, this, Piece.PieceTypeE.QUEEN);
        if (turn == 0)
            whiteList.Add(boardPieces[p.getX(), p.getY()]);
        else
            blackList.Add(boardPieces[p.getX(), p.getY()]);
    }

    //Gets the point enPassant refers to currently
    public Point getEnPassant()
    {
        return enPassant;
    }

    //Highlights square (x,y)
    public void highlightSquare(int x, int y)
    {
        //Part of Milestone 2
    }

    //Removes highlighting from every square
    public void unhighlight()
    {
        //Part of Milestone 2
    }

    //Checks if there is any legal moves to make
    bool isCheckmate()
    {
        for (int i = 0; i < 7; i++)
            for (int j = 0; j < 7; j++)
                if (boardPieces[i, j] != null && boardPieces[i, j].getAllegiance() == turn)
                    if (boardPieces[i, j].canMoveList().Count > 0)
                        return false;
        return true;
    }

    //Compute a score for the player indicated
    public int computePlayerScore(int inScore, PlayerE currPlayer)
    {
        foreach (Piece p in whiteList)
            inScore += (p.getPieceScore() + p.canMoveList().Count);
        foreach (Piece p in blackList)
            inScore -= (p.getPieceScore() + p.canMoveList().Count);
        return inScore;
    }

    //Easy AI implementation
    //Picks a random valid piece and a random space it may move to
    //Hopefully not too computationally expensive
    public void runEasyAI()
    {
        bool flag = true;
        if(turn == (int)PlayerE.White)
        {
            while (flag)
            {
                int randPieceInt = Random.Range(0, whiteList.Count);
                Piece randPiece = whiteList[randPieceInt];
                List<Point> pointList = randPiece.canMoveList();
                if(pointList != null)
                {
                    Point randomPoint = pointList[Random.Range(0, pointList.Count)];
                    randPiece.tryToMove(randomPoint);
                    flag = false;
                }
            }
        }
        else
        {
            while (flag)
            {
                int randPieceInt = Random.Range(0, blackList.Count);
                Piece randPiece = blackList[randPieceInt];
                List<Point> pointList = randPiece.canMoveList();
                if (pointList != null)
                {
                    Point randomPoint = pointList[Random.Range(0, pointList.Count)];
                    randPiece.tryToMove(randomPoint);
                    flag = false;
                }
            }
        }
    }

    //Normal AI implementation
    //Look at best move to a depth of 3 which is maximum depth with current efficiency
    public void runNormalAI()
    {
        bool flag = true;
        if (turn == (int)PlayerE.White)
        {
            while (flag)
            {
                int randPieceInt = Random.Range(0, whiteList.Count);
                Piece randPiece = whiteList[randPieceInt];
                List<Point> pointList = randPiece.canMoveList();
                if (pointList != null)
                {
                    Point randomPoint = pointList[Random.Range(0, pointList.Count)];
                    randPiece.tryToMove(randomPoint);
                    flag = false;
                }
            }
        }
        else
        {
            while (flag)
            {
                int randPieceInt = Random.Range(0, blackList.Count);
                Piece randPiece = blackList[randPieceInt];
                List<Point> pointList = randPiece.canMoveList();
                if (pointList != null)
                {
                    Point randomPoint = pointList[Random.Range(0, pointList.Count)];
                    randPiece.tryToMove(randomPoint);
                    flag = false;
                }
            }
        }
    }
}