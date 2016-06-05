﻿using UnityEngine;
using System.Collections.Generic;
using System;

namespace Application {
	
	public class Controller14 : MonoBehaviour
	{
		public Board gameBoard = new Board();
		
		RaycastHit hit;
		
		GameObject interactionPiece;
		
		float startPosX, startPosZ;
		int playerNo = 1;
		Boolean pieceChangedToKing = false;
		
		List<PlayerPiece> moveablePieces = new List<PlayerPiece> ();
		List<PlayerPiece> takeablePieces = new List<PlayerPiece> ();
		
		
		
		//TODO fix issue where player number is switch twice when changed to king
		
		//FIX issue where piece is transformed into a king and messes up take moves
		//FIX issue moving pieces in correct range, they can move up or down two spaces 
		//FIX up and left right move transforming incorrectly
		
		
		//TAKE UP AND LEFT NOT DESTROYING PIECE!!!
		void Start () {
			gameBoard.SetupPlayerArray ();
		}
		
		void Update () {
			takeablePieces.Clear ();
			moveablePieces.Clear ();
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			Debug.DrawRay (ray.origin, ray.direction * 100, Color.red);
			
			//WORKS PICKING UP AND PUTTING DOWN OBJECTS BUT NOT DRAGGING
			if (Input.GetMouseButtonDown (0)) {
				if ((Physics.Raycast (ray, out hit)) && hit.collider.tag.Contains("Player") ) {	
					startPosX = hit.collider.transform.localPosition.x;
					startPosZ = hit.collider.transform.localPosition.z;
					interactionPiece = hit.collider.gameObject;

					/**if(playerHasTakeableMoves(playerNo)) {
						if (interactionPiece.tag.Contains("Player" + playerNo.ToString()) && canTake((int)startPosX, (int)startPosZ)) {
							interactionPiece.transform.position = new Vector3 (hit.collider.transform.localPosition.x, 1.0f, hit.collider.transform.localPosition.z);
						}
						else {
							interactionPiece = null;
						}
					}
					else**/


					 if (!playerHasTakeableMoves(playerNo)) {
						if (interactionPiece.tag.Contains("Player" + playerNo.ToString()) && canMove((int)startPosX, (int)startPosZ)) {
							interactionPiece.transform.position = new Vector3 (hit.collider.transform.localPosition.x, 1.0f, hit.collider.transform.localPosition.z);
						}
						else {
							interactionPiece = null;
						}
					}
					else {
						interactionPiece = null;
					}
				}
				else {
					interactionPiece = null;
				}
			}
			if (Input.GetMouseButtonUp (0) && interactionPiece != null) {
				if (Physics.Raycast (ray, out hit)) {
					
					float tempz = hit.collider.transform.localPosition.z;
					float tempx = hit.collider.transform.localPosition.x;
					
					//Take section
					//if(canTake((int)startPosX, (int)startPosZ) && (playerHasTakeableMoves(playerNo) == true)) {
					if(canTake((int)startPosX, (int)startPosZ)) {
						Debug.Log ("In can take block");
						interactionPiece.transform.position = new Vector3(startPosX, 0.1f, startPosZ);
						if((tempx > startPosX) && ((tempz - startPosZ > 1.5) && (tempz - startPosZ < 2.5)) && canTakeDownAndRight((int)startPosX, (int)startPosZ)) {
							takeDownAndRight((int) startPosX, (int) startPosZ, interactionPiece);
							interactionPiece.transform.position = new Vector3(startPosX + 2, 0.1f,startPosZ + 2);	
							Ray rayRay = new Ray();
							rayRay.origin = new Vector3(startPosX, 0.1f, startPosZ);
							rayRay.direction = new Vector3(2.0f, 0.1f, 1.5f);
							Debug.DrawRay(rayRay.origin = new Vector3(startPosX, 0.1f, startPosZ), new Vector3(2.0f, 0.1f, 1.5f));
							if(Physics.Raycast(rayRay, out hit)) {
								Destroy (hit.transform.gameObject);
							}
							Debug.Log ("taken down and right!");
							takeablePieces.Clear();
							if(!canTake((int)interactionPiece.transform.position.x, (int)interactionPiece.transform.position.x) && pieceChangedToKing == false) {
								changePlayer();
							}
							else {
								Debug.Log("can take again after taking down and right");
							}
						}
						
						//TODO FIX!!!!
						else if((tempx > startPosX) && (tempz - startPosZ < -1.5) && ((tempz - startPosZ) > -2.5) && canTakeDownAndLeft((int)startPosX, (int)startPosZ)) {
							takeDownAndLeft((int) startPosX, (int) startPosZ, interactionPiece);
							interactionPiece.transform.position = new Vector3(startPosX + 2, 0.1f,startPosZ - 2);
							Ray rayRay = new Ray();
							rayRay.origin = new Vector3(startPosX, 0.1f, startPosZ);
							rayRay.direction = new Vector3(2.0f, 0.1f,-1.5f);
							Debug.DrawRay(rayRay.origin = new Vector3(startPosX, 0.1f, startPosZ), new Vector3(2.0f, 0.1f,-1.5f));
							if(Physics.Raycast(rayRay, out hit)) {
								
								Destroy (hit.transform.gameObject);
							}
							Debug.Log ("taken down and left!");
							takeablePieces.Clear();
							if(!canTake((int)interactionPiece.transform.position.x, (int)interactionPiece.transform.position.x) && pieceChangedToKing == false) {
								changePlayer();
							}
							else {
								Debug.Log("can take again after taking down and left");
							}
						}
						else if((tempx < startPosX) && ((tempz - startPosZ < -1.5) && (tempz - startPosZ > -3)) && (tempx - startPosX < -1) && canTakeUpAndLeft((int)startPosX, (int)startPosZ)) {
							takeUpAndLeft((int) startPosX, (int) startPosZ, interactionPiece);
							interactionPiece.transform.position = new Vector3(startPosX - 2, 0.1f,startPosZ - 2);
							Ray rayRay = new Ray();
							rayRay.origin = new Vector3(startPosX, 0.1f, startPosZ);
							rayRay.direction = new Vector3(-2.0f, 0.1f,-1.5f);
							Debug.DrawRay(rayRay.origin = new Vector3(startPosX, 0.1f, startPosZ), new Vector3(-2.0f, 0.1f,-1.5f));
							if(Physics.Raycast(rayRay, out hit)) {
								
								Destroy (hit.transform.gameObject);
							}
							Debug.Log ("taken up and left!");
							takeablePieces.Clear();
							if(!canTake ((int)interactionPiece.transform.position.x, (int)interactionPiece.transform.position.x) && pieceChangedToKing == false) {
								changePlayer();
							}
							else {
								Debug.Log("can take again after taking up and left");
							}
						}
						else if((tempx < startPosX) && ((tempz - startPosZ > 1.5) && (tempz - startPosZ < 3)) && (tempx - startPosX < -1) && canTakeUpAndRight((int)startPosX, (int)startPosZ)) {
							takeUpAndRight((int) startPosX, (int) startPosZ, interactionPiece);
							interactionPiece.transform.position = new Vector3(startPosX - 2, 0.1f,startPosZ + 2);
							Ray rayRay = new Ray();
							rayRay.origin = new Vector3(startPosX, 0.1f, startPosZ);
							rayRay.direction = new Vector3(-2.0f, 0.1f,1.5f);
							Debug.DrawRay(rayRay.origin = new Vector3(startPosX, 0.1f, startPosZ), new Vector3(-2.0f, 0.1f,1.5f));
							
							if(Physics.Raycast(rayRay, out hit)) {
								
								Destroy (hit.transform.gameObject);
							}
							Debug.Log ("taken up and right!");
							if(!canTake((int)interactionPiece.transform.position.x, (int)interactionPiece.transform.position.x) && pieceChangedToKing == false) {
								changePlayer();
							}
							else {
								Debug.Log("can take again after taking up and right");
							}
							//startPosX = null;
							//startPosZ = null;
						}
						else {
							interactionPiece.transform.position = new Vector3(startPosX, 0.1f, startPosZ);
						}
					}
					
					//Movement section
					else if (canMove((int)startPosX, (int)startPosZ)) { // && (playerHasTakeableMoves(playerNo) == false)) {
						interactionPiece.transform.position = new Vector3(startPosX, 0.1f, startPosZ);
						Debug.Log ("In can move block");
						if((tempx > startPosX) && ((tempz - startPosZ) < 1.5) && ((tempz - startPosZ) > 0) && canMoveDownAndRight((int)startPosX, (int)startPosZ)) {
							moveDownAndRight ((int) startPosX, (int) startPosZ, interactionPiece);
							interactionPiece.transform.position = new Vector3(tempx, 0.1f, tempz);
							Debug.Log("moved down and right");
						}
						else if((tempx > startPosX) && ((tempz - startPosZ) < 0) && ((tempz - startPosZ) > -1.5) && canMoveDownAndLeft((int)startPosX, (int)startPosZ)) {
							moveDownAndLeft ((int) startPosX, (int) startPosZ, interactionPiece);
							interactionPiece.transform.position = new Vector3(tempx, 0.1f, tempz);
							Debug.Log("moved down and left");
						}
						else if ((tempx < startPosX) && ((tempz - startPosZ) < 1.5) && ((tempz - startPosZ) > 0) && ((tempz - startPosZ) < 1.5) && canMoveUpAndRight((int)startPosX, (int)startPosZ)) {
							moveUpAndRight((int) startPosX, (int) startPosZ, interactionPiece);
							interactionPiece.transform.position = new Vector3(startPosX - 1, 0.1f, startPosZ + 1);
							Debug.Log("moved up and right");
						}
						else if ((tempx < startPosX) && ((tempz - startPosZ) < 0) && ((tempz - startPosZ) > -1.5) && ((tempz - startPosZ) < 0) && canMoveUpAndLeft((int)startPosX, (int)startPosZ)) {
							moveUpAndLeft((int) startPosX, (int) startPosZ, interactionPiece);
							interactionPiece.transform.position = new Vector3(tempx, 0.1f, tempz);
							Debug.Log("moved up and left");
						}
						else {
							moveablePieces.Clear (); 
							interactionPiece.transform.position = new Vector3(startPosX, 0.1f, startPosZ); 
						}
					}
					else {
						interactionPiece.transform.position = new Vector3(startPosX, 0.1f, startPosX); //TODO this should be start pos?
					}
				}
				else{
					interactionPiece.transform.position = new Vector3(startPosX, 0.1f, startPosX);
				}
			}
		}
		
