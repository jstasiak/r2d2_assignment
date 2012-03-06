using UnityEngine;
using System.Collections;

public class playerAttack : MonoBehaviour {
	public GameObject spell;
	private Transform rightHand;
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown("space")){
			//the spawn position will be the players right hand
			//plus the radius of the hand and the diameter of the spell
			Vector3 spawnPos = rightHand.transform.position + 
				new Vector3(0, 0, rightHand.renderer.bounds.extents.z + spell.renderer.bounds.size.z);
			//instantiate the spell at given position, facing the players forward direction
			GameObject temp = (GameObject)GameObject.Instantiate(spell, 
				spawnPos, transform.rotation);
			//set the spell type
			temp.SendMessage("setSpellType", 
				offensiveSpellProperties.spellTypeEnum.fire);
		}
	}
	
	void Start(){
		//link to the right hand
		rightHand = transform.Find("Model/rightHand");
		if(rightHand == null){
			Debug.Log("Could not find right hand");
		}
	}
}
