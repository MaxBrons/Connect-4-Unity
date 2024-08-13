
using System;
using System.Threading;
using System.Threading.Tasks;
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

        /// <summary>
        /// Wait for an object to become valid or until the timeout is reached.
        /// </summary>
        /// <param name="obj">The object to check for validity</param>
        /// <param name="msTimeout">The max time to wait.</param>
        /// <returns></returns>
        public static async Task<bool> WaitForValidObject(object obj, int msTimeout = 500)
        {
            CancellationTokenSource cancelationToken = new(msTimeout);
            TaskCompletionSource<bool> result = new();

            await Task.Run(async () =>
            {
                while (!cancelationToken.IsCancellationRequested)
                {
                    if (obj != null)
                        return;

                    await Task.Delay(Math.Min(100, msTimeout));
                }
            }, cancelationToken.Token);

            if (obj != null)
                return true;

            return false;
        }
    }
}