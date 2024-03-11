﻿using XmppSharp.Attributes;
using XmppSharp.Xmpp.Dom;

namespace XmppSharp.Protocol.Sasl;

[XmppTag("failure", Namespace.Sasl)]
public class Failure : Element
{
    public Failure() : base("failure", Namespace.Sasl)
    {

    }

    public Failure(FailureCondition? condition, string? message = default)
    {
        Condition = condition;
        Message = message;
    }

    public FailureCondition? Condition
    {
        get
        {
            foreach (var (key, value) in XmppEnum.GetValues<FailureCondition>())
            {
                if (HasTag(key))
                    return value;
            }

            return default;
        }
        set
        {
            if (Condition.TryUnwrap(out var oldValue))
                RemoveTag(XmppEnum.ToXml(oldValue));

            if (value.TryUnwrap(out var result))
                SetTag(XmppEnum.ToXml(result));
        }
    }

    public string Message
    {
        get => GetTag("text");
        set => SetTag("text", value);
    }
}
