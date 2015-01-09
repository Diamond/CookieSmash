using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CookieScript : MonoBehaviour {
	public List<Sprite> sprites;
	public List<Sprite> highlightSprites;

	public int locationX = 0;
	public int locationY = 0;

	private int  _cookieChoice = 0;
	private bool _selected = false;

	private GameControllerScript _gcScript;

	private float _lastMouseX = 0.0f;
	private float _lastMouseY = 0.0f;

	private string[] _cookieNames;

	private const float SCROLL_THRESHOLD = 0.10f;
	private const float SMOOTHING        = 7.5f;

	void Start() {
		_cookieNames = new string[6]  { "Croissant", "Cupcake", "Danish", "Donut", "Macaroon", "SugarCookie" };
		_cookieChoice = Random.Range (0, sprites.Count);
		_gcScript = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameControllerScript>();
		Display ();
	}

	public void Display() {
		if (Selected) {
			this.GetComponent<SpriteRenderer>().sprite = highlightSprites[_cookieChoice];
		} else {
			this.GetComponent<SpriteRenderer>().sprite = sprites[_cookieChoice];
		}
	}

	public void OnMouseDown() {
		if (_gcScript.swapInProgress) return;
		_gcScript.DeselectAll();
		Selected = true;
		_lastMouseX = Input.mousePosition.x;
		_lastMouseY = Input.mousePosition.y;
	}

	public void OnMouseDrag() {
		if (!Selected) return;
		if (_gcScript.swapInProgress) return;

		float xdis = Mathf.Abs(Input.mousePosition.x - _lastMouseX);
		float ydis = Mathf.Abs (Input.mousePosition.y - _lastMouseY);
		if (xdis <= SCROLL_THRESHOLD && ydis <= SCROLL_THRESHOLD) return;

		if (xdis > ydis) {
			// Scroll Horizontally
			if (Input.mousePosition.x > _lastMouseX) {
				// Scroll right
				if (locationX >= 8 || !_gcScript.ValidSwap(locationX + 1, locationY)) return;
				DoSwap (locationX + 1, locationY);
			} else {
				// Scroll left
				if (locationX <= 0 || !_gcScript.ValidSwap(locationX - 1, locationY)) return;
				DoSwap (locationX - 1, locationY);
			}
		} else {
			// Scroll Vertically
			if (Input.mousePosition.y > _lastMouseY) {
				// Scroll up
				if (locationY >= 8 || !_gcScript.ValidSwap(locationX, locationY + 1)) return;
				DoSwap (locationX, locationY + 1);
			} else {
				// Scroll down
				if (locationY <= 0 || !_gcScript.ValidSwap(locationX, locationY - 1)) return;
				DoSwap (locationX, locationY - 1);
			}
		}
	}

	void DoSwap(int x, int y) {
		_gcScript.swapInProgress = true;
		GameObject other = _gcScript.GetObjAt(x, y);

		SwapTo (other, true);

		other.GetComponent<CookieScript>().SwapTo (this.gameObject, false);
		other.GetComponent<CookieScript>().locationX = locationX;
		other.GetComponent<CookieScript>().locationY = locationY;

		locationX = x;
		locationY = y;
	}
	
	public void SwapTo(GameObject other, bool onTop=true) {
		if (!onTop) {
			this.transform.position = new Vector3(transform.position.x, transform.position.y, 2.5f);
		}
		StartCoroutine(SwapPositions(transform.position, other.transform.position));
	}

	IEnumerator SwapPositions(Vector3 from, Vector3 target) {
		while (Vector3.Distance(transform.position, target) >= 0.01f) {
			transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * SMOOTHING);
			yield return null;
		}
		this.transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
		Selected = false;
		_gcScript.RedrawCookies();
		_gcScript.swapInProgress = false;
	}

	public bool Selected {
		get {
			return _selected;
		}
		set {
			_selected = value;
			Display ();
		}
	}

	public string ToString() {
		return Name () + " (" + locationX.ToString() + ", " + locationY.ToString() + ")";
	}

	public string Name() {
		return _cookieNames[_cookieChoice];
	}

	public void StopSwap() {
		StopAllCoroutines();
		this.transform.position = new Vector3(transform.position.x, transform.position.y, 0.0f);
		Selected = false;
		if (_gcScript) {
			_gcScript.swapInProgress = false;
		}
	}
}
