﻿using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece {

    public Bishop()
    {

    }

    public Bishop(int all, Point p, Board b, PieceTypeE t) : base(all, p, b, t)
    {
		if(all == 0)
			Instantiate(whitebishop, new Vector3(p.turnToWorld()[0], 0.25, p.turnToWorld()[1]), Quaternion.identity);
		else
			Instantiate(blackbishop, new Vector3(p.turnToWorld()[0], 0.25, p.turnToWorld()[1]), Quaternion.identity);
    }

    //Create list of valid moves moving along diagonal until finding illegal move
    override public List<Point> canMoveList()
    {
        List<Point> retMoveList = new List<Point>();
        bool[] flagArray = { true, true, true, true };
        for (int i = 1; flagArray[0] && i < 8; i++)
        {
            Point p = new Point(loc.getX() + i, loc.getY() + i);
            if (canMove(p) == MoveTypesE.ILLEGAL)
                flagArray[0] = false;
            else
                retMoveList.Add(p);
        }
        for (int i = 1; flagArray[1] && i < 8; i++)
        {
            Point p = new Point(loc.getX() - i, loc.getY() + i);
            if (canMove(p) == MoveTypesE.ILLEGAL)
                flagArray[1] = false;
            else
                retMoveList.Add(p);
        }
        for (int i = 1; flagArray[2] && i < 8; i++)
        {
            Point p = new Point(loc.getX() + i, loc.getY() - i);
            if (canMove(p) == MoveTypesE.ILLEGAL)
                flagArray[2] = false;
            else
                retMoveList.Add(p);
        }
        for (int i = 1; flagArray[3] && i < 8; i++)
        {
            Point p = new Point(loc.getX() - i, loc.getY() - i);
            if (canMove(p) == MoveTypesE.ILLEGAL)
                flagArray[3] = false;
            else
                retMoveList.Add(p);
        }
        return retMoveList;
    }

    //The bishop moves to any square (except as limited by Article 4.2) on the diagonals on which it stands.
    override public MoveTypesE canMove(Point p)
    {
        MoveTypesE mt = base.canMove(p);
        if (mt == MoveTypesE.ILLEGAL)
            return MoveTypesE.ILLEGAL;
        int dy = p.getY() - loc.getY();
        int dx = p.getX() - loc.getX();

        if (System.Math.Abs(dy) == System.Math.Abs(dx))
        {
            int signFactorX = (dx * System.Math.Abs(dx) > 0) ? 1 : -1;
            int signFactorY = (dy * System.Math.Abs(dy) > 0) ? 1 : -1;
            for (int i = 1; i < System.Math.Abs(dx); i++)
            {
                if (gameBoard.pieceAt(loc.getX() + signFactorX * i, loc.getY() + signFactorY * i) != null)
                    return MoveTypesE.ILLEGAL;
            }
        }
        return mt;
    }
}
