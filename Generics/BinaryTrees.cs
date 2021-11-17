using System;
using System.Collections;
using System.Collections.Generic;


namespace Generics.BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T>
        where T : IComparable<T>
    {
        private BinaryTree<T> left;
        public BinaryTree<T> Left
        {
            get
            {
                if (left == null)
                {
                    left = new BinaryTree<T>();
                }
                return left;
            }
            set { left = value; }
        }
        private BinaryTree<T> right;
        public BinaryTree<T> Right
        {
            get
            {
                if (right == null)
                {
                    right = new BinaryTree<T>();
                }
                return right;
            }
            set { right = value; }
        }
        public BinaryTree<T> Parent { get; set; }

        public T Value { get; set; } = default;

        public bool IsEmpty { get; set; } 

        public void Add(T value)
        {
            if (!IsEmpty)
            {
                Value = value;
                IsEmpty = true;
                Parent = null;
                return;
            }
            else
            {
                if (value.CompareTo(this.Value) == 1)
                    Insert(value, Right, this);
                else 
                    Insert(value, Left, this);
            }
        }

        private void Insert(T value, BinaryTree<T> currentNode, BinaryTree<T> parent)
        {
            if (!currentNode.IsEmpty)
            {
                currentNode.Value = value;
                currentNode.Parent = parent;
                currentNode.IsEmpty = true;
                return;
            }

            int comparisonValue = value.CompareTo(currentNode.Value);
            if (comparisonValue == 1)
            {
                Insert(value, currentNode.Right, currentNode);
            }
            else if (comparisonValue == -1)
            {
                Insert(value, currentNode.Left, currentNode);
            }
        }
		
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new BinaryTreeEnumerator<T>(this);
        }
    }

    public static class BinaryTree
    {
        public static BinaryTree<int> Create(params int[] items)
        {
            BinaryTree<int> binaryTree = new BinaryTree<int>();
            for (int i = 0; i < items.Length; i++)
            {
                binaryTree.Add(items[i]);
            }
            return binaryTree;
        }
    }

    class BinaryTreeEnumerator<T> : IEnumerator<T>
   where T : IComparable<T>
    {
        private BinaryTree<T> OriginalTree { get; set; }
        private BinaryTree<T> CurrentNode { get; set; }
        object IEnumerator.Current => Current;
        public T Current => CurrentNode.Value;

        public BinaryTreeEnumerator(BinaryTree<T> tree)=>OriginalTree = tree;

        public bool MoveNext()
        {
            if (!OriginalTree.IsEmpty)
            {
                return false;
            }

            if (CurrentNode == null)
                CurrentNode = FindMostLeft(OriginalTree);
            else
            {
                if (CurrentNode.Right.IsEmpty)
                    CurrentNode = FindMostLeft(CurrentNode.Right);
                else
                {
                    T currentValue = CurrentNode.Value;
                    while (CurrentNode.IsEmpty)
                    {
                        CurrentNode = CurrentNode.Parent;
                        if (CurrentNode != null)
                        {
                            if (Current.CompareTo(currentValue) < 0)
                                continue;
                        }
                        break;
                    }
                }
            }
            return CurrentNode != null;
        }

        public void Reset()
        {
            CurrentNode = new BinaryTree<T>();
        }

        BinaryTree<T> FindMostLeft(BinaryTree<T> start)
        {
            BinaryTree<T> node = start;
            while (true)
            {
                if (node.Left.IsEmpty)
                {
                    node = node.Left;
                    continue;
                }
                break;
            }
            return node;
        }

        public void Dispose()
        {

        }
    }
}
