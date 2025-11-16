using UnityEngine;

namespace Gameplay.AI
{
    public interface IGuardAlertable
    {
        void OnCryAlert(Vector3 sourcePosition, float radius);
    }
}