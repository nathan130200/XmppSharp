import { promises as fs } from 'node:fs';
import { IndentedStringBuilder, kebabToPascalCase } from './util.mjs';
import path from 'node:path';

let baseDir = path.join(process.cwd(), 'data');
let distDir = path.join(process.cwd(), 'dist');

for (let file of await fs.readdir(baseDir)
    .then(r => r.map(x => path.join(baseDir, x)))) {

    var content = JSON.parse(await fs.readFile(file));

    var typeNamespace = content.namespace;
    var typeName = content.name;

    var members = content.values.map(function (item) {
        if (typeof (item) == 'string')
            return { name: item, comment: null };

        return item;
    });

    var sb = new IndentedStringBuilder()
        .writeln('using XmppSharp.Attributes;')
        .writeln();

    if (typeNamespace && typeNamespace.length) {
        sb.writeln(`namespace ${typeNamespace};`)
            .writeln();
    }

    sb.writeln('[XmppEnum]')
        .writeln(`public enum ${typeName}`)
        .writeln('{')
        .indent();

    for (let i = 0; i < members.length; i++) {
        let entry = members[i];

        if (entry.comment) {
            sb.writeln('/// <summary>')
                .writeln(`/// ${entry.comment}`)
                .writeln('/// </summary>');
        }

        var end = (i < members.length - 1) ? ',' : '';

        if (entry.name || entry.rawName)
            sb.writeln(`[XmppEnumMember("${entry.name || entry.rawName}")]`);

        let fieldName = entry.memberName || kebabToPascalCase(entry.name);
        sb.writeln(`${fieldName}${end}`);

        if (end)
            sb.writeln();
    }

    sb.unindent()
        .write('}');

    await fs.writeFile(path.join(distDir, `${typeName}.g.cs`), sb.toString());
}