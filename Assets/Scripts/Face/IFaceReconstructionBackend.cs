using System;
using UnityEngine;

namespace BellaVirtualLife.Face
{
    [Serializable]
    public class FaceReconstructionResult
    {
        public Vector3[] identityParameters;
        public Texture2D projectedFaceTexture;
        public string backendName;
    }

    /// <summary>
    /// Implement this interface with a licensed offline face reconstruction model.
    /// A single image does not contain complete 3D facial information.
    /// </summary>
    public interface IFaceReconstructionBackend
    {
        bool IsReady { get; }
        void Reconstruct(Texture2D source, Action<FaceReconstructionResult, string> completed);
    }
}
