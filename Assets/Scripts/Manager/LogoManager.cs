using UnityEngine;
using System.Collections;

using TJPlugins;

public class LogoManager : TJManager {

    #region     INSTANCE
    private static LogoManager m_Instance;
    public static LogoManager instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new LogoManager();
            }

            return m_Instance;
        }
    }
    #endregion  INSTANCE

    #region     OVERRIDE METHODS
    public override void Initialize()
    {
        MainManager.instance.StartCoroutine(StartProcess(2f));
    }

    public override void Process()
    {
    }

    public override void Destroy()
    {
        base.Destroy();
    }
    #endregion  OVERRIDE METHODS


    #region PRIVATE FUNC

    private IEnumerator StartProcess(float fTimer)
    {
        yield return new WaitForSeconds(fTimer);

        while (true)
        {
            yield return true;
                {
                //LoadingManager.instance.ChanageMainFlow(eMAIN_FLOW.eTITLE);
                LoadingManager.instance.ChanageMainFlow(eMAIN_FLOW.eINGAME);
                    break;
                }
        }
    }

    #endregion PRIVATE FUNC
}
