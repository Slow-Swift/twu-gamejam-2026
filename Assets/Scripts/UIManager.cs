using UnityEngine.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image trashAmountSlider;
    [SerializeField] Image heldTrashSlider;

    [SerializeField] TrashPile trashPile;
    [SerializeField] Player player;

    void Update()
    {
        trashAmountSlider.fillAmount = trashPile.trashAmount / 100f;
        heldTrashSlider.fillAmount = (trashPile.trashAmount + player.heldTrash) / 100f;
    }
}
