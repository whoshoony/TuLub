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
    private RaycastHit m_Hit;
    private bool m_bTouchPress;
    private Vector2 m_FirstPickBlockLocInfo;
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
        m_FirstPickBlockLocInfo = new Vector2();
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
                    m_bTouchPress = true;
                }

                if(Input.GetMouseButtonUp(0))
                {
                    if (m_bTouchPress) 
                    {
                        SetTouchReleased();
                    }    
                }

                if(m_bTouchPress)
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

    private void SetTouchReleased()
    {
        if (m_BlockBombPool.Count > 2)
        {
            for (int i = 0; i < m_BlockBombPool.Count; ++i)
            {
                m_BlockBombPool[i].SetBlockState(eBLOCK_STATE.EXPLOSION);
            }
        }

        Debug.Log("Pool Count = " + m_BlockBombPool.Count);
        for (int i = 0; i < m_BlockBombPool.Count; ++i)
        {
            m_BlockBombPool[i].m_PickUpObj.SetActive(false);
        }
        
        m_BlockBombPool.Clear();
        m_BlockNamePool.Clear();
        m_bTouchPress = false;
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

    private bool Check_IsNearBlockHor(int posHor)
    {        
        if(m_FirstPickBlockLocInfo.x == posHor || m_FirstPickBlockLocInfo.x-1 == posHor || m_FirstPickBlockLocInfo.x+1 == posHor)
        {
            return true;
        }
        return false;
    }

    private bool Check_IsNearBlockVer(int posVer)
    {
        if (m_FirstPickBlockLocInfo.y == posVer || m_FirstPickBlockLocInfo.y - 1 == posVer || m_FirstPickBlockLocInfo.y + 1 == posVer)
        {
            return true;
        }
        return false;
    }

    private bool Check_IsNearBlock(int hx, int vy)
    {
        //(-1, 0)
        if(m_FirstPickBlockLocInfo.x-1 == hx && m_FirstPickBlockLocInfo.y == vy)
        {
            return true;
        }
        //(0, -1)
        else if(m_FirstPickBlockLocInfo.x == hx && m_FirstPickBlockLocInfo.y-1 == vy)
        {
            return true;
        }
        //(+1, 0)
        else if(m_FirstPickBlockLocInfo.x+1 == hx && m_FirstPickBlockLocInfo.y == vy)
        {
            return true;
        }
        //(+1, +1)
        else if(m_FirstPickBlockLocInfo.x+1 == hx && m_FirstPickBlockLocInfo.y+1 == vy)
        {
            return true;
        }
        //(0, +1)
        else if(m_FirstPickBlockLocInfo.x == hx && m_FirstPickBlockLocInfo.y+1 == vy)
        {
            return true;
        }
        //(-1, +1)
        else if(m_FirstPickBlockLocInfo.x-1 == hx && m_FirstPickBlockLocInfo.y+1 == vy)
        {
            return true;
        }
        return false;
    }

    private bool Check_CanMakeLine(BlockBase bBase)
    {
        if(m_BlockBombPool.Count>0)
        {
            Debug.Log("origin = "+m_FirstPickBlockLocInfo+" / "+bBase.HIdx+", "+bBase.VIdx);
            //if(Check_IsNearBlockHor(bBase.HIdx) && Check_IsNearBlockVer(bBase.VIdx))
            if(Check_IsNearBlock(bBase.HIdx, bBase.VIdx))
            {
                m_FirstPickBlockLocInfo.x = bBase.HIdx;
                m_FirstPickBlockLocInfo.y = bBase.VIdx;
                return true;
            }
        }
        else
        {
            //Adding for the first time
            m_FirstPickBlockLocInfo.x = bBase.HIdx;
            m_FirstPickBlockLocInfo.y = bBase.VIdx;
            return true;
        }
        return false;
    }

    //## Process
    private void Proc_BlockSelect()
    {
        Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out m_Hit))
        {
            if (Check_CanTakeToPool(m_Hit.collider.gameObject.name))
            {
                BlockBase bBase = m_Hit.collider.gameObject.GetComponent<BlockBase>();
                //have to check : touched block is near origin block
                if(Check_CanMakeLine(bBase))
                {
                    if (Check_SameType(bBase))
                    {
                        bBase.SetBlockState(eBLOCK_STATE.PICKUP);
                        m_BlockBombPool.Add(bBase);
                        Debug.Log("Touched Block = " + m_Hit.collider.gameObject + "/" + bBase.m_BlockType);
                    }
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

