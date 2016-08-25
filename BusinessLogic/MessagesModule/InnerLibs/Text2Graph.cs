using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Business_Logic.MessagesModule.InnerLibs.Text2Graph {

    public enum TextNodeType {
        NONE,
        root,
        plain,
        repeater,
        conditional
    }

    public class TextParseException : Exception {
        public TextParseException(string msg) : base(msg) { }
    }

    public interface INodeMarkUpSpecification {
        string OpenMark { get; }
        string CloseMark { get; }
        string NodeClose_OpenMarkPostfix { get; }

        IDictionary<string, TextNodeType> Keywords { get; }
        bool DestroyNewLineAfterMarkUp { get; }
        string NewLineSymbol { get; }
    }

    public class DefaultMarkUpSpecification : INodeMarkUpSpecification {

        public string OpenMark { get; set; } 
        public string CloseMark { get; set; } 
        public string NodeClose_OpenMarkPostfix { get; set; } 

        public IDictionary<string, TextNodeType> Keywords { get; set; } 

        public bool DestroyNewLineAfterMarkUp { get; set; } 
        public string NewLineSymbol { get; set; } 

        public DefaultMarkUpSpecification () {
            OpenMark = "{|";
            CloseMark = "|}";
            NodeClose_OpenMarkPostfix = "|";
            Keywords = new Dictionary<string, TextNodeType> {
                {"repeat",TextNodeType.repeater },
                {"if",TextNodeType.conditional },
            };
            DestroyNewLineAfterMarkUp = true;
            NewLineSymbol = Environment.NewLine;
        }
    }

    public class TextNode {
        public IEnumerable<TextNode> Childs { get { return childs.AsEnumerable(); } }
        public TextNodeType Type { get { return type; } }
        public string Content { get { return content; } }
        public string ParamsString { get { return paramsString; } }

        readonly TextNodeType type;
        readonly string content;
        string paramsString;
        List<TextNode> childs = new List<TextNode>();
        TextNode parent;
        StringBuilder builder = new StringBuilder();

        /// <summary>
        /// Splits the raw text to nodes, according to given INodeMarkUpSpecification.
        /// if no INodeMarkUpSpecification default specifiction will be used.
        /// Returns rootNode.
        /// </summary>
        /// <returns></returns>
        public static TextNode SplitTextToNodes(string rawText, INodeMarkUpSpecification specs = null) {
            if (specs == null)
                specs = new DefaultMarkUpSpecification();
            TextNode root = new TextNode(TextNodeType.root);
            MakeNodesRecoursive(new TextProccesor(rawText), root, specs);
            return root;
        }

        /// <summary>
        /// Creates a Plain TextNode
        /// </summary>
        public TextNode(string content) {
            this.content = content;
            type = TextNodeType.plain;
        }

        /// <summary>
        /// Creates a TextNode that can be a parent
        /// </summary>
        public TextNode(TextNodeType type) {
            if (type == TextNodeType.plain)
                throw new ArgumentException("Plain textNode should have content. Use another constructor");
            this.type = type;
        }

        public void AddChild(TextNode tn) {
            if (type == TextNodeType.plain)
                throw new InvalidOperationException("plain node can not have childs!");
            childs.Add(tn);
            if (tn.parent != null)
                throw new InvalidOperationException("textNode already has parent!");
            tn.parent = this;
        }

        static void MakeNodesRecoursive(TextProccesor proc, TextNode root, INodeMarkUpSpecification specs,
                                     NodeHeader openedHeader = null) {
            TextNodeType currType = openedHeader == null ? TextNodeType.NONE : openedHeader.type;
            TextNode currNode;

            while (proc.IsNext()) {
                currNode = null;
                var text = proc.NextTill(specs.OpenMark); // look for "{|"
                if (!string.IsNullOrEmpty(text))
                    currNode = new TextNode(text);
                if (!proc.IsNext()) {
                    if (currNode != null)
                        root.AddChild(currNode);
                    break;
                }
                proc.Move(specs.OpenMark.Length); // skips "{|"
                var newNodeHeader = new NodeHeader(proc.NextTill(specs.CloseMark), specs); //look for "}"
                proc.Move(specs.CloseMark.Length); // skips "}"
                if (specs.DestroyNewLineAfterMarkUp)
                    proc.TryEat(specs.NewLineSymbol);
                if (newNodeHeader.Enclosing) {
                    if (newNodeHeader.type == currType) {
                        if (currNode != null)
                            root.AddChild(currNode);
                        break; //node done
                    }
                    else
                        throw new TextParseException("Unexpected enclosing keyword: " + newNodeHeader.Name);
                }
                var subRootNode = new TextNode(newNodeHeader.type);
                subRootNode.paramsString = newNodeHeader.Params;
                if (currNode != null)
                    root.AddChild(currNode);
                root.AddChild(subRootNode);
                MakeNodesRecoursive(proc, subRootNode, specs, newNodeHeader);
            }
        }

        static TextNodeType StrToNodeType(string str, INodeMarkUpSpecification specs) {
            try {
                return specs.Keywords[str];
            }
            catch (KeyNotFoundException) {
                throw new TextParseException("Unknown node type: " + str);
            }
        }

        class NodeHeader {
            public readonly TextNodeType type;
            public readonly string Name;
            public bool Enclosing;

            public readonly string Params;

            static string[] splitters = new string[] { " " };

            public NodeHeader(string nodeHeaderFullString, INodeMarkUpSpecification specs) {
                var strings = nodeHeaderFullString.Split(splitters, StringSplitOptions.RemoveEmptyEntries);
                if (strings[0].StartsWith(specs.NodeClose_OpenMarkPostfix)) {
                    Enclosing = true;
                    strings[0] = strings[0].Substring(1);
                }
                Name = strings[0];
                type = StrToNodeType(strings[0], specs);
                Params = nodeHeaderFullString;
            }
        }

        public override string ToString() {
            if (type == TextNodeType.plain)
                return type.ToString()+ ": " + Content;
            else
                return type.ToString() + " childs: " + childs.Count;
        }
    }

    class TextProccesor {
        StringBuilder text;
        int length;
        int pos = 0;

        StringBuilder sb = new StringBuilder();

        public char Current { get { return text[pos]; } }

        public TextProccesor(string rawText) {
            text = new StringBuilder(rawText);
            length = rawText.Length;
        }

        /// <summary>
        /// Returns if there 'fromNow' characters exists ahead
        /// </summary>
        public bool IsNext(int fromNow = 1) {
            return pos + fromNow < length;
        }

        /// <summary>
        /// Returns Current symbol and goes to next
        /// </summary>
        public char Next() {
            return text[pos++];
        }

        /// <summary>
        /// Returns 'count' symbols from Current and goes to 'count' forward
        /// </summary>
        public string Next(int count) {
            var s = text.ToString(pos, count);
            pos += count;
            return s;
        }

        public void Move(int count = 1) {
            pos += count;
        }

        /// <summary>
        /// returns string from current position to specific char.
        /// char WILL NOT be included, current position will be on this char.
        /// if no matching occurs, whole string till end will be returned
        /// </summary>
        public string NextTill(char ch) {
            sb.Clear();
            while (IsNext() && Current != ch) {
                sb.Append(Current);
                Move();
            }
            if (Current != ch) {
                sb.Append(Current);
                Move();
            }
            return sb.ToString();
        }

        /// <summary>
        /// returns string from current position to specific string 'str'.
        /// string 'str' WILL NOT be included, current position will be on first symbol of the 'str'.
        /// if no matching occurs, whole string till end will be returned
        /// </summary>
        public string NextTill(string str) {
            int strL = str.Length;
            sb.Clear();
            while (IsNext(strL) && text.ToString(pos, strL) != str) {
                sb.Append(Current);
                Move();
            }
            if (text.Length <= pos + strL) {
                sb.Append(text.ToString(pos, text.Length - pos));
                Move(text.Length - pos);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Move on 'str'.Length forward, if 'str' matches on current position.
        /// </summary>
        public bool TryEat(string str) {
            bool result = text.ToString(pos, str.Length) == str;
            if (result)
                Move(str.Length);
            return result;
        }
    }

}

