import sys
from utilities import IndentedStringIO,kebab_to_pascal_case

params = [
    {
        'name': 'StreamErrorCondition',
        'namespace': 'XmppSharp.Protocol.Base',
        'values': [
            'bad-format',
            'bad-namespace-prefix',
            'conflict',
            'connection-timeout',
            'host-gone',
            'host-unknown',
            'improper-addressing',
            'internal-server-error',
            'invalid-from',
            'invalid-namespace',
            'invalid-xml',
            'not-authorized',
            'not-well-formed',
            'policy-violation',
            'remote-connection-failed',
            'reset',
            'resource-constraint',
            'restricted-xml',
            'see-other-host',
            'system-shutdown',
            'undefined-condition',
            'unsupported-encoding',
            'unsupported-feature',
            'unsupported-stanza-type',
            'unsupported-version',
        ]
    }
]
text = IndentedStringIO()
text.writeln('using System.CodeDom.Compiler;')
text.writeln('using XmppSharp.Attributes;')
text.writeln();

for enum in params:
    
    if enum.get('namespace') != None:
        text.writeln('namespace {}'.format(enum['namespace']))
        text.writeln('{')
        text.indent()
    
    text.writeln('[XmppEnum, GeneratedCode("Python","{}")]'.format(sys.version))
    text.writeln('public enum {}'.format(enum['name']))
    text.writeln('{')
    text.indent()
    
    for member in enum['values']:
        text.writeln('[XmppEnumMember("{}")]'.format(member))
        text.writeln('{},'.format(kebab_to_pascal_case(member)))
        text.writeln()
    
    text.unindent()
    text.writeln('}')
    
    if enum.get('namespace') != None:
        text.unindent()
        text.writeln('}')
        
    text.writeln()

with open("generate_enums.g.cs", "w") as file:
    file.write(text.getvalue())
    file.flush()