using UnityEngine;
using System.Collections;

public class offensiveSpellBehaviour : MonoBehaviour {
	public float movementSpeed, maxDistance = 100.0f;
	private int damageAmt;
	private Vector3 startPos;
	private SpellProperties properties;
	private Transform parent;
	
	// Use this for initialization
	void Start() {
		parent = transform.parent;
		//tag = "spell";
		//start position is used to destroy object after a certain distance
		startPos = parent.transform.position;
		//set velocity in forward direction
		//rigidbody.velocity = transform.forward * movementSpeed;
		properties = parent.GetComponent<SpellProperties>();
		damageAmt = properties.spellDamage;
	}
	
	//if it collides with player, apply damage and destroy it
	void OnCollisionEnter(Collision col) {
		this.InteractWithCollider(col.collider);
	}

	void OnTriggerEnter(Collider col) {
		this.InteractWithCollider(col);
	}

	private void InteractWithCollider(Collider col) {
		if(col.name.ToLower().Contains("player")){
			col.SendMessage("Damage", damageAmt);
		}else if(col.name.ToLower().Contains("spell")){
			col.BroadcastMessage("Damage", properties, SendMessageOptions.DontRequireReceiver);
		}
		/*var properties = collider.GetComponent<PlayerProperties>();
		if(properties != null) {
			properties.Damage(damageAmt);
		}*/
	
		Destroy(gameObject);
	}
	
	//if its gone too far without hitting the player, destroy it
	void Update(){
		parent.transform.Translate(parent.transform.forward * movementSpeed * Time.deltaTime);
		
		if(Vector3.Distance(startPos, parent.transform.position) > maxDistance)
			Destroy(parent.gameObject);
	}
}