		private bool canMove(int x, int y) {
			Debug.Log ("Player " + playerNo + " can move: " + (canMoveDownAndLeft (x, y) || canMoveDownAndRight (x, y) || canMoveUpAndLeft (x, y) || canMoveUpAndRight (x, y)));
			return (canMoveDownAndLeft (x, y) || canMoveDownAndRight (x, y) || canMoveUpAndLeft (x, y) || canMoveUpAndRight (x, y));
		}
		
		private bool canTake(int x, int y) {
			Debug.Log ("Player " + playerNo + " can take: " + (canTakeUpAndLeft (x, y) || canTakeUpAndRight (x, y) || canTakeDownAndLeft (x, y) || canTakeDownAndRight (x, y)));
			return (canTakeUpAndLeft (x, y) || canTakeUpAndRight (x, y) || canTakeDownAndLeft (x, y) || canTakeDownAndRight (x, y));
		}
		
		private int getOpponent (int playerNo) {
			if (playerNo == 1)
				return 2;
			else 
				return 1;
		}
		
		private void changePlayer () {
			String s = "Change player from " + playerNo;
			if (playerNo == 1) {
				playerNo = 2;
				pieceChangedToKing = false;
			} else {
				playerNo = 1;
				pieceChangedToKing = false;
			}
			takeablePieces = new List<PlayerPiece> ();
			moveablePieces = new List<PlayerPiece> ();
			Debug.Log (s + " to:" + playerNo);
		}
		
