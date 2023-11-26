using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private float moveSpeed;

    private Item item;
    private int count;

    public void Initialize(Item item, int count)
    {
        this.item = item;
        sr.sprite = item.image;
        this.count = count;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(MoveAndCollect(other.transform));

        }
    }

    public IEnumerator MoveAndCollect(Transform target)
    {
        Destroy(boxCollider);

        while (transform.position != target.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            yield return 0;
        }

        //add item to player inventory
        int itemsNotAdded = GameManager.Instance.playerInventory.AddItem(item, count);


        if (itemsNotAdded == 0)
        {
            Destroy(gameObject);
        }
        else
        {
            count = itemsNotAdded;
        }

    }

}
