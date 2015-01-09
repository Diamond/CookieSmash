using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameControllerScript : MonoBehaviour {

	public GameObject cookie;
	public GameObject tile;

	public bool swapInProgress = false;

	private List<GameObject> _tiles;
	private List<GameObject> _cookies;

	//private int[,] _cookieMap;

	public const int TILES_X = 9;
	public const int TILES_Y = 9;

	private Level _level;

	// Use this for initialization
	void Start () {
		_tiles = new List<GameObject>();
		_cookies = new List<GameObject>();
		_level = new Level();

		DrawObjects (tile, _tiles, 5.0f);
		DrawObjects (cookie, _cookies);

		RedrawCookies();

		Input.simulateMouseWithTouches = true;
	}

	void DrawObjects(GameObject obj, List<GameObject> objList, float zpos=0.0f) {
		Vector3 size  = tile.GetComponent<SpriteRenderer>().bounds.size;
		float offsetX = ((size.x * (TILES_X - 1)) / 2);
		float offsetY = ((size.y * (TILES_Y - 1)) / 2);
		
		for (int y = 0; y < TILES_Y; y++) {
			for (int x = 0; x < TILES_X; x++) {
				if (!_level.ValidAt(x, y)) continue; 
				float tilex   = (size.x * x) - offsetX;
				float tiley   = (size.y * y) - offsetY;
				GameObject newObj = Instantiate(obj) as GameObject;
				newObj.transform.position = new Vector3(tilex, tiley, zpos);
				objList.Add(newObj);

				CookieScript cs = obj.GetComponent<CookieScript>();
				if (cs) {
					cs.locationX = x;
					cs.locationY = y;
				}
			}
		}
	}

	public void DeselectAll() {
		foreach (GameObject c in _cookies) {
			c.GetComponent<CookieScript>().Selected = false;
		}
	}

	public GameObject GetObjAt(int x, int y) {
		foreach (GameObject c in _cookies) {
			CookieScript cs = c.GetComponent<CookieScript>();
			if (cs) {
				if (cs.locationX == x && cs.locationY == y) {
					return c;
				} 
			}
		}
		return null;
	}

	public void RedrawCookies() {
		Vector3 size  = tile.GetComponent<SpriteRenderer>().bounds.size;
		float offsetX = ((size.x * (TILES_X - 1)) / 2);
		float offsetY = ((size.y * (TILES_Y - 1)) / 2);

		foreach (GameObject c in _cookies) {
			CookieScript cs = c.GetComponent<CookieScript>();
			if (cs) {
				float tilex   = (size.x * cs.locationX) - offsetX;
				float tiley   = (size.y * cs.locationY) - offsetY;
				cs.transform.position =  new Vector3(tilex, tiley, 0.0f);
				cs.StopSwap();
			}
		}
	}

	public bool ValidSwap(int x, int y) {
		return (_level.ValidAt(x, y) && x >= 0 && x < TILES_X && y >= 0 && y < TILES_Y);
	}
}

class Level {
	private int[,] _tiles;
	
	public const int EMPTY = 0;
	public const int TILE  = 1;
	
	public Level() {
		_tiles = new int[GameControllerScript.TILES_X, GameControllerScript.TILES_Y] {
			{1, 1, 1, 1, 0, 1, 1, 1, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
			{0, 0, 1, 1, 1, 1, 1, 0, 0},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
			{1, 1, 1, 1, 1, 1, 1, 1, 1},
			{1, 1, 1, 1, 0, 1, 1, 1, 1},
		};
	}
	
	public bool ValidAt(int x, int y) {
		if (_tiles[y, x] == TILE) return true;
		return false;
	}
}
