using UnityEngine;
using System.Collections;

public class HexagonFieldMap
{
    public eFIELDMAP_STATE m_FMapState;
    public eBLOCK_TYPE m_FMapBlockType;
    public Vector2 m_MapPos; //x: horizental y:vertical

    public HexagonFieldMap()
    {
        m_MapPos = new Vector2(0, 0);
    }

    public void InitHexaFieldMap(eFIELDMAP_STATE state, eBLOCK_TYPE type, int hx, int hy)
    {
        m_FMapState = state;
        m_FMapBlockType = type;
        m_MapPos.x = hx;
        m_MapPos.y = hy;
    }

}
