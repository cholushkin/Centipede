using Alg;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class AppStateManager : Singleton<AppStateManager>
{
    public interface IAppState
    {
        void AppStateEnter(bool animated);
        void AppStateLeave(bool animated);
        void AppStateInitialization();
        string GetName();
    }

    public abstract class AppState<T> : Singleton<T>, IAppState where T : Singleton<T>
    {
        public abstract void AppStateEnter(bool animated);
        public abstract void AppStateLeave(bool animated);

        public void AppStateInitialization()
        {
            AssignInstance();
        }

        public string GetName()
        {
            return GetType().ToString();
        }
    }

    private IAppState[] _ownedStates;
    private IAppState _currenState;
    public Transform StartState;

    protected override void Awake()
    {
        base.Awake();
        Debug.LogFormat("AppStates available {0}", transform.childCount);

        // initialize states
        _ownedStates = new IAppState[transform.childCount];
        for (int i = 0; i < transform.childCount; ++i)
        {
            var child = transform.GetChild(i);
            var state = child.GetComponent<IAppState>();
            _ownedStates[i] = state;
            Assert.IsNotNull(_ownedStates[i], "state should be: public class StateName : AppStateManager.AppState<StateName>");
            state.AppStateInitialization();
        }
    }

    void Start()
    {
        if (StartState == null)
        {
            Debug.LogWarningFormat("The AppStateManager on {0} has no starting state.", gameObject.name);
            return;
        }
        Start(StartState.GetComponent<IAppState>(), false);
    }

    public void Start(IAppState state, bool animated)
    {
        Debug.LogFormat("Starting state '{0}'", state.GetName());
        if (_currenState != null && _currenState == state)
        {
            Debug.LogWarning("Restarting same state");
        }

        var nextState = _ownedStates.FirstOrDefault(s => s == state);
        Assert.IsNotNull(nextState, string.Format("AppStateManager: {0} doesn't own the state {1}", name, state.GetName()));

        // hope StateLeave won't call Start
        if (_currenState != null)
            _currenState.AppStateLeave(animated);

        _currenState = nextState;
        if (_currenState != null)
            _currenState.AppStateEnter(animated);
    }

    [ContextMenu("DbgPrintCurrentState")]
    void DbgPrintCurrentState()
    {
        Debug.Log(GetCurrentState());
    }

    public IAppState GetCurrentState()
    {
        return _currenState;
    }
}