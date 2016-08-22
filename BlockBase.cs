using UnityEngine;
using System.Collections;

public class BlockBase : MonoBehaviour {
    #region MEMBER_VAR
    private HexagonField m_HField;
    public eBLOCK_TYPE m_BlockType;
    public GameObject m_BlockObj;
    public eBLOCK_STATE m_BlockState;
    public Vector2 m_StayPosition; // related STAY_BASE_POSITION
    public Vector2 m_GoalPosition;
    public Vector2 m_DropPosition;
    private float m_DropNormalizeY;
    public bool m_bDropStart;

    public int m_HIdx;
    public int m_VIdx;

    //Const
    private const float BLOCK_GAP_WIDTH = 0.78f;
    private const float BLOCK_GAP_HEIGHT = 0.86f;

    private const float BLOCK_STARTPOS_X = 1.66f;
    private const float BLOCK_STARTPOS_Y = 2.22f;

    private const float STAY_BASE_YPOSITION = 6; 

    private const int DROP_SPEED = 40;

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
    public void SetBlockProperty(int hx, int vy, float oddPosY, eBLOCK_TYPE type)
    {
        Vector2 tmpPos = new Vector2();

        m_BlockObj = transform.gameObject;

        m_HField = m_BlockObj.transform.parent.GetComponent<HexagonField>();

        m_BlockType = type; 

        m_HIdx = hx;
        m_VIdx = vy;
        m_bDropStart = false;

        tmpPos.x = BLOCK_STARTPOS_X + (m_HIdx * BLOCK_GAP_WIDTH); 
        tmpPos.y = BLOCK_STARTPOS_Y + (m_VIdx * BLOCK_GAP_HEIGHT) + oddPosY;
        m_StayPosition = new Vector2(tmpPos.x, tmpPos.y + STAY_BASE_YPOSITION);
        m_DropPosition = m_StayPosition;
        m_GoalPosition = new Vector2(tmpPos.x, tmpPos.y);

        m_DropNormalizeY = (m_GoalPosition.y - m_StayPosition.y) / DROP_SPEED;

        m_BlockObj.transform.localPosition = m_StayPosition;
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
        }
    }

    private void Proc_Drop()
    {
        if(m_bDropStart)
        {
            Move_Drop();
        }
    }

    private void Move_Drop()
    {
        m_DropPosition.y = m_DropPosition.y + m_DropNormalizeY;
        
        m_BlockObj.transform.localPosition = m_DropPosition;
        
        if(m_BlockObj.transform.localPosition.y <= m_GoalPosition.y)
        {
            m_BlockObj.transform.localPosition = m_GoalPosition;
            //Debug.Log("vIdx = "+m_VIdx);
            if(m_VIdx == m_HField.m_Blocks[0].Length-1)
            {
                //m_HField.SetMainGameState(eGAME_STATE.READY);
                m_HField.SetMainGameState(eGAME_STATE.PLAY);
            }
            m_BlockState = eBLOCK_STATE.IDLE;
        }
    }
    #endregion PRIVATE_FUNC
}
