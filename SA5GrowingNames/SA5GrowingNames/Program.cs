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

            var tree = new PrefixTree<char, bool>();

            foreach (var row in rows)
            {
                tree.AddString(row, true);
            }


            var output = tree.FindLongestChainLength().ToString();

            File.WriteAllText(OutputFileName, output);
        }
    }

    public class PrefixTree<TChar, TTag> where TTag : struct
    {
        private readonly PrefixTreeNode _root = new PrefixTreeNode(null, default);

        public void AddString(IEnumerable<TChar> s, TTag tag)
        {
            var node = _root;

            foreach (var c in s)
            {
                node = node.UpdateChild(c);
            }

            node.Tag = tag;
        }

        public int FindLongestChainLength()
        {
            var rates = new Dictionary<PrefixTreeNode, int>();
            var queue = new Queue<PrefixTreeNode>();

            rates[_root] = 0;
            queue.Enqueue(_root);

            while (queue.Any())
            {
                var node = queue.Dequeue();
                var rate = Math.Max(rates[node.ParentLink ?? node], rates[node.GetSuffixLink()]);

                if (node.Tag != null)
                {
                    rate++;
                }

                rates[node] = rate;

                foreach (var child in node.Children.Values)
                {
                    queue.Enqueue(child);
                }
            }

            return rates.Values.Max();
        }

        private class PrefixTreeNode
        {
            private Dictionary<TChar, PrefixTreeNode> AutoMove { get; } = new Dictionary<TChar, PrefixTreeNode>();

            private PrefixTreeNode SuffixLink { get; set; }

            public Dictionary<TChar, PrefixTreeNode> Children { get; } = new Dictionary<TChar, PrefixTreeNode>();

            public PrefixTreeNode ParentLink { get; }

            public TChar Char { get; }

            public TTag? Tag { get; set; }

            public PrefixTreeNode(PrefixTreeNode parent, TChar c)
            {
                ParentLink = parent;
                Char = c;
            }

            public PrefixTreeNode UpdateChild(TChar c)
            {
                if (!Children.ContainsKey(c))
                {
                    Children[c] = new PrefixTreeNode(this, c);
                }

                return Children[c];
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

            public PrefixTreeNode GetAutoMove(TChar c)
            {
                if (!AutoMove.ContainsKey(c))
                {
                    if (Children.ContainsKey(c))
                    {
                        AutoMove[c] = Children[c];
                    }
                    else if (ParentLink == null)
                    {
                        AutoMove[c] = this;
                    }
                    else
                    {
                        AutoMove[c] = GetSuffixLink().GetAutoMove(c);
                    }
                }

                return AutoMove[c];
            }
        }
    }
}