		private void takeUpAndRight (int x, int y, GameObject playerPiece) {
			if (canTakeUpAndRight (x, y)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
				gameBoard.removePiece (x, y);
				gameBoard.removePiece (x - 1, y + 1);
				gameBoard.AddPlayerPiece (piece, x - 2, y + 2);
				if(piece.isKing == false && x-2 == 0) {
					piece.isKing = true;
					Debug.Log ("Changed player to king");
					playerPiece.transform.localScale = new Vector3 (1.0f, 1.5f, 1.0f);
					changePlayer();
					pieceChangedToKing = true;
				}
				Debug.Log ("got here 1");
			}
		}
		
		private void takeUpAndLeft (int x, int y, GameObject playerPiece) {
			if (canTakeUpAndLeft (x, y)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
				gameBoard.removePiece (x, y);
				gameBoard.removePiece (x - 1, y - 1);
				gameBoard.AddPlayerPiece (piece, x - 2, y - 2);
				if(piece.isKing == false && x - 2  == 0) {
					piece.isKing = true;
					Debug.Log ("Changed player to king");
					playerPiece.transform.localScale = new Vector3 (1.0f, 1.5f, 1.0f);
					changePlayer();
					pieceChangedToKing = true;
				}
				Debug.Log ("got here 2");
			}
		}
		
