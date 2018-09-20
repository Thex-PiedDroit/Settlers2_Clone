
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

	private List<GridNode> m_pNeighboursAsList = null;

	private bool m_bFree = true;

	#endregion


	public void SetPosInGrid(int iX, int iY)
	{
		m_tPosInGrid = new Vector2(iX, iY);
		transform.position = new Vector3(iX, 0.0f, -iY) * GridManager.s_iUnitSize;
	}

	public void SetFree(bool bFree)
	{
		m_bFree = bFree;
	}

	public bool IsFree()
	{
		return m_bFree;
	}

	public List<GridNode> GetNeighboursAsList()
	{
		if (m_pNeighboursAsList == null)
		{
			m_pNeighboursAsList = new List<GridNode>(6);

			if (m_pWestNode != null)
				m_pNeighboursAsList.Add(m_pWestNode);
			if (m_pEastNode != null)
				m_pNeighboursAsList.Add(m_pEastNode);
			if (m_pNorthNode != null)
				m_pNeighboursAsList.Add(m_pNorthNode);
			if (m_pSouthNode != null)
				m_pNeighboursAsList.Add(m_pSouthNode);
			if (m_pNorthWestNode != null)
				m_pNeighboursAsList.Add(m_pNorthWestNode);
			if (m_pSouthEastNode != null)
				m_pNeighboursAsList.Add(m_pSouthEastNode);
		}

		return m_pNeighboursAsList;
	}
}
