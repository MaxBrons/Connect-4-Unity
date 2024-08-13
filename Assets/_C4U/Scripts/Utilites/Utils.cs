
using UnityEngine;

namespace C4U.Utilities
{
    /// <summary>
    /// A class for storing utility methods.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Fire a raycast from a screen point and return the hit info.
        /// </summary>
        /// <param name="screenPosition">The screen position to fire the ray from.</param>
        /// <param name="hit">The resulting hit info from the raycast.</param>
        /// <returns></returns>
        public static bool RaycastFromScreen(this Camera cam, Vector2 screenPosition, out RaycastHit hit)
        {
            Ray ray = cam.ScreenPointToRay(screenPosition);

            Physics.Raycast(ray.origin, ray.direction, out hit);

            if (!hit.transform)
                return false;

            return true;
        }
    }
}