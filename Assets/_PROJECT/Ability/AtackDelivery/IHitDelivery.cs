using System.Collections.Generic;
using UnityEngine;

public interface IHitDelivery {
    void Deliver(Vector3 origin, IPlayer target, List<IPlayer> targetList, IEffect effect);
}