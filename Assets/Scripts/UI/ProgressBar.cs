﻿using UnityEngine;
using PickAR.Managers;

namespace PickAR.UI {

    /// <summary>
    /// Displays the user's progress through the job.
    /// </summary>
    class ProgressBar : MonoBehaviour {
        
        /// <summary> The right anchor of the progress bar at full progress. </summary>
        private float anchorMaxX;
        /// <summary> The progress bar transform. </summary>
        private RectTransform rectTransform;

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            rectTransform = GetComponent<RectTransform>();
            anchorMaxX = rectTransform.anchorMax.x;
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        private void Update() {
            Vector2 newAnchor = rectTransform.anchorMax;
            float anchorMinX = rectTransform.anchorMin.x;
            newAnchor.x = anchorMinX + (anchorMaxX - anchorMinX) * JobManager.instance.progressFraction;
            rectTransform.anchorMax = newAnchor;
        }
    }
}