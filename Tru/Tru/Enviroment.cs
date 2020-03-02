namespace Tru {
    /// Represents the environment of a Tru program as a mapping of names to values.
    public class Environment {
        private class Node {
            public string name;
            public TruVal val;
            public Node next; // Environment will be a linked list.

            public Node(string name, TruVal val, Node next) {
                this.name = name; this.val = val; this.next = next;
            }

            /// Makes a complete copy of the linked list.
            public Node Copy() {
                return new Node(this.name, this.val, (this.next != null) ? this.next.Copy() : null);
            }

            /// Returns the last node in the linked list.
            public Node Head() {
                Node current = this;
                while (current.next != null)
                    current = current.next;
                return current;
            }

        }

        private Node list;

        private Environment(Node list) { this.list = list; }

        /// Creates an environment from a list of (string, TruVal) tuples.
        /// Values later in bindings are "higher" priority in case of name conflicts.
        public Environment( (string name, TruVal val)[] bindings = null ) {
            Node node = null;

            if (bindings != null)
                foreach (var binding in bindings)
                    node = new Node(binding.name, binding.val, node);

            this.list = node;
        }

        /// Returns the value corresponding to name in the enviroment, or throws an exception if
        /// if it doesn't exists.
        public TruVal Find(string name) {
            Node currentNode = this.list;

            while (currentNode != null && currentNode.name != name) {
                currentNode = currentNode.next;
            }

            if (currentNode == null) {
                throw new System.ArgumentException($"Free variable {name}.");
            } else {
                return currentNode.val;
            }

        }

        /// Create a new environment with the given binding added. Returns the new environment.
        /// The new binding will be higher priority than the old ones.
        public Environment ExtendLocal(string name, TruVal val) { // basically cons function
            return new Environment( new Node(name, val, this.list) );
        }

        /// Returns a new environment that contains all the bindings in this and env.
        /// Bindings in env will be higher priority than those in this.
        public Environment ExtendLocalAll(Environment env) {
            if (env.list != null) {
                Node newList = env.list.Copy();
                newList.Head().next = this.list;
                return new Environment(newList);
            } else {
                return new Environment(this.list); // appending an empty environment does nothing.
            }
        }


        /// Modifies the current environment to contain a new binding.
        /// The same as ExtendLocal, except it mutates this instead of returning a new environment
        /// The new binding will be higher priority than the old ones.
        public void ExtendGlobal(string name, TruVal val) {
            this.list = new Node(name, val, this.list);
        }

        /// Modifies the current environment to contain a new binding.
        /// The same as ExtendLocal, except it mutates this instead of returning a new environment
        /// The new binding will be higher priority than the old ones.
        public void ExtendGlobal(Environment env) {
            if (env.list != null) {
                Node newList = env.list.Copy();
                newList.Head().next = this.list;
                this.list = newList;
            }
            // appending an empty environment does nothing.
        }
    }
}