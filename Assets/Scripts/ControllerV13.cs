using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Application {

	public class ControllerV13 : MonoBehaviour {
		public Board gameBoard = new Board();
		private LogicController logicController;

		private RaycastHit hit, hit2;
		private Ray rayRay = new Ray();

		private GameObject interactionPiece;

		private int startPosX, startPosZ, playerNo = 1, aiPieceStartPosX, aiPieceStartPosZ, playerTwoPiecesCount = 11;

		private Boolean pieceChangedToKing = false, moved = false;

		private Queue<String> moveableAiPieces = new Queue<String>();

		private void Start () {
			gameBoard.SetupPlayerArray ();
			logicController = new LogicController (gameBoard);
			getAIQueueMoves ();
		}

		private void Update () {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if (playerNo == 2) {

				//below section is breadth first search
								if(logicController.playerHasTakeableMoves(2, gameBoard) && playerTwoPiecesCount > -1 ) {
									AITake();
								} else if (playerTwoPiecesCount > -1) {
									AIQueueMove();
									getAIQueueMoves();
								}	 

				//below section is depth first search
//				if (playerNo == 2) {
//					if(logicController.playerHasTakeableMoves(2, gameBoard)) {
//						AITake();
//					} else {
//						AIMove();
//					}
//				}
			} else {
				if (Input.GetMouseButtonDown (0)) {
					if ((Physics.Raycast (ray, out hit)) && hit.collider.tag.Contains("Player" + playerNo.ToString())) {	
						startPosX = (int)hit.collider.transform.localPosition.x;
						startPosZ = (int)hit.collider.transform.localPosition.z;
						interactionPiece = hit.collider.gameObject;
						if (logicController.playerHasTakeableMoves (playerNo, gameBoard) && canTake (startPosX,startPosZ)) {
							interactionPiece.transform.position = new Vector3 (hit.collider.transform.localPosition.x, 1.0f, hit.collider.transform.localPosition.z);
						} else if (logicController.canMove (startPosX, startPosZ, gameBoard) && (!logicController.playerHasTakeableMoves (playerNo, gameBoard))) {
							interactionPiece.transform.position = new Vector3 (hit.collider.transform.localPosition.x, 1.0f, hit.collider.transform.localPosition.z);

						} else {
							interactionPiece = null;
						}
					} else {
						interactionPiece = null;
					}
				}
				if (Input.GetMouseButtonUp (0) && interactionPiece != null && Physics.Raycast (ray, out hit)) {
					if (logicController.canTake (startPosX, startPosZ, gameBoard)) {
						take (hit.collider.transform.localPosition.x, hit.collider.transform.localPosition.z);
						playerTwoPiecesCount--;
					} else if (logicController.canMove (startPosX, startPosZ, gameBoard)) {
						move ((int)hit.collider.transform.localPosition.x, (int)hit.collider.transform.localPosition.z);
						hit = new RaycastHit();
					} else {
						interactionPiece.transform.position = new Vector3 (startPosX, 0.1f, startPosX);
					}
				}
			}
		}

		private void AITake() {
			float i = 0, j = 0;
			System.Threading.Thread.Sleep(500);

			while (j < 8) {
				while(i < 8) {
					rayRay.origin = (new Vector3 (i, 1.0f, j));
					rayRay.direction = (new Vector3 (0 , -1.0f, 0));
					if ((Physics.Raycast (rayRay, out hit) && hit.collider.tag.Contains("Player2"))) {
						interactionPiece = hit.collider.gameObject;
						aiPieceStartPosX = (int)hit.transform.position.x;
						aiPieceStartPosZ = (int)hit.transform.position.z;
						if (logicController.canTakeUpAndRight (aiPieceStartPosX, aiPieceStartPosZ, gameBoard)) {
							takeUpAndRight (aiPieceStartPosX, aiPieceStartPosZ, interactionPiece);
							takeWithAiPiece (hit, -2, 2, -1, 1);
							goto Found;
						} else if(logicController.canTakeUpAndLeft (aiPieceStartPosX, aiPieceStartPosZ, gameBoard)) {
							takeUpAndLeft (aiPieceStartPosX, aiPieceStartPosZ, interactionPiece);
							takeWithAiPiece (hit, -2, -2, -1, -1);
							goto Found;
						} else if(logicController.canTakeDownAndRight (aiPieceStartPosX, aiPieceStartPosZ, gameBoard)) {
							takeDownAndRight (aiPieceStartPosX, aiPieceStartPosZ, interactionPiece);
							takeWithAiPiece (hit, 2, 2, 1, 1);
							goto Found;
						} else if(logicController.canTakeDownAndLeft (aiPieceStartPosX, aiPieceStartPosZ, gameBoard)) {
							takeDownAndLeft (aiPieceStartPosX, aiPieceStartPosZ, interactionPiece);
							takeWithAiPiece (hit, 2, -2, 1, -1);
							goto Found;
						}
					}
					i++;
				}
				j++;
				i = 0;
			}
			Found: ;
		}

		private void takeWithAiPiece (RaycastHit hit, int moveToXPos, int moveToZPos, int enemyXPos, int enemyZPos ) {
			destroyPiece ((int)hit.transform.position.x, (int)hit.transform.position.z, moveToXPos, moveToZPos);
			hit.collider.gameObject.transform.position = new Vector3 (this.hit.transform.position.x + enemyXPos, 0.1f, this.hit.transform.position.z + enemyZPos );
			rayRay.origin = new Vector3 (aiPieceStartPosX + moveToXPos, 1.0f, aiPieceStartPosZ + moveToZPos +(float)moveToZPos); //TODO: HUH?
			rayRay.direction = (new Vector3 (0 , -1.0f, 0));
			Physics.Raycast (rayRay, out hit2);
			if (moveToXPos == 0) {
				transformPieceToKing(gameBoard.returnPlayerPiece(moveToXPos, moveToZPos), hit2.collider.gameObject);
			}
		}

		private void getAIQueueMoves() {
			float i = 0, j = 0;
			Ray aiRay = new Ray();
			while (j < 8) {
				while(i < 8) {
					aiRay.origin = (new Vector3 (i, 1.0f, j));
					aiRay.direction = (new Vector3 (0 , -1.0f, 0));
					if ((Physics.Raycast (aiRay, out hit) && hit.collider.tag.Contains("Player2"))) {
						if(!moveableAiPieces.Contains(hit.transform.position.x.ToString() + "," + hit.transform.position.z.ToString ())) {
							if(logicController.canMove((int)hit.transform.position.x, (int)hit.transform.position.z, gameBoard)) {
								moveableAiPieces.Enqueue(hit.transform.position.x.ToString() + "," + hit.transform.position.z.ToString ());
							}
						}
					}
					i++;
				}
				j++;
				i = 0;
			}
		}

		//SHOULD BE ABLE TO USEHIT COLLIDER POSITIONS NOW THAT IT HAS BEEN IDENTIFIED THAT IT WAS INTERACTION PIECE THAT WAS CAUSING THE ISSUE
		private void AIMove() {
			int i = 0, j = 0;
			System.Threading.Thread.Sleep(500);
			Ray aiRay = new Ray();
			while (j < 8) {
				while(i < 8) {
					aiRay.origin = (new Vector3 (i, 1.0f, j));
					aiRay.direction = (new Vector3 (0 , -1.0f, 0));
					if ((Physics.Raycast (aiRay, out hit) && hit.collider.tag.Contains("Player2"))) {
						if (logicController.canMoveUpAndRight ((int)hit.transform.position.x, (int)hit.transform.position.z, gameBoard)) {
							moveUpAndRight (i, j, hit.collider.gameObject);
							hit.collider.gameObject.transform.position = new Vector3 (hit.transform.position.x - 1, 0.1f, hit.transform.position.z + 1);
							goto Found;
						} else if(logicController.canMoveUpAndLeft ((int)hit.transform.position.x, (int)hit.transform.position.z, gameBoard)) {
							moveUpAndLeft (i, j, hit.collider.gameObject);
							hit.collider.gameObject.transform.position = (new Vector3 ((int)hit.transform.position.x - 1, 0.1f, (int)hit.transform.position.z - 1));
							goto Found;
						} else if(logicController.canMoveDownAndRight ((int)hit.transform.position.x, (int)hit.transform.position.z, gameBoard)) {
							moveDownAndRight(i, j, hit.collider.gameObject);
							hit.collider.gameObject.transform.position = (new Vector3 ((int)hit.transform.position.x + 1, 0.1f, (int)hit.transform.position.z + 1));
							goto Found;
						} else if(logicController.canMoveDownAndLeft ((int)hit.transform.position.x, (int)hit.transform.position.z, gameBoard)) {
							moveDownAndLeft (i, j, hit.collider.gameObject);
							hit.collider.gameObject.transform.position = (new Vector3 ((int)hit.transform.position.x + 1, 0.1f, (int)hit.transform.position.z - 1));
							goto Found;
						}
					}
					i++;
				}
				j++;
				i = 0;
			}
			Found: ;
			hit = new RaycastHit();
		}

		//AI MOVE KING TRANSFORM NEEDS FIXING
		private void AIQueueMove() {
			System.Threading.Thread.Sleep(500);
			Ray aiRay = new Ray();
			while(true) {
				String s = moveableAiPieces.Dequeue ().ToString();
				String[] positions = s.Split(',');
				aiRay.origin = (new Vector3 (Int32.Parse(positions[0]), 1.0f, Int32.Parse(positions[1])));
				aiRay.direction = (new Vector3 (0 , -1.0f, 0));
				if ((Physics.Raycast (aiRay, out hit) && hit.collider.tag.Contains("Player2"))) {
					if (logicController.canMoveUpAndRight ((int)hit.transform.position.x, (int)hit.transform.position.z, gameBoard)) {
						moveUpAndRight (Int32.Parse(positions[0]), Int32.Parse(positions[1]), hit.collider.gameObject);
						hit.collider.gameObject.transform.position = new Vector3 (hit.transform.position.x - 1, 0.1f, hit.transform.position.z + 1);
						break;
					} else if (logicController.canMoveUpAndLeft ((int)hit.transform.position.x, (int)hit.transform.position.z, gameBoard)) {
						moveUpAndLeft (Int32.Parse(positions[0]), Int32.Parse(positions[1]), hit.collider.gameObject);
						hit.collider.gameObject.transform.position = (new Vector3 ((int)hit.transform.position.x - 1, 0.1f, (int)hit.transform.position.z - 1));
						break;
					} else if (logicController.canMoveDownAndRight ((int)hit.transform.position.x, (int)hit.transform.position.z, gameBoard)) {
						moveDownAndRight (Int32.Parse(positions[0]), Int32.Parse(positions[1]), hit.collider.gameObject);
						hit.collider.gameObject.transform.position = (new Vector3 ((int)hit.transform.position.x + 1, 0.1f, (int)hit.transform.position.z + 1));
						break;
					} else if (logicController.canMoveDownAndLeft ((int)hit.transform.position.x, (int)hit.transform.position.z, gameBoard)) {
						moveDownAndLeft (Int32.Parse(positions[0]), Int32.Parse(positions[1]), hit.collider.gameObject);
						hit.collider.gameObject.transform.position = (new Vector3 ((int)hit.transform.position.x + 1, 0.1f, (int)hit.transform.position.z - 1));
						break;
					}
				}	
			}
			hit = new RaycastHit();
		}

		private bool canMove (int x, int y) {
			return (logicController.canMoveDownAndLeft (x, y, gameBoard) || logicController.canMoveDownAndRight (x, y, gameBoard) || logicController.canMoveUpAndLeft (x, y, gameBoard) || logicController.canMoveUpAndRight (x, y, gameBoard));
		}

		private bool canTake (int x, int y) {
			return (logicController.canTakeUpAndLeft (x, y, gameBoard) || logicController.canTakeUpAndRight (x, y, gameBoard) || logicController.canTakeDownAndLeft (x, y, gameBoard) || logicController.canTakeDownAndRight (x, y, gameBoard));
		}

		private void move (int tempx, int tempz) {
			interactionPiece.transform.position = new Vector3(startPosX, 0.1f, startPosZ);
			if ((tempx > startPosX) && ((tempz - startPosZ) < 1.5) && ((tempz - startPosZ) > 0) && (tempx - startPosX > 0) && (tempx - startPosX < 1.5) && logicController.canMoveDownAndRight (startPosX, startPosZ, gameBoard)) {
				moveDownAndRight(startPosX, startPosZ, interactionPiece);
				interactionPiece.transform.position = new Vector3 (tempx, 0.1f, tempz);
			} else if ((tempx > startPosX) && ((tempz - startPosZ) < 0) && ((tempz - startPosZ) > -1.5) && (tempx - startPosX > 0) && (tempx - startPosX < 1.5) && logicController.canMoveDownAndLeft (startPosX, startPosZ, gameBoard)) {
				moveDownAndLeft(startPosX, startPosZ, interactionPiece);
				interactionPiece.transform.position = new Vector3 (tempx, 0.1f, tempz);
			} else if ((tempx < startPosX) && ((tempz - startPosZ) < 1.5) && ((tempz - startPosZ) > 0) && ((tempz - startPosZ) < 1.5) && (tempx - startPosX < 0) && (tempx - startPosX > -1.5) && logicController.canMoveUpAndRight (startPosX, startPosZ, gameBoard)) {
				moveUpAndRight(startPosX, startPosZ, interactionPiece);
				interactionPiece.transform.position = new Vector3 (tempx, 0.1f, tempz);
			} else if ((tempx < startPosX) && ((tempz - startPosZ) < 0) && ((tempz - startPosZ) > -1.5) && ((tempz - startPosZ) < 0) && (tempx - startPosX < 0) && (tempx - startPosX > -1.5) && logicController.canMoveUpAndLeft (startPosX, startPosZ, gameBoard)) {
				moveUpAndLeft(startPosX, startPosZ, interactionPiece);
				interactionPiece.transform.position = new Vector3 (tempx, 0.1f, tempz);
			} else {
				interactionPiece.transform.position = new Vector3 (startPosX, 0.1f, startPosZ);
			}
		}

		private void take (float tempx, float tempz) {
			interactionPiece.transform.position = new Vector3 (startPosX, 0.1f, startPosZ);
			if ((tempx > startPosX) && ((tempz - startPosZ > 1.5) && (tempz - startPosZ < 2.5)) && logicController.canTakeDownAndRight (startPosX, startPosZ, gameBoard)) {
				takeDownAndRight (startPosX, startPosZ, interactionPiece);
				destroyPiece(startPosX, startPosZ, 2, 2);
			} else if ((tempx > startPosX) && (tempz - startPosZ < -1.5) && ((tempz - startPosZ) > -2.5) && logicController.canTakeDownAndLeft (startPosX, startPosZ, gameBoard)) {
				takeDownAndLeft (startPosX, startPosZ, interactionPiece);
				destroyPiece(startPosX, startPosZ, 2, -2);
			} else if ((tempx < startPosX) && ((tempz - startPosZ < -1.5) && (tempz - startPosZ > -2.5)) && (tempx - startPosX < -1) && logicController.canTakeUpAndLeft (startPosX, startPosZ, gameBoard)) {
				takeUpAndLeft (startPosX, startPosZ, interactionPiece);
				destroyPiece(startPosX, startPosZ, -2, -2);
			} else if ((tempx < startPosX) && ((tempz - startPosZ > 1.5) && (tempz - startPosZ < 2.5)) && (tempx - startPosX < -1) && logicController.canTakeUpAndRight (startPosX, startPosZ, gameBoard)) {
				takeUpAndRight (startPosX, startPosZ, interactionPiece);
				destroyPiece(startPosX, startPosZ, -2, 2);
			} else {
				interactionPiece.transform.position = new Vector3 (startPosX, 0.1f, startPosZ);
			}
		}

		private void destroyPiece(int aiStartPosX, int aiStartPosZ, int directionX, int directionZ) {
			interactionPiece.transform.position = new Vector3 (aiStartPosX + directionX, 0.1f, aiStartPosZ + directionZ);
			rayRay.origin = new Vector3 (aiStartPosX, 0.1f, aiStartPosZ);
			rayRay.direction = new Vector3 (directionX, 0.1f, directionZ);
			if (Physics.Raycast (rayRay, out hit)) {
				Destroy (hit.transform.gameObject);
			}
			if ((!canTake (aiStartPosX + directionX, aiStartPosZ + directionZ) && pieceChangedToKing == false)
				&& (((aiStartPosX + directionX) > -1) && ((aiStartPosX + directionX) < 8) 
					&& ((aiStartPosZ + directionZ) > -1) && ((aiStartPosZ + directionZ) < 8))) {
				changePlayer ();
			}
		}

		private void changePlayer () {
			playerNo = playerNo == 1 ? 2 : 1;
			pieceChangedToKing = false;
		}

		private void transformPieceToKing(PlayerPiece piece, GameObject playerPiece)  {
			piece.isKing = true;
			playerPiece.transform.localScale = new Vector3 (1.0f, 1.5f, 1.0f);
			changePlayer();
			pieceChangedToKing = true;
		}

		//x values are supplied as either -1 1, -2 or 2. Positive values will move the piece down the board visually negative will move up 
		//z values are supplied as either -1 1, -2 or 2. Positive values will move the piece rightvisually negative will move left 
		private void updateBoardOnTake(int x, int y, int enemyXPos, int enemyYPos, int emptyXPos, int emptyYPos, GameObject playerPiece) {
			PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
			gameBoard.removePiece (x, y);
			gameBoard.removePiece (x + enemyXPos, y + enemyYPos);
			gameBoard.AddPlayerPiece (piece, x + emptyXPos, y + emptyYPos);
			if (piece.isKing == false && (x + emptyXPos == 0 || x + emptyXPos == 7)) {
				piece.isKing = true;
				transformPieceToKing(piece, playerPiece);
			}
		}

		private void takeUpAndRight (int x, int y, GameObject playerPiece) {
			if (logicController.canTakeUpAndRight (x, y, gameBoard)) {
				updateBoardOnTake (x, y, -1, 1, -2, 2, playerPiece);
			}
		}

		private void takeUpAndLeft (int x, int y, GameObject playerPiece) {
			if (logicController.canTakeUpAndLeft (x, y, gameBoard)) {
				updateBoardOnTake (x, y, -1, -1, -2, -2, playerPiece);
			}
		}

		private void takeDownAndLeft(int x, int y, GameObject playerPiece) {
			if (logicController.canTakeDownAndLeft (x, y, gameBoard)) {
				updateBoardOnTake (x, y, 1, -1, 2, -2, playerPiece);
			}
		}

		private void takeDownAndRight (int x, int y, GameObject playerPiece) {
			if (logicController.canTakeDownAndRight (x, y, gameBoard)) {
				updateBoardOnTake (x, y, 1, 1, 2, 2, playerPiece);
			}
		}

		//x values are supplied as either -1 or 1. Positive values will move the piece down the board visually negative will move up 
		//z values are supplied as either -1 or 1. Positive values will move the piece rightvisually negative will move left 
		private void updateGameBoardOnMove (int x, int y, int moveToPosX, int moveToPosY, int playerNumber, GameObject playerPiece, PlayerPiece piece) {
			if (piece.playerNo == playerNumber || piece.isKing == true) {
				gameBoard.removePiece (x, y);
				gameBoard.AddPlayerPiece (piece, x + moveToPosX , y + moveToPosY);
				if(piece.isKing == false && (x + moveToPosX == 0 || x + moveToPosX == 7)) {
					piece.isKing = true;
					transformPieceToKing(piece, playerPiece);
					changePlayer ();
				}
				moved = false;
			}
		}

		private void moveDownAndRight (int x, int y, GameObject playerPiece) {
			if (logicController.canMoveDownAndRight (x, y, gameBoard)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
				moved = true;
				updateGameBoardOnMove(x, y, 1, 1, piece.playerNo, playerPiece, piece);
				if (x != 0 || x != 7 && moved == false) {
					changePlayer ();
				}
			}
		}

		private void moveDownAndLeft (int x, int y, GameObject playerPiece) {
			if (logicController.canMoveDownAndLeft (x, y, gameBoard)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
				moved = true;
				updateGameBoardOnMove(x, y, 1, -1, piece.playerNo, playerPiece, piece);
				if (x != 0 || x != 7 && moved == false) {
					changePlayer ();
				}
			}
		}

		private void moveUpAndRight (int x, int y, GameObject playerPiece) {
			if (logicController.canMoveUpAndRight (x, y, gameBoard)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
				moved = true;
				updateGameBoardOnMove (x, y, -1, 1, piece.playerNo, playerPiece, piece);
				if (x != 0 || x != 7 && moved == false) {
					changePlayer ();
				}
			}
		}

		private void moveUpAndLeft (int x, int y, GameObject playerPiece) {
			if (logicController.canMoveUpAndLeft (x, y, gameBoard)) {
				PlayerPiece piece = gameBoard.returnPlayerPiece (x, y);
				moved = true;
				updateGameBoardOnMove(x, y, -1, -1, piece.playerNo, playerPiece, piece);
				if (x != 0 || x != 7 && moved == false) {
					changePlayer ();
				}
			}
		}
	}
}
