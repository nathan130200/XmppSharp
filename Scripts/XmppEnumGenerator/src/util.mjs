import { Buffer } from 'node:buffer';
import { format } from 'node:util';

class IndentedStringBuilder {
    constructor() {
        this.buf = '';
        this.depth = 0;
    }

    indent() {
        this.depth++;
        return this;
    }

    unindent() {
        if (this.depth > 0)
            this.depth--;

        return this;
    }

    _write(v) {
        this.buf += v;
    }

    write(value) {
        if (value && value.length) {
            this._write(value);
        }

        return this;
    }

    writeln(value) {
        this._write('\t'.repeat(this.depth));

        if (value && value.length) {
            this._write(value);
        }

        this._write('\n');

        return this;
    }

    toString() {
        return this.buf;
    }
}

function kebabToPascalCase(text) {
    return text.split('-').map(function (token) {
        return token.charAt(0).toUpperCase()
            + token.substring(1).toLowerCase();
    }).join('');
}

export {
    IndentedStringBuilder,
    kebabToPascalCase
};