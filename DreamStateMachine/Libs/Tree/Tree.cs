using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DreamStateMachine
{
    class Tree<T>
    {
        NodeList<T> nodes;

        public Tree(){
            nodes = new NodeList<T>();
        }
        public Tree(Node<T> rootNode)
        {
            nodes = new NodeList<T>();
            nodes[0] = rootNode;
        }

        public void setRoot(Node<T> rootNode)
        {
            nodes.Add(rootNode);
        }
    }
}
