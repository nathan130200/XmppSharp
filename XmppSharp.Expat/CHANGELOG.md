# XMPP# Expat
This package implements the Expat parser in Xmpp Sharp library.

[![github](https://img.shields.io/badge/XMPP%23_%20Expat-ffe000?style=flat-square&logo=github&label=Github)](https://github.com/nathan130200/XmppSharp)

### Common Types
- `ExpatXmppParser`

### P/Invoking
The library `expat-bindings` depends on native library `libexpat` installed on your system. I've pre-compiled libexpat binaries in XMPP# repo. Check in [github actions page](https://github.com/nathan130200/XmppSharp/actions/workflows/vcpkg.yml).

### Version History

*1.0.0*

- Initial Release

___

*1.0.1*

- Minor improvements.
- Better namespace handler (using namespace stack to track different XML scopes).
- All tests passing.

*1.0.2*
___

- Updated `expat-bindings` library to newer version.
- Small code change
- All tests passing.