﻿using UnityEngine;
using System.Collections.Generic;
using PickAR.Navigation;
using PickAR.Objects;

namespace PickAR.Managers {
    /// <summary>
    /// Keeps track of the user's current order picking job.
    /// </summary>
    public class JobManager : MonoBehaviour {
        
        /// <summary> The items targeted by the user. </summary>
        private List<Item> targetItems;
        /// <summary> The total number of items in the job. </summary>
        private int totalItems;
        /// <summary> The item to pick up next. </summary>
        private Item currentItem;
        /// <summary> The object used to highlight the current object. </summary>
        [SerializeField]
        [Tooltip("The object used to highlight the current object.")]
        private GameObject itemHighlight;
        /// <summary> The user of the application. </summary>
        [SerializeField]
        [Tooltip("The user of the application.")]
        private User user;
        /// <summary> The point where the user started at. </summary>
        public Vector3 startPoint {
            get;
            private set;
        }
        /// <summary> Whether a job is currently in progress. </summary>
        public bool jobActive {
            get;
            private set;
        }
        /// <summary> The percent of the job that is completed. </summary>
        public float progressFraction {
            get;
            private set;
        }
        /// <summary> The aisle number of the current item. </summary>
        public string itemAisle {
            get { return currentItem == null ? "" : currentItem.aisleNumber; }
        }
        /// <summary> Whether the user has finished the job and is heading back to the start point. </summary>
        public bool jobFinished {
            get { return jobActive && remainingItems == 0; }
        }
        /// <summary> The number of items left to collect. </summary>
        public int remainingItems {
            get { return targetItems.Count; }
        }

        /// <summary> Debug items to be targeted by the user. </summary>
        [SerializeField]
        [Tooltip("Debug items to be targeted by the user.")]
        private Item[] targetArray;

        /// <summary> The singleton instance of the object. </summary>
        public static JobManager instance {
            get;
            private set;
        }

        /// <summary>
        /// Initializes the singleton instance of the object.
        /// </summary>
        private void Awake() {
            instance = this;
        }

        /// <summary>
        /// Initializes the object.
        /// </summary>
        private void Start() {
            targetItems = new List<Item>();
            progressFraction = 1;
            CreateJob(targetArray);
        }

        /// <summary>
        /// Updates the object.
        /// </summary>
        private void Update() {
            if (jobActive) {
                if (jobFinished) {
                    SetCurrentPoint(startPoint);
                } else {
                    PickNextItem();
                }
            }
        }

        /// <summary>
        /// Creates a new job for the user to complete.
        /// </summary>
        /// <param name="targetArray">Items that need to be collected to complete the job.</param>
        public void CreateJob(Item[] targetArray) {
            targetItems.Clear();
            foreach (Item item in targetArray) {
                targetItems.Add(item);
            }
            totalItems = targetArray.Length;
            startPoint = user.transform.position;
            SetJobActive(true);
            progressFraction = 0;
        }

        /// <summary>
        /// Attempts to collect an item. Removes the item if it is contained in the job.
        /// </summary>
        /// <param name="itemID">The ID of the item being collected.</param>
        public void SelectItem(int itemID) {
            foreach (Item item in targetItems) {
                if (item.itemID == itemID) {
                    RemoveItem(item);
                    return;
                }
            }
            Debug.Log("Wrong item!");
        }

        /// <summary>
        /// Removes an item from the job when it is collected.
        /// </summary>
        /// <param name="item">The item that was collected.</param>
        public void RemoveItem(Item item) {
            targetItems.Remove(item);
            if (targetItems.Count == 0) {
                progressFraction = 1;
            } else {
                progressFraction = (totalItems - remainingItems) / (float) totalItems;
            }
        }

        /// <summary>
        /// Does something when the job is complete.
        /// </summary>
        public void CompleteJob() {
            SetJobActive(false);
            Navigator.instance.RemoveLines();
        }

        /// <summary>
        /// Gets the next item to go to.
        /// </summary>
        /// <returns>The next item to go to.</returns>
        public void PickNextItem() {
            Item closestItem = null;
            float closestDistance = Mathf.Infinity;
            foreach (Item item in targetItems) {
                float currentDistance = Navigator.instance.GetPathDistance(user.transform.position, item.transform.position);
                if (closestDistance > currentDistance) {
                    closestItem = item;
                    closestDistance = currentDistance;
                }
            }
            currentItem = closestItem;
            SetCurrentPoint(currentItem.transform.position);
        }

        /// <summary>
        /// Sets the current point that the user is headed towards.
        /// </summary>
        /// <param name="point">The current point that the user is headed towards..</param>
        private void SetCurrentPoint(Vector3 point) {
            itemHighlight.transform.position = point;
            Navigator.instance.DrawShortestPath(user.transform.position, point);
        }

        /// <summary>
        /// Sets whether or not the job is active.
        /// </summary>
        /// <param name="jobActive">If set to <c>true</c> job active.</param>
        private void SetJobActive(bool jobActive) {
            this.jobActive = jobActive;
            itemHighlight.SetActive(jobActive);
        }
    }
}