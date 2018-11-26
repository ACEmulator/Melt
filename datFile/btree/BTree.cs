namespace BTreeNamespace
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    /// <summary>
    /// Based on markmeeus/btree-dotnet at https://github.com/markmeeus/btree-dotnet
    /// --
    /// Based on BTree chapter in "Introduction to Algorithms", by Thomas Cormen, Charles Leiserson, Ronald Rivest.
    /// 
    /// This implementation is not thread-safe, and user must handle thread-safety.
    /// </summary>
    public class cBTree
    {
        public cBTree(int degree)
        {
            if (degree < 2)
            {
                throw new ArgumentException("BTree degree must be at least 2", "degree");
            }

            this.Root = new Node(degree);
            this.Degree = degree;
            this.Height = 1;
        }

        public Node Root { get; private set; }

        public int Degree { get; private set; }

        public int Height { get; private set; }

        /// <summary>
        /// Searches a entry in the BTree, returning the entry with it.
        /// </summary>
        /// <param name="entry">Entry being searched.</param>
        /// <returns>Entry for that entry, null otherwise.</returns>
        public uint Search(uint entry)
        {
            return this.SearchInternal(this.Root, entry);
        }

        /// <summary>
        /// Inserts a new entry in the BTree. This
        /// operation splits nodes as required to keep the BTree properties.
        /// </summary>
        /// <param name="newEntry">Entry to be inserted.</param>
        public void Insert(uint newEntry)
        {
            // there is space in the root node
            if (!this.Root.HasReachedMaxEntries)
            {
                this.InsertNonFull(this.Root, newEntry);
                return;
            }

            // need to create new node and have it split
            Node oldRoot = this.Root;
            this.Root = new Node(this.Degree);
            this.Root.Children.Add(oldRoot);
            this.SplitChild(this.Root, 0, oldRoot);
            this.InsertNonFull(this.Root, newEntry);

            this.Height++;
        }

        /// <summary>
        /// Deletes a entry from the BTree. This operations moves entries and nodes
        /// as required to keep the BTree properties.
        /// </summary>
        /// <param name="entryToDelete">Entry to be deleted.</param>
        public void Delete(uint entryToDelete)
        {
            this.DeleteInternal(this.Root, entryToDelete);

            // if root's last entry was moved to a child node, remove it
            if (this.Root.Entries.Count == 0 && !this.Root.IsLeaf)
            {
                this.Root = this.Root.Children.Single();
                this.Height--;
            }
        }

        /// <summary>
        /// Internal method to delete entries from the BTree
        /// </summary>
        /// <param name="node">Node to use to start search for the entry.</param>
        /// <param name="entryToDelete">Entry to be deleted.</param>
        private void DeleteInternal(Node node, uint entryToDelete)
        {
            int i = node.Entries.TakeWhile(comparedEntry => entryToDelete.CompareTo(comparedEntry) > 0).Count();

            // found entry in node, so delete if from it
            if (i < node.Entries.Count && node.Entries[i].CompareTo(entryToDelete) == 0)
            {
                this.DeleteEntryFromNode(node, entryToDelete, i);
                return;
            }

            // delete entry from subtree
            if (!node.IsLeaf)
            {
                this.DeleteEntryFromSubtree(node, entryToDelete, i);
            }
        }

        /// <summary>
        /// Helper method that deletes a entry from a subtree.
        /// </summary>
        /// <param name="parentNode">Parent node used to start search for the entry.</param>
        /// <param name="entryToDelete">Entry to be deleted.</param>
        /// <param name="subtreeIndexInNode">Index of subtree node in the parent node.</param>
        private void DeleteEntryFromSubtree(Node parentNode, uint entryToDelete, int subtreeIndexInNode)
        {
            Node childNode = parentNode.Children[subtreeIndexInNode];

            // node has reached min # of entries, and removing any from it will break the btree property,
            // so this block makes sure that the "child" has at least "degree" # of nodes by moving an 
            // entry from a sibling node or merging nodes
            if (childNode.HasReachedMinEntries)
            {
                int leftIndex = subtreeIndexInNode - 1;
                Node leftSibling = subtreeIndexInNode > 0 ? parentNode.Children[leftIndex] : null;

                int rightIndex = subtreeIndexInNode + 1;
                Node rightSibling = subtreeIndexInNode < parentNode.Children.Count - 1
                                                ? parentNode.Children[rightIndex]
                                                : null;

                if (leftSibling != null && leftSibling.Entries.Count > this.Degree - 1)
                {
                    // left sibling has a node to spare, so this moves one node from left sibling 
                    // into parent's node and one node from parent into this current node ("child")
                    childNode.Entries.Insert(0, parentNode.Entries[subtreeIndexInNode - 1]);
                    parentNode.Entries[subtreeIndexInNode - 1] = leftSibling.Entries.Last();
                    leftSibling.Entries.RemoveAt(leftSibling.Entries.Count - 1);

                    if (!leftSibling.IsLeaf)
                    {
                        childNode.Children.Insert(0, leftSibling.Children.Last());
                        leftSibling.Children.RemoveAt(leftSibling.Children.Count - 1);
                    }
                }
                else if (rightSibling != null && rightSibling.Entries.Count > this.Degree - 1)
                {
                    // right sibling has a node to spare, so this moves one node from right sibling 
                    // into parent's node and one node from parent into this current node ("child")
                    childNode.Entries.Add(parentNode.Entries[subtreeIndexInNode]);
                    parentNode.Entries[subtreeIndexInNode] = rightSibling.Entries.First();
                    rightSibling.Entries.RemoveAt(0);

                    if (!rightSibling.IsLeaf)
                    {
                        childNode.Children.Add(rightSibling.Children.First());
                        rightSibling.Children.RemoveAt(0);
                    }
                }
                else
                {
                    // this block merges either left or right sibling into the current node "child"
                    if (leftSibling != null)
                    {
                        childNode.Entries.Insert(0, parentNode.Entries[subtreeIndexInNode - 1]);
                        var oldEntries = childNode.Entries;
                        childNode.Entries = leftSibling.Entries;
                        childNode.Entries.AddRange(oldEntries);
                        if (!leftSibling.IsLeaf)
                        {
                            var oldChildren = childNode.Children;
                            childNode.Children = leftSibling.Children;
                            childNode.Children.AddRange(oldChildren);
                        }

                        parentNode.Children.RemoveAt(leftIndex);
                        parentNode.Entries.RemoveAt(subtreeIndexInNode - 1);
                    }
                    else
                    {
                        Debug.Assert(rightSibling != null, "Node should have at least one sibling");
                        childNode.Entries.Add(parentNode.Entries[subtreeIndexInNode]);
                        childNode.Entries.AddRange(rightSibling.Entries);
                        if (!rightSibling.IsLeaf)
                        {
                            childNode.Children.AddRange(rightSibling.Children);
                        }

                        parentNode.Children.RemoveAt(rightIndex);
                        parentNode.Entries.RemoveAt(subtreeIndexInNode);
                    }
                }
            }

            // at this point, we know that "child" has at least "degree" nodes, so we can
            // move on - this guarantees that if any node needs to be removed from it to
            // guarantee BTree's property, we will be fine with that
            this.DeleteInternal(childNode, entryToDelete);
        }

        /// <summary>
        /// Helper method that deletes entry from a node that contains it, be this
        /// node a leaf node or an internal node.
        /// </summary>
        /// <param name="node">Node that contains the entry.</param>
        /// <param name="entryToDelete">Entry to be deleted.</param>
        /// <param name="entryIndexInNode">Index of entry within the node.</param>
        private void DeleteEntryFromNode(Node node, uint entryToDelete, int entryIndexInNode)
        {
            // if leaf, just remove it from the list of entries (we're guaranteed to have
            // at least "degree" # of entries, to BTree property is maintained
            if (node.IsLeaf)
            {
                node.Entries.RemoveAt(entryIndexInNode);
                return;
            }

            Node predecessorChild = node.Children[entryIndexInNode];
            if (predecessorChild.Entries.Count >= this.Degree)
            {
                uint predecessorEntry = this.GetLastEntry(predecessorChild);
                this.DeleteInternal(predecessorChild, predecessorEntry);
                node.Entries[entryIndexInNode] = predecessorEntry;
            }
            else
            {
                Node successorChild = node.Children[entryIndexInNode + 1];
                if (successorChild.Entries.Count >= this.Degree)
                {
                    uint successorEntry = this.GetFirstEntry(successorChild);
                    this.DeleteInternal(successorChild, successorEntry);
                    node.Entries[entryIndexInNode] = successorEntry;
                }
                else
                {
                    predecessorChild.Entries.Add(node.Entries[entryIndexInNode]);
                    predecessorChild.Entries.AddRange(successorChild.Entries);
                    predecessorChild.Children.AddRange(successorChild.Children);

                    node.Entries.RemoveAt(entryIndexInNode);
                    node.Children.RemoveAt(entryIndexInNode + 1);

                    this.DeleteInternal(predecessorChild, entryToDelete);
                }
            }
        }

        /// <summary>
        /// Helper method that gets the last entry (i.e. rightmost entry) for a given node.
        /// </summary>
        private uint GetLastEntry(Node node)
        {
            if (node.IsLeaf)
            {
                return node.Entries.Last();
            }

            return this.GetLastEntry(
                node.Children.Last()
            );
        }

        /// <summary>
        /// Helper method that gets the first entry (i.e. leftmost entry) for a given node.
        /// </summary>
        private uint GetFirstEntry(Node node)
        {
            if (node.IsLeaf)
            {
                return node.Entries.First();
            }

            return this.GetFirstEntry(
                node.Children.First()
            );
        }

        /// <summary>
        /// Helper method that search for a entry in a given BTree.
        /// </summary>
        /// <param name="node">Node used to start the search.</param>
        /// <param name="entry">Entry to be searched.</param>
        /// <returns>Entry object with entry information if found, null otherwise.</returns>
        private uint SearchInternal(Node node, uint entry)
        {
            int i = node.Entries.TakeWhile(compareToEntry => entry.CompareTo(compareToEntry) > 0).Count();

            if (i < node.Entries.Count && node.Entries[i].CompareTo(entry) == 0)
            {
                return node.Entries[i];
            }

            return node.IsLeaf ? 0 : this.SearchInternal(node.Children[i], entry);
        }

        /// <summary>
        /// Helper method that splits a full node into two nodes.
        /// </summary>
        /// <param name="parentNode">Parent node that contains node to be split.</param>
        /// <param name="nodeToBeSplitIndex">Index of the node to be split within parent.</param>
        /// <param name="nodeToBeSplit">Node to be split.</param>
        private void SplitChild(Node parentNode, int nodeToBeSplitIndex, Node nodeToBeSplit)
        {
            if(nodeToBeSplit.Entries[nodeToBeSplitIndex] == 65822)
            { }

            var newNode = new Node(this.Degree);

            uint entryToMove = nodeToBeSplit.Entries[this.Degree - 1];
            parentNode.Entries.Insert(nodeToBeSplitIndex, entryToMove);
            parentNode.Children.Insert(nodeToBeSplitIndex + 1, newNode);

            List<uint> entriesToMoveToNewNode = nodeToBeSplit.Entries.GetRange(this.Degree, this.Degree - 1);
            newNode.Entries.AddRange(entriesToMoveToNewNode);

            // remove also Entries[this.Degree - 1], which is the one to move up to the parent
            nodeToBeSplit.Entries.RemoveRange(this.Degree - 1, this.Degree);

            if (!nodeToBeSplit.IsLeaf)
            {
                List<Node> nodesToMoveToNewNode = nodeToBeSplit.Children.GetRange(this.Degree, this.Degree);
                newNode.Children.AddRange(nodesToMoveToNewNode);
                nodeToBeSplit.Children.RemoveRange(this.Degree, this.Degree);
            }

            //parentNode.Entries.Insert(nodeToBeSplitIndex, nodeToBeSplit.Entries[this.Degree - 1]);
            //parentNode.Children.Insert(nodeToBeSplitIndex + 1, newNode);

            //newNode.Entries.AddRange(nodeToBeSplit.Entries.GetRange(this.Degree, this.Degree - 1));

            //// remove also Entries[this.Degree - 1], which is the one to move up to the parent
            //nodeToBeSplit.Entries.RemoveRange(this.Degree - 1, this.Degree);

            //if (!nodeToBeSplit.IsLeaf)
            //{
            //    newNode.Children.AddRange(nodeToBeSplit.Children.GetRange(this.Degree, this.Degree));
            //    nodeToBeSplit.Children.RemoveRange(this.Degree, this.Degree);
            //}
        }

        private void InsertNonFull(Node node, uint newEntry)
        {
            if (newEntry == 65822)
            { }

            int positionToInsert = node.Entries.TakeWhile(entry => newEntry.CompareTo(entry) > 0).Count();

            // leaf node
            if (node.IsLeaf)
            {
                node.Entries.Insert(positionToInsert, newEntry);
                return;
            }

            // non-leaf
            Node child = node.Children[positionToInsert];
            if (child.HasReachedMaxEntries)
            {
                this.SplitChild(node, positionToInsert, child);
                if (newEntry.CompareTo(node.Entries[positionToInsert]) > 0)
                {
                    positionToInsert++;
                }
            }

            this.InsertNonFull(node.Children[positionToInsert], newEntry);
        }
    }
}