using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Business_Logic.MessagesModule.InnerLibs.Text2Graph;
using Business_Logic.MessagesModule.DataObjects;
using Business_Logic.MessagesModule.EntitiesExtensions;

namespace Business_Logic.MessagesModule.Mechanisms {

    public class MessageProducerException : Exception {
        public MessageProducerException (string Message) : base (Message) {
        }
    }

    class MessageProductionErrorContainer {
        public List<TextNodeFillErorr> fillErrors = new List<TextNodeFillErorr>();
        public List<TextParseException> parseExceptions = new List<TextParseException>();
    }

    class TextNodeFillErorr {

    }

    public class MessageProducer {

        TextNode headerTempl;
        TextNode bodyTempl;
        IEnumerable<KeyValuePair<string,string>> wildcards;
        MessageProductionErrorContainer errorContainer = new MessageProductionErrorContainer();

        public MessageProducer(IMessageTemplate templ, IEnumerable<KeyValuePair<string, string>> wildcards, INodeMarkUpSpecification specs) {
            try {
                headerTempl = TextNode.SplitTextToNodes(templ.MsgHeader + "  ", specs);
                bodyTempl = TextNode.SplitTextToNodes(templ.MsgBody + "  ", specs);
            }
            catch (TextParseException e) {
                errorContainer.parseExceptions.Add(e);
                throw new MessageProducerException("Text parse Exception: " + e.Message);
            }
            this.wildcards = wildcards;
        }

        public void ChangeWildCards (IEnumerable<KeyValuePair<string, string>> newWildcards) {
            wildcards = newWildcards;
        }

        /// <summary>
        /// Takes data grouped by adress.
        /// </summary>
        public Message Produce(IGrouping<string,IDictionary<string,object>> data, MessageType type) {
            Message msg = new Message();
            msg.Adress = data.Key;

            msg.Header = ProcNode(headerTempl, data).Trim();
            msg.Body = ProcNode(bodyTempl, data).Trim();

            msg.Type = type;
            return msg;
        }

        string ProcNode(TextNode node, IGrouping<string, IDictionary<string, object>> data, int iterNum = 0) {
            StringBuilder sb = new StringBuilder();

            if (node.Type == TextNodeType.plain) 
                sb.Append(GetStringWithReplacements(node.Content, data.Skip(iterNum).First()));
            else {
                if (node.Type == TextNodeType.conditional) {
                    //node.ParamsString - here all params from node, you can evaluate condition here
                    bool conditionPassed = false;
                    try {
                        var parsedCondition = (GetStringWithReplacements(node.ParamsString, data.Skip(iterNum).First()));
                        var pars = parsedCondition.Split(' ');
                        var simpleCond = pars[1];
                        if (simpleCond.StartsWith("!")) {
                            simpleCond = simpleCond.Substring(1);
                            conditionPassed = !bool.Parse(simpleCond);
                        }
                        else
                            conditionPassed = bool.Parse(simpleCond);
                    }
                    catch { conditionPassed = false; }
                    if (conditionPassed)
                        foreach (var cnode in node.Childs)
                            sb.Append(ProcNode(cnode, data, iterNum));
                }
                else if (node.Type == TextNodeType.repeater) {
                    var curIterNum = 0;
                    foreach (var d in data) {
                        foreach (var cnode in node.Childs)
                            sb.Append(ProcNode(cnode, data, curIterNum));
                        curIterNum++;
                    }
                }
                else {
                    foreach (var cnode in node.Childs) 
                        sb.Append(ProcNode(cnode, data, iterNum));
                }
            }
            return sb.ToString();
        }

        string GetStringWithReplacements(string stringWithWildcards, IDictionary<string, object> data) {
            StringBuilder sb = new StringBuilder(stringWithWildcards);
            foreach (var kv in wildcards) {
                //TODO CHECK AND ADD AN ERROR IF NO FIELD IN DICTIONARY
                sb.Replace(kv.Key, data[kv.Value].ToString());
            }
            return sb.ToString();
        }

    }

}
