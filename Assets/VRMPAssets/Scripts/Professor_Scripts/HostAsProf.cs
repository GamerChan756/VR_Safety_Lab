
using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class HostAsProf : NetworkBehaviour
{

    //[SerializeField] private GameObject hostControlPanel;

    public bool isProfessor = false;

    [SerializeField] GameObject studentOptions;

    [SerializeField] GameObject professorOptions;


    private void Start()
    {
        studentOptions.SetActive(true);
        professorOptions.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        // Use IsHost if using a older version; Use IsSessionOwner for Unity 6
        if (IsSessionOwner) 
        {
            isProfessor = true;
            Debug.Log("I am host");
            professorOptions.SetActive(true);
            studentOptions.SetActive(false);

        }
        else
        {
            isProfessor = false;
            studentOptions.SetActive(true);
            professorOptions.SetActive(false);
        }
    }

}
