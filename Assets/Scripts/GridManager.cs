
using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class GridManager : MonoBehaviour
{
#region Variables (public)

	public GridNode m_pNodePrefab = null;

	public float s_fUnitSize = 5.0f;
	public float s_fGridScaleY = 0.75f;

	public int m_iGridSizeX = 50;
	public int m_iGridSizeY = 50;

	#endregion

#region Variables (private)

	private List<GridNode> m_pGrid = null;

	#endregion


	private void Start()
	{
		InitGrid();

		GridNode pFrom = GetNodeAtPos(new Vector2(0.0f, 4.0f));
		GridNode pTo = GetNodeAtPos(new Vector2(4.0f, 2.0f));

		Queue<GridNode> pPath = Pathfinder.GetPathToDestination(pTo, pFrom);

		GridNode pCurrentNode = pPath.Dequeue();
		Debug.DrawLine(pCurrentNode.transform.position, pCurrentNode.transform.position + (Vector3.up * 2.0f), Color.white);

		while (pPath.Count > 0)
		{
			pCurrentNode = pPath.Dequeue();
			Debug.DrawLine(pCurrentNode.transform.position, pCurrentNode.transform.position + (Vector3.up * 2.0f), Color.magenta);
		}

		Debug.DrawLine(pCurrentNode.transform.position, pCurrentNode.transform.position + (Vector3.up * 2.0f), Color.red);
		Debug.Break();
	}

	private void InitGrid()
	{
		int iTotalGridSize = m_iGridSizeX * m_iGridSizeY;
		m_pGrid = new List<GridNode>(iTotalGridSize);

		for (int i = 0; i < iTotalGridSize; ++i)
		{
			int iX = i % m_iGridSizeX;
			int iY = i / m_iGridSizeX;

			GridNode pNewNode = Instantiate(m_pNodePrefab, transform);
			pNewNode.SetPosInGrid(iX, iY, s_fUnitSize, s_fGridScaleY);

			if (iX == 3 && iY > 0)
				pNewNode.SetFree(false);

			m_pGrid.Add(pNewNode);
			InitLastNodeNeighbours();
		}
	}

	private void InitLastNodeNeighbours()
	{
		GridNode pNewNode = m_pGrid[m_pGrid.Count - 1];
		Vector2 tPosInGrid = pNewNode.m_tPosInGrid;

		bool bOddRow = tPosInGrid.y % 2 == 1;

		GridNode pWestNode = GetNodeAtPos(tPosInGrid + Vector2.left);
		if (pWestNode != null)
		{
			pNewNode.m_pWestNode = pWestNode;
			pWestNode.m_pEastNode = pNewNode;
		}

		Vector2 tNorthWestDirection = bOddRow ? Vector2.up : Vector2.one;
		GridNode pNorthWestNode = GetNodeAtPos(tPosInGrid - tNorthWestDirection);
		if (pNorthWestNode != null)
		{
			pNewNode.m_pNorthWestNode = pNorthWestNode;
			pNorthWestNode.m_pSouthEastNode = pNewNode;
		}

		Vector2 tNorthEastDirection = bOddRow ? new Vector2(-1.0f, 1.0f) : Vector2.up;
		GridNode pNorthEastNode = GetNodeAtPos(tPosInGrid - tNorthEastDirection);
		if (pNorthEastNode != null)
		{
			pNewNode.m_pNorthEastNode = pNorthEastNode;
			pNorthEastNode.m_pSouthWestNode = pNewNode;
		}
	}

	public GridNode GetNodeAtPos(Vector2 tPosInGrid)
	{
		int iGridX = (int)(tPosInGrid.x);
		int iGridY = (int)(tPosInGrid.y);

		if (iGridX < 0 || iGridX >= m_iGridSizeX ||
			iGridY < 0 || iGridY >= m_iGridSizeY)
			return null;

		int iIndexInList = iGridX + (iGridY * m_iGridSizeX);

		return iIndexInList <= m_pGrid.Count ? m_pGrid[iIndexInList] : null;
	}
}
