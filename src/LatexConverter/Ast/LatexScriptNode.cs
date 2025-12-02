using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexConverter.Ast
{

    public interface ILatexNode
    {
        string ToString();
    }

    public abstract class LatexNode : ILatexNode
    {
        public abstract override string ToString();
    }

    public class LatexTextNode : LatexNode
    {
        public string Text { get; set; }

        public LatexTextNode(string text)
        {
            Text = text ?? "";
        }

        public override string ToString() => Text;
    }

    public class LatexCommandNode : LatexNode
    {
        public string Command { get; set; }
        public List<LatexNode> Args { get; set; } = new List<LatexNode>();

        public LatexCommandNode(string command)
        {
            Command = command;
        }

        public override string ToString()
        {
            if (Args.Count == 0)
                return $"\\{Command}";

            var argsString = string.Join("", Args.Select(arg => $"{{{arg}}}"));
            return $"\\{Command}{argsString}";
        }
    }

    public class LatexGroupNode : LatexNode
    {
        public string OpenGroup { get; set; }
        public string CloseGroup { get; set; }
        public List<LatexNode> Children { get; set; } = new List<LatexNode>();

        public LatexGroupNode(string openGroup, string closeGroup)
        {
            OpenGroup = openGroup;
            CloseGroup = closeGroup;
        }

        public override string ToString()
        {
            return $"{OpenGroup}{string.Join("", Children)}{CloseGroup}";
        }
    }

    public enum ScriptTypeEnum
    {
        Subscript,
        Superscript
    }

    public class LatexScriptNode : LatexNode
    {
        public ScriptTypeEnum ScriptType { get; set; }
        public LatexNode Base { get; set; }
        public LatexNode Script { get; set; }

        public LatexScriptNode(ScriptTypeEnum scriptType, LatexNode baseNode, LatexNode script)
        {
            ScriptType = scriptType;
            Base = baseNode;
            Script = script;
        }

        public override string ToString()
        {
            char scriptChar = ScriptType == ScriptTypeEnum.Subscript ? '_' : '^';
            return $"{Base}{scriptChar}{{{Script}}}";
        }
    }

}
