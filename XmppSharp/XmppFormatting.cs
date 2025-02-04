namespace XmppSharp;

[Flags]
public enum XmppFormatting
{
    None,
    Indented = 1 << 0,
    OmitDuplicatedNamespaces = 1 << 1,
    OmitXmlDeclaration = 1 << 2,
    NewLineOnAttributes = 1 << 3,
    DoNotEscapeUriAttributes = 1 << 4,
    CheckCharacters = 1 << 5,


    Default
        = OmitDuplicatedNamespaces
        | OmitXmlDeclaration
        | CheckCharacters,
}