using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpAndDump : MonoBehaviour
{


    public float speed = 2f;

    public AudioSource audioSource;
    public AudioClip audioClip;

    bool isCoillided = false;


    private void Start()
    {
        gameObject.GetComponent<SpriteRenderer>().material = GameManager.Instance.nftMaterialArrayList[GameManager.Instance.web3Manager.pumpAndDumpNFTCurrentLevel - 1];
    }

    void Update()
    {

        transform.position = Vector3.MoveTowards(transform.position, new Vector3(-12f, transform.position.y, 0f), speed * Time.deltaTime);

        if (transform.position.x <= -12f && !isCoillided)
        {
            Destroy(gameObject);
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {

            isCoillided = true;

            float resetTime = 5 * GameManager.Instance.nftMultiplierList[GameManager.Instance.web3Manager.pumpAndDumpNFTCurrentLevel - 1];


            other.GetComponent<PlayerController>().isPumpAndDump = true;
            other.GetComponent<PlayerController>().isPumping = true;
            //StopCoroutine(other.GetComponent<PlayerController>().ResetPumpAndDump());
            //other.GetComponent<PlayerController>().StartCoroutine(other.GetComponent<PlayerController>().ResetPumpAndDump());

            StartCoroutine(GameManager.Instance.playerController.ResetPumpAndDump(resetTime));


            audioSource.PlayOneShot(audioClip);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;


            GameManager.Instance.web3Manager.pumpAndDumpNFTCount += 1;


            Destroy(gameObject, (2*resetTime) + 1);

        }

    }


}
