using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabBase : MonoBehaviour
{
    /* Public member variables */
    public Sprite sprite;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
