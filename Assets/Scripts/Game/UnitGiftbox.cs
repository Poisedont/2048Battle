using UnityEngine;

public class UnitGiftbox : MonoBehaviour
{
    public int GridPos { get; set; }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("UnitOpp"))
        {
            GridManager.Instance.CollectUnitGift(GridPos, false);
            DestroyDelay(0.2f, false);
        }
        else if (other.gameObject.CompareTag("UnitAlly"))
        {
            GridManager.Instance.CollectUnitGift(GridPos, true);
            DestroyDelay(0.2f, true);
        }
    }

    void DestroyDelay(float delay, bool isOpen)
    {
        //show effect
        if (isOpen)
        {
            gameObject.LeanScale(Vector3.one * 3, delay).setEaseInBack(); // to ra
        }
        else
        {
            gameObject.LeanScale(Vector3.one / 3, delay).setEaseInBack(); // nhỏ lại
        }
        gameObject.LeanAlpha(0, delay).setEaseInCirc();
        Destroy(gameObject, delay);
    }
}