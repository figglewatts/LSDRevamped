using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Torii.UI
{
    public interface IPopulator<TContained>
    {
        void Populate(List<TContained> with);
        void Clear();
    }
}