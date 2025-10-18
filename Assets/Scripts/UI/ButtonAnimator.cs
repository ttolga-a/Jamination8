using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI; // Bu satýrý eklemeyi unutma!

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator anim;
    [SerializeField] private Image highlighter;

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        anim.SetBool("isHighlighted", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.SetBool("isHighlighted", false);
    }

    public void CloseAnimOnClick()
    {
        anim.SetBool("isHighlighted", false);
    }
}