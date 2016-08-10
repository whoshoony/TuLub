using UnityEngine;
using System.Collections.Generic;

using TJPlugins;


#region ENUM
public enum eMAIN_FLOW
{
    eLOGO,
    eTITLE,
    eCONTENTS,
    eINGAME
}
#endregion ENUM

public class MainManager : MonoBehaviour {
    #region MEMBER_VAR

    [SerializeField]
    private eMAIN_FLOW m_ePrevFlow;
    [SerializeField]
    private eMAIN_FLOW m_eCurrentFlow;

    private TJManager m_FlowManager;
    private List<TJManager> m_SystemManagers;
    #endregion MEMBER_VAR


    #region     INSTANCE
    private static MainManager m_Instance;
    public static MainManager instance
    {
        get
        {
    //        if (m_Instance == null)
    //        {
    //            m_Instance = //new MainManager();
    //        }

            return m_Instance;
        }
    }
    #endregion  INSTANCE


    #region     PUBLIC PROPERTIES
    public eMAIN_FLOW prevFlow
    {
        get { return m_ePrevFlow; }
        set { m_ePrevFlow = value; }
    }
    public eMAIN_FLOW currentFlow
    {
        get { return m_eCurrentFlow; }
        set { m_eCurrentFlow = value; }
    }

    public TJManager flowManager
    {
        get { return m_FlowManager; }
        set { m_FlowManager = value; }
    }
    #endregion  PUBLIC PROPERTIES


    #region UNITY_FUNC
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if(m_Instance == null)
        {
            m_Instance = gameObject.GetComponent<MainManager>();
        }

        m_SystemManagers = new List<TJManager>();
        
        InitFlow();

        m_SystemManagers.Add(LoadingManager.instance);


        m_FlowManager.Initialize();
    }

    // Use this for initialization
    void Start()
    {
    }


    void Update()
    {
        if (m_FlowManager != null)
        {
            m_FlowManager.Process();
        }

        if (m_SystemManagers != null)
        {
            for (int i = 0; i < m_SystemManagers.Count; i++)
            {
                m_SystemManagers[i].Process();
            }
        }

        // 안드로이드 뒤로가기시 처리
        if (Input.GetKeyDown(KeyCode.Escape) == true)
        {
            //PopupManager.instance.ShowPopupTwoButton("게임을 종료 하시겠습니까?", "취소", "확인", null, delegate { ApplicationExit(); }, "알림");
        }
    }
    #endregion UNITY_FUNC


    #region PRIVATE_FUNC
    private void OnDisable()
    {
        if (m_SystemManagers != null)
        {
            for (int i = 0; i < m_SystemManagers.Count; i++)
            {
                m_SystemManagers[i].Destroy();
            }
        }
    }

    private void InitFlow()
    {
        switch (m_eCurrentFlow)
        {
            case eMAIN_FLOW.eLOGO: m_FlowManager = LogoManager.instance; break;
        //    case eMAIN_FLOW.eTITLE: m_FlowManager = TitleManager.instance; break;
        //    case eMAIN_FLOW.eCONTENTS: m_FlowManager = MyLobbyManager.instance; break;
            case eMAIN_FLOW.eINGAME: m_FlowManager = InGameManager.instance; break;
        }
    }
    #endregion PRIVATE_FUNC
}

