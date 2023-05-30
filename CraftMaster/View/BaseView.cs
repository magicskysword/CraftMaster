using System;
using System.Collections.Generic;
using CraftMaster.Builder;
using CraftMaster.Reference;
using Kingmaker.Items;

namespace CraftMaster.View;

public abstract class BaseView
{
    public void Reset()
    {
        OnReset();
    }

    protected abstract void OnReset();
}