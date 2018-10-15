
using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class GridNode : MonoBehaviour
{
#region Variables (public)

	public GridNode m_pNorthWestNode = null;
	public GridNode m_pNorthEastNode = null;
	public GridNode m_pSouthWestNode = null;
	public GridNode m_pSouthEastNode = null;
	public GridNode m_pWestNode = null;
	public GridNode m_pEastNode = null;

	public Vector2 m_tPosInGrid = Vector2.zero;

	#endregion

#region Variables (private)

	private List<GridNode> m_pNeighboursAsList = null;

	private bool m_bFree = true;

	#endregion


	public void SetPosInGrid(int iX, int iY, float fUnitSize, float fGridScaleY)
	{
		m_tPosInGrid = new Vector2(iX, iY);

		float fPosX = iX;
		if (iY % 2 == 1)
			fPosX += 0.5f;

		float fPosY = -iY * fGridScaleY;

		transform.position = new Vector3(fPosX, 0.0f, fPosY) * fUnitSize;
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

			if (m_pNorthEastNode != null)
				m_pNeighboursAsList.Add(m_pNorthEastNode);
			if (m_pSouthWestNode != null)
				m_pNeighboursAsList.Add(m_pSouthWestNode);
			if (m_pNorthWestNode != null)
				m_pNeighboursAsList.Add(m_pNorthWestNode);
			if (m_pSouthEastNode != null)
				m_pNeighboursAsList.Add(m_pSouthEastNode);
			if (m_pWestNode != null)
				m_pNeighboursAsList.Add(m_pWestNode);
			if (m_pEastNode != null)
				m_pNeighboursAsList.Add(m_pEastNode);
		}

		return m_pNeighboursAsList;
	}
}
