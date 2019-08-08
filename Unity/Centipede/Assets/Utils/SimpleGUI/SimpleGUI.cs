using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameGUI
{
    public class SimpleGUI : MonoBehaviour
    {
        public string StartingScreenName;
        public Transform ScreensRoot;
        public GUIScreenBase[] Screens;

        private readonly Stack<GUIScreenBase> _screenStack = new Stack<GUIScreenBase>(); // note: _screenStack is a logical representation, and the hierarchy is current state of the screens

        void Awake()
        {
            if (ScreensRoot == null)
                ScreensRoot = transform;

            // disable screens 
            foreach (Transform child in ScreensRoot)
                child.gameObject.SetActive(false);
        }

        void Start()
        {
            // activating default screen
            if (string.IsNullOrEmpty(StartingScreenName))
                return;
            Debug.LogFormat("Activating starting screen {0}", StartingScreenName);
            PushScreen(StartingScreenName);
        }


        // return current active screen
        public GUIScreenBase GetCurrentScreen()
        {
            if (_screenStack.Count == 0)
                return null;
            return _screenStack.Peek();
        }

        // pop current screen and returns it
        public GUIScreenBase PopScreen(string expectedScreenToPop = null)
        {
            var screenOnTop = GetCurrentScreen();
            GUIScreenBase popped;
            if (screenOnTop == null)
            {
                Debug.LogWarningFormat("Expecting to pop screen: {0}, but there is nothing to pop!", expectedScreenToPop);
                return null;
            }

            if (expectedScreenToPop != null)
            {
                if (expectedScreenToPop == screenOnTop.name)
                {
                    popped = _screenStack.Pop();
                    Assert.IsTrue(popped == screenOnTop);
                    screenOnTop.StartDisappearAnimation();
                    return screenOnTop;
                }
                Debug.LogErrorFormat("Expecting to pop screen '{0}', but got '{1}'", expectedScreenToPop, screenOnTop.name);
                return null;
            }
            popped = _screenStack.Pop();
            screenOnTop.StartDisappearAnimation();
            Assert.IsTrue(popped == screenOnTop);
            return popped;
        }

        internal void OnScreenPoped(GUIScreenBase screen)
        {
            if (screen.IsModal)
                ModalNotify(screen, false);

            // hide or die
            if (screen.IsDestroyOnPop)
                Destroy(screen.gameObject);
            else
                gameObject.SetActive(false);
        }


        public void PushScreen(string screenName)
        {
            // obtain screen
            var screenTr = ObtainScreen(screenName);
            Assert.IsNotNull(screenTr, "SimpleGUI: couldn't obtain screen: " + screenName);
            screenTr.gameObject.SetActive(true);
            var screen = screenTr.GetComponent<GUIScreenBase>();
            screen.OnNewScreenPushed();
            Assert.IsNotNull(screen, string.Format("SimpleGUI: screen {0} must be derived from GUIScreenBase", screenName));

            // push it on the top of the tree
            screenTr.SetAsLastSibling();
            screen.StartAppearAnimation();
            _screenStack.Push(screen);

            // notify all active screens in case if current screen is a modal screen
            if (screen.IsModal)
                ModalNotify(screen, true);
        }

        private void ModalNotify(GUIScreenBase screen, bool isUnderModal)
        {
            // note: we rely on visual instead of logic( the stack) because blocking and unblocking other screens 
            // happens according to visual of the screen, while they are still on the screen
            var foundSelf = false;
            for (int i = ScreensRoot.childCount - 1; i >= 0; --i)
            {
                var s = ScreensRoot.GetChild(i).gameObject;
                var baseScreen = s.GetComponent<GUIScreenBase>();
                if (baseScreen == screen) // starting from the screen that call ModalNotify
                {
                    foundSelf = true;
                    continue;
                }
                if (!foundSelf) // skip screens that are above 
                    continue;
                if (!s.activeSelf) // skip disabled (disabled == dead)
                    continue;

                baseScreen.OnBecomeUnderModal(isUnderModal); // notify
                if (baseScreen.IsModal) // below the first modal is already being controlled by that modal, so stop notifying
                    return;
            }
        }

        private Transform ObtainScreen(string screenName)
        {
            // try to reuse screen, find screen in children of current SimpleGUI
            var screenTransform = ScreensRoot.Find(screenName);

            // create new by name
            if (screenTransform == null)
            {
                var screenPrefab = Screens.FirstOrDefault(n => screenName == n.name);
                Assert.IsNotNull(screenPrefab, "SimpleGUI: can't find screen named " + screenName);
                screenTransform = Instantiate(screenPrefab.transform, ScreensRoot);
                screenTransform.name = screenName;
            }
            else
            {
                Assert.IsTrue(screenTransform.GetComponent<GUIScreenBase>() != null, "");
                Assert.IsFalse(screenTransform.GetComponent<GUIScreenBase>().IsInTransaction, "");
            }
            return screenTransform;
        }

        [ContextMenu("DbgPrintStack")]
        void DbgPrintStack()
        {
            Debug.Log(">>>>> _screenStack");
            foreach (var guiScreenBase in _screenStack)
            {
                Debug.LogFormat("  >Screen name:{0}, IsModal:{1}, IsInputEnabled:{2}, IsInTransaction:{3}, inputEnabledRefs:{4}",
                    guiScreenBase.name,
                    guiScreenBase.IsModal,
                    guiScreenBase.IsInputEnabled,
                    guiScreenBase.IsInTransaction,
                    guiScreenBase.GetRefsCount());
            }
        }
    }
}