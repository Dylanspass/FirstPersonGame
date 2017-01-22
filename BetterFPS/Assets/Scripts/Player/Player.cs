using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool>{}

public class Player : NetworkBehaviour {

	[SerializeField] ToggleEvent onToggleShared;
	[SerializeField] ToggleEvent onToggleLocal;
	[SerializeField] ToggleEvent onToggleRemote;
	[SerializeField] float respawnTime = 5f;

	GameObject mainCamera;
	NetworkAnimator anim;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<NetworkAnimator>();
		mainCamera = Camera.main.gameObject;

		EnablePlayer();
	}

	void Update()
	{
		if(!isLocalPlayer)
		{
			return;
		}

		anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
		anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));
	}
	
	// Update is called once per frame
	void DisablePlayer()
	{

		if(isLocalPlayer)
		{
			mainCamera.SetActive(true);
		}

		onToggleShared.Invoke(false);

		if(isLocalPlayer)
		{
			onToggleLocal.Invoke(false);
		}
		else
		{
			onToggleRemote.Invoke(false);
		}
	}

	void EnablePlayer()
	{

		if(isLocalPlayer)
		{
			mainCamera.SetActive(false);
		}

		onToggleShared.Invoke(true);

		if(isLocalPlayer)
		{
			onToggleLocal.Invoke(true);
		}
		else
		{
			onToggleRemote.Invoke(true);
		}
	}

	public void Die()
	{
		DisablePlayer();

		Invoke("Respawn", respawnTime);

		//anim.SetTrigger("Died");
	}

	void Respawn()
	{
		if(isLocalPlayer)
		{
			Transform spawn = NetworkManager.singleton.GetStartPosition();
			transform.position = spawn.position;
			transform.rotation = spawn.rotation;

			anim.SetTrigger("Restart");
		}

		EnablePlayer();
	}
}
