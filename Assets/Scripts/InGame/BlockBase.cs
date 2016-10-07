using UnityEngine;
using System.Collections;

public class BlockBase : MonoBehaviour {
    #region MEMBER_VAR
    //## Prop
    private HexagonField m_HField;
    public eBLOCK_TYPE m_BlockType;
    public GameObject m_BlockObj;
    public GameObject m_PickUpObj;
    public eBLOCK_STATE m_BlockState;
    public Vector2 m_StayPosition; // related STAY_BASE_POSITION
    public Vector2 m_GoalPosition;
    public Vector2 m_DropPosition;
    private float m_DropNormalizeY;
    public bool m_bDropTarget;
    public bool m_bDropStart;
    public bool m_bImLastDrop;

    private int m_HIdx;
    private int m_VIdx;

    //## Block State
    //# pickup
    private bool m_bStPu;
    //private float m_ChangeScale;
    Vector2 m_PickUpScale;

    //## Const
    private const float BLOCK_GAP_WIDTH = 0.78f;
    private const float BLOCK_GAP_HEIGHT = 0.86f;

    private const float BLOCK_STARTPOS_X = 1.66f;
    private const float BLOCK_STARTPOS_Y = 2.22f;

    private const float STAY_BASE_YPOSITION = 6; 

    private const int DROP_SPEED = 40;

    private const float CHANGE_SCALE_VALUE = 0.2f;

    private const int LAST_BLOCK_VINDEX = 11;//5;

    #endregion MEMBER_VAR


    #region UNITY_FUNC
    public void Start()
    {
        //
    }

    public void Update()
    {
        UpdateBlockState();
    }

    #endregion UNITY_FUNC



    #region PUBLIC_FUNC
    //## Set
    public void SetBlockProperty(int hx, int vy, float oddPosY, eBLOCK_TYPE type)
    {
        Vector2 tmpPos = new Vector2();

        m_BlockObj = transform.gameObject;

        m_HField = m_BlockObj.transform.parent.GetComponent<HexagonField>();

        m_BlockType = type; 

        m_HIdx = hx;
        m_VIdx = vy;
        //Debug.Log("idx = "+m_HIdx+", "+m_VIdx);

        m_PickUpScale = new Vector2(1, 1);
        m_bDropTarget = m_bDropStart = m_bImLastDrop = false;
        m_bStPu = false;
        //m_ChangeScale = 1;

        if(m_VIdx == LAST_BLOCK_VINDEX)
        {
            m_bImLastDrop = true;
            //Debug.Break();
        }

        tmpPos.x = BLOCK_STARTPOS_X + (m_HIdx * BLOCK_GAP_WIDTH); 
        tmpPos.y = BLOCK_STARTPOS_Y + (m_VIdx * BLOCK_GAP_HEIGHT) + oddPosY;
        m_StayPosition = new Vector2(tmpPos.x, tmpPos.y + STAY_BASE_YPOSITION);
        m_DropPosition = m_StayPosition;
        m_GoalPosition = new Vector2(tmpPos.x, tmpPos.y);

        m_DropNormalizeY = (m_GoalPosition.y - m_StayPosition.y) / DROP_SPEED;

        m_BlockObj.transform.localPosition = m_StayPosition;

        m_PickUpObj = Instantiate(Resources.Load("Prefabs/PickUp")) as GameObject;
        m_PickUpObj.transform.localPosition = m_GoalPosition;
        m_PickUpObj.SetActive(false);
        //Debug.Log("idx = " + m_HIdx + ", " + m_VIdx+"/"+ m_PickUpObj.transform.localPosition);
    }

    public void SerSwitchBlockProp(int hx, int vy, Vector2 goalPos)//, eBLOCK_STATE state)
    {
        m_HIdx = hx;
        m_VIdx = vy;
        m_GoalPosition = goalPos;
        m_bDropTarget = true;
        m_bDropStart = false;
        
        //SetBlockState(state);
    }

    public void SetBlockState(eBLOCK_STATE state)
    {
        m_BlockState = state;

        switch(state)
        {
            case eBLOCK_STATE.PICKUP:
                m_bStPu = true;
                m_PickUpObj.SetActive(true);
                break;
            case eBLOCK_STATE.EXPLOSION:
                m_BlockObj.SetActive(false);
                m_PickUpObj.SetActive(false);
                break;
        }
    }
    #endregion PUBLIC_FUNC


    #region PRIVATE_FUNC
    private void UpdateBlockState()
    {
        switch(m_BlockState)
        {
            case eBLOCK_STATE.DROP:
                Proc_Drop();
                break;
            case eBLOCK_STATE.IDLE:
                break;
            case eBLOCK_STATE.PICKUP:
                Proc_PickUp();
                break;
        }
    }

    //##  Process
    private void Proc_Drop()
    {
        if(m_bDropStart)
        {
            Move_Drop();
        }
        //else
        //{
        //    Debug.Log("I'm not start drop ; "+m_VIdx);
        //}
    }

    private bool m_bStRwPuScale = false;
    private void Proc_PickUp()
    {
        int axis = 0;
        if (m_bStPu)
        { 
            axis = Random.Range(0, 1);

            switch(axis)
            {
                case 0:
                    m_PickUpScale.x -= CHANGE_SCALE_VALUE;
                    break;
                case 1:
                    m_PickUpScale.y -= CHANGE_SCALE_VALUE;
                    break;
            }

            m_BlockObj.transform.localScale = m_PickUpScale;

            if(m_PickUpScale.x <= (-1+ CHANGE_SCALE_VALUE) || m_PickUpScale.y <= (-1+ CHANGE_SCALE_VALUE))
            {
                m_bStRwPuScale = true;
                m_bStPu = false;
            }
        }

        if(m_bStRwPuScale)
        {
            switch (axis)
            {
                case 0:
                    m_PickUpScale.x += CHANGE_SCALE_VALUE;
                    break;
                case 1:
                    m_PickUpScale.y += CHANGE_SCALE_VALUE;
                    break;
            }

            m_BlockObj.transform.localScale = m_PickUpScale;

            if (m_PickUpScale.x >= 1 || m_PickUpScale.y >= 1)
            {
                m_PickUpScale.x = m_PickUpScale.y = 1;
                m_BlockObj.transform.localScale = m_PickUpScale;
                m_bStRwPuScale = false;
            }
        }
    }

    private void Move_Drop()
    {
        m_DropPosition.y = m_DropPosition.y + m_DropNormalizeY;
        
        m_BlockObj.transform.localPosition = m_DropPosition;
        
        if(m_BlockObj.transform.localPosition.y <= m_GoalPosition.y)
        {
            m_BlockObj.transform.localPosition = m_GoalPosition;
            m_StayPosition = m_GoalPosition;
            //Debug.Log("vIdx = "+m_VIdx+"/"+( m_HField.m_Blocks[0].Length - 1));
            if (m_bImLastDrop)
            {
                m_bDropStart = false;
                m_HField.m_dropStep = 0;
                m_HField.SetMainGameState(eGAME_STATE.PLAY);
                
            }
            m_BlockState = eBLOCK_STATE.IDLE;
        }
    }
    #endregion PRIVATE_FUNC

    #region PROPERTY
    public int HIdx
    { 
        get { return m_HIdx; }
    }
    public int VIdx
    {
        get { return m_VIdx; }
    }

    #endregion PROPERTY
}
