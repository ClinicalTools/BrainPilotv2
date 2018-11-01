using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInteractState : SelectableStateAction
{

    public UISelectableElement uiElement;

    public override void Activate()
    {
        base.Activate();
    }

    public override void Deactivate()
    {
        base.Deactivate();
    }

    public override void Load()
    {
        base.Load();

        uiElement = (UISelectableElement)element;
        if (uiElement == null)
        {
            Debug.LogWarning("A UI InteractState component has been passed an ordinary non-UI selectable! It will remove itself but we should try to avoid this because it's a performance hit. Object: " + element.transform.name);
            this.Remove();
        }
    }

    public override void Remove()
    {


        base.Remove();

    }


}