		private void takeDownAndLeft(int x, int y, GameObject playerPiece) {
			if (canTakeDownAndLeft (x, y)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece(x, y);
				gameBoard.removePiece(x, y);
				gameBoard.removePiece(x + 1, y - 1);
				gameBoard.AddPlayerPiece(piece, x + 2, y - 2);
				if(piece.isKing == false && x + 2 == 7) {
					piece.isKing = true;
					Debug.Log ("Changed player to king");
					playerPiece.transform.localScale = new Vector3 (1.0f, 1.5f, 1.0f);
					changePlayer();
					pieceChangedToKing = true;
				}
				Debug.Log ("got here 3");
			}
		}
		
		private void takeDownAndRight (int x, int y, GameObject playerPiece) {
			if (canTakeDownAndRight (x, y)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
				gameBoard.removePiece (x, y);
				gameBoard.removePiece(x + 1, y + 1);
				gameBoard.AddPlayerPiece(piece, x + 2, y + 2);
				if(piece.isKing == false && x + 2 == 7) {
					piece.isKing = true;
					Debug.Log ("Changed player to king");
					playerPiece.transform.localScale = new Vector3 (1.0f, 1.5f, 1.0f);
					changePlayer();
					pieceChangedToKing = true;
				}
				Debug.Log ("got here 4");
			}
		}
		
		private void moveDownAndRight (int x, int y, GameObject playerPiece) {
			if (canMoveDownAndRight (x, y)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
				if(piece.playerNo == 1 || piece.isKing == true) {
					gameBoard.removePiece (x, y);
					gameBoard.AddPlayerPiece (piece, x + 1, y + 1);
					
					if(piece.isKing == false && x + 1 == 7) {
						piece.isKing = true;
						Debug.Log ("Changed player to king");
						playerPiece.transform.localScale = new Vector3 (1.0f, 1.5f, 1.0f);
					}
				}
				changePlayer();
				Debug.Log ("got here 5");
			}
		}
		
