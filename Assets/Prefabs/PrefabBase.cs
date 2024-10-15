using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabBase : MonoBehaviour
{
    /* Public member variables */
    public Sprite sprite;
    public SpriteRenderer spriteRenderer;
    public GameController gameController;

    /* Grid Cell location */
    public Vector3Int cellGridPosition;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;

        GameObject gameControllerObj = GameObject.FindWithTag("GameController");
        gameController = gameControllerObj.GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
