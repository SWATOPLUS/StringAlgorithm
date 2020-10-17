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

            var rawGroups = Enumerable.Range(0, rows.Length)
                .GroupBy(x => rows[x].Length)
                .OrderBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.ToHashSet());

            var groups = new HashSet<int>[rawGroups.Keys.Max() + 1];

            for (var i = 0; i < groups.Length; i++)
            {
                if (rawGroups.ContainsKey(i))
                {
                    groups[i] = rawGroups[i];
                }
                else
                {
                    groups[i] = new HashSet<int>();
                }
            }

            var ranks = new Dictionary<int, int>();

            for (var i = 0; i < groups.Length; i++)
            {
                if (groups[i].Count == 0)
                {
                    continue;
                }

                var tree = new PrefixTree<int>();

                foreach (var rowId in groups.Take(i).SelectMany(x => x))
                {
                    tree.AddString(rows[rowId], rowId);
                }

                foreach (var name in groups[i])
                {
                    var maxRank = 0;

                    foreach (var rowId in tree.FindAllSubstrings(rows[name]))
                    {
                        var rank = ranks[rowId];
                        var text = rows[rowId];

                        if (ranks[rowId] > maxRank)
                        {
                            maxRank = rank;
                        }

                        groups[text.Length - 1].Remove(rowId);
                    }

                    ranks[name] = maxRank + 1;
                }
            }

            var output = ranks.Values.Max().ToString();

            File.WriteAllText(OutputFileName, output);
        }
    }

    public class PrefixTree<T> where T : struct
    {
        private const char AlphabetStart = 'a';
        private const char AlphabetEnd = 'z';
        private const int AlphabetSize = AlphabetEnd - AlphabetStart + 1;

        private readonly PrefixTreeLeaf _root = new PrefixTreeLeaf(null, -1);

        public void AddString(string s, T tag)
        {
            var node = _root;

            foreach (var code in s.Select(EncodeChar))
            {
                node = node.UpdateChild(code);
            }

            node.Tag = tag;
        }

        private static int EncodeChar(char c)
        {
            return c - AlphabetStart;
        }

        private PrefixTreeLeaf GetSuffixLink(PrefixTreeLeaf node)
        {
            if (node.SuffixLink == null)
            {
                if (node == _root || node.ParentLink == _root)
                {
                    node.SuffixLink = _root;
                }
                else
                {
                    node.SuffixLink = GetAutoMove(GetSuffixLink(node.ParentLink), node.Code);
                }
            }

            return node.SuffixLink;
        }

        public IEnumerable<T> FindAllSubstrings(string s)
        {
            var node = _root;

            for (var i = 0; i < s.Length; i++)
            {
                var code = EncodeChar(s[i]);

                node = GetAutoMove(node, code);

                for (var u = node; u != _root; u = GetSuffixFlaggedLink(u))
                {
                    if (u.Tag != null)
                    {
                        yield return (u.Tag.Value);
                    }
                }
            }
        }

        private PrefixTreeLeaf GetAutoMove(PrefixTreeLeaf node, int code)
        {
            if (node.AutoMove[code] == null)
            {
                if (node.Children[code] != null)
                {
                    node.AutoMove[code] = node.Children[code];
                }
                else if (node == _root)
                {
                    node.AutoMove[code] = _root;
                }
                else
                {
                    node.AutoMove[code] = GetAutoMove(GetSuffixLink(node), code);
                }
            }

            return node.AutoMove[code];
        }
        private PrefixTreeLeaf GetSuffixFlaggedLink(PrefixTreeLeaf node)
        {
            if (node.SuffixFlaggedLink == null)
            {
                var suffixLink = GetSuffixLink(node);
                if (suffixLink == _root)
                {
                    node.SuffixFlaggedLink = _root;
                }
                else
                {
                    if (suffixLink.Tag != null)
                    {
                        node.SuffixFlaggedLink = suffixLink;
                    }
                    else
                    {
                        node.SuffixFlaggedLink = GetSuffixFlaggedLink(suffixLink);
                    }
                }
            }

            return node.SuffixFlaggedLink;
        }

        private class PrefixTreeLeaf
        {
            public PrefixTreeLeaf[] Children { get; }

            public PrefixTreeLeaf[] AutoMove { get; }

            public PrefixTreeLeaf SuffixLink { get; set; }

            public PrefixTreeLeaf SuffixFlaggedLink { get; set; }

            public PrefixTreeLeaf ParentLink { get; }

            public int Code { get; }

            public T? Tag { get; set; }

            public PrefixTreeLeaf(PrefixTreeLeaf parent, int code)
            {
                Children = new PrefixTreeLeaf[AlphabetSize];
                AutoMove = new PrefixTreeLeaf[AlphabetSize];
                ParentLink = parent;
                Code = code;
            }

            public PrefixTreeLeaf UpdateChild(int code)
            {
                if (Children[code] == null)
                {
                    Children[code] = new PrefixTreeLeaf(this, code);
                }

                return Children[code];
            }
        }
    }
}
