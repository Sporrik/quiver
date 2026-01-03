using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Audio;

[RequireComponent(typeof(Selectable))]
public class UIHoverClickSfx : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private SoundID _hoverId = SoundID.Sfx_UIHover;
    [SerializeField] private SoundID _clickId = SoundID.Sfx_UIClick;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        AudioBus.Sfx(_hoverId);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsInteractable()) return;
        AudioBus.Sfx(_clickId);
    }

    private bool IsInteractable()
    {
        var selectable = GetComponent<Selectable>();
        return selectable == null || selectable.interactable;
    }
}