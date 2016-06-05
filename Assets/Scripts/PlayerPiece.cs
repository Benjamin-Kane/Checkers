﻿using System;
using UnityEngine;

public class PlayerPiece {

	public int playerNo;
	public Boolean isKing = false;

	public PlayerPiece (int playerNo, Boolean isKing) {
		this.playerNo = playerNo;
		this.isKing = isKing;
	}

	public int GetPlayerNumber() {
		return playerNo;
	}

	public String PrintPlayer() {
		return "Player Number: " + playerNo.ToString () + " is King? " + isKing.ToString() ;
	}

	public void SetKing () {
		isKing = true;
	}

	public Boolean CheckIsKing() {
		return isKing;
	}
}