using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class playerAttack : MonoBehaviour {
	public GameObject spell;
	private Transform playerCam;
	private Transform rightHand;
	private Vector3 spawnPos;
	
	private SpellElement elem = SpellElement.invalid;
	private SpellType type = SpellType.invalid;
	
	private playerUI ui;
	
	ArrayList points = new ArrayList();
	Vector3 currentPosition;
	Vector3 initialPosition;
	Event currentEvent;
	public AudioClip attackSound;
	
	void Start(){
		//link to the right hand
		rightHand = transform.FindChild("model/rightHand");
		playerCam = transform.FindChild("Main Camera");
		if(rightHand == null || playerCam == null){
			Debug.Log("Could not find components");
		}
		currentEvent = null;
		
		ui = gameObject.GetComponentInChildren<playerUI>();
	}

	float GetQuadDamageMultiplier() {
		var quad = this.gameObject.GetComponentInChildren<QuadDamage>();
		return quad != null ? quad.DamageMultipier : 1.0f;
	}
	
	void OnGUI() {
        currentEvent = Event.current;
    }
	
	// Update is called once per frame
	void Update () {
		if(currentEvent == null){
		}else if (currentEvent.type == EventType.MouseDown) {
			initialPosition = -Input.mousePosition;
			points.Clear();
			points.Add(initialPosition);
		} else if(currentEvent.type == EventType.MouseDrag) {
			currentPosition = -Input.mousePosition;
			points.Add(currentPosition);
		}
		
		if(gameObject.name == "player2"){
			if(currentEvent != null && currentEvent.type == EventType.MouseUp) {				
				var recognizer = gameObject.AddComponent<HMMRecognizer>();
				var mouseGestures = new MouseGestures();
				var gesture = mouseGestures.GetGestureFromPoints(points);
				
//				foreach(int g in gesture.HmmDirections)
//					Debug.Log(g);
				try {
					var hmm = recognizer.hmmEvalute(gesture.HmmDirections);
					Debug.Log(string.Format("Recognized gesture: {0}", hmm));
					
					gameObject.SendMessage("handleGesture", hmm, SendMessageOptions.DontRequireReceiver);
				}
				catch (UnityException e) {
					Debug.Log(e);
				}
				
				GameObject.Destroy(recognizer);
			}
		}
	}
	
	//set spell properties based off passed gesture
	void handleGesture(GestureEnum gest){
		if(gest == GestureEnum.V_DOWN){
			elem = SpellElement.earth;
			type = SpellType.invalid;
		}else if(gest == GestureEnum.V_UP){
			elem = SpellElement.fire;
			type = SpellType.invalid;
		}else if(gest == GestureEnum.SQUARE){
			elem = SpellElement.water;
			type = SpellType.invalid;
		}else if(gest == GestureEnum.HORIZONTAL_LINE)
			type = SpellType.offensive;
		else if(gest == GestureEnum.VERTICAL_LINE)
			type = SpellType.defensive;
		
		if(elem != SpellElement.invalid){
			ui.SetEquippedSpell(elem);
			if(type != SpellType.invalid)
				castSpell();
		}
	}


	void castSpell() {
		Dictionary<int, object> spellParams = new Dictionary<int, object>();
		Rune selectedSpell;
		Inventory playerInv = gameObject.GetComponent<Inventory>();
		
		print(elem + " " + type);
		
		if(elem == SpellElement.invalid || type == SpellType.invalid){
			clearSpellData();
			return;
		}
		
		//if we have a run for a spell, remove it		
		RuneType runeToCheck;
		if(elem == SpellElement.earth)
			runeToCheck = RuneType.Earth;
		else if(elem == SpellElement.fire)
			runeToCheck = RuneType.Fire;
		else
			runeToCheck = RuneType.Water;
		
		if((selectedSpell = playerInv.GetRune(runeToCheck)) == null){
			clearSpellData();
			return;
		}
		playerInv.Remove(selectedSpell);
		
		Debug.Log(DefensiveSpellBehaviour.spellDamageMultiplier(gameObject, elem) );
		
		spellParams[(int)SpellParameter.element] = elem;
		spellParams[(int)SpellParameter.type] = type;
		spellParams[(int)SpellParameter.damageMultiplier] = 
				DefensiveSpellBehaviour.spellDamageMultiplier(gameObject, elem) 
				* GetQuadDamageMultiplier();
		
		
		if(type == SpellType.offensive)
			spawnPos = rightHand.position + transform.forward * (rightHand.renderer.bounds.extents.z + spell.renderer.bounds.extents.z);
		else
			spawnPos = transform.position;
			
		spellParams[(int)SpellParameter.parent] = transform;
		
		//instantiate the spell at given position, facing the players forward direction
		//GameObject tempSpell = Instantiate(spell, spawnPos, transform.rotation) as GameObject;
		//cast spell
		GameObject tempSpell = Instantiate(spell, spawnPos, Quaternion.Euler(playerCam.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, 0)) as GameObject;
//		tempSpell.transform.parent = transform;
		tempSpell.SendMessage("setSpellProperties", spellParams);
		
		if(this.attackSound != null) {
			AudioUtil.PlaySound(this.attackSound, this.gameObject.transform.position);
		}
		
		clearSpellData();
	}
	
	//spell data
	void clearSpellData(){
		elem = SpellElement.invalid;
		type = SpellType.invalid;
	}
}