		private void moveDownAndLeft (int x, int y, GameObject playerPiece) {
			if (canMoveDownAndLeft (x, y)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
				if(piece.playerNo == 1 || piece.isKing == true) {
					gameBoard.removePiece (x, y);
					gameBoard.AddPlayerPiece (piece, x + 1, y - 1);
					if(piece.isKing == false && x + 1 == 7) {
						piece.isKing = true;
						Debug.Log ("Changed player to king");
						playerPiece.transform.localScale = new Vector3 (1.0f, 1.5f, 1.0f);
						
					}
				}
				changePlayer();
				Debug.Log ("got here 6");
			}
		}
		
		private void moveUpAndLeft (int x, int y, GameObject playerPiece) {
			if (canMoveUpAndLeft (x, y)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
				if(piece.playerNo == 2 || piece.isKing == true) {
					gameBoard.removePiece (x, y);
					gameBoard.AddPlayerPiece (piece, x - 1, y - 1);
					if(piece.isKing == false && x - 1 == 0) {
						piece.isKing = true;
						Debug.Log ("Changed player to king");
						playerPiece.transform.localScale = new Vector3 (1.0f, 1.5f, 1.0f);
					}
				}
				changePlayer();
				Debug.Log ("got here 7");
			}
		}
		
		private void moveUpAndRight (int x, int y, GameObject playerPiece) {
			if (canMoveUpAndRight (x, y)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
				if(piece.playerNo == 2 || piece.isKing == true) {
					gameBoard.removePiece (x, y);
					gameBoard.AddPlayerPiece (piece, x - 1, y + 1);
					if(piece.isKing == false && x - 1 == 0) {
						piece.isKing = true;
						Debug.Log ("Changed player to king");
						playerPiece.transform.localScale = new Vector3 (1.0f, 1.5f, 1.0f);
					}
				}
				changePlayer();
				Debug.Log ("got here 8");
			}
		}
		
		private bool canMoveDownAndRight (int x, int y) {
			PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
			if ((piece != null) && (piece.playerNo == 1 || piece.isKing == true)) {
				if ((y + 1 < 8) && (x + 1 < 8)) {
					if (gameBoard.returnPlayerPiece (x + 1, y + 1) == null) {
						return true;
					} 
				}
			}
			return false;
		}
		
		private bool canMoveDownAndLeft (int x, int y) {
			PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
			if ((piece != null) && (piece.playerNo == 1 || piece.isKing == true)) {
				if ((y - 1 > -1) && (x + 1 < 8)) {
					if (gameBoard.returnPlayerPiece (x + 1, y - 1) == null) {
						return true;
					}
				}
			}
			return false;	
		}
		
		private bool canMoveUpAndRight (int x, int y) {
			PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
			if ((piece != null) && (piece.playerNo == 2 || piece.isKing == true)) {
				if ((y + 1 < 8) && (x - 1 > -1)) {
					if (gameBoard.returnPlayerPiece (x - 1, y + 1) == null) {
						return true;
					}
				}
			}
			return false;
		}
		
		private bool canMoveUpAndLeft (int x, int y)	{
			PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
			if ((piece != null) && (piece.playerNo == 2 || piece.isKing == true)) {
				if ((y - 1 > -1) && (x - 1 > -1)) {
					if (gameBoard.returnPlayerPiece (x - 1, y - 1) == null) {
						return true;
					} 
				}
			}
			return false;
		}
		
		private bool canTakeDownAndRight (int x, int y) {
			PlayerPiece currentPiece = gameBoard.returnPlayerPiece (x, y);
			
			if (currentPiece != null && (currentPiece.playerNo == 1 || currentPiece.isKing == true)) {
				
				if ((y + 2 < 8) && (x + 2 < 8)) {
					PlayerPiece enemy = gameBoard.returnPlayerPiece (x + 1, y + 1);
					
					if (enemy != null) {
						
						if (enemy.GetPlayerNumber () == getOpponent (currentPiece.GetPlayerNumber ())) {
							
							if (gameBoard.returnPlayerPiece (x + 2, y + 2) == null) {
								
								Debug.Log ("can take down and right");
								return true; 
							}
						} 
					}
				}
			}
			return false;
		}
		
