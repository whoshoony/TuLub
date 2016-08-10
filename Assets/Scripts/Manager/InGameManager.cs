using UnityEngine;
using System.Collections;

using TJPlugins;

public class InGameManager : TJManager
{
    #region INSTANCE
    private static InGameManager m_Instance;
    public static InGameManager instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new InGameManager();
            }

            return m_Instance;
        }
    }
    #endregion  INSTANCE

    #region ENUM
    public enum eSTATUS
    {
        ePREPARE,
        eREADY,
        ePLAY,
        eEND
    }
    public eSTATUS m_eStatus;
    #endregion ENUM


    #region MEMBER_VAR
    private GameObject m_RootObj;
    private GameObject m_SceneInGameObj;
    private HexagonField m_HexaField;
    #endregion MEMBER_VAR


    #region     OVERRIDE METHODS
    public override void Initialize()
    {
        m_RootObj = GameObject.Find("Root");
        m_eStatus = eSTATUS.ePREPARE;
    }

    public override void Process()
    {
        switch(m_eStatus)
        {
            case eSTATUS.ePREPARE:
                /// Load
                Debug.Log("InGameManager eStatus = READY");
                //Scene object Load
                m_SceneInGameObj = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Scn_InGame")) as GameObject;
                m_SceneInGameObj.transform.parent = m_RootObj.transform;

                //Map Load
                GameObject mapObj = MonoBehaviour.Instantiate(Resources.Load("Prefabs/BackGround_01")) as GameObject;
                SetIntantiatedGameObject(mapObj, new Vector2(4, 6), m_SceneInGameObj);
                GameObject topFieldObj = MonoBehaviour.Instantiate(Resources.Load("Prefabs/TopField_01")) as GameObject;
                SetIntantiatedGameObject(topFieldObj, new Vector2(4, 9.4f), m_SceneInGameObj);

                //Hexagon Field
                m_HexaField = m_SceneInGameObj.transform.FindChild("HexagonField").GetComponent<HexagonField>();
                m_HexaField.CreateBlocks(m_HexaField.gameObject.transform);
                m_eStatus = eSTATUS.eREADY;
                break;
            case eSTATUS.eREADY:
                //m_HexaField.m_GameState = eGAME_STATE.READY;
                m_HexaField.SetBeginState();
                m_eStatus = eSTATUS.ePLAY;
                break;
            case eSTATUS.ePLAY:
                Debug.Log("InGameManager eStatus = ePLAY");
                m_eStatus = eSTATUS.eEND;
                break;
            case eSTATUS.eEND:
                break;
        }
    }

    public override void Destroy()
    {
        base.Destroy();
    }
    #endregion  OVERRIDE METHODS



    #region PRIVATE_FUNC
    private void SetIntantiatedGameObject(GameObject myObj, Vector2 position, GameObject parent)
    {
        myObj.transform.parent = parent.transform;
        myObj.transform.localPosition = position;
    }
    #endregion PRIVATE_FUNC
}
