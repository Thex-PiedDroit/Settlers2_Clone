
using UnityEngine;
using System.Collections.Generic;


static public class Pathfinder
{
#region Variables (public)



	#endregion

#region Variables (private)

	private class PathfinderNode
	{
		public PathfinderNode m_pParentNode = null;
		public GridNode m_pCorrespondingGridNode = null;

		public float m_fDistanceFromStart = 0.0f;
		public int m_iEstimatedDistanceFromDestination = 0;


		public PathfinderNode(GridNode pCorrespondingGridNode)
		{
			m_pCorrespondingGridNode = pCorrespondingGridNode;
		}

		public void SetValues(PathfinderNode pParent, float fDistanceFromstart, int iEstimatedDistanceFromDestination)
		{
			m_pParentNode = pParent;
			m_fDistanceFromStart = fDistanceFromstart;
			m_iEstimatedDistanceFromDestination = iEstimatedDistanceFromDestination;
		}

		public float GetScore()
		{
			return m_fDistanceFromStart + m_iEstimatedDistanceFromDestination;
		}
	}

	#endregion


	static public Queue<GridNode> GetPathToDestination(GridNode pDestination, GridNode pFromNode)
	{
		List<PathfinderNode> pOpenList = new List<PathfinderNode>();
		List<PathfinderNode> pClosedList = new List<PathfinderNode>();

		pOpenList.Add(new PathfinderNode(pFromNode));

		bool bFoundPath = false;

		do
		{
			int iCheapestOpenNodeIndex;
			PathfinderNode pCurrentNode = GetCheapestNodeFromList(pOpenList, out iCheapestOpenNodeIndex);
			pOpenList.RemoveAt(iCheapestOpenNodeIndex);
			pClosedList.Add(pCurrentNode);

			if (pCurrentNode.m_pCorrespondingGridNode == pDestination)
			{
				bFoundPath = true;
				break;
			}

			PopulateOpenListWithNeighbours(pCurrentNode, pDestination, ref pOpenList, pClosedList);

		} while (pOpenList.Count > 0);

		if (!bFoundPath)
		{
			Debug.LogError("Couldn't find a path between " + pFromNode + " to " + pDestination);
			return null;
		}

		return RetracePathFromEnd(pClosedList, pFromNode);
	}

	static private void PopulateOpenListWithNeighbours(PathfinderNode pCurrentNode, GridNode pDestination, ref List<PathfinderNode> pOpenList, List<PathfinderNode> pClosedList)
	{
		List<GridNode> pCurrentNodeNeighbours = pCurrentNode.m_pCorrespondingGridNode.GetNeighboursAsList();
		GridNode pWestNode = pCurrentNode.m_pCorrespondingGridNode.m_pWestNode;
		GridNode pEastNode = pCurrentNode.m_pCorrespondingGridNode.m_pEastNode;

		for (int i = 0; i < pCurrentNodeNeighbours.Count; ++i)
		{
			GridNode pCurrentNeighbour = pCurrentNodeNeighbours[i];

			if (!pCurrentNeighbour.IsFree() || FindNodeInList(pCurrentNeighbour, pClosedList) != -1)
				continue;


			PathfinderNode pNode = null;
			float fDistanceFromStartForHere = pCurrentNode.m_fDistanceFromStart + 1.0f;
			if (pCurrentNeighbour == pWestNode || pCurrentNeighbour == pEastNode)
				fDistanceFromStartForHere += 0.5f;			// While this is not technically true, this is to account for the possible backtracking that a horizontal movement might occur later down the path

			int iEstimatedDistanceFromDestinationForHere = GetDistanceBetweenGridNodes(pCurrentNeighbour, pDestination);

			bool bShouldUpdateAttributes = false;


			int iIndexInOpenList = FindNodeInList(pCurrentNeighbour, pOpenList);

			if (iIndexInOpenList == -1)
			{
				pNode = new PathfinderNode(pCurrentNeighbour);
				pOpenList.Add(pNode);

				bShouldUpdateAttributes = true;
			}
			else
			{
				pNode = pOpenList[iIndexInOpenList];
				bShouldUpdateAttributes = fDistanceFromStartForHere + iEstimatedDistanceFromDestinationForHere < pNode.GetScore();
			}

			if (bShouldUpdateAttributes)
				pNode.SetValues(pCurrentNode, fDistanceFromStartForHere, iEstimatedDistanceFromDestinationForHere);
		}
	}

	static private PathfinderNode GetCheapestNodeFromList(List<PathfinderNode> pList, out int iIndex)
	{
		iIndex = 0;
		PathfinderNode pCheapestNode = pList[0];
		float fCheapestCost = pCheapestNode.GetScore();

		for (int i = 1; i < pList.Count; ++i)
		{
			PathfinderNode pCurrentNode = pList[i];
			float fCurrentNodeScore = pCurrentNode.GetScore();

			if (fCurrentNodeScore < fCheapestCost || (fCurrentNodeScore == fCheapestCost && pCurrentNode.m_iEstimatedDistanceFromDestination < pCheapestNode.m_iEstimatedDistanceFromDestination))
			{
				pCheapestNode = pCurrentNode;
				fCheapestCost = fCurrentNodeScore;

				iIndex = i;
			}
		}

		return pCheapestNode;
	}

	static private int FindNodeInList(GridNode pNode, List<PathfinderNode> pList)
	{
		for (int i = 0; i < pList.Count; ++i)
		{
			if (pList[i].m_pCorrespondingGridNode == pNode)
				return i;
		}

		return -1;
	}

	static public int GetDistanceBetweenGridNodes(GridNode pFrom, GridNode pTo)
	{
		Vector2 tFromPos = pFrom.m_tPosInGrid;
		Vector2 tToPos = pTo.m_tPosInGrid;
		Vector2 tDifference = tToPos - tFromPos;

		bool bGoingRight = tDifference.x >= 0.0f;

		tDifference.x = Mathf.Abs(tDifference.x);
		tDifference.y = Mathf.Abs(tDifference.y);

		int iLongestPathPossible = (int)(tDifference.x + tDifference.y);

		int iShortcutsCount = (int)(tDifference.y) / 2;
		if (tDifference.y % 2 == 1)
		{
			bool bStartsOnOddRow = tFromPos.y % 2 == 1;

			if (!(bGoingRight ^ bStartsOnOddRow))	// If both are true or both are false
				++iShortcutsCount;
		}

		iShortcutsCount = Mathf.Min(iShortcutsCount, (int)(tDifference.x));		// Can't take more shortcuts than horizontally possible

		return iLongestPathPossible - iShortcutsCount;
	}

	static private Queue<GridNode> RetracePathFromEnd(List<PathfinderNode> pClosedList, GridNode pStart)
	{
		List<GridNode> pReversePath = new List<GridNode>(pClosedList.Count);

		PathfinderNode pCurrentNode = pClosedList[pClosedList.Count - 1];
		pReversePath.Add(pCurrentNode.m_pCorrespondingGridNode);

		while (pCurrentNode.m_pCorrespondingGridNode != pStart)
		{
			pCurrentNode = pCurrentNode.m_pParentNode;
			pReversePath.Add(pCurrentNode.m_pCorrespondingGridNode);
		}

		Queue<GridNode> pPath = new Queue<GridNode>(pReversePath.Count);

		for (int i = pReversePath.Count - 1; i >= 0; --i)
			pPath.Enqueue(pReversePath[i]);

		return pPath;
	}
}
