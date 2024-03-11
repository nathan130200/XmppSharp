from io import StringIO

def kebab_to_pascal_case(input_string: str) -> str:
    words = input_string.split("-")
    return "".join([word.title() for word in words])

class IndentedStringIO(StringIO):
    def __init__(self, indent_chars: str = '  ', newline_chars: str = '\n'):
        StringIO.__init__(self)
        self._depth = 0
        self._newline_chars = newline_chars
        self._indent_chars = indent_chars
        
    def indent(self):
        self._depth += 1
    
    def unindent(self):
        if self._depth > 0:
            self._depth -= 1
            
    def _write_indent(self) -> int:
        return super().write(self._indent_chars * self._depth)
        
    def write(self, text: str = None) -> int:
        n = self._write_indent()
        
        if text != None:
            n += super().write(text)
        
        return n
    
    def writeln(self, text: str =None) -> int:
        self.write(text)
        super().write(self._newline_chars)