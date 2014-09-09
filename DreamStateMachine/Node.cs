using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DreamStateMachine
{
    public class Node<T>
    {
        // Private member-variables
        private T data;
        private NodeList<T> children = null;

        public Node() {
            children = new NodeList<T>();
        }

        public Node(T data){
            this.data = data;
            children = new NodeList<T>();
        }

        public Node(T data, NodeList<T> children)
        {
            this.data = data;
            this.children = children;
        }

        public T Value
        {
            get
            {
                return data;
            }
            set
            {
                data = value;
            }
        }

        public NodeList<T> Children
        {
            get
            {
                return children;
            }
            set
            {
                children = value;
            }
        }
    }
}
