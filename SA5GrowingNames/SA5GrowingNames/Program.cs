using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SA5GrowingNames
{
    internal static class Program
    {
        private const string InputFileName = "input.txt";

        private const string OutputFileName = "output.txt";

        private static void Main()
        {
            var rows = File.ReadAllLines(InputFileName)
                .Skip(1)
                .ToArray();

            var tree = new PrefixTree();

            foreach (var row in rows)
            {
                tree.AddString(row);
            }


            var output = tree.FindLongestChainLength().ToString();

            File.WriteAllText(OutputFileName, output);
        }
    }

    public class PrefixTree
    {
        private readonly PrefixTreeNode _root = new PrefixTreeNode(null, default);

        private readonly List<PrefixTreeNode> _flagNodes = new List<PrefixTreeNode>();

        public void AddString(string s)
        {
            var node = _root;

            for (var i = 0; i < s.Length; i++)
            {
                node = node.UpdateChild((byte)(s[i] - 'a'));
            }

            node.Flag = true;

            _flagNodes.Add(node);
        }

        public int FindLongestChainLength()
        {
            var stack = new Stack<PrefixTreeNode>();
            var maxRate = 0;

            _root.Rate = 0;
            _flagNodes.ForEach(stack.Push);

            while (stack.Count > 0)
            {
                var node = stack.Peek();
                var parent = node.ParentLink;
                var suffixLink = node.GetSuffixLink();

                if (parent.Rate == null)
                {
                    stack.Push(parent);

                    continue;
                }

                if (suffixLink.Rate == null)
                {
                    stack.Push(suffixLink);

                    continue;
                }

                stack.Pop();

                var rate = Math.Max(parent.Rate.Value, suffixLink.Rate.Value);

                if (node.Flag)
                {
                    rate++;
                }

                node.Rate = rate;

                if (maxRate < rate)
                {
                    maxRate = rate;
                }
            }

            return maxRate;
        }

        private class PrefixTreeNode
        {
            private static int NodeId;
            private static readonly Dictionary<(int, byte), PrefixTreeNode> AutoMoves = new Dictionary<(int, byte), PrefixTreeNode>();
            public static readonly Dictionary<(int, byte), PrefixTreeNode> Children = new Dictionary<(int, byte), PrefixTreeNode>();

            public readonly int _nodeId = NodeId++;

            private PrefixTreeNode SuffixLink;

            public PrefixTreeNode GetChildren(byte c)
            {
                Children.TryGetValue((_nodeId, c), out var result);

                return result;
            }


            public void SetChildren(byte c, PrefixTreeNode value) => Children[(_nodeId, c)] = value;

            private PrefixTreeNode GetAutoMoveNode(byte c)
            {
                AutoMoves.TryGetValue((_nodeId, c), out var result);

                return result;
            }

            private void SetAutoMoveNode(byte c, PrefixTreeNode value) => AutoMoves[(_nodeId, c)] = value;

            public readonly PrefixTreeNode ParentLink;

            public readonly byte Char;

            public int? Rate;

            public bool Flag;

            public PrefixTreeNode(PrefixTreeNode parent, byte c)
            {
                if (parent == null)
                {
                    Rate = 0;
                }

                ParentLink = parent;
                Char = c;
            }

            public PrefixTreeNode UpdateChild(byte c)
            {
                var child = GetChildren(c);

                if (child == null)
                {
                    child = new PrefixTreeNode(this, c);

                    SetChildren(c, child);
                }

                return child;
            }

            public PrefixTreeNode GetSuffixLink()
            {
                if (SuffixLink == null)
                {
                    if (ParentLink?.ParentLink == null)
                    {
                        SuffixLink = ParentLink ?? this;
                    }
                    else
                    {
                        SuffixLink = ParentLink.GetSuffixLink().GetAutoMove(Char);
                    }
                }

                return SuffixLink;
            }

            public PrefixTreeNode GetAutoMove(byte c)
            {
                if (GetAutoMoveNode(c) == null)
                {
                    if (GetChildren(c) != null)
                    {
                        SetAutoMoveNode(c, GetChildren(c));
                    }
                    else if (ParentLink == null)
                    {
                        SetAutoMoveNode(c, this);
                    }
                    else
                    {
                        SetAutoMoveNode(c, GetSuffixLink().GetAutoMove(c));
                    }
                }

                return GetAutoMoveNode(c);
            }
        }
    }
}