		private bool canTakeDownAndLeft (int x, int y) {
			PlayerPiece currentPiece = gameBoard.returnPlayerPiece (x, y);
			
			if (currentPiece != null && (currentPiece.playerNo == 1 || currentPiece.isKing == true)) {
				
				if ((y - 2 > -1) && (x + 2 < 8)) {
					PlayerPiece enemy = gameBoard.returnPlayerPiece (x + 1, y - 1);
					
					if (enemy != null) {
						
						if (enemy.GetPlayerNumber () == getOpponent (currentPiece.GetPlayerNumber ())) {
							
							if (gameBoard.returnPlayerPiece (x + 2, y - 2) == null) {
								Debug.Log ("can down and left");
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		
		private bool canTakeUpAndLeft (int x, int y) {
			PlayerPiece currentPiece = gameBoard.returnPlayerPiece (x, y);
			if (currentPiece != null && (currentPiece.playerNo == 2 || currentPiece.isKing == true)) {
				
				if ((y - 2 > - 1) && (x - 2 > - 1)) {
					
					PlayerPiece enemy = gameBoard.returnPlayerPiece (x - 1, y - 1);
					if (enemy != null) {
						
						if (enemy.GetPlayerNumber () == getOpponent (currentPiece.GetPlayerNumber ())) {
							
							if (gameBoard.returnPlayerPiece (x - 2, y - 2) == null) {
								Debug.Log ("can take up and left");
								return true;
							}
						}
					}			
				}
			} 
			return false;
		}
		
		private bool canTakeUpAndRight (int x, int y) {
			PlayerPiece currentPiece = gameBoard.returnPlayerPiece (x, y);
			if (currentPiece != null && (currentPiece.playerNo == 2 || currentPiece.isKing == true)) {
				
				if ((x - 2 > - 1) && (y + 2 < 8)) {
					
					PlayerPiece enemy = gameBoard.returnPlayerPiece (x - 1, y + 1);
					if (enemy != null) {
						
						if (enemy.GetPlayerNumber () == getOpponent (currentPiece.GetPlayerNumber ())) {
							
							if (gameBoard.returnPlayerPiece (x - 2, y + 2) == null) {
								Debug.Log ("can take up and right");
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		
		private List<PlayerPiece> returnTakeablePieces (int playerNumber) {
			
			int i = 0, j = 0;
			while (j < 8) {
				while (i < 8) {
					if (canTakeDownAndRight (i, j) || canTakeDownAndLeft (i, j) || canTakeUpAndRight (i, j) || canTakeUpAndLeft (i, j)) {
						if(canTakeDownAndRight (i, j))
							Debug.Log("returning takeable moves and can take down and right{" + i.ToString() + "," + j.ToString() + "}");
						if(canTakeDownAndLeft (i, j))
							Debug.Log("returning takeable moves and can take down and left{" + i.ToString() + "," + j + "}");
						if(canTakeUpAndRight (i, j))
							Debug.Log("returning takeable moves and can take up and right {" + i.ToString() + "," + j.ToString() + "}");
						if(canTakeUpAndLeft (i, j))
							Debug.Log("returning takeable moves and can take up and left{" + i.ToString() + "," + j.ToString() + "}");
						takeablePieces.Add (gameBoard.returnPlayerPiece (i, j));
					}
					i++;
				}
				j++;
				i = 0;
			}
			return takeablePieces;
		}
		
		private bool playerHasTakeableMoves(int temp) {
			List<PlayerPiece> takeablePieces = returnTakeablePieces (temp);
			return (takeablePieces.Count > 0);
		}
	}
}