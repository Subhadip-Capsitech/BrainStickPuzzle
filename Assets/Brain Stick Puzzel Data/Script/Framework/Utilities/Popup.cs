using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace MS
{
    public class Popup : MonoBehaviour
    {
        public static Popup current;

        [Header("Anim Setting")]
        public Animator animator;
        public AnimationClip openClip, closeClip;
        [Header("Popup Setting")]
        public bool isOpen;
        public bool closeOnEsc;
        bool isPlaying;
        public UnityEvent onClose, onOpen;

        Popup lastOpenPopup;

        void Start()
        {
            AnimationEvent onOpenStop = new AnimationEvent();
            onOpenStop.functionName = "onStopAnim";
            onOpenStop.time = openClip.length;
            openClip.AddEvent(onOpenStop);

            AnimationEvent onCloseStop = new AnimationEvent();
            onCloseStop.functionName = "onStopAnim";
            onCloseStop.time = openClip.length;
            closeClip.AddEvent(onCloseStop);

            if (isOpen)
            {
                Open();
            }
            else
            {
                Close();
            }
        }

        [ContextMenu("Open")]
        public void Open()
        {
            if (isPlaying || isOpen)
                return;

            lastOpenPopup = current;
            current = this;

            isOpen = true;
            isPlaying = true;
            animator.Play(openClip.name);
            onOpen.Invoke();
        }

        [ContextMenu("Close")]
        public void Close()
        {
            if (isPlaying || !isOpen)
                return;

            isPlaying = true;
            animator.Play(closeClip.name);
            isOpen = false;
            current = lastOpenPopup;
            lastOpenPopup = null;
            onClose.Invoke();
        }

        public void onStopAnim()
        {
            isPlaying = false;
        }

        /*
		 * 
		 * This will paste in comman script in scene
		 * 
		 * 
		void Update()
		{
			if (Popup.current!=null&& Popup.current.closeOnEsc && Popup.current.isOpen && Input.GetKeyUp (KeyCode.Escape)) {
				Popup.current.Close ();
			}
		}
		*/
    }
}