using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

public class HexagonField : MonoBehaviour
{
    #region MEMBER_VAR
    public eGAME_STATE m_GameState;
    public BlockBase[][] m_Blocks;

    private float m_DropStartTime;
    public int m_dropStep;

    //const
    private const int FIELD_WIDTH = 7;
    public const int FIELD_HEIGHT = 6;

    private const float BLOCK_GAP_HEIGHT = 0.86f;

    //Ingame
    private Camera m_Cam;
    private List<BlockBase> m_BlockBombPool;
    private List<string> m_BlockNamePool;
    private RaycastHit hit;
    private bool m_bMousePress;
    #endregion MEMBER_VAR

    #region UNITY_FUNC
    public void Awake()
    {
    }

    // Use this for initialization
    void Start()
    {
        m_BlockBombPool = new List<BlockBase>();
        m_BlockNamePool = new List<string>();
        m_Cam = Camera.main;
    }



    // Update is called once per frame
    void Update()
    {
        switch (m_GameState)
        {
            case eGAME_STATE.DROP:
                if ((Time.time - m_DropStartTime) > 0.05f)
                {
                    if (m_dropStep < FIELD_HEIGHT)
                    {
                        for (int i = 0; i < m_Blocks.Length; ++i)
                        {
                            m_Blocks[i][m_dropStep].m_bDropStart = true;
                            //Debug.Log("drop step = " + m_dropStep);
                        }
                        m_dropStep++;
                    }

                    m_DropStartTime = Time.time;
                }
                break;
            case eGAME_STATE.PLAY:
                if(Input.GetMouseButtonDown(0))
                {
                    m_bMousePress = true;
                }

                if(Input.GetMouseButtonUp(0))
                {
                    if (m_bMousePress) 
                    {
                        Debug.Log("Pool Count = " + m_BlockBombPool.Count);
                        m_BlockBombPool.Clear();
                        m_BlockNamePool.Clear();
                        m_bMousePress = false;
                    }    
                }

                if(m_bMousePress)
                {
                    Proc_BlockSelect();
                }
                break;
        }
    }
    #endregion UNITY_FUNC



    #region PRIVATE_FUNC
    //## Get

    //## Set - BlockState
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

    //## Check
    private bool Check_CanTakeToPool(string name)
    {
        if(m_BlockNamePool.Count>0)
        {
            if(m_BlockNamePool.Contains(name) == false)
            {
                m_BlockNamePool.Add(name);
                return true;
            }         
        }
        else
        {
            m_BlockNamePool.Add(name);
            return true;
        }
        return false;
    }

    private bool Check_SameType(BlockBase bBase)
    {
        if(m_BlockBombPool.Count>0)
        {
            if(m_BlockBombPool[m_BlockBombPool.Count-1].m_BlockType == bBase.m_BlockType)
            {
                return true;
            }
        }
        else
        {
            return true;
        }
        return false;
    }

    //## Process
    private void Proc_BlockSelect()
    {
        Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (Check_CanTakeToPool(hit.collider.gameObject.name))
            {
                BlockBase bBase = hit.collider.gameObject.GetComponent<BlockBase>();
                if (Check_SameType(bBase))
                {
                    m_BlockBombPool.Add(bBase);
                    Debug.Log("Touched Block = " + hit.collider.gameObject+"/"+bBase.m_BlockType);
                }
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
                if (i % 2 == 1)
                {
                    oddPosY = BLOCK_GAP_HEIGHT / 2;
                }
                else
                {
                    oddPosY = 0;
                }
                blockObj = Instantiate(Resources.Load("Prefabs/Blocks_" + randBlock)) as GameObject;
                blockObj.transform.parent = parentObj;
                blockObj.name = "Block_" + randBlock + "_" + i + "_" + k;
                m_Blocks[i][k] = blockObj.AddComponent<BlockBase>();
                m_Blocks[i][k].SetBlockProperty(i, k, oddPosY, (eBLOCK_TYPE)randBlock);
            }
        }
    }

    public void SetMainGameState(eGAME_STATE state)
    {
        m_GameState = state;

        switch (state)
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
        if (m_Blocks != null)
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

