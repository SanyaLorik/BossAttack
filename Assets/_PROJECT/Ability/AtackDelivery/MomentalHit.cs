using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MomentalHit : IHitDelivery {
    public void Deliver(Vector3 origin, IPlayer target, TargetType typeToAtack, List<IPlayer> targetList, IEffect effect) {
        effect.ApplyEffect(target);
    }
}