using XmppSharp.Attributes;

namespace XmppSharp.Protocol.Extensions.DataForms;

[XmppEnum]
public enum FormType
{
    [XmppMember("cancel")]
    Cancel,

    [XmppMember("form")]
    Prompt,

    [XmppMember("result")]
    Result,

    [XmppMember("submit")]
    Submit,
}