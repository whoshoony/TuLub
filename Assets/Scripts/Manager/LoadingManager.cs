using UnityEngine;

using TJPlugins;

public sealed class LoadingManager : TJManager
{
    #region     INSTANCE
    private static LoadingManager m_Instance;
    public static LoadingManager instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new LoadingManager();
            }

            return m_Instance;
        }
    }
    #endregion  INSTANCE



    #region     PRIVATE MEMBERS
    public enum eSTATUS : byte
    {
        ePREPARE = 0,
        eREADY,
        eSHOW,
        eCLEAR_PREV,
        ePREPARE_NEXT,
        eHIDE,
        eEND,
        eCHANGE_FLOW,
    }

    private eMAIN_FLOW   m_eChangeFlow;
    private eSTATUS     m_eStatus;
    private bool        m_IsRun;
    #endregion  PRIVATE MEMBERS



    #region     PUBLIC PROPERTIES
    public eSTATUS status
    {
        get { return m_eStatus; }
        set { m_eStatus = value; }
    }

    public eMAIN_FLOW flow
    {
        get { return m_eChangeFlow; }
        set { m_eChangeFlow = value; }
    }
    #endregion  PUBLIC PROPERTIES



    #region     PRIVATE METHODS
    private LoadingManager()
    {
        m_eChangeFlow   = eMAIN_FLOW.eLOGO;
        m_eStatus       = eSTATUS.ePREPARE;
        m_IsRun         = false;
    }
    #endregion  PRIVATE METHODS



    #region     PUBLIC METHODS
    public void ChanageMainFlow(eMAIN_FLOW eValue)
    {
        //Debug.Log("ChanageMainFlow() .isRun =-"+m_IsRun);
        if (m_IsRun) return;
        if (m_eChangeFlow == eMAIN_FLOW.eINGAME)
        {
            Debug.Log("ChagngeMainFlow () ");
        }
        m_eChangeFlow = eValue;
        m_eStatus     = eSTATUS.ePREPARE;

        m_IsRun       = true;
    }

    public void PrepareNext()
    {
        m_eStatus = eSTATUS.ePREPARE_NEXT;
    }

    public override void Process()
    {
        if (!m_IsRun) return;

        switch (m_eStatus)
        {
            case eSTATUS.ePREPARE:
                {
                    PrepareNext();
                }
                break;

            case eSTATUS.eREADY:
                {
                    PrepareNext();
                }
                break;

            case eSTATUS.eSHOW:
                {
                }
                break;

            case eSTATUS.ePREPARE_NEXT:
                {
                    switch (MainManager.instance.currentFlow)
                    {
                        case eMAIN_FLOW.eLOGO:      LogoManager.instance.Destroy();   break;
                        //case eMAIN_FLOW.eTITLE:     TitleManager.instance.Destroy();  break;
                        //case eMAIN_FLOW.eCONTENTS: MyLobbyManager.instance.Destroy(); break;
                        case eMAIN_FLOW.eINGAME:    InGameManager.instance.Destroy(); break;
                    }

                    MainManager.instance.prevFlow    = MainManager.instance.currentFlow;
                    MainManager.instance.currentFlow = m_eChangeFlow;

                    switch (MainManager.instance.currentFlow)
                    {
                        case eMAIN_FLOW.eLOGO:      LogoManager.instance.Initialize();   break;
                        //case eMAIN_FLOW.eTITLE:     TitleManager.instance.Initialize();  break;
                        //case eMAIN_FLOW.eCONTENTS:  MyLobbyManager.instance.Initialize();  break;
                        case eMAIN_FLOW.eINGAME:    InGameManager.instance.Initialize(); break;
                    }

                    m_eStatus = eSTATUS.eHIDE;
                }
                break;

            case eSTATUS.eHIDE:
                {
                    //GameObject.DestroyImmediate(Loading.instance.gameObject);

                    m_eStatus = eSTATUS.eEND;
                }
                break;

            case eSTATUS.eEND:
                {
                    m_eStatus = eSTATUS.eCHANGE_FLOW;
                }
                break;

            case eSTATUS.eCHANGE_FLOW:
                {
                    switch (MainManager.instance.currentFlow)
                    {
                        case eMAIN_FLOW.eLOGO:      MainManager.instance.flowManager = LogoManager.instance;   break;
                        //case eMAIN_FLOW.eTITLE:     MainManager.instance.flowManager = TitleManager.instance;  break;
                        //case eMAIN_FLOW.eCONTENTS:  MainManager.instance.flowManager = MyLobbyManager.instance;   break;
                        case eMAIN_FLOW.eINGAME:    MainManager.instance.flowManager = InGameManager.instance; break;
                    }

                    m_IsRun = false;
                }
                break;
        }
    }
    #endregion  PUBLIC METHODS
}