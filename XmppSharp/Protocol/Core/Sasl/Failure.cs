﻿using XmppSharp.Attributes;
using XmppSharp.Dom;

namespace XmppSharp.Protocol.Core.Sasl;

[XmppTag("failure", Namespaces.Sasl)]
public class Failure : Element
{
    public Failure() : base("failure", Namespaces.Sasl)
    {

    }

    public Failure(FailureCondition? condition, string? text = default) : this()
    {
        Condition = condition;
        Text = text;
    }

    public FailureCondition? Condition
    {
        get
        {
            foreach (var (name, value) in XmppEnum.GetXmlMapping<FailureCondition>())
            {
                if (HasTag(name, Namespaces.Sasl))
                    return value;
            }

            return default;
        }
        set
        {
            foreach (var name in XmppEnum.GetXmlNames<FailureCondition>())
                RemoveTag(name, Namespaces.Sasl);

            if (value.HasValue)
                SetTag(x =>
                {
                    x.TagName = XmppEnum.ToXml(value)!;
                    x.Namespace = Namespaces.Sasl;
                });

        }
    }

    public string? Text
    {
        get => GetTag("text", Namespaces.Sasl);
        set
        {
            RemoveTag("text", Namespaces.Sasl);

            if (value != null)
            {
                SetTag(x =>
                {
                    x.TagName = "text";
                    x.Namespace = Namespaces.Sasl;
                    x.Value = value;
                });
            }
        }
    }
}
