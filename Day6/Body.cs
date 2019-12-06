using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Day6
{
    public class Body : IEnumerable<Body>
    {
        public string ID { get; }

        public Body Parent { get; private set; }

        private readonly List<Body> _children;

        public Body (string id)
        {
            ID = id;
            _children = new List<Body>();
        }

        public void Add(Body body)
        {
            if (body.Parent != null)
            {
                body.Parent._children.Remove(body);
            }
            body.Parent = this;
            _children.Add(body);
        }

        public IEnumerable<Body> GetNodeAndDescendants() 
        {
            return new[] { this }
                   .Concat(_children.SelectMany(child => child.GetNodeAndDescendants()));
        }

        public int ParentCount
        {
            get
            {
                if(Parent == null)
                {
                    return 0;
                }

                return 1 + Parent.ParentCount;
            }
        }

        public int ChildrenCount
        { get { return this._children.Count; } }

        public IEnumerator<Body> GetEnumerator()
        {
            return _children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }



        public Body Find(string ID)
        {
            return GetNodeAndDescendants()
                         .FirstOrDefault(node => node.ID == ID);
        }

        public int SumChild()
        {
            var nodes = GetNodeAndDescendants();

            int r = 0;
            foreach (Body body in nodes)
            {
                r += body.ParentCount;
            }

            return r;
        }

        public List<string> PathToRoot()
        {
            List<string> path = new List<string>();
            if(Parent == null)
            {
                return path;
            }
            path.Add(Parent.ID);
            path.AddRange(Parent.PathToRoot());
            return path;
        }

        public void MakeRoot()
        {
            this.Parent = null;
        }
    }
}
