using UnityEngine;
using System;

public class PlayerProperties : MonoBehaviour {
    [SerializeField]
	private float health;
	private playerUI myPlayerUI;
	
	public float Health {
		get { return this.health; }
		private set {
			this.health = value;
			
			//check if dead and act accordingly
			if(this.health <= 0) {
				this.playerDeath();
			}
		}
	}

	public float minHealth = 0.0f, maxHealth = 100.0f, startHealth = 100.0f;
	
	public event EventHandler Death;
	
	private void OnDeath() {
		var handler = this.Death;
		if(handler != null) {
			handler(this, EventArgs.Empty);
		}
	}


    // Use this for initialization
	void Start () {
		//init health
		this.Health = startHealth;
		myPlayerUI = gameObject.transform.Find("Main Camera").GetComponent<playerUI>();
	}
	
	public void Damage(SpellProperties properties){
		//aplly damage to health
		float resistanceMultiplier = DefensiveSpellBehaviour.spellResistanceMultiplier(gameObject, properties.spellElem);
		this.Health -= resistanceMultiplier * properties.spellDamage;
		//Sets a bool in the players UI to draw the damage indicator
		myPlayerUI.bDrawDamage = true;
	}
	
	public void Heal(float hp) {
		this.Health = Math.Min(this.Health + hp, this.maxHealth);
	}
	
	private void playerDeath(){
		this.OnDeath();
	}

    public void Update() {
       
    }
}
