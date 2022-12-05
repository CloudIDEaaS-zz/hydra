#region License
/* 
 * Copyright (C) 1999-2015 John K�ll�n.
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2, or (at your option)
 * any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; see the file COPYING.  If not, write to
 * the Free Software Foundation, 675 Mass Ave, Cambridge, MA 02139, USA.
 */
#endregion

using System;
using System.Collections.Generic;

namespace Decompiler.Core.Lib
{
	/// <summary>
	/// Encapsulates Tarjan's algorithm for finding strongly connected components in a directed graph.
	/// </summary>
	/// <remarks>
	/// The algorithm itself is generic, and uses the graph and processScc procedures to perform the 
    /// actual work.
	/// </remarks>
	public class SccFinder<T>
	{
        private DirectedGraph<T> graph;
        private Action<IList<T>> processScc;
        private Action<T> firstVisit;
        private int nextDfs = 0;
		private Stack<Node> stack = new Stack<Node>();
		private Dictionary<T,Node> map = new Dictionary<T,Node>();

        public SccFinder(DirectedGraph<T> graph, Action<IList<T>> processScc) :
            this(graph, x => { }, processScc) 
        {
        }

        public SccFinder(DirectedGraph<T> graph, Action<T> firstVisit, Action<IList<T>> processScc)
        {
            this.graph = graph;
            this.firstVisit = firstVisit;
            this.processScc = processScc;
            this.nextDfs = 0;
        }

		private Node AddNode(T o)
		{
			Node node;
            if (!map.TryGetValue(o, out node))
			{
				node = new Node(o);
				map[o] = node;
			}
			return node;
		}

        private void Dfs(Node node)
        {
            firstVisit(node.o);

            node.dfsNumber = nextDfs++;
            node.visited = true;
            node.low = node.dfsNumber;
            stack.Push(node);
            foreach (Node o in GetSuccessors(node))
            {
                if (!o.visited)
                {
                    Dfs(o);
                    node.low = Math.Min(node.low, o.low);
                }
                if (o.dfsNumber < node.dfsNumber && stack.Contains(o))
                {
                    node.low = Math.Min(o.dfsNumber, node.low);
                }
            }
            if (node.low == node.dfsNumber)
            {
                List<T> scc = new List<T>();
                Node x;
                do
                {
                    x = stack.Pop();
                    scc.Add(x.o);
                } while (x != node);
                processScc(scc);
            }
        }

        public void Find(T start)
        {
            if (!map.ContainsKey(start))
                Dfs(AddNode(start));
        }

        private IEnumerable<Node> GetSuccessors(Node node)
        {
            foreach (T successor in graph.Successors(node.o))
            {
                yield return AddNode(successor);
            }
        }

		private class Node
		{
			public int dfsNumber;
			public bool visited;
			public int low;
			public T o;

			public Node(T o)
			{
				this.o = o;
			}
		}
	}
}
