
using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class GridNode : MonoBehaviour
{
#region Variables (public)

	public GridNode m_pNorthNode = null;
	public GridNode m_pWestNode = null;
	public GridNode m_pEastNode = null;
	public GridNode m_pSouthNode = null;
	public GridNode m_pNorthWestNode = null;
	public GridNode m_pSouthEastNode = null;

	public Vector2 m_tPosInGrid = Vector2.zero;

	#endregion

#region Variables (private)



	#endregion


	public void SetPosInGrid(int iX, int iY)
	{
		m_tPosInGrid = new Vector2(iX, iY);
		transform.position = new Vector3(iX, 0.0f, -iY) * GridManager.s_iUnitSize;
	}
}
