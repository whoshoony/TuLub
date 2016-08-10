using UnityEngine;
using System.Collections;

#region ENUM
public enum eBLOCK_TYPE : byte
{
    //normal
    RED = 0,
    YELLOW,
    GREEN,
    NAVY,
    PURPLE,

    //special
    ORANGE,
    BLUE,
    PINK
}

public enum eBLOCK_STATE
{
    READY,
    IDLE,
    DROP,
    PICKUP,
    WAIT,
    EXPLOSION,
    DESTROY
}

public enum eGAME_STATE //ingamemanager에 있는 State를 사용한다. 아니다 이미 플레이 상태에서 게임의 스테이트를 말한다.
{
    DROP,
    READY,
    PLAY,
    FINISH
}
#endregion ENUM

public class HexagonField : MonoBehaviour {
    #region MEMBER_VAR
    public eGAME_STATE m_GameState;
    public BlockBase[][] m_Blocks;

    private float m_DropStartTime;
    public int m_dropStep;

    //const
    private const int FIELD_WIDTH = 7;
    public const int FIELD_HEIGHT = 6;

    private const float BLOCK_GAP_HEIGHT = 0.86f;
    #endregion MEMBER_VAR

    #region UNITY_FUNC
    public void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(m_GameState == eGAME_STATE.DROP)
        {
            if ((Time.time - m_DropStartTime) > 0.05f)
            {
                if (m_dropStep < FIELD_HEIGHT)
                {
                    for (int i = 0; i < m_Blocks.Length; ++i)
                    {
                        m_Blocks[i][m_dropStep].m_bDropStart = true;
                        Debug.Log("drop step = " + m_dropStep);
                    }
                    m_dropStep++;
                }

                m_DropStartTime = Time.time;
            }

            //if (m_dropStep> FIELD_HEIGHT)
            //{
            //    SetMainGameState(eGAME_STATE.READY);
            //}
            
        }
    }
    #endregion UNITY_FUNC



    #region PRIVATE_FUNC
    //Set BlockState
    private void SetBlocksState(eBLOCK_STATE state)
    {
        for (int i = 0; i < m_Blocks.Length; ++i)
        {
            for (int k = 0; k < m_Blocks[i].Length; ++k)
            {
                m_Blocks[i][k].m_BlockState = state;
            }
        }
    }
    
    #endregion PRIVATE_FUNC


    #region PUBLIC_FUNC
    public void CreateBlocks(Transform parentObj)
    {
        GameObject blockObj = null;
        float oddPosY = 0; //zigzag

        m_Blocks = new BlockBase[FIELD_WIDTH][];

        for (int i = 0; i < m_Blocks.Length; ++i)
        {
            m_Blocks[i] = new BlockBase[FIELD_HEIGHT];

            for (int k = 0; k < m_Blocks[i].Length; ++k)
            {
                int randBlock = Random.Range((int)eBLOCK_TYPE.RED, (int)eBLOCK_TYPE.PURPLE);
                if(i%2 == 1)
                {
                    oddPosY = BLOCK_GAP_HEIGHT / 2;
                }
                else
                {
                    oddPosY = 0;
                }
                blockObj = Instantiate(Resources.Load("Prefabs/Blocks_" + randBlock)) as GameObject;
                blockObj.transform.parent = parentObj;
                m_Blocks[i][k] = blockObj.AddComponent<BlockBase>();
                m_Blocks[i][k].SetBlockProperty(i, k, oddPosY);
            }
        }
    }

    public void SetMainGameState(eGAME_STATE state)
    {
        m_GameState = state;

        switch(state)
        {
            case eGAME_STATE.DROP:
                m_DropStartTime = Time.time;
                SetBlocksState(eBLOCK_STATE.DROP);
                break;
            case eGAME_STATE.READY:
                //

                break;
            case eGAME_STATE.PLAY:
                break;
        }
    }

    public void SetBeginState()
    {
        SetMainGameState(eGAME_STATE.DROP);
    }


    public void DestroyHexagonField()
    {
        if(m_Blocks != null)
        {
            for (int i = 0; i < m_Blocks.Length; ++i)
            {
                for (int k = 0; k < m_Blocks[i].Length; ++k)
                {
                    if (m_Blocks[i][k] != null)
                    {
                        m_Blocks[i][k] = null;
                    }
                }

                if (m_Blocks[i] != null)
                {
                    m_Blocks[i] = null;
                }
            }
            m_Blocks = null;
        }
    }
    #endregion PUBLIC_FUNC


    #region PROPERTY
    #endregion PROPERTY
    
}

