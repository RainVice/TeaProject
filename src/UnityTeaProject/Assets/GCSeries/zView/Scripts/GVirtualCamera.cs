using System;
using UnityEngine;

namespace GCSeries.zView
{
    public abstract class GVirtualCamera : MonoBehaviour
    {
        /// <summary>
        /// Set up the VirtualCamera.
        /// </summary>
        /// 
        /// <remarks>
        /// Performs any setup related operations specific to the mode the 
        /// VirtualCamera is associated with. This method will be called once
        /// per ModeSetupPhase when the specified connection has transitioned
        /// to the ConnectionState.ModeSetup state.
        /// </remarks>
        /// 
        /// <param name="zView">
        /// A reference to the zView API Monobehaviour script.
        /// </param>
        /// <param name="connection">
        /// The connection to perform the VirtualCamera's setup phase for.
        /// </param>
        /// <param name="phase">
        /// The mode setup phase for the specified connection.
        /// </param>
        public abstract void SetUp(GView zView, IntPtr connection, GView.ModeSetupPhase phase);

        /// <summary>
        /// Tear down the VirtualCamera.
        /// </summary>
        /// 
        /// <remarks>
        /// Performs any cleanup related operations specific to the mode the
        /// VirtualCamera is associated with.
        /// </remarks>
        public abstract void TearDown();

        /// <summary>
        /// Render a single frame.
        /// </summary>
        /// 
        /// <param name="zView">
        /// A reference to the zView API Monobehaviour script.
        /// </param>
        /// <param name="connection">
        /// The connection to perform the VirtualCamera's render for.
        /// </param>
        /// <param name="receivedFrame">
        /// The received frame from the specified connection.
        /// </param>
        public abstract void Render(GView zView, IntPtr connection, IntPtr receivedFrame);

        /// <summary>
        /// Get the native texture pointer associated with the VirtualCamera's 
        /// offscreen render texture.
        /// </summary>
        /// 
        /// <returns>
        /// The native texture pointer associated with the VirtualCamera's offscreen
        /// render texture.
        /// </returns>
        public abstract IntPtr[] GetNativeTexturePtr(out int count);

        /// <summary>
        /// Get VirtualCamera's offscreen render texture.
        /// </summary>
        /// <returns>VirtualCamera's render texture.</returns>
        public abstract RenderTexture[] GetRenderTexture(out int count);
    }
}